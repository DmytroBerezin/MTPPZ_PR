using System;

namespace Lab11_2_Decorator
{

    public abstract class Subscription
    {
        public abstract string GetDescription();
        public abstract double GetCost();
    }

    public class BasicSubscription : Subscription
    {
        public override string GetDescription() => "Базова підписка (SD-якість, онлайн-перегляд)";
        public override double GetCost() => 149.00;
    }

    public abstract class SubscriptionDecorator : Subscription
    {
        protected readonly Subscription _subscription;

        protected SubscriptionDecorator(Subscription subscription)
        {
            _subscription = subscription;
        }

        public override string GetDescription() => _subscription.GetDescription();
        public override double GetCost() => _subscription.GetCost();
    }

    public class HdDecorator : SubscriptionDecorator
    {
        public HdDecorator(Subscription subscription) : base(subscription) { }

        public override string GetDescription() => $"{_subscription.GetDescription()} + HD/4K якість";
        public override double GetCost() => _subscription.GetCost() + 60.00;
    }

    public class OfflineDecorator : SubscriptionDecorator
    {
        public OfflineDecorator(Subscription subscription) : base(subscription) { }

        public override string GetDescription() => $"{_subscription.GetDescription()} + Завантаження офлайн";
        public override double GetCost() => _subscription.GetCost() + 45.00;
    }

    public class AdFreeDecorator : SubscriptionDecorator
    {
        public AdFreeDecorator(Subscription subscription) : base(subscription) { }

        public override string GetDescription() => $"{_subscription.GetDescription()} + Без реклами (Ad-Free)";
        public override double GetCost() => _subscription.GetCost() + 50.00;
    }

    class Program
    {
        static void PrintSubscriptionInfo(string planTitle, Subscription sub)
        {
            Console.WriteLine($"=== {planTitle} ===");
            Console.WriteLine($"Опис: {sub.GetDescription()}");
            Console.WriteLine($"Вартість: {sub.GetCost():F2} грн/міс\n");
        }

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Subscription basic = new BasicSubscription();
            PrintSubscriptionInfo("Тариф 'Старт'", basic);

            Subscription hdPlan = new HdDecorator(new BasicSubscription());
            PrintSubscriptionInfo("Тариф 'Кіноман HD'", hdPlan);

            Subscription fullPlan = new AdFreeDecorator(
                new OfflineDecorator(
                    new HdDecorator(
                        new BasicSubscription())));
            PrintSubscriptionInfo("Тариф 'Преміум Все включено'", fullPlan);
        }
    }
}