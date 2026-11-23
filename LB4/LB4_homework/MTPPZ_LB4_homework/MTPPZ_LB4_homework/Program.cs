using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ITCompanyAutomation
{
    public class ServiceTicket
    {
        public int TicketId { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public bool IsUrgent { get; set; }
        public string Status { get; set; }
        public double HoursSpent { get; set; }

        public ServiceTicket(int ticketId, string clientName, string description, bool isUrgent = false)
        {
            TicketId = ticketId;
            ClientName = clientName;
            Description = description;
            IsUrgent = isUrgent;
            Status = "Нова";
            HoursSpent = 0.0;
        }
    }

    public class ITCompanySystem
    {
        private const decimal HourlyRate = 800.0m;
        private readonly Dictionary<int, ServiceTicket> _tickets = new Dictionary<int, ServiceTicket>();
        private const string LogFile = "it_service.log";

        private void Log(string message)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            File.AppendAllText(LogFile, entry + Environment.NewLine, Encoding.UTF8);
        }

        public void CreateTicket(int ticketId, string client, string desc, bool urgent)
        {
            var ticket = new ServiceTicket(ticketId, client, desc, urgent);
            _tickets[ticketId] = ticket;
            Log($"Створено заявку #{ticketId} від клієнта '{client}' (Термінова: {urgent})");
            Console.WriteLine($"Заявку #{ticketId} успішно зареєстровано.");
        }

        public void CompleteTicket(int ticketId, double hours)
        {
            if (!_tickets.TryGetValue(ticketId, out ServiceTicket ticket))
            {
                Console.WriteLine("Помилка: Заявку не знайдено.");
                return;
            }

            if (hours <= 0)
            {
                Console.WriteLine("Бізнес-правило: Кількість витрачених годин має бути більшою за 0!");
                return;
            }

            ticket.HoursSpent = hours;
            ticket.Status = "Виконано";
            Log($"Заявку #{ticketId} виконано. Витрачено годин: {hours}");
            Console.WriteLine($"Статус заявки #{ticketId} змінено на 'Виконано'.");
        }

        public void GenerateInvoice(int ticketId)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (!_tickets.TryGetValue(ticketId, out ServiceTicket ticket))
            {
                Console.WriteLine("Заявку не знайдено.");
                return;
            }

            if (ticket.Status != "Виконано")
            {
                Console.WriteLine("Помилка бізнес-правила: Неможливо виставити рахунок за невиконану заявку!");
                return;
            }

            decimal multiplier = ticket.IsUrgent ? 1.5m : 1.0m;
            decimal totalCost = (decimal)ticket.HoursSpent * HourlyRate * multiplier;
            sw.Stop();

            Console.WriteLine($"\n--- РАХУНОК-ФАКТУРА №{ticket.TicketId} ---");
            Console.WriteLine($"Клієнт: {ticket.ClientName}");
            Console.WriteLine($"Послуга: {ticket.Description}");
            Console.WriteLine($"Витрачено часу: {ticket.HoursSpent} год | Коефіцієнт терміновості: {multiplier}x");
            Console.WriteLine($"До сплати: {totalCost:F2} грн");
            Console.WriteLine($"[Час генерації рахунку: {sw.Elapsed.TotalSeconds:F6} сек]\n");

            Log($"Виставлено рахунок за заявкою #{ticketId} на суму {totalCost:F2} грн");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            ITCompanySystem itSys = new ITCompanySystem();
            itSys.CreateTicket(1, "ТОВ 'Вектор'", "Налаштування корпоративного VPN", urgent: true);

            itSys.GenerateInvoice(1);

            itSys.CompleteTicket(1, hours: 3.5);
            itSys.GenerateInvoice(1);

            Console.WriteLine("Натисніть Enter для виходу...");
            Console.ReadLine();
        }
    }
}