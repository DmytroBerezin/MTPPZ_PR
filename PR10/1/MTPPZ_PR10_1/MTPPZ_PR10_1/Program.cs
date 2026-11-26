using System;

namespace Task1_SoC
{
    public interface IOrderRepository
    {
        void Save(string item);
    }

    public class OrderRepository : IOrderRepository
    {
        public void Save(string item)
        {
            Console.WriteLine($"[DB] Saving order '{item}' to database...");
        }
    }

    public interface INotificationService
    {
        void SendConfirmation(string item);
    }

    public class EmailNotificationService : INotificationService
    {
        public void SendConfirmation(string item)
        {
            Console.WriteLine($"[Email] Sending confirmation for '{item}'...");
        }
    }


    public class OrderProcessor
    {
        private readonly IOrderRepository _repository;
        private readonly INotificationService _notificationService;

        public OrderProcessor(IOrderRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public void ProcessOrder(string item)
        {
            _repository.Save(item);
            _notificationService.SendConfirmation(item);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var processor = new OrderProcessor(new OrderRepository(), new EmailNotificationService());
            processor.ProcessOrder("Ноутбук ASUS ROG");
        }
    }
}