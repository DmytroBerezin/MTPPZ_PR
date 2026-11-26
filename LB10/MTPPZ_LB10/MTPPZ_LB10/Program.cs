using System;

namespace SolidNotificationRefactoring
{

    public interface INotificationSender
    {
        void Send(string message);
    }

    public interface INotificationRepository
    {
        void Save(string message);
    }

    public interface ILogger
    {
        void Log(string message);
    }


    public class DatabaseNotificationRepository : INotificationRepository
    {
        public void Save(string message)
        {
            Console.WriteLine($"[DB] Збережено в базу даних: {message}");
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss} — Відправлено сповіщення: {message}");
        }
    }


    public class EmailNotificationSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[EMAIL] Відправка листа: {message}");
        }
    }

    public class SmsNotificationSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[SMS] Відправка SMS: {message}");
        }
    }

    public class PushNotificationSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[PUSH] Відправка Push-повідомлення: {message}");
        }
    }

    public class TelegramNotificationSender : INotificationSender
    {
        public void Send(string message)
        {
            Console.WriteLine($"[TELEGRAM] Відправка повідомлення в Telegram: {message}");
        }
    }


    public class NotificationService
    {
        private readonly INotificationSender _sender;
        private readonly INotificationRepository _repository;
        private readonly ILogger _logger;

        public NotificationService(
            INotificationSender sender,
            INotificationRepository repository,
            ILogger logger)
        {
            _sender = sender;
            _repository = repository;
            _logger = logger;
        }

        public void Notify(string message)
        {
            _sender.Send(message);
            _repository.Save(message);
            _logger.Log(message);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            INotificationRepository repository = new DatabaseNotificationRepository();
            ILogger logger = new ConsoleLogger();

            INotificationSender[] senders =
            {
                new EmailNotificationSender(),
                new SmsNotificationSender(),
                new PushNotificationSender(),
                new TelegramNotificationSender()
            };

            foreach (var sender in senders)
            {
                var service = new NotificationService(sender, repository, logger);
                service.Notify("Ваше замовлення №1042 успішно підтверджено!");
                Console.WriteLine(new string('-', 60));
            }
        }
    }
}