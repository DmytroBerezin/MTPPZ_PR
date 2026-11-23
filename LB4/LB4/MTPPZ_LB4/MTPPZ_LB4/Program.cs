using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StoreAutomation
{
    public class Product
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsAdultOnly { get; set; }

        public Product(string barcode, string name, decimal price, int stock, bool isAdultOnly = false)
        {
            Barcode = barcode;
            Name = name;
            Price = price;
            Stock = stock;
            IsAdultOnly = isAdultOnly;
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }

    public class StoreSystem
    {
        private readonly Dictionary<string, Product> _products;
        private readonly Dictionary<string, User> _users;
        private User _currentUser;
        private const string LogFile = "shop_system.log";

        public StoreSystem()
        {
            // Імітація бази даних товарів
            _products = new Dictionary<string, Product>
            {
                { "101", new Product("101", "Хліб Український", 28.50m, 15, false) },
                { "102", new Product("102", "Молоко 2.5%", 42.00m, 10, false) },
                { "103", new Product("103", "Напій енергетичний", 55.00m, 5, true) },
                { "104", new Product("104", "Шоколад чорний", 64.00m, 0, false) }
            };

            // Користувачі системи
            _users = new Dictionary<string, User>
            {
                { "ivan", new User("ivan", "1234", "cashier") },
                { "admin", new User("admin", "admin", "admin") }
            };
        }

        // Підтримуваність: логування подій у файл
        private void Log(string level, string message)
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - [{level}] - {message}";
            File.AppendAllText(LogFile, logEntry + Environment.NewLine, Encoding.UTF8);
        }

        // Безпека: авторизація
        public bool Login(string username, string password)
        {
            if (_users.TryGetValue(username, out User user) && user.Password == password)
            {
                _currentUser = user;
                Log("INFO", $"Користувач {username} ({user.Role}) увійшов у систему.");
                return true;
            }
            Log("WARNING", $"Невдала спроба входу для користувача: {username}");
            return false;
        }

        // Продуктивність: пошук за штрих-кодом із заміром часу (< 1.0 сек)
        public Product FindProduct(string barcode)
        {
            Stopwatch sw = Stopwatch.StartNew();
            _products.TryGetValue(barcode, out Product product);
            sw.Stop();

            Console.WriteLine($"[Метрика продуктивності]: Пошук виконано за {sw.Elapsed.TotalSeconds:F6} сек (ліміт < 1.0 сек).");
            return product;
        }

        public void CreateReceipt()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("Помилка доступу! Спочатку авторизуйтесь.");
                return;
            }

            List<Product> cart = new List<Product>();
            decimal totalSum = 0m;

            Console.WriteLine("\n--- Новий чек (введіть '0' для завершення, 'list' для списку товарів) ---");

            while (true)
            {
                Console.Write("Введіть штрих-код товару: ");
                string code = Console.ReadLine()?.Trim();

                if (code == "0") break;

                if (code == "list")
                {
                    foreach (var p in _products.Values)
                    {
                        Console.WriteLine($"  Код: {p.Barcode} | {p.Name} | {p.Price:F2} грн | Залишок: {p.Stock} | 18+: {(p.IsAdultOnly ? "Так" : "Ні")}");
                    }
                    continue;
                }

                Product product = FindProduct(code);
                if (product == null)
                {
                    Console.WriteLine("Помилка: Товар із таким штрих-кодом не знайдено.");
                    Log("ERROR", $"Спроба сканування неіснуючого коду: {code}");
                    continue;
                }

                // Бізнес-правило 1: перевірка залишку на складі
                if (product.Stock <= 0)
                {
                    Console.WriteLine($"Відмова: Товару '{product.Name}' немає в наявності на складі!");
                    Log("WARNING", $"Спроба продажу відсутнього товару: {product.Name}");
                    continue;
                }

                // Бізнес-правило 2: перевірка віку 18+
                if (product.IsAdultOnly)
                {
                    Console.Write($"УВАГА! '{product.Name}' має обмеження 18+. Покупцю є 18 років? (y/n або 1/0): ");
                    string confirm = Console.ReadLine()?.Trim().ToLower();
                    if (confirm != "y" && confirm != "yes" && confirm != "1" && confirm != "так")
                    {
                        Console.WriteLine("Продаж заборонено згідно з бізнес-правилами магазину!");
                        Log("INFO", $"Блокування продажу товару 18+ ({product.Name}) касиром {_currentUser.Username}");
                        continue;
                    }
                }

                product.Stock--;
                cart.Add(product);
                totalSum += product.Price;
                Console.WriteLine($"Додано: {product.Name} — {product.Price:F2} грн");
            }

            if (cart.Count > 0)
            {
                Console.WriteLine("\n================ ФІСКАЛЬНИЙ ЧЕК ================");
                Console.WriteLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Касир: {_currentUser.Username}");
                foreach (var item in cart)
                {
                    Console.WriteLine($"- {item.Name,-25} {item.Price,8:F2} грн");
                }
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($"РАЗОМ ДО СПЛАТИ: {totalSum:F2} грн");
                Console.WriteLine("================================================\n");
                Log("INFO", $"Сформовано чек на суму {totalSum:F2} грн. Позицій: {cart.Count}");
            }
            else
            {
                Console.WriteLine("Чек скасовано (порожній).");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            StoreSystem system = new StoreSystem();
            Console.WriteLine("=== СИСТЕМА АВТОМАТИЗАЦІЇ МАГАЗИНУ ===");
            Console.WriteLine("Тестові дані для входу -> Логін: ivan | Пароль: 1234");

            while (true)
            {
                Console.Write("Логін: ");
                string u = Console.ReadLine();
                Console.Write("Пароль: ");
                string p = Console.ReadLine();

                if (system.Login(u, p))
                {
                    Console.WriteLine($"Вітаємо, {u}!\n");
                    break;
                }
                Console.WriteLine("Невірний логін або пароль. Спробуйте ще раз.\n");
            }

            system.CreateReceipt();
            Console.WriteLine("Натисніть Enter для виходу...");
            Console.ReadLine();
        }
    }
}