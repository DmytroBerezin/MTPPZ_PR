using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopClassDiagramModule
{
    // Абстрактний клас (базовий для учасників процесу)
    public abstract class Person
    {
        protected int Id { get; }
        public string FullName { get; }

        protected Person(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }

        public abstract string GetRoleInfo();
    }

    // Успадкування: Покупець
    public class Customer : Person
    {
        private int _bonusPoints;
        public int BonusPoints => _bonusPoints;

        public Customer(int id, string fullName, int initialPoints = 0)
            : base(id, fullName)
        {
            _bonusPoints = initialPoints;
        }

        public void AddBonus(decimal purchaseAmount)
        {
            _bonusPoints += (int)(purchaseAmount * 0.05m); // 5% бонусів
        }

        public override string GetRoleInfo() => $"Покупець: {FullName} (Бонуси: {_bonusPoints} балів)";
    }

    // Успадкування: Касир
    public class Cashier : Person
    {
        public int TerminalNumber { get; }

        public Cashier(int id, string fullName, int terminalNumber)
            : base(id, fullName)
        {
            TerminalNumber = terminalNumber;
        }

        public override string GetRoleInfo() => $"Касир: {FullName} (Каса №{TerminalNumber})";
    }

    // Інтерфейс оплати (поліморфізм)
    public interface IPaymentMethod
    {
        bool Pay(decimal amount);
        string GetPaymentDetails();
    }

    // Реалізація інтерфейсу: Оплата карткою
    public class CardPayment : IPaymentMethod
    {
        private readonly string _cardNumber;

        public CardPayment(string cardNumber)
        {
            _cardNumber = cardNumber;
        }

        public bool Pay(decimal amount)
        {
            return amount > 0 && _cardNumber.Length >= 4;
        }

        public string GetPaymentDetails()
        {
            string masked = _cardNumber[^4..];
            return $"Безготівковий розрахунок (картка **** {masked})";
        }
    }

    // Реалізація інтерфейсу: Оплата готівкою
    public class CashPayment : IPaymentMethod
    {
        private readonly decimal _cashProvided;
        public decimal Change { get; private set; }

        public CashPayment(decimal cashProvided)
        {
            _cashProvided = cashProvided;
        }

        public bool Pay(decimal amount)
        {
            if (_cashProvided < amount) return false;
            Change = _cashProvided - amount;
            return true;
        }

        public string GetPaymentDetails() => $"Готівка (внесено: {_cashProvided:F2} грн, решта: {Change:F2} грн)";
    }

    // Товар
    public class Product
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Stock { get; private set; }

        public Product(int id, string name, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void DecreaseStock(int qty)
        {
            if (qty > Stock)
                throw new InvalidOperationException($"Недостатньо товару '{Name}' на складі.");
            Stock -= qty;
        }
    }

    // Позиція замовлення (частина композиції)
    public class OrderLine
    {
        public Product Product { get; }
        public int Quantity { get; }

        public OrderLine(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public decimal GetSubtotal() => Product.Price * Quantity;
    }

    // Замовлення (чек)
    public class Order
    {
        private readonly List<OrderLine> _lines = new();
        public int OrderId { get; }
        public Cashier Cashier { get; }
        public Customer? Customer { get; }
        public bool IsPaid { get; private set; }

        public Order(int orderId, Cashier cashier, Customer? customer = null)
        {
            OrderId = orderId;
            Cashier = cashier;
            Customer = customer;
        }

        public void AddProduct(Product product, int qty)
        {
            if (IsPaid)
                throw new InvalidOperationException("Неможливо змінити вже оплачений чек.");

            product.DecreaseStock(qty);
            _lines.Add(new OrderLine(product, qty));
        }

        public decimal CalculateTotal() => _lines.Sum(l => l.GetSubtotal());

        public void Checkout(IPaymentMethod payment)
        {
            decimal total = CalculateTotal();
            if (!payment.Pay(total))
            {
                Console.WriteLine("Помилка оплати: недостатньо коштів або відхилено транзакцію.");
                return;
            }

            IsPaid = true;
            Customer?.AddBonus(total);

            Console.WriteLine($"================ ЧЕК №{OrderId} ================");
            Console.WriteLine(Cashier.GetRoleInfo());
            if (Customer != null) Console.WriteLine(Customer.GetRoleInfo());
            Console.WriteLine("-----------------------------------------");
            foreach (var line in _lines)
            {
                Console.WriteLine($"{line.Product.Name} x{line.Quantity} = {line.GetSubtotal():F2} грн");
            }
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"РАЗОМ ДО СПЛАТИ: {total:F2} грн");
            Console.WriteLine($"Спосіб оплати: {payment.GetPaymentDetails()}");
            Console.WriteLine("=========================================");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var cashier = new Cashier(1, "Олена Савченко", 3);
            var customer = new Customer(105, "Дмитро Березін", 120);

            var milk = new Product(1, "Молоко 2.5%", 42.50m, 30);
            var bread = new Product(2, "Хліб житній", 28.00m, 25);
            var cheese = new Product(3, "Сир твердий 200г", 115.00m, 15);

            var order = new Order(501, cashier, customer);
            order.AddProduct(milk, 2);
            order.AddProduct(bread, 1);
            order.AddProduct(cheese, 1);

            IPaymentMethod payment = new CashPayment(300.00m);
            order.Checkout(payment);
        }
    }
}