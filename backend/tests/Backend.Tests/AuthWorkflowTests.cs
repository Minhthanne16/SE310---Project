using Backend.Application;
using Backend.Application.Auth;
using Backend.Application.Common.Security;
using Backend.Application.DTOs.Auth;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Backend.Tests;

public sealed class AuthWorkflowTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly ISender _sender;

    public AuthWorkflowTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplicationServices();
        services.AddSingleton<IUserRepository>(_users);
        services.AddSingleton<IPasswordHasher, FakePasswordHasher>();
        services.AddSingleton<IAccessTokenIssuer, FakeTokenIssuer>();
        _sender = services.BuildServiceProvider().GetRequiredService<ISender>();
    }

    [Fact]
    public async Task LoginSucceedsWithCaseInsensitiveEmailAndReturnsUser()
    {
        var user = _users.Add("22520001", "an.nv@truong.edu.vn", "matkhau123", UserRole.Student, mustChange: false);

        var result = await _sender.Send(new LoginCommand(new LoginDto { Email = "  AN.NV@truong.edu.vn ", Password = "matkhau123" }));

        Assert.True(result.Success);
        Assert.Equal(user.Id, result.Data!.User.Id);
        Assert.Equal(AppRoles.Student, result.Data.User.Role);
        Assert.False(result.Data.User.MustChangePassword);
        Assert.Equal($"token-for-{user.Id}", result.Data.AccessToken.Value);
        Assert.NotNull(user.LastLoginAt);
    }

    [Fact]
    public async Task LoginReportsMustChangePasswordForInitialPassword()
    {
        _users.Add("22520001", "an.nv@truong.edu.vn", "22520001", UserRole.Student, mustChange: true);

        var result = await _sender.Send(new LoginCommand(new LoginDto { Email = "an.nv@truong.edu.vn", Password = "22520001" }));

        Assert.True(result.Success);
        Assert.True(result.Data!.User.MustChangePassword);
    }

    [Theory]
    [InlineData("an.nv@truong.edu.vn", "sai-mat-khau1")]
    [InlineData("khong.ton.tai@truong.edu.vn", "matkhau123")]
    public async Task LoginFailsWithOneGenericMessage(string email, string password)
    {
        _users.Add("22520001", "an.nv@truong.edu.vn", "matkhau123", UserRole.Student, mustChange: false);

        var result = await _sender.Send(new LoginCommand(new LoginDto { Email = email, Password = password }));

        Assert.False(result.Success);
        Assert.Equal(LoginHandler.InvalidCredentialsMessage, result.Message);
    }

    [Fact]
    public async Task LoginRejectsDeactivatedAccount()
    {
        var user = _users.Add("GV001", "gv@truong.edu.vn", "matkhau123", UserRole.Lecturer, mustChange: false);
        user.IsActive = false;

        var result = await _sender.Send(new LoginCommand(new LoginDto { Email = "gv@truong.edu.vn", Password = "matkhau123" }));

        Assert.False(result.Success);
        Assert.Equal(LoginHandler.InvalidCredentialsMessage, result.Message);
    }

    [Fact]
    public async Task LoginRequiresEmailAndPassword()
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sender.Send(new LoginCommand(new LoginDto { Email = "", Password = "" })));
    }

    [Fact]
    public async Task ChangeInitialPasswordClearsFlagAndRotatesSecurityStamp()
    {
        var user = _users.Add("22520001", "an.nv@truong.edu.vn", "22520001", UserRole.Student, mustChange: true);
        var oldStamp = user.SecurityStamp;

        var result = await _sender.Send(new ChangeInitialPasswordCommand(user.Id,
            new ChangeInitialPasswordDto { CurrentPassword = "22520001", NewPassword = "matkhau123" }));

        Assert.True(result.Success);
        Assert.False(result.Data!.User.MustChangePassword);
        Assert.False(user.MustChangePassword);
        Assert.NotEqual(oldStamp, user.SecurityStamp);
        Assert.Equal("hash:matkhau123", user.PasswordHash);
    }

    [Fact]
    public async Task ChangeInitialPasswordRejectsWrongCurrentPassword()
    {
        var user = _users.Add("22520001", "an.nv@truong.edu.vn", "22520001", UserRole.Student, mustChange: true);

        var result = await _sender.Send(new ChangeInitialPasswordCommand(user.Id,
            new ChangeInitialPasswordDto { CurrentPassword = "sai", NewPassword = "matkhau123" }));

        Assert.False(result.Success);
        Assert.True(user.MustChangePassword);
    }

    [Fact]
    public async Task ChangeInitialPasswordRejectsUserCodeAsNewPassword()
    {
        var user = _users.Add("sv22520001", "an.nv@truong.edu.vn", "tam12345", UserRole.Student, mustChange: true);

        var result = await _sender.Send(new ChangeInitialPasswordCommand(user.Id,
            new ChangeInitialPasswordDto { CurrentPassword = "tam12345", NewPassword = "sv22520001" }));

        Assert.False(result.Success);
        Assert.True(user.MustChangePassword);
    }

    [Fact]
    public async Task ChangeInitialPasswordIsOnlyForAccountsThatMustChange()
    {
        var user = _users.Add("22520001", "an.nv@truong.edu.vn", "matkhau123", UserRole.Student, mustChange: false);

        var result = await _sender.Send(new ChangeInitialPasswordCommand(user.Id,
            new ChangeInitialPasswordDto { CurrentPassword = "matkhau123", NewPassword = "matkhau456" }));

        Assert.False(result.Success);
    }

    [Theory]
    [InlineData("abc1234")]   // ngắn hơn 8 ký tự
    [InlineData("abcdefgh")]  // không có số
    [InlineData("12345678")]  // không có chữ
    [InlineData("22520001")]  // trùng mật khẩu hiện tại
    public async Task ChangeInitialPasswordValidatesPasswordRules(string newPassword)
    {
        await Assert.ThrowsAsync<ValidationException>(() => _sender.Send(new ChangeInitialPasswordCommand(Guid.NewGuid(),
            new ChangeInitialPasswordDto { CurrentPassword = "22520001", NewPassword = newPassword })));
    }

    [Fact]
    public async Task GetCurrentUserFailsForDeactivatedAccount()
    {
        var user = _users.Add("22520001", "an.nv@truong.edu.vn", "matkhau123", UserRole.Student, mustChange: false);
        user.IsActive = false;

        var result = await _sender.Send(new GetCurrentUserQuery(user.Id));

        Assert.False(result.Success);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hash:{password}";
        public bool Verify(string password, string hash) => hash == $"hash:{password}";
    }

    private sealed class FakeTokenIssuer : IAccessTokenIssuer
    {
        public IssuedAccessToken Issue(User user) => new($"token-for-{user.Id}", DateTime.UtcNow.AddHours(1));
    }
}

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<Guid, User> _users = [];

    public User Add(string code, string email, string password, UserRole role, bool mustChange, Func<string, string>? hash = null)
    {
        var user = new User
        {
            UserCode = code,
            FullName = $"Người dùng {code}",
            Email = User.NormalizeEmail(email),
            PasswordHash = (hash ?? (p => $"hash:{p}"))(password),
            Role = role,
            MustChangePassword = mustChange
        };
        _users.Add(user.Id, user);
        return user;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.GetValueOrDefault(id));

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.Values.FirstOrDefault(u => u.Email == normalizedEmail));

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
