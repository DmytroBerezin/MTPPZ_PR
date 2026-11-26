using System;
using System.Collections.Generic;

namespace ItCompanySystem.Core.Models
{
    public enum SlaLevel
    {
        Standard,
        Business,
        Enterprise24x7
    }

    public class ServiceContract
    {
        public int ContractId { get; set; }
        public string ClientCompany { get; set; }
        public string ServiceName { get; set; }
        public SlaLevel Sla { get; set; }
        public decimal BaseMonthlyRate { get; set; }

        public ServiceContract(int id, string client, string service, SlaLevel sla, decimal baseRate)
        {
            ContractId = id;
            ClientCompany = client;
            ServiceName = service;
            Sla = sla;
            BaseMonthlyRate = baseRate;
        }
    }
}

namespace ItCompanySystem.Infrastructure.Storage
{
    using ItCompanySystem.Core.Models;

    public class ContractRepository
    {
        private readonly List<ServiceContract> _contracts = new();

        public void Add(ServiceContract contract) => _contracts.Add(contract);

        public IReadOnlyList<ServiceContract> GetAll() => _contracts.AsReadOnly();
    }
}

namespace ItCompanySystem.Services.Billing
{
    using ItCompanySystem.Core.Models;
    using ItCompanySystem.Infrastructure.Storage;

    public class BillingService
    {
        private readonly ContractRepository _repository;

        public BillingService(ContractRepository repository)
        {
            _repository = repository;
        }

        public void RegisterContract(ServiceContract contract)
        {
            _repository.Add(contract);
        }

        public decimal CalculateMonthlyInvoice(ServiceContract contract)
        {
            decimal multiplier = contract.Sla switch
            {
                SlaLevel.Standard => 1.0m,
                SlaLevel.Business => 1.25m,
                SlaLevel.Enterprise24x7 => 1.6m,
                _ => 1.0m
            };

            return contract.BaseMonthlyRate * multiplier;
        }

        public IEnumerable<ServiceContract> GetAllContracts() => _repository.GetAll();
    }
}

namespace ItCompanySystem.App.ClientUI
{
    using ItCompanySystem.Core.Models;
    using ItCompanySystem.Infrastructure.Storage;
    using ItCompanySystem.Services.Billing;

    public class ManagerDashboard
    {
        private readonly BillingService _billingService;

        public ManagerDashboard(BillingService billingService)
        {
            _billingService = billingService;
        }

        public void ShowInvoices()
        {
            Console.WriteLine("=== ЩОМІСЯЧНІ РАХУНКИ ЗА ІТ-ПОСЛУГИ ===");
            foreach (var c in _billingService.GetAllContracts())
            {
                decimal invoice = _billingService.CalculateMonthlyInvoice(c);
                Console.WriteLine($"Договір #{c.ContractId} | Клієнт: {c.ClientCompany} | Послуга: {c.ServiceName} ({c.Sla}) | До сплати: {invoice:F2} грн");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var repo = new ContractRepository();
            var billing = new BillingService(repo);

            billing.RegisterContract(new ServiceContract(1, "ТОВ 'ФінТех'", "Адміністрування серверів", SlaLevel.Enterprise24x7, 15000m));
            billing.RegisterContract(new ServiceContract(2, "ПП 'Логістик'", "Хмарне резервне копіювання", SlaLevel.Business, 6000m));

            var dashboard = new ManagerDashboard(billing);
            dashboard.ShowInvoices();
        }
    }
}