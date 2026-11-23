using System;
using System.Collections.Generic;

namespace PracticalWork1
{
    // ==========================================
    // 1. Зв'язок 1:1 (Автомобіль — Техпаспорт)
    // ==========================================
    public class TechnicalPassport
    {
        public string PassportNumber { get; set; }
        public string IssueDate { get; set; }

        public TechnicalPassport(string passportNumber, string issueDate)
        {
            PassportNumber = passportNumber;
            IssueDate = issueDate;
        }
    }

    public class Car
    {
        public string Brand { get; set; }
        public string LicensePlate { get; set; }
        public TechnicalPassport TechPassport { get; set; } // Зв'язок 1:1

        public Car(string brand, string licensePlate, TechnicalPassport techPassport)
        {
            Brand = brand;
            LicensePlate = licensePlate;
            TechPassport = techPassport;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"[1:1] Автомобіль: {Brand} ({LicensePlate}), " +
                              $"Техпаспорт №: {TechPassport.PassportNumber} від {TechPassport.IssueDate}");
        }
    }

    // ==========================================
    // 2. Зв'язок 1:N (Факультет — Кафедра)
    // ==========================================
    public class Department
    {
        public string Name { get; set; }

        public Department(string name)
        {
            Name = name;
        }
    }

    public class Faculty
    {
        public string Name { get; set; }
        public List<Department> Departments { get; set; } = new List<Department>(); // Зв'язок 1:N

        public Faculty(string name)
        {
            Name = name;
        }

        public void AddDepartment(Department department)
        {
            Departments.Add(department);
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"\n[1:N] Факультет: {Name}");
            Console.WriteLine("Кафедри у складі:");
            foreach (var dep in Departments)
            {
                Console.WriteLine($" - {dep.Name}");
            }
        }
    }

    // ==========================================
    // 3. Зв'язок M:N (Викладач — Дисципліна)
    // ==========================================
    public class Discipline
    {
        public string Title { get; set; }
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        public Discipline(string title)
        {
            Title = title;
        }
    }

    public class Teacher
    {
        public string FullName { get; set; }
        public List<Discipline> Disciplines { get; set; } = new List<Discipline>(); // Зв'язок M:N

        public Teacher(string fullName)
        {
            FullName = fullName;
        }

        public void AddDiscipline(Discipline discipline)
        {
            if (!Disciplines.Contains(discipline))
            {
                Disciplines.Add(discipline);
                discipline.Teachers.Add(this); // Двосторонній зв'язок для M:N
            }
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"\nВикладач: {FullName}");
            Console.Write("Дисципліни: ");
            foreach (var d in Disciplines)
            {
                Console.Write($"[{d.Title}] ");
            }
            Console.WriteLine();
        }
    }

    // ==========================================
    // Головний клас та точка входу
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Практична робота №1 (Варіант 3) ===\n");

            // 1. Перевірка зв'язку 1:1
            TechnicalPassport passport = new TechnicalPassport("AB123456", "15.01.2022");
            Car car = new Car("Toyota Camry", "KA1234BX", passport);
            car.DisplayInfo();

            // 2. Перевірка зв'язку 1:N
            Faculty faculty = new Faculty("Факультет комп'ютерних наук");
            faculty.AddDepartment(new Department("Кафедра інженерії програмного забезпечення"));
            faculty.AddDepartment(new Department("Кафедра штучного інтелекту"));
            faculty.DisplayInfo();

            // 3. Перевірка зв'язку M:N
            Teacher t1 = new Teacher("Іваненко І. І.");
            Teacher t2 = new Teacher("Петренко П. П.");

            Discipline d1 = new Discipline("Об'єктно-орієнтоване програмування");
            Discipline d2 = new Discipline("Бази даних");

            // Налаштування зв'язків між викладачами та дисциплінами
            t1.AddDiscipline(d1);
            t1.AddDiscipline(d2);
            t2.AddDiscipline(d1);

            Console.WriteLine("\n[M:N] Викладачі та їх дисципліни:");
            t1.DisplayInfo();
            t2.DisplayInfo();

            Console.ReadLine();
        }
    }
}