using System;
using System.Collections.Generic;

namespace RestaurantBehavioralPatternsModule
{

    public interface IRestaurantMediator
    {
        void Notify(object sender, string eventCode, string details);
    }

    public abstract class RestaurantColleague
    {
        protected readonly IRestaurantMediator _mediator;

        protected RestaurantColleague(IRestaurantMediator mediator)
        {
            _mediator = mediator;
        }
    }

    public class WaiterColleague : RestaurantColleague
    {
        public WaiterColleague(IRestaurantMediator mediator) : base(mediator) { }

        public void CreateNewOrder(int tableNumber, string dishName)
        {
            string info = $"Столик №{tableNumber}: {dishName}";
            Console.WriteLine($"[Офіціант] Прийняв замовлення ({info}).");
            _mediator.Notify(this, "OrderCreated", info);
        }

        public void DeliverToTable(string info)
        {
            Console.WriteLine($"[Офіціант] Страву подано гостям ({info}).");
        }
    }

    public class KitchenColleague : RestaurantColleague
    {
        public KitchenColleague(IRestaurantMediator mediator) : base(mediator) { }

        public void PrepareOrder(string info)
        {
            Console.WriteLine($"[Кухня] Приготовано страву ({info}).");
            _mediator.Notify(this, "DishReady", info);
        }
    }

    public class CashierColleague : RestaurantColleague
    {
        public CashierColleague(IRestaurantMediator mediator) : base(mediator) { }

        public void RegisterOrderInBill(string info)
        {
            Console.WriteLine($"[Каса] Додано позицію до рахунку ({info}).");
        }
    }

    public class RestaurantMediator : IRestaurantMediator
    {
        public WaiterColleague? Waiter { get; set; }
        public KitchenColleague? Kitchen { get; set; }
        public CashierColleague? Cashier { get; set; }

        public void Notify(object sender, string eventCode, string details)
        {
            if (eventCode == "OrderCreated")
            {
                Kitchen?.PrepareOrder(details);
                Cashier?.RegisterOrderInBill(details);
            }
            else if (eventCode == "DishReady")
            {
                Waiter?.DeliverToTable(details);
            }
        }
    }


    public interface IMenuIterator
    {
        bool HasNext();
        string Next();
    }

    public class MenuIterator : IMenuIterator
    {
        private readonly List<string> _items;
        private int _index = 0;

        public MenuIterator(List<string> items)
        {
            _items = items;
        }

        public bool HasNext() => _index < _items.Count;

        public string Next() => _items[_index++];
    }

    public class RestaurantMenuCollection
    {
        private readonly List<string> _dishes = new();

        public void AddDish(string dish) => _dishes.Add(dish);

        public IMenuIterator CreateIterator() => new MenuIterator(_dishes);
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== 1. ПЕРЕГЛЯД МЕНЮ РЕСТОРАНУ (ITERATOR) ===");
            var menu = new RestaurantMenuCollection();
            menu.AddDish("Борщ український з пампушками — 165 грн");
            menu.AddDish("Стейк Рібай — 540 грн");
            menu.AddDish("Тірамісу класичний — 140 грн");

            IMenuIterator iterator = menu.CreateIterator();
            int number = 1;
            while (iterator.HasNext())
            {
                Console.WriteLine($"{number++}. {iterator.Next()}");
            }

            Console.WriteLine("\n=== 2. ВЗАЄМОДІЯ ПЕРСОНАЛУ РЕСТОРАНУ (MEDIATOR) ===");
            var mediator = new RestaurantMediator();
            var waiter = new WaiterColleague(mediator);
            var kitchen = new KitchenColleague(mediator);
            var cashier = new CashierColleague(mediator);

            mediator.Waiter = waiter;
            mediator.Kitchen = kitchen;
            mediator.Cashier = cashier;

            waiter.CreateNewOrder(7, "Стейк Рібай");
        }
    }
}