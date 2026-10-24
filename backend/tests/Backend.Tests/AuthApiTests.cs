using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Auth;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Backend.Tests;

public sealed class AuthApiTests : IClassFixture<AuthApiTests.AuthApiFactory>
{
    private const string CookieName = "qlkltn_access";
    private readonly AuthApiFactory _factory;

    public AuthApiTests(AuthApiFactory factory) => _factory = factory;

    [Fact]
    public async Task LoginSetsHttpOnlyCookieAndReturnsUserWithoutToken()
    {
        var email = _factory.AddUser(UserRole.Student, "matkhau123", mustChange: false);

        var response = await Client().PostAsJsonAsync("/api/auth/login", new { email, password = "matkhau123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"), c => c.StartsWith($"{CookieName}="));
        Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=lax", setCookie, StringComparison.OrdinalIgnoreCase);
        var body = await ReadJson(response);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal("Student", body.GetProperty("data").GetProperty("role").GetString());
        Assert.False(body.GetProperty("data").GetProperty("mustChangePassword").GetBoolean());
        Assert.DoesNotContain(CookieValue(response), await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task LoginWithWrongPasswordReturns401ApiResponse()
    {
        var email = _factory.AddUser(UserRole.Student, "matkhau123", mustChange: false);

        var response = await Client().PostAsJsonAsync("/api/auth/login", new { email, password = "sai-mat-khau1" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
        var body = await ReadJson(response);
        Assert.False(body.GetProperty("success").GetBoolean());
        Assert.Equal(LoginHandler.InvalidCredentialsMessage, body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task ProtectedEndpointWithoutCookieReturns401ApiResponse()
    {
        var response = await Client().GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False((await ReadJson(response)).GetProperty("success").GetBoolean());
    }

    [Theory]
    [InlineData(UserRole.Student, "student", HttpStatusCode.OK)]
    [InlineData(UserRole.Student, "lecturer", HttpStatusCode.Forbidden)]
    [InlineData(UserRole.Student, "academic-affairs", HttpStatusCode.Forbidden)]
    [InlineData(UserRole.Lecturer, "lecturer", HttpStatusCode.OK)]
    [InlineData(UserRole.Lecturer, "academic-affairs", HttpStatusCode.Forbidden)]
    [InlineData(UserRole.AcademicAffairs, "academic-affairs", HttpStatusCode.OK)]
    [InlineData(UserRole.AcademicAffairs, "staff", HttpStatusCode.OK)]
    [InlineData(UserRole.Lecturer, "staff", HttpStatusCode.OK)]
    [InlineData(UserRole.Student, "staff", HttpStatusCode.Forbidden)]
    [InlineData(UserRole.Student, "any", HttpStatusCode.OK)]
    public async Task RoleBasedAuthorization(UserRole role, string endpoint, HttpStatusCode expected)
    {
        var cookie = await LoginCookie(_factory.AddUser(role, "matkhau123", mustChange: false), "matkhau123");

        var response = await Send(HttpMethod.Get, $"/api/test-rbac/{endpoint}", cookie);

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
            Assert.False((await ReadJson(response)).GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task InitialPasswordTokenOnlyAllowsProfileAndPasswordChange()
    {
        var email = _factory.AddUser(UserRole.Student, "22520001", mustChange: true, code: "22520001");
        var initialCookie = await LoginCookie(email, "22520001");

        var me = await Send(HttpMethod.Get, "/api/auth/me", initialCookie);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        Assert.True((await ReadJson(me)).GetProperty("data").GetProperty("mustChangePassword").GetBoolean());
        Assert.Equal(HttpStatusCode.Forbidden, (await Send(HttpMethod.Get, "/api/test-rbac/student", initialCookie)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await Send(HttpMethod.Get, "/api/test-rbac/any", initialCookie)).StatusCode);

        var change = await Send(HttpMethod.Post, "/api/auth/change-initial-password", initialCookie,
            new { currentPassword = "22520001", newPassword = "matkhau123" });
        Assert.Equal(HttpStatusCode.OK, change.StatusCode);
        var newCookie = CookieValue(change);

        Assert.Equal(HttpStatusCode.OK, (await Send(HttpMethod.Get, "/api/test-rbac/student", newCookie)).StatusCode);
        // Đổi mật khẩu xoay SecurityStamp nên token cũ hết hiệu lực.
        Assert.Equal(HttpStatusCode.Unauthorized, (await Send(HttpMethod.Get, "/api/auth/me", initialCookie)).StatusCode);
    }

    [Fact]
    public async Task ChangeInitialPasswordWithWrongCurrentPasswordReturns400()
    {
        var email = _factory.AddUser(UserRole.Student, "22520001", mustChange: true, code: "22520001");
        var cookie = await LoginCookie(email, "22520001");

        var response = await Send(HttpMethod.Post, "/api/auth/change-initial-password", cookie,
            new { currentPassword = "sai12345", newPassword = "matkhau123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False((await ReadJson(response)).GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task DeactivatedAccountTokenIsRejected()
    {
        var email = _factory.AddUser(UserRole.Lecturer, "matkhau123", mustChange: false);
        var cookie = await LoginCookie(email, "matkhau123");
        _factory.Users.Find(email).IsActive = false;

        Assert.Equal(HttpStatusCode.Unauthorized, (await Send(HttpMethod.Get, "/api/test-rbac/lecturer", cookie)).StatusCode);
    }

    [Fact]
    public async Task LogoutExpiresCookie()
    {
        var response = await Client().PostAsync("/api/auth/logout", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"), c => c.StartsWith($"{CookieName}="));
        Assert.Contains("expires=Thu, 01 Jan 1970", setCookie, StringComparison.OrdinalIgnoreCase);
    }

    private HttpClient Client() => _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

    private async Task<string> LoginCookie(string email, string password)
    {
        var response = await Client().PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        return CookieValue(response);
    }

    private async Task<HttpResponseMessage> Send(HttpMethod method, string url, string cookie, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Cookie", $"{CookieName}={cookie}");
        if (body is not null) request.Content = JsonContent.Create(body);
        return await Client().SendAsync(request);
    }

    private static string CookieValue(HttpResponseMessage response)
    {
        var header = response.Headers.GetValues("Set-Cookie").Single(c => c.StartsWith($"{CookieName}="));
        return header[(CookieName.Length + 1)..header.IndexOf(';')];
    }

    private static async Task<JsonElement> ReadJson(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

    public sealed class AuthApiFactory : WebApplicationFactory<Program>
    {
        public TestUserStore Users { get; } = new();

        public string AddUser(UserRole role, string password, bool mustChange, string? code = null) =>
            Users.Add(role, password, mustChange, code);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(TestConfiguration.Values));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUserRepository>();
                services.AddSingleton<IUserRepository>(Users.Repository);
                services.AddControllers().AddApplicationPart(typeof(RbacProbeController).Assembly);
            });
        }
    }

    public sealed class TestUserStore
    {
        private int _sequence;
        private readonly Dictionary<string, User> _byEmail = [];
        internal InMemoryUserRepository Repository { get; } = new();

        public string Add(UserRole role, string password, bool mustChange, string? code)
        {
            var n = Interlocked.Increment(ref _sequence);
            var email = $"user{n}.{Guid.NewGuid():N}@truong.edu.vn";
            var user = Repository.Add(code ?? $"U{n:D5}", email, password, role, mustChange, p => BCrypt.Net.BCrypt.HashPassword(p, 4));
            lock (_byEmail) _byEmail[email] = user;
            return email;
        }

        public User Find(string email)
        {
            lock (_byEmail) return _byEmail[email];
        }
    }
}

internal static class TestConfiguration
{
    public static readonly Dictionary<string, string?> Values = new()
    {
        ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=se310_test;Username=test;Password=test",
        ["InitializeDatabase"] = "false",
        ["SeedSampleProducts"] = "false",
        ["Authentication:Jwt:SigningKey"] = "test-signing-key-that-is-at-least-32-bytes-long",
        ["Authentication:Jwt:Issuer"] = "se310-api",
        ["Authentication:Jwt:Audience"] = "se310-web"
    };
}

[ApiController]
[Route("api/test-rbac")]
public sealed class RbacProbeController : ControllerBase
{
    [HttpGet("student"), Authorize(Roles = AppRoles.Student)]
    public IActionResult Student() => Ok();

    [HttpGet("lecturer"), Authorize(Roles = AppRoles.Lecturer)]
    public IActionResult Lecturer() => Ok();

    [HttpGet("academic-affairs"), Authorize(Roles = AppRoles.AcademicAffairs)]
    public IActionResult AcademicAffairs() => Ok();

    [HttpGet("staff"), Authorize(Roles = $"{AppRoles.Lecturer},{AppRoles.AcademicAffairs}")]
    public IActionResult Staff() => Ok();

    [HttpGet("any"), Authorize]
    public IActionResult Any() => Ok();
}
