using System;
using System.Collections.Generic;
using System.Text;

namespace StoreStateMachine
{
    public enum ReceiptState
    {
        Created,
        Filling,
        AwaitingPayment,
        Paid,
        Cancelled,
        Refunded 
    }

    public class ReceiptStateMachine
    {
        public ReceiptState CurrentState { get; private set; }
        private readonly List<string> _items = new List<string>();
        private decimal _totalAmount = 0m;

        public ReceiptStateMachine()
        {

            CurrentState = ReceiptState.Created;
            OnEntry(CurrentState);
        }

        private void OnEntry(ReceiptState state)
        {
            switch (state)
            {
                case ReceiptState.Created:
                    Console.WriteLine("[ENTRY -> Створено]: Ініціалізовано новий порожній чек.");
                    break;
                case ReceiptState.Filling:
                    Console.WriteLine($"[ENTRY -> Наповнення]: Поточна сума чеку: {_totalAmount:F2} грн.");
                    break;
                case ReceiptState.AwaitingPayment:
                    Console.WriteLine($"[ENTRY -> ОчікуванняОплати]: Зміну товарів заблоковано. До сплати: {_totalAmount:F2} грн.");
                    break;
                case ReceiptState.Paid:
                    Console.WriteLine("[ENTRY -> Оплачено]: Чек фіскалізовано! Товари списано зі складу.");
                    break;
                case ReceiptState.Cancelled:
                    Console.WriteLine("[ENTRY -> Скасовано]: Операцію скасовано, кошик очищено.");
                    break;
                case ReceiptState.Refunded:
                    Console.WriteLine($"[ENTRY -> Повернення]: Покупцю повернуто {_totalAmount:F2} грн. Товар повернуто на склад.");
                    break;
            }
        }

        private void OnExit(ReceiptState state)
        {
            if (state == ReceiptState.AwaitingPayment)
            {
                Console.WriteLine("[EXIT <- ОчікуванняОплати]: Знято блокування касового терміналу.");
            }
        }

        public void AddItem(string itemName, decimal price, int stockAvailable)
        {
            Console.WriteLine($"\n>>> Подія: додати_товар ('{itemName}', {price:F2} грн)");

            if (CurrentState != ReceiptState.Created && CurrentState != ReceiptState.Filling)
            {
                Console.WriteLine($"[ПОМИЛКА ПЕРЕХОДУ]: Неможливо додати товар у стані '{CurrentState}'.");
                return;
            }

            if (stockAvailable <= 0)
            {
                Console.WriteLine($"[ЗАХИСНА УМОВА НЕ ВИКОНАНА]: Товару '{itemName}' немає в наявності!");
                return;
            }

            _items.Add(itemName);
            _totalAmount += price;

            CurrentState = ReceiptState.Filling;
            OnEntry(CurrentState);
        }

        public void ProceedToPayment()
        {
            Console.WriteLine("\n>>> Подія: перейти_до_оплати");

            if (CurrentState != ReceiptState.Filling)
            {
                Console.WriteLine($"[ПОМИЛКА ПЕРЕХОДУ]: Перехід до оплати можливий лише зі стану 'Filling' (поточний: '{CurrentState}').");
                return;
            }

            if (_items.Count == 0)
            {
                Console.WriteLine("[ЗАХИСНА УМОВА НЕ ВИКОНАНА]: Чек порожній!");
                return;
            }

            CurrentState = ReceiptState.AwaitingPayment;
            OnEntry(CurrentState);
        }

        public void ConfirmPayment(decimal paidAmount)
        {
            Console.WriteLine($"\n>>> Подія: підтвердити_оплату (внесено {paidAmount:F2} грн)");

            if (CurrentState != ReceiptState.AwaitingPayment)
            {
                Console.WriteLine($"[ПОМИЛКА ПЕРЕХОДУ]: Оплата не очікується у стані '{CurrentState}'.");
                return;
            }

            if (paidAmount < _totalAmount)
            {
                Console.WriteLine($"[ЗАХИСНА УМОВА НЕ ВИКОНАНА]: Недостатньо коштів! Потрібно {_totalAmount:F2} грн.");
                return;
            }

            OnExit(CurrentState);
            CurrentState = ReceiptState.Paid;
            if (paidAmount > _totalAmount)
            {
                Console.WriteLine($"[ЕФЕКТ]: Видача решти: {paidAmount - _totalAmount:F2} грн.");
            }
            OnEntry(CurrentState);
        }

        public void CancelReceipt()
        {
            Console.WriteLine("\n>>> Подія: скасувати_чек");

            if (CurrentState != ReceiptState.Filling && CurrentState != ReceiptState.AwaitingPayment)
            {
                Console.WriteLine($"[ПОМИЛКА ПЕРЕХОДУ]: Неможливо скасувати чек у стані '{CurrentState}'.");
                return;
            }

            OnExit(CurrentState);
            _items.Clear();
            _totalAmount = 0m;
            CurrentState = ReceiptState.Cancelled;
            OnEntry(CurrentState);
        }

        public void RefundReceipt()
        {
            Console.WriteLine("\n>>> Подія: оформити_повернення");

            if (CurrentState != ReceiptState.Paid)
            {
                Console.WriteLine($"[ЗАХИСНА УМОВА НЕ ВИКОНАНА]: Повернення можливе лише для оплаченого чеку (поточний стан: '{CurrentState}').");
                return;
            }

            CurrentState = ReceiptState.Refunded;
            OnEntry(CurrentState);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== МОДЕЛЮВАННЯ СТАНІВ СИСТЕМИ МАГАЗИНУ ===");

            var receipt = new ReceiptStateMachine();

            // 1. Спроба перейти до оплати порожнього чеку (перевірка захисту)
            receipt.ProceedToPayment();

            // 2. Додавання товарів (перехід Created -> Filling)
            receipt.AddItem("Хліб Український", 28.50m, stockAvailable: 10);
            receipt.AddItem("Шоколад чорний", 64.00m, stockAvailable: 0); // Не спрацює через guard
            receipt.AddItem("Молоко 2.5%", 42.00m, stockAvailable: 5);

            // 3. Перехід до оплати (Filling -> AwaitingPayment)
            receipt.ProceedToPayment();

            // 4. Спроба додати товар під час оплати (заблоковано станом)
            receipt.AddItem("Кава", 120.00m, stockAvailable: 3);

            // 5. Спроба оплати недостатньою сумою (guard) та успішна оплата (AwaitingPayment -> Paid)
            receipt.ConfirmPayment(50.00m);
            receipt.ConfirmPayment(100.00m);

            // 6. Повернення товару (Paid -> Refunded)
            receipt.RefundReceipt();

            Console.WriteLine("\nНатисніть Enter для виходу...");
            Console.ReadLine();
        }
    }
}