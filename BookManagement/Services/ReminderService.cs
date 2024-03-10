using BookManagement.DbContexts;

namespace BookManagement.Services
{
    public class ReminderService :IHostedService, IDisposable
    {
        private readonly Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        public ReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _timer = new Timer(CheckDueDates, null, TimeSpan.Zero, TimeSpan.FromDays(1)); // Check daily
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void CheckDueDates(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BookLibraryContext>();

                // Get books with due dates one day from now and before
                var dueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
                var booksDueTomorrow = dbContext.BorrowedBooks.Where(
                    b => b.BorrowedUntil <= dueDate).ToList();

                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                foreach (var BorrowEntry in booksDueTomorrow)
                {
                    // Send reminder email to book borrower
                    var user = dbContext.Users.Find(BorrowEntry.UserID);
                    if (user == null)
                        return;

                    var book = dbContext.Books.Find(BorrowEntry.BookID);
                    if (book == null)
                        return;
                    await emailService.SendEmailAsync(user.Email, "Reminder: Return Book", $"Please return '{book.Title}' by {BorrowEntry.BorrowedUntil.ToString("yyyy-MM-dd")}.");
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
