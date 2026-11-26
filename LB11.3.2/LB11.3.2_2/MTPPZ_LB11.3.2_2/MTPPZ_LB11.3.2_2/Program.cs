using System;

namespace ItCompanyStrategyPatternModule
{
    public interface IBillingStrategy
    {
        string PlanTitle { get; }
        decimal CalculateServiceFee(int hoursWorked, int serversCount);
    }

    public class HourlyTimeAndMaterialStrategy : IBillingStrategy
    {
        private readonly decimal _hourlyRate;
        public string PlanTitle => $"Погодинна оплата ({_hourlyRate:F2} грн/год)";

        public HourlyTimeAndMaterialStrategy(decimal hourlyRate = 900m)
        {
            _hourlyRate = hourlyRate;
        }

        public decimal CalculateServiceFee(int hoursWorked, int serversCount)
        {
            return hoursWorked * _hourlyRate;
        }
    }

    public class FixedMonthlyRetainerStrategy : IBillingStrategy
    {
        private readonly decimal _pricePerServer;
        public string PlanTitle => $"Абонентське обслуговування ({_pricePerServer:F2} грн/сервер)";

        public FixedMonthlyRetainerStrategy(decimal pricePerServer = 3500m)
        {
            _pricePerServer = pricePerServer;
        }

        public decimal CalculateServiceFee(int hoursWorked, int serversCount)
        {
            return serversCount * _pricePerServer;
        }
    }

    public class Enterprise24x7SlaStrategy : IBillingStrategy
    {
        private readonly decimal _baseSlaFee;
        private readonly decimal _pricePerServer;
        public string PlanTitle => "Enterprise SLA 24/7 (пріоритетна підтримка)";

        public Enterprise24x7SlaStrategy(decimal baseSlaFee = 12000m, decimal pricePerServer = 4000m)
        {
            _baseSlaFee = baseSlaFee;
            _pricePerServer = pricePerServer;
        }

        public decimal CalculateServiceFee(int hoursWorked, int serversCount)
        {
            int extraHours = Math.Max(0, hoursWorked - 20);
            return _baseSlaFee + (serversCount * _pricePerServer) + (extraHours * 750m);
        }
    }


    public class ItContractBillingContext
    {
        private IBillingStrategy _billingStrategy;
        public string ClientName { get; }

        public ItContractBillingContext(string clientName, IBillingStrategy initialStrategy)
        {
            ClientName = clientName;
            _billingStrategy = initialStrategy;
        }

        public void ChangeBillingPlan(IBillingStrategy newStrategy)
        {
            _billingStrategy = newStrategy;
        }

        public void GenerateInvoice(int hoursWorked, int serversCount)
        {
            decimal total = _billingStrategy.CalculateServiceFee(hoursWorked, serversCount);
            Console.WriteLine($"Клієнт: {ClientName} | Тариф: {_billingStrategy.PlanTitle}");
            Console.WriteLine($"  Параметри місяця: {serversCount} серверів, {hoursWorked} год робіт -> До сплати: {total:F2} грн\n");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var contract = new ItContractBillingContext("ТОВ 'ФінТех Україна'", new HourlyTimeAndMaterialStrategy(950m));
            contract.GenerateInvoice(hoursWorked: 28, serversCount: 6);

            contract.ChangeBillingPlan(new FixedMonthlyRetainerStrategy(3500m));
            contract.GenerateInvoice(hoursWorked: 28, serversCount: 6);

            contract.ChangeBillingPlan(new Enterprise24x7SlaStrategy());
            contract.GenerateInvoice(hoursWorked: 28, serversCount: 6);
        }
    }
}