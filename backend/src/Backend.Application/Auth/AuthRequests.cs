using Backend.Application.Common;
using Backend.Application.Common.Security;
using Backend.Application.DTOs.Auth;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Backend.Application.Auth;

public record LoginCommand(LoginDto Credentials) : IRequest<ApiResponse<AuthenticatedSession>>;
public record ChangeInitialPasswordCommand(Guid UserId, ChangeInitialPasswordDto Passwords) : IRequest<ApiResponse<AuthenticatedSession>>;
public record GetCurrentUserQuery(Guid UserId) : IRequest<ApiResponse<CurrentUserDto>>;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Credentials.Email).NotEmpty().WithMessage("Nhập email để đăng nhập.").MaximumLength(256);
        RuleFor(x => x.Credentials.Password).NotEmpty().WithMessage("Nhập mật khẩu để đăng nhập.").MaximumLength(128);
    }
}

public sealed class ChangeInitialPasswordValidator : AbstractValidator<ChangeInitialPasswordCommand>
{
    public ChangeInitialPasswordValidator()
    {
        RuleFor(x => x.Passwords.CurrentPassword).NotEmpty().WithMessage("Nhập mật khẩu hiện tại.");
        RuleFor(x => x.Passwords.NewPassword)
            .MinimumLength(8).WithMessage("Mật khẩu mới tối thiểu 8 ký tự.")
            .MaximumLength(64).WithMessage("Mật khẩu mới tối đa 64 ký tự.")
            .Matches("[a-zA-Z]").WithMessage("Mật khẩu mới phải có chữ.")
            .Matches("[0-9]").WithMessage("Mật khẩu mới phải có số.")
            .NotEqual(x => x.Passwords.CurrentPassword).WithMessage("Mật khẩu mới phải khác mật khẩu hiện tại.");
    }
}

internal static class UserMapping
{
    public static CurrentUserDto ToCurrentUserDto(this User user) => new()
    {
        Id = user.Id,
        UserCode = user.UserCode,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.ToString(),
        MustChangePassword = user.MustChangePassword
    };
}

public sealed class LoginHandler(IUserRepository users, IPasswordHasher hasher, IAccessTokenIssuer tokens)
    : IRequestHandler<LoginCommand, ApiResponse<AuthenticatedSession>>
{
    // Thiết kế màn Đăng nhập: một dòng lỗi chung, không nói ô nào sai.
    public const string InvalidCredentialsMessage = "Email hoặc mật khẩu không đúng. Kiểm tra lại hoặc dùng Quên mật khẩu.";

    private static string? s_dummyHash;

    public async Task<ApiResponse<AuthenticatedSession>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(User.NormalizeEmail(request.Credentials.Email), cancellationToken);
        if (user is null)
        {
            // Vẫn băm để thời gian phản hồi không tiết lộ email có tồn tại hay không.
            s_dummyHash ??= hasher.Hash(Guid.NewGuid().ToString());
            hasher.Verify(request.Credentials.Password, s_dummyHash);
            return ApiResponse<AuthenticatedSession>.Fail(InvalidCredentialsMessage);
        }

        if (!hasher.Verify(request.Credentials.Password, user.PasswordHash) || !user.IsActive)
            return ApiResponse<AuthenticatedSession>.Fail(InvalidCredentialsMessage);

        user.LastLoginAt = DateTime.UtcNow;
        await users.UpdateAsync(user, cancellationToken);

        var message = user.MustChangePassword ? "Cần đổi mật khẩu trước khi vào hệ thống" : "Đăng nhập thành công";
        return ApiResponse<AuthenticatedSession>.Ok(new AuthenticatedSession(user.ToCurrentUserDto(), tokens.Issue(user)), message);
    }
}

public sealed class ChangeInitialPasswordHandler(IUserRepository users, IPasswordHasher hasher, IAccessTokenIssuer tokens)
    : IRequestHandler<ChangeInitialPasswordCommand, ApiResponse<AuthenticatedSession>>
{
    public async Task<ApiResponse<AuthenticatedSession>> Handle(ChangeInitialPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return ApiResponse<AuthenticatedSession>.Fail("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa");
        if (!user.MustChangePassword)
            return ApiResponse<AuthenticatedSession>.Fail("Tài khoản không cần đổi mật khẩu lần đầu");
        if (!hasher.Verify(request.Passwords.CurrentPassword, user.PasswordHash))
            return ApiResponse<AuthenticatedSession>.Fail("Mật khẩu hiện tại không đúng");
        if (request.Passwords.NewPassword == user.UserCode)
            return ApiResponse<AuthenticatedSession>.Fail("Mật khẩu mới phải khác mật khẩu ban đầu (mã người dùng)");

        user.PasswordHash = hasher.Hash(request.Passwords.NewPassword);
        user.MustChangePassword = false;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.UpdatedAt = DateTime.UtcNow;
        await users.UpdateAsync(user, cancellationToken);

        return ApiResponse<AuthenticatedSession>.Ok(new AuthenticatedSession(user.ToCurrentUserDto(), tokens.Issue(user)), "Đã đổi mật khẩu");
    }
}

public sealed class GetCurrentUserHandler(IUserRepository users)
    : IRequestHandler<GetCurrentUserQuery, ApiResponse<CurrentUserDto>>
{
    public async Task<ApiResponse<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken);
        return user is { IsActive: true }
            ? ApiResponse<CurrentUserDto>.Ok(user.ToCurrentUserDto())
            : ApiResponse<CurrentUserDto>.Fail("Tài khoản không tồn tại hoặc đã bị vô hiệu hóa");
    }
}
