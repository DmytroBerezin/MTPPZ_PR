using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopSequenceModule
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int productId, string name, decimal price, int stock)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            Stock = stock;
        }
    }

    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public List<OrderItem> Items { get; } = new List<OrderItem>();
        public bool IsPaid { get; set; }
        public string TransactionId { get; set; } = string.Empty;

        public decimal TotalAmount => Items.Sum(item => item.Product.Price * item.Quantity);

        public Order(int orderId)
        {
            OrderId = orderId;
        }
    }

    public class InventoryService
    {
        private readonly Dictionary<int, Product> _products;

        public InventoryService(Dictionary<int, Product> products)
        {
            _products = products;
        }

        public bool CheckStock(int productId, int qty)
        {
            return _products.TryGetValue(productId, out var product) && product.Stock >= qty;
        }

        public Product GetProduct(int productId) => _products[productId];

        public bool ReserveAndDeductStock(Order order)
        {
            foreach (var item in order.Items)
            {
                if (!CheckStock(item.Product.ProductId, item.Quantity))
                    return false;
            }

            foreach (var item in order.Items)
            {
                item.Product.Stock -= item.Quantity;
            }

            return true;
        }
    }

    public class PaymentGateway
    {
        public (bool Success, string TransactionId) Charge(decimal amount, string cardNumber)
        {
            if (cardNumber.Length == 16 && amount > 0)
            {
                string txId = $"TX-{Math.Abs(cardNumber.GetHashCode()) % 100000}";
                return (true, txId);
            }
            return (false, string.Empty);
        }
    }

    public class OrderService
    {
        private readonly InventoryService _inventory;
        private readonly PaymentGateway _payment;
        private readonly Dictionary<int, Order> _orders = new Dictionary<int, Order>();

        public OrderService(InventoryService inventory, PaymentGateway payment)
        {
            _inventory = inventory;
            _payment = payment;
        }

        public Order CreateOrder(int orderId)
        {
            var order = new Order(orderId);
            _orders[orderId] = order;
            return order;
        }

        public decimal AddItemToOrder(int orderId, int productId, int qty)
        {
            var order = _orders[orderId];
            if (!_inventory.CheckStock(productId, qty))
            {
                throw new InvalidOperationException($"Недостатньо товару ID={productId} на складі.");
            }

            var product = _inventory.GetProduct(productId);
            order.Items.Add(new OrderItem(product, qty));
            return order.TotalAmount;
        }

        public string ProcessCheckout(int orderId, string cardNumber)
        {
            var order = _orders[orderId];
            var (success, txId) = _payment.Charge(order.TotalAmount, cardNumber);

            if (!success)
            {
                return "Помилка оплати: відхилено платіжним шлюзом.";
            }

            if (_inventory.ReserveAndDeductStock(order))
            {
                order.IsPaid = true;
                order.TransactionId = txId;
                return $"=== ФІСКАЛЬНИЙ ЧЕК №{order.OrderId} ===\n" +
                       $"Транзакція: {txId}\n" +
                       $"Сума до сплати: {order.TotalAmount:F2} грн\n" +
                       $"Статус: ОПЛАЧЕНО";
            }

            return "Помилка: товар закінчився під час оформлення.";
        }
    }

    public class CashRegisterUI
    {
        private readonly OrderService _orderService;

        public CashRegisterUI(OrderService orderService)
        {
            _orderService = orderService;
        }

        public void ScanProduct(int orderId, int productId, int qty)
        {
            decimal subtotal = _orderService.AddItemToOrder(orderId, productId, qty);
            Console.WriteLine($"[Каса] Додано товар ID={productId} (x{qty}). Поточна сума: {subtotal:F2} грн");
        }

        public void ConfirmAndPay(int orderId, string cardNumber)
        {
            Console.WriteLine("[Каса] Обробка платежу...");
            string receipt = _orderService.ProcessCheckout(orderId, cardNumber);
            Console.WriteLine(receipt);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var catalog = new Dictionary<int, Product>
            {
                { 101, new Product(101, "Кава зернова 1кг", 540.00m, 15) },
                { 102, new Product(102, "Шоколад чорний", 65.50m, 40) }
            };

            var inventoryService = new InventoryService(catalog);
            var paymentGateway = new PaymentGateway();
            var orderService = new OrderService(inventoryService, paymentGateway);
            var cashUI = new CashRegisterUI(orderService);

            orderService.CreateOrder(1);
            cashUI.ScanProduct(1, 101, 1);
            cashUI.ScanProduct(1, 102, 2);
            cashUI.ConfirmAndPay(1, "4149499123456789");
        }
    }
}