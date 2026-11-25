using System;
using System.Collections.Generic;
using System.Linq;

namespace ItServiceSequenceModule
{
    public class Engineer
    {
        public int EngineerId { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Engineer(int engineerId, string name, string specialization)
        {
            EngineerId = engineerId;
            Name = name;
            Specialization = specialization;
        }
    }

    public class ServiceTicket
    {
        public int TicketId { get; set; }
        public string ClientName { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public Engineer? AssignedEngineer { get; set; }
        public string Status { get; set; } = "NEW";
        public string Report { get; set; } = string.Empty;

        public ServiceTicket(int ticketId, string clientName, string serviceType, string description)
        {
            TicketId = ticketId;
            ClientName = clientName;
            ServiceType = serviceType;
            Description = description;
        }
    }

    public class EngineerPool
    {
        private readonly List<Engineer> _engineers;

        public EngineerPool(List<Engineer> engineers)
        {
            _engineers = engineers;
        }

        public Engineer? FindAvailableEngineer(string serviceType)
        {
            var engineer = _engineers.FirstOrDefault(e => e.IsAvailable && e.Specialization == serviceType);
            if (engineer != null)
            {
                engineer.IsAvailable = false;
            }
            return engineer;
        }
    }

    public class TicketManager
    {
        private readonly EngineerPool _pool;
        private readonly Dictionary<int, ServiceTicket> _tickets = new Dictionary<int, ServiceTicket>();
        private int _nextId = 1;

        public TicketManager(EngineerPool pool)
        {
            _pool = pool;
        }

        public ServiceTicket CreateTicket(string clientName, string serviceType, string description)
        {
            var ticket = new ServiceTicket(_nextId++, clientName, serviceType, description);
            var engineer = _pool.FindAvailableEngineer(serviceType);

            if (engineer != null)
            {
                ticket.AssignedEngineer = engineer;
                ticket.Status = "IN_PROGRESS";
            }
            else
            {
                ticket.Status = "WAITING_FOR_ENGINEER";
            }

            _tickets[ticket.TicketId] = ticket;
            return ticket;
        }

        public ServiceTicket CompleteWork(int ticketId, string workReport)
        {
            var ticket = _tickets[ticketId];
            ticket.Status = "COMPLETED";
            ticket.Report = workReport;

            if (ticket.AssignedEngineer != null)
            {
                ticket.AssignedEngineer.IsAvailable = true;
            }

            return ticket;
        }
    }

    public class ServicePortal
    {
        private readonly TicketManager _manager;

        public ServicePortal(TicketManager manager)
        {
            _manager = manager;
        }

        public int SubmitRequest(string clientName, string serviceType, string description)
        {
            var ticket = _manager.CreateTicket(clientName, serviceType, description);
            string engName = ticket.AssignedEngineer != null ? ticket.AssignedEngineer.Name : "не призначено";
            Console.WriteLine($"[Портал] Заявка #{ticket.TicketId} ({ticket.Status}). Інженер: {engName}");
            return ticket.TicketId;
        }

        public void CloseRequest(int ticketId, string report)
        {
            var ticket = _manager.CompleteWork(ticketId, report);
            Console.WriteLine($"[Портал] Заявка #{ticket.TicketId} закрита. Звіт: {ticket.Report}");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var pool = new EngineerPool(new List<Engineer>
            {
                new Engineer(1, "Олександр Коваль", "Network"),
                new Engineer(2, "Ірина Мельник", "Cloud")
            });

            var manager = new TicketManager(pool);
            var portal = new ServicePortal(manager);

            int ticketId = portal.SubmitRequest("ТОВ 'Альфа'", "Network", "Налаштування VPN для офісу");
            portal.CloseRequest(ticketId, "VPN-тунель налаштовано та протестовано.");
        }
    }
}