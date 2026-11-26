using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopCommandPatternModule
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class ReceiptReceiver
    {
        private readonly List<(string Name, decimal Price)> _items = new();
        private decimal _discountPercent = 0m;

        public void AddItem(string name, decimal price)
        {
            _items.Add((name, price));
            Console.WriteLine($"  [Чек] Додано товар: '{name}' ({price:F2} грн)");
        }

        public void RemoveItem(string name, decimal price)
        {
            var index = _items.FindLastIndex(i => i.Name == name && i.Price == price);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                Console.WriteLine($"  [Чек] Скасовано товар: '{name}' (-{price:F2} грн)");
            }
        }

        public decimal SetDiscount(decimal newDiscountPercent)
        {
            decimal oldDiscount = _discountPercent;
            _discountPercent = newDiscountPercent;
            Console.WriteLine($"  [Чек] Встановлено знижку: {_discountPercent}% (було: {oldDiscount}%)");
            return oldDiscount;
        }

        public void PrintReceipt()
        {
            decimal subtotal = _items.Sum(i => i.Price);
            decimal discountAmount = subtotal * (_discountPercent / 100m);
            decimal total = subtotal - discountAmount;

            Console.WriteLine("\n=== ПІДСУМКОВИЙ ЧЕК МАГАЗИНУ ===");
            foreach (var (name, price) in _items)
            {
                Console.WriteLine($"- {name}: {price:F2} грн");
            }
            Console.WriteLine($"Сума: {subtotal:F2} грн | Знижка ({_discountPercent}%): -{discountAmount:F2} грн");
            Console.WriteLine($"ДО СПЛАТИ: {total:F2} грн\n");
        }
    }

    public class AddItemCommand : ICommand
    {
        private readonly ReceiptReceiver _receipt;
        private readonly string _productName;
        private readonly decimal _price;

        public AddItemCommand(ReceiptReceiver receipt, string productName, decimal price)
        {
            _receipt = receipt;
            _productName = productName;
            _price = price;
        }

        public void Execute() => _receipt.AddItem(_productName, _price);

        public void Undo() => _receipt.RemoveItem(_productName, _price);
    }

    public class ApplyDiscountCommand : ICommand
    {
        private readonly ReceiptReceiver _receipt;
        private readonly decimal _newDiscount;
        private decimal _previousDiscount;

        public ApplyDiscountCommand(ReceiptReceiver receipt, decimal newDiscount)
        {
            _receipt = receipt;
            _newDiscount = newDiscount;
        }

        public void Execute()
        {
            _previousDiscount = _receipt.SetDiscount(_newDiscount);
        }

        public void Undo()
        {
            _receipt.SetDiscount(_previousDiscount);
        }
    }

    public class PosTerminalInvoker
    {
        private readonly Stack<ICommand> _commandHistory = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }

        public void UndoLastCommand()
        {
            if (_commandHistory.Count == 0)
            {
                Console.WriteLine("  [POS] Немає операцій для скасування.");
                return;
            }

            Console.WriteLine("[POS] Натиснуто 'Скасувати останню дію (Undo)':");
            ICommand lastCommand = _commandHistory.Pop();
            lastCommand.Undo();
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var receipt = new ReceiptReceiver();
            var posTerminal = new PosTerminalInvoker();

            posTerminal.ExecuteCommand(new AddItemCommand(receipt, "Кава в зернах 500г", 340.00m));
            posTerminal.ExecuteCommand(new AddItemCommand(receipt, "Шоколад чорний", 65.00m));
            posTerminal.ExecuteCommand(new AddItemCommand(receipt, "Помилковий товар (Сир)", 190.00m));
            posTerminal.ExecuteCommand(new ApplyDiscountCommand(receipt, 10m));


            posTerminal.UndoLastCommand();
            posTerminal.UndoLastCommand();

            posTerminal.ExecuteCommand(new ApplyDiscountCommand(receipt, 5m));

            receipt.PrintReceipt();
        }
    }
}