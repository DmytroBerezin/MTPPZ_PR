using System;
using System.Text;

namespace ITCompanyStateMachine
{
    public enum TicketState
    {
        New,
        InProgress,
        InReview,
        Closed,
        Rejected
    }

    public class TicketStateMachine
    {
        public int TicketId { get; }
        public TicketState CurrentState { get; private set; }
        public string AssignedEngineer { get; private set; }
        public double HoursSpent { get; private set; }

        public TicketStateMachine(int id)
        {
            TicketId = id;
            CurrentState = TicketState.New;
            Console.WriteLine($"[ENTRY -> Нова]: Заявку #{TicketId} зареєстровано в системі.");
        }

        public void AssignEngineer(string engineerName, bool isEngineerAvailable)
        {
            Console.WriteLine($"\n>>> Подія: призначити_інженера ({engineerName})");

            if (CurrentState != TicketState.New)
            {
                Console.WriteLine($"[ПОМИЛКА]: Призначення можливе лише зі стану 'New' (поточний: '{CurrentState}').");
                return;
            }

            if (!isEngineerAvailable)
            {
                Console.WriteLine($"[ЗАХИСНА УМОВА]: Інженер {engineerName} зараз зайнятий іншим проєктом!");
                return;
            }

            AssignedEngineer = engineerName;
            CurrentState = TicketState.InProgress;
            Console.WriteLine($"[ENTRY -> ВРоботі]: Виконавцем призначено {AssignedEngineer}. Роботу розпочато.");
        }

        public void SubmitForReview(double hours)
        {
            Console.WriteLine($"\n>>> Подія: відправити_на_перевірку (годин: {hours})");

            if (CurrentState != TicketState.InProgress)
            {
                Console.WriteLine($"[ПОМИЛКА]: Відправити на перевірку можна лише зі стану 'InProgress'.");
                return;
            }

            if (hours <= 0)
            {
                Console.WriteLine("[ЗАХИСНА УМОВА]: Кількість витрачених годин має бути більшою за 0!");
                return;
            }

            HoursSpent += hours;
            CurrentState = TicketState.InReview;
            Console.WriteLine($"[ENTRY -> НаПеревірці]: Заявка очікує підтвердження клієнта (всього годин: {HoursSpent}).");
        }

        public void ReturnForRevision(string comment)
        {
            Console.WriteLine($"\n>>> Подія: повернути_на_доопрацювання ('{comment}')");

            if (CurrentState != TicketState.InReview)
            {
                Console.WriteLine("[ПОМИЛКА]: Повернути можна лише заявку зі стану 'InReview'.");
                return;
            }

            CurrentState = TicketState.InProgress;
            Console.WriteLine("[ENTRY -> ВРоботі]: Заявку повернуто інженеру на виправлення.");
        }

        public void ApproveAndClose()
        {
            Console.WriteLine("\n>>> Подія: прийняти_роботу");

            if (CurrentState != TicketState.InReview)
            {
                Console.WriteLine("[ПОМИЛКА]: Закрити можна лише заявку, що пройшла перевірку ('InReview').");
                return;
            }

            CurrentState = TicketState.Closed;
            decimal invoiceAmount = (decimal)HoursSpent * 800m;
            Console.WriteLine($"[ЕФЕКТ]: Сформовано рахунок на суму {invoiceAmount:F2} грн.");
            Console.WriteLine("[ENTRY -> Закрита]: Заявку успішно закрито та архівовано.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== МОДЕЛЮВАННЯ СТАНІВ ЗАЯВКИ ІТ-КОМПАНІЇ ===");

            var ticket = new TicketStateMachine(101);

            // Перехід New -> InProgress
            ticket.AssignEngineer("Олександр Коваль", isEngineerAvailable: true);

            // Перевірка захисту [витрачені_години > 0] та перехід InProgress -> InReview
            ticket.SubmitForReview(0);
            ticket.SubmitForReview(4.5);

            // Повернення на доопрацювання InReview -> InProgress
            ticket.ReturnForRevision("Потрібно додати налаштування резервного копіювання");

            // Повторна здача та закриття InProgress -> InReview -> Closed
            ticket.SubmitForReview(1.5);
            ticket.ApproveAndClose();

            Console.WriteLine("\nНатисніть Enter для виходу...");
            Console.ReadLine();
        }
    }
}