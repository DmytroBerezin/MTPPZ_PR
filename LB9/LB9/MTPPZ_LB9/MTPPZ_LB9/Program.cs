using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ShopDeploymentModule
{
    public class PosConfigArtifact
    {
        public string TerminalId { get; set; } = "POS-01";
        public string ServerEndpoint { get; set; } = "https://192.168.1.10:5001/api/sales";
        public string Protocol { get; set; } = "HTTPS/JSON";
    }

    public class SalePacketDto
    {
        public string TerminalId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class DatabaseServerNode
    {
        public string HostAddress => "192.168.1.20:5432";
        private readonly Dictionary<int, (string Name, decimal Price, int Stock)> _dbTable = new()
        {
            { 101, ("Кава в зернах 1кг", 520.00m, 12) },
            { 102, ("Чай зелений 250г", 145.00m, 30) }
        };

        public (bool Success, string Message, decimal Total) ExecuteSaleTransaction(int productId, int qty)
        {
            Console.WriteLine($"  [DatabaseServer @ {HostAddress}] Отримано SQL-транзакцію для товару ID={productId}...");

            if (!_dbTable.TryGetValue(productId, out var item))
                return (false, "Товар не знайдено в БД.", 0m);

            if (item.Stock < qty)
                return (false, $"Недостатньо залишку на складі (доступно: {item.Stock}).", 0m);

            _dbTable[productId] = (item.Name, item.Price, item.Stock - qty);
            decimal total = item.Price * qty;
            return (true, $"Списано '{item.Name}' x{qty}. Залишок: {item.Stock - qty} шт.", total);
        }
    }

    public class ShopAppServerNode
    {
        public string NodeName => "ShopAppServer (Ubuntu / .NET 8 Runtime)";
        private readonly DatabaseServerNode _dbNode;

        public ShopAppServerNode(DatabaseServerNode dbNode)
        {
            _dbNode = dbNode;
        }

        public string HandleHttpPostRequest(string jsonPayload)
        {
            Console.WriteLine($" [ShopAppServer] Прийнято HTTP POST запит: {jsonPayload}");
            var packet = JsonSerializer.Deserialize<SalePacketDto>(jsonPayload)!;

            var dbResult = _dbNode.ExecuteSaleTransaction(packet.ProductId, packet.Quantity);

            var response = new
            {
                Status = dbResult.Success ? "200 OK" : "400 Bad Request",
                Terminal = packet.TerminalId,
                Details = dbResult.Message,
                AmountToPay = dbResult.Total
            };

            return JsonSerializer.Serialize(response);
        }
    }

    public class PosTerminalNode
    {
        private readonly PosConfigArtifact _config;
        private readonly ShopAppServerNode _appServer;

        public PosTerminalNode(PosConfigArtifact config, ShopAppServerNode appServer)
        {
            _config = config;
            _appServer = appServer;
        }

        public void SendSaleToServer(int productId, int quantity)
        {
            Console.WriteLine($"[POSTerminal {_config.TerminalId}] Підключення до {_config.ServerEndpoint} ({_config.Protocol})...");

            var dto = new SalePacketDto
            {
                TerminalId = _config.TerminalId,
                ProductId = productId,
                Quantity = quantity
            };

            string jsonRequest = JsonSerializer.Serialize(dto);
            string jsonResponse = _appServer.HandleHttpPostRequest(jsonRequest);

            Console.WriteLine($"[POSTerminal {_config.TerminalId}] Відповідь сервера: {jsonResponse}\n");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var dbServer = new DatabaseServerNode();
            var appServer = new ShopAppServerNode(dbServer);
            var posConfig = new PosConfigArtifact();
            var posTerminal = new PosTerminalNode(posConfig, appServer);

            posTerminal.SendSaleToServer(101, 2);
            posTerminal.SendSaleToServer(102, 5);
        }
    }
}