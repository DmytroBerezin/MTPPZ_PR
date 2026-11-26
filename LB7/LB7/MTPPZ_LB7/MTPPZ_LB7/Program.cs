using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopSystem.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public Product(int id, string title, decimal price, int stockQuantity)
        {
            Id = id;
            Title = title;
            Price = price;
            StockQuantity = stockQuantity;
        }
    }

    public class Receipt
    {
        public int ReceiptId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<(Product Product, int Quantity)> Items { get; } = new();
        public decimal TotalBeforeDiscount { get; set; }
        public decimal FinalTotal { get; set; }
    }
}

namespace ShopSystem.DataAccess
{
    using ShopSystem.Domain;

    public interface IProductRepository
    {
        Product? GetById(int id);
        IEnumerable<Product> GetAll();
        void Update(Product product);
    }

    public class InMemoryProductRepository : IProductRepository
    {
        private readonly Dictionary<int, Product> _storage = new()
        {
            { 1, new Product(1, "Ноутбук Lenovo", 28500m, 5) },
            { 2, new Product(2, "Миша бездротова", 850m, 20) },
            { 3, new Product(3, "Клавіатура механічна", 2400m, 10) }
        };

        public Product? GetById(int id) => _storage.TryGetValue(id, out var p) ? p : null;

        public IEnumerable<Product> GetAll() => _storage.Values;

        public void Update(Product product) => _storage[product.Id] = product;
    }
}

namespace ShopSystem.BusinessLogic
{
    using ShopSystem.Domain;
    using ShopSystem.DataAccess;

    public class DiscountCalculator
    {
        public decimal ApplyDiscount(decimal totalAmount)
        {
            // Знижка 10% при покупці від 10 000 грн
            return totalAmount >= 10000m ? totalAmount * 0.90m : totalAmount;
        }
    }

    public class SalesService
    {
        private readonly IProductRepository _repository;
        private readonly DiscountCalculator _discountCalculator;
        private int _receiptCounter = 100;

        public SalesService(IProductRepository repository, DiscountCalculator discountCalculator)
        {
            _repository = repository;
            _discountCalculator = discountCalculator;
        }

        public Receipt ProcessSale(Dictionary<int, int> cart)
        {
            var receipt = new Receipt { ReceiptId = ++_receiptCounter };
            decimal rawTotal = 0m;

            foreach (var (productId, qty) in cart)
            {
                var product = _repository.GetById(productId)
                    ?? throw new ArgumentException($"Товар ID={productId} не знайдено.");

                if (product.StockQuantity < qty)
                    throw new InvalidOperationException($"Недостатньо товару '{product.Title}' на складі.");

                product.StockQuantity -= qty;
                _repository.Update(product);

                receipt.Items.Add((product, qty));
                rawTotal += product.Price * qty;
            }

            receipt.TotalBeforeDiscount = rawTotal;
            receipt.FinalTotal = _discountCalculator.ApplyDiscount(rawTotal);
            return receipt;
        }
    }
}

namespace ShopSystem.Presentation
{
    using ShopSystem.BusinessLogic;
    using ShopSystem.DataAccess;

    public class CashDeskConsole
    {
        private readonly SalesService _salesService;
        private readonly IProductRepository _repository;

        public CashDeskConsole(SalesService salesService, IProductRepository repository)
        {
            _salesService = salesService;
            _repository = repository;
        }

        public void RunDemo()
        {
            Console.WriteLine("=== КАТАЛОГ ТОВАРІВ МАГАЗИНУ ===");
            foreach (var p in _repository.GetAll())
            {
                Console.WriteLine($"ID: {p.Id} | {p.Title} | Ціна: {p.Price} грн | Залишок: {p.StockQuantity} шт.");
            }

            var cart = new Dictionary<int, int>
            {
                { 1, 1 }, // 1 ноутбук
                { 2, 2 }  // 2 миші
            };

            var receipt = _salesService.ProcessSale(cart);

            Console.WriteLine($"\n=== ЧЕК №{receipt.ReceiptId} ({receipt.CreatedAt:g}) ===");
            foreach (var (product, qty) in receipt.Items)
            {
                Console.WriteLine($"- {product.Title} x{qty} = {product.Price * qty} грн");
            }
            Console.WriteLine($"Сума без знижки: {receipt.TotalBeforeDiscount:F2} грн");
            Console.WriteLine($"До сплати зі знижкою: {receipt.FinalTotal:F2} грн");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IProductRepository repo = new InMemoryProductRepository();
            var discountCalc = new DiscountCalculator();
            var salesService = new SalesService(repo, discountCalc);
            var ui = new CashDeskConsole(salesService, repo);

            ui.RunDemo();
        }
    }
}