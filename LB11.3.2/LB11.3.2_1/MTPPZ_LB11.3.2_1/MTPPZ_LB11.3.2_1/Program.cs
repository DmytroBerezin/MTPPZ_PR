using System;

namespace ShopStrategyPatternModule
{

    public interface IDiscountStrategy
    {
        string StrategyName { get; }
        decimal CalculateFinalPrice(decimal rawAmount);
    }

    public class RegularCustomerStrategy : IDiscountStrategy
    {
        public string StrategyName => "Стандартний тариф (без знижки)";

        public decimal CalculateFinalPrice(decimal rawAmount) => rawAmount;
    }

    public class LoyaltyCardStrategy : IDiscountStrategy
    {
        private readonly decimal _discountPercent;
        public string StrategyName => $"Картка постійного клієнта (-{_discountPercent}%)";

        public LoyaltyCardStrategy(decimal discountPercent = 10m)
        {
            _discountPercent = discountPercent;
        }

        public decimal CalculateFinalPrice(decimal rawAmount)
        {
            return rawAmount * (1m - _discountPercent / 100m);
        }
    }

    public class SeasonalPromoStrategy : IDiscountStrategy
    {
        public string StrategyName => "Акція 'Щасливі години' (-500 грн при покупці від 3000 грн)";

        public decimal CalculateFinalPrice(decimal rawAmount)
        {
            return rawAmount >= 3000m ? rawAmount - 500m : rawAmount;
        }
    }

    public class CheckoutContext
    {
        private IDiscountStrategy _strategy;

        public CheckoutContext(IDiscountStrategy initialStrategy)
        {
            _strategy = initialStrategy;
        }

        public void SetStrategy(IDiscountStrategy newStrategy)
        {
            _strategy = newStrategy;
        }

        public void PrintReceipt(int receiptNumber, decimal rawAmount)
        {
            decimal finalAmount = _strategy.CalculateFinalPrice(rawAmount);
            Console.WriteLine($"=== ЧЕК №{receiptNumber} ===");
            Console.WriteLine($"Застосована стратегія: {_strategy.StrategyName}");
            Console.WriteLine($"Початкова сума: {rawAmount:F2} грн");
            Console.WriteLine($"До сплати: {finalAmount:F2} грн (економія: {rawAmount - finalAmount:F2} грн)\n");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            decimal cartTotal = 4200.00m;
            var checkout = new CheckoutContext(new RegularCustomerStrategy());

            checkout.PrintReceipt(101, cartTotal);

            checkout.SetStrategy(new LoyaltyCardStrategy(15m));
            checkout.PrintReceipt(102, cartTotal);

            checkout.SetStrategy(new SeasonalPromoStrategy());
            checkout.PrintReceipt(103, cartTotal);
        }
    }
}