namespace BookManagement.Services
{
    /// <summary>
    /// FAKE service for sending emails asynchronously.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// FakeSends email asynchronously.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message content of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendEmailAsync(string email, string subject, string message);
    }
    /// <summary>
    /// implement FAKE <see cref="IEmailService"/> interface that uses SMTP for sending emails.
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        /// <summary>
        /// FAKESends email asynchronously
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message content of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            Console.WriteLine($"Sending email to: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
            await Task.CompletedTask;
        }
    }
}
