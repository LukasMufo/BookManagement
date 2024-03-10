namespace BookManagement.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    public class SmtpEmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Implement sending email using SmtpClient or any other email service
            // This is a placeholder implementation
            Console.WriteLine($"Sending email to: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
            await Task.CompletedTask;
        }
    }
}
