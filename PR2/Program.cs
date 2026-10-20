using System;
using System.Collections.Generic;

namespace PracticalWork2
{
    // ==========================================
    // 1. Модель Техпаспорта (Валідація за словником)
    // ==========================================
    public class TechnicalPassport
    {
        public string PassportNumber { get; private set; }
        public DateTime IssueDate { get; private set; }

        public TechnicalPassport(string passportNumber, DateTime issueDate)
        {
            // Правило валідації: номер техпаспорта не порожній
            if (string.IsNullOrWhiteSpace(passportNumber))
                throw new ArgumentException("Номер техпаспорта не може бути порожнім!");

            // Правило валідації: дата не з майбутнього
            if (issueDate > DateTime.Now)
                throw new ArgumentException("Дата видачі не може бути у майбутньому!");

            PassportNumber = passportNumber;
            IssueDate = issueDate;
        }
    }

    public class Car
    {
        public string Brand { get; set; }
        public string LicensePlate { get; set; }
        public TechnicalPassport TechPassport { get; set; }

        public Car(string brand, string licensePlate, TechnicalPassport techPassport)
        {
            if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("Марка та номерний знак є обов'язковими!");

            Brand = brand;
            LicensePlate = licensePlate;
            TechPassport = techPassport ?? throw new ArgumentNullException(nameof(techPassport));
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"[Автомобіль] Марка: {Brand} | Номер: {LicensePlate} | Техпаспорт №: {TechPassport.PassportNumber} ({TechPassport.IssueDate:dd.MM.yyyy})");
        }
    }

    // ==========================================
    // 2. Модель Кафедри та Факультету (Композиція)
    // ==========================================
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Department(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва кафедри не може бути порожньою!");

            Id = id;
            Name = name;
        }
    }

    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Композиція: Факультет володіє списками своїх кафедр
        private readonly List<Department> _departments = new List<Department>();

        public Faculty(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddDepartment(int deptId, string deptName)
        {
            // Факультет сам відповідає за створення своїх кафедр (Композиція)
            _departments.Add(new Department(deptId, deptName));
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"\n[Факультет] {Name}");
            Console.WriteLine("Кафедри у складі:");
            foreach (var d in _departments)
            {
                Console.WriteLine($"  - ID: {d.Id}, Назва: {d.Name}");
            }
        }
    }

    // ==========================================
    // 3. Модель Викладача та Дисципліни (Агрегація)
    // ==========================================
    public class Discipline
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public Discipline(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        // Агрегація: Дисципліни існують окремо від викладача
        public List<Discipline> Disciplines { get; private set; } = new List<Discipline>();

        public Teacher(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }

        public void AssignDiscipline(Discipline discipline)
        {
            if (discipline != null && !Disciplines.Contains(discipline))
            {
                Disciplines.Add(discipline);
            }
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"\n[Викладач] {FullName}");
            Console.Write("Дисципліни: ");
            foreach (var d in Disciplines)
            {
                Console.Write($"[{d.Title}] ");
            }
            Console.WriteLine();
        }
    }

    // ==========================================
    // Головний клас програми
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Практична робота №2 (Варіант 3) ===\n");

            try
            {
                // 1. Демонстрація валідації за Словником термінів
                TechnicalPassport passport = new TechnicalPassport("TP987654", new DateTime(2021, 5, 20));
                Car car = new Car("Audi A6", "KA7777BP", passport);
                car.DisplayInfo();

                // 2. Демонстрація Композиції (Факультет — Кафедра)
                Faculty faculty = new Faculty(1, "Факультет інформаційних технологій");
                faculty.AddDepartment(101, "Кафедра кібербезпеки");
                faculty.AddDepartment(102, "Кафедра комп'ютерних наук");
                faculty.DisplayInfo();

                // 3. Демонстрація Агрегації (Викладач — Дисципліна)
                Discipline oop = new Discipline(1, "Об'єктно-орієнтоване програмування");
                Discipline architecture = new Discipline(2, "Архітектура ПЗ");

                Teacher teacher = new Teacher(1, "Сидоренко О. М.");
                teacher.AssignDiscipline(oop);
                teacher.AssignDiscipline(architecture);
                teacher.DisplayInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка валідації: {ex.Message}");
            }

            Console.ReadLine();
        }
    }
}