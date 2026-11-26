using System;
using System.Collections.Generic;
using System.Linq;

namespace ItCompanyClassDiagramModule
{
    public abstract class Employee
    {
        public int Id { get; }
        public string Name { get; }
        public decimal HourlyRate { get; }

        protected Employee(int id, string name, decimal hourlyRate)
        {
            Id = id;
            Name = name;
            HourlyRate = hourlyRate;
        }

        public abstract decimal CalculateTaskCost(int hours);
    }

    public class SystemAdministrator : Employee
    {
        public string Certification { get; }

        public SystemAdministrator(int id, string name, decimal hourlyRate, string certification)
            : base(id, name, hourlyRate)
        {
            Certification = certification;
        }

        // Надбавка 15% за критичну інфраструктуру
        public override decimal CalculateTaskCost(int hours) => HourlyRate * hours * 1.15m;
    }

    public class SoftwareDeveloper : Employee
    {
        public string TechStack { get; }

        public SoftwareDeveloper(int id, string name, decimal hourlyRate, string techStack)
            : base(id, name, hourlyRate)
        {
            TechStack = techStack;
        }

        public override decimal CalculateTaskCost(int hours) => HourlyRate * hours;
    }

    public interface INotifiable
    {
        void SendNotification(string message);
    }

    public class ClientCompany : INotifiable
    {
        public string CompanyName { get; }
        public string ContactEmail { get; }

        public ClientCompany(string companyName, string contactEmail)
        {
            CompanyName = companyName;
            ContactEmail = contactEmail;
        }

        public void SendNotification(string message)
        {
            Console.WriteLine($"[Email -> {ContactEmail} ({CompanyName})]: {message}");
        }
    }

    public class ServiceTask
    {
        public string Title { get; }
        public int EstimatedHours { get; }
        public Employee AssignedSpecialist { get; }

        public ServiceTask(string title, int estimatedHours, Employee specialist)
        {
            Title = title;
            EstimatedHours = estimatedHours;
            AssignedSpecialist = specialist;
        }

        public decimal GetCost() => AssignedSpecialist.CalculateTaskCost(EstimatedHours);
    }

    public class ItProject
    {
        private readonly List<ServiceTask> _tasks = new();
        public string ProjectName { get; }
        public ClientCompany Client { get; }

        public ItProject(string projectName, ClientCompany client)
        {
            ProjectName = projectName;
            Client = client;
        }

        public void AddTask(ServiceTask task) => _tasks.Add(task);

        public void CompleteProject()
        {
            decimal totalCost = _tasks.Sum(t => t.GetCost());

            Console.WriteLine($"=== ЗВІТ ЗА ПРОЄКТОМ: '{ProjectName}' ===");
            foreach (var task in _tasks)
            {
                Console.WriteLine($"- Завдання: {task.Title} | Виконавець: {task.AssignedSpecialist.Name} | Годин: {task.EstimatedHours} | Вартість: {task.GetCost():F2} грн");
            }
            Console.WriteLine($"Загальна вартість робіт: {totalCost:F2} грн");

            Client.SendNotification($"Роботи за проєктом '{ProjectName}' завершено. Сума до сплати: {totalCost:F2} грн.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var client = new ClientCompany("ТОВ 'МедіаГруп'", "it-director@mediagroup.ua");
            var sysAdmin = new SystemAdministrator(1, "Андрій Бойко", 800m, "AWS Solutions Architect");
            var dev = new SoftwareDeveloper(2, "Марія Коваленко", 950m, "C# / .NET 8");

            var project = new ItProject("Міграція корпоративного порталу в хмару", client);
            project.AddTask(new ServiceTask("Налаштування хмарних серверів та безпеки", 10, sysAdmin));
            project.AddTask(new ServiceTask("Розробка API інтеграції", 16, dev));

            project.CompleteProject();
        }
    }
}