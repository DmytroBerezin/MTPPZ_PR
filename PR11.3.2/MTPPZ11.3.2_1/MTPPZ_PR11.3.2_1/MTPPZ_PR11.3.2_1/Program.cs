using System;
using System.Collections.Generic;

namespace CompanyStateAndMementoModule
{
    public interface IProjectState
    {
        string StateName { get; }
        void StartWork(CompanyProjectContext context);
        void CompleteWork(CompanyProjectContext context);
    }

    public class DraftState : IProjectState
    {
        public string StateName => "Чернетка (Draft)";

        public void StartWork(CompanyProjectContext context)
        {
            Console.WriteLine($"  [State] Проєкт '{context.ProjectTitle}' затверджено та переведено у роботу.");
            context.SetState(new InProgressState());
        }

        public void CompleteWork(CompanyProjectContext context)
        {
            Console.WriteLine($"  [State] Помилка: неможливо завершити проєкт '{context.ProjectTitle}', доки він у статусі чернетки.");
        }
    }

    public class InProgressState : IProjectState
    {
        public string StateName => "В роботі (In Progress)";

        public void StartWork(CompanyProjectContext context)
        {
            Console.WriteLine($"  [State] Проєкт '{context.ProjectTitle}' вже виконується.");
        }

        public void CompleteWork(CompanyProjectContext context)
        {
            Console.WriteLine($"  [State] Усі завдання за проєктом '{context.ProjectTitle}' виконано. Статус змінено на 'Завершено'.");
            context.SetState(new CompletedState());
        }
    }

    public class CompletedState : IProjectState
    {
        public string StateName => "Завершено (Completed)";

        public void StartWork(CompanyProjectContext context)
        {
            Console.WriteLine($"  [State] Заборонено: проєкт '{context.ProjectTitle}' вже закритий і переданий в архів.");
        }

        public void CompleteWork(CompanyProjectContext context)
        {
            Console.WriteLine($"  [State] Проєкт '{context.ProjectTitle}' вже має статус завершеного.");
        }
    }

    public class ProjectMemento
    {
        public string SavedTitle { get; }
        public decimal SavedBudget { get; }
        public IProjectState SavedState { get; }
        public DateTime CreatedAt { get; }

        public ProjectMemento(string title, decimal budget, IProjectState state)
        {
            SavedTitle = title;
            SavedBudget = budget;
            SavedState = state;
            CreatedAt = DateTime.Now;
        }
    }


    public class CompanyProjectContext
    {
        private IProjectState _currentState;
        public string ProjectTitle { get; set; }
        public decimal Budget { get; set; }

        public CompanyProjectContext(string title, decimal budget)
        {
            ProjectTitle = title;
            Budget = budget;
            _currentState = new DraftState();
        }

        public void SetState(IProjectState newState) => _currentState = newState;

        public void BeginExecution() => _currentState.StartWork(this);

        public void FinishExecution() => _currentState.CompleteWork(this);

        public ProjectMemento CreateSnapshot()
        {
            Console.WriteLine($"[Memento] Створено контрольну точку: '{ProjectTitle}', Бюджет: {Budget:F2} грн, Стан: {_currentState.StateName}");
            return new ProjectMemento(ProjectTitle, Budget, _currentState);
        }

        public void RestoreSnapshot(ProjectMemento memento)
        {
            ProjectTitle = memento.SavedTitle;
            Budget = memento.SavedBudget;
            _currentState = memento.SavedState;
            Console.WriteLine($"[Memento] Відновлено стан проєкту від {memento.CreatedAt:HH:mm:ss}: '{ProjectTitle}', Бюджет: {Budget:F2} грн, Стан: {_currentState.StateName}");
        }

        public void PrintStatus()
        {
            Console.WriteLine($"-> Поточний стан проєкту: '{ProjectTitle}' | Бюджет: {Budget:F2} грн | Статус: {_currentState.StateName}");
        }
    }
    public class ProjectHistoryCaretaker
    {
        private readonly Stack<ProjectMemento> _history = new();

        public void Save(ProjectMemento memento) => _history.Push(memento);

        public ProjectMemento? Undo() => _history.Count > 0 ? _history.Pop() : null;
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var history = new ProjectHistoryCaretaker();
            var project = new CompanyProjectContext("Впровадження ERP-системи", 250000m);
            project.PrintStatus();

            project.FinishExecution();

            history.Save(project.CreateSnapshot());

            project.BeginExecution();
            project.Budget = 320000m;
            project.PrintStatus();

            history.Save(project.CreateSnapshot());

            project.FinishExecution();
            project.ProjectTitle = "АРХІВ_ПОМИЛКОВА_НАЗВА";
            project.PrintStatus();

            Console.WriteLine("\nВиконуємо відкат до попередньої контрольної точки:");
            var previousSnapshot = history.Undo();
            if (previousSnapshot != null)
            {
                project.RestoreSnapshot(previousSnapshot);
            }
            project.PrintStatus();
        }
    }
}