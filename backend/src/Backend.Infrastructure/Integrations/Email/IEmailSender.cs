namespace Backend.Infrastructure.Integrations.Email;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string text, CancellationToken cancellationToken = default);
}
