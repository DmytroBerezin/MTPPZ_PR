using System;
using System.Collections.Generic;

namespace StoreAutomation
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public Product(int productId, string name, decimal price, int stockQuantity)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            StockQuantity = stockQuantity;
        }

        public bool ReduceStock(int quantity)
        {
            if (StockQuantity >= quantity)
            {
                StockQuantity -= quantity;
                return true;
            }
            return false;
        }
    }

    public static class PaymentProcessor
    {
        public static bool ProcessPayment(decimal amount, string paymentType)
        {
            Console.WriteLine($"[Платіжний шлюз] Проведення платежу на суму {amount:F2} грн (Тип: {paymentType})...");
            // Імітація успішної авторизації платежу в зовнішній системі
            return true;
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public Dictionary<Product, int> Items { get; private set; } = new Dictionary<Product, int>();
        public bool IsPaid { get; set; } = false;

        public Order(int orderId)
        {
            OrderId = orderId;
        }

        public bool AddItem(Product product, int quantity)
        {
            if (product.StockQuantity >= quantity)
            {
                if (Items.ContainsKey(product))
                    Items[product] += quantity;
                else
                    Items[product] = quantity;

                return true;
            }
            Console.WriteLine($"[Помилка] Недостатньо товару '{product.Name}' на складі!");
            return false;
        }

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Key.Price * item.Value;
            }
            return total;
        }
    }

    public class StoreSystem
    {
        private List<Product> _products = new List<Product>();
        private List<Order> _orders = new List<Order>();

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }

        public bool Checkout(Order order, string paymentType)
        {
            decimal total = order.CalculateTotal();
            Console.WriteLine($"\n--- Розрахунок замовлення №{order.OrderId} ---");
            Console.WriteLine($"Загальна сума до сплати: {total:F2} грн");

            // Перевірка та списування товарів зі складу
            foreach (var item in order.Items)
            {
                if (!item.Key.ReduceStock(item.Value))
                {
                    Console.WriteLine($"[Помилка] Не вдалося завершити замовлення. Товар '{item.Key.Name}' закінчився.");
                    return false;
                }
            }

            // Обробка платежу
            if (PaymentProcessor.ProcessPayment(total, paymentType))
            {
                order.IsPaid = true;
                _orders.Add(order);
                Console.WriteLine("[Успіх] Замовлення успішно оплачено та зафіксовано в системі!");
                PrintReceipt(order);
                return true;
            }
            else
            {
                Console.WriteLine("[Помилка] Помилка під час проведення платежу.");
                return false;
            }
        }

        private void PrintReceipt(Order order)
        {
            Console.WriteLine("\n======== ЧЕК ========");
            foreach (var item in order.Items)
            {
                Console.WriteLine($"{item.Key.Name} x{item.Value} = {item.Key.Price * item.Value:F2} грн");
            }
            Console.WriteLine($"РАЗОМ: {order.CalculateTotal():F2} грн");
            Console.WriteLine("=====================\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            StoreSystem store = new StoreSystem();
            Product p1 = new Product(1, "Ноутбук", 25000.0m, 5);
            Product p2 = new Product(2, "Мишка бездротова", 600.0m, 10);

            store.AddProduct(p1);
            store.AddProduct(p2);

            Order newOrder = new Order(101);
            newOrder.AddItem(p1, 1);
            newOrder.AddItem(p2, 2);

            store.Checkout(newOrder, "Картка");
        }
    }
}