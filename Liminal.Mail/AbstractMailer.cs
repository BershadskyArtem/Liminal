namespace Liminal.Mail;

public abstract class AbstractMailer
{
    public abstract Task<bool> SendEmailAsync(string email, string content);
}