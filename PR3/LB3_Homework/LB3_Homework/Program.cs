using System;

namespace ITCompanyAutomation
{
    public class ITService
    {
        public int ServiceId { get; set; }
        public string Title { get; set; }
        public decimal HourlyRate { get; set; }

        public ITService(int serviceId, string title, decimal hourlyRate)
        {
            ServiceId = serviceId;
            Title = title;
            HourlyRate = hourlyRate;
        }
    }

    public class ServiceContract
    {
        public int ContractId { get; set; }
        public string ClientName { get; set; }
        public ITService Service { get; set; }
        public int EstimatedHours { get; set; }
        public string Status { get; set; } = "Created";

        public ServiceContract(int contractId, string clientName, ITService service, int estimatedHours)
        {
            ContractId = contractId;
            ClientName = clientName;
            Service = service;
            EstimatedHours = estimatedHours;
        }

        public decimal CalculateCost()
        {
            return Service.HourlyRate * EstimatedHours;
        }
    }

    public class ITCompanyManager
    {
        public void ExecuteContract(ServiceContract contract)
        {
            Console.WriteLine($"\n--- Обробка контракту №{contract.ContractId} для {contract.ClientName} ---");
            Console.WriteLine($"Послуга: {contract.Service.Title}");
            Console.WriteLine($"Оцінка вартості: {contract.CalculateCost():F2} грн");

            contract.Status = "In Progress";
            Console.WriteLine("Статус: Виконання послуги розпочато...");

            contract.Status = "Completed";
            Console.WriteLine("Статус: Послугу успішно надано, контракт закрито.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ITService service = new ITService(1, "Розробка Web-сайту", 800.0m);
            ServiceContract contract = new ServiceContract(501, "ТОВ 'Вектор'", service, 40);

            ITCompanyManager manager = new ITCompanyManager();
            manager.ExecuteContract(contract);
        }
    }
}