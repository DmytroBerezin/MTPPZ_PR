using System;

namespace Practical11_FactoryMethod
{

    public abstract class Course
    {
        public string Title { get; }
        public int DurationWeeks { get; }

        protected Course(string title, int durationWeeks)
        {
            Title = title;
            DurationWeeks = durationWeeks;
        }

        public abstract void EnrollStudent(string studentName);
        public abstract string GetFormatDetails();
    }
    public class OnlineCourse : Course
    {
        public string PlatformUrl { get; }

        public OnlineCourse(string title, int durationWeeks, string platformUrl)
            : base(title, durationWeeks)
        {
            PlatformUrl = platformUrl;
        }

        public override void EnrollStudent(string studentName)
        {
            Console.WriteLine($"[Online] Студента '{studentName}' зараховано на курс \"{Title}\".");
            Console.WriteLine($"         Доступ до відеолекцій відкрито на платформі: {PlatformUrl}");
        }

        public override string GetFormatDetails() =>
            $"Онлайн-формат ({DurationWeeks} тижнів), платформа: {PlatformUrl}";
    }

    public class OfflineCourse : Course
    {
        public string CampusAddress { get; }
        public string RoomNumber { get; }

        public OfflineCourse(string title, int durationWeeks, string campusAddress, string roomNumber)
            : base(title, durationWeeks)
        {
            CampusAddress = campusAddress;
            RoomNumber = roomNumber;
        }

        public override void EnrollStudent(string studentName)
        {
            Console.WriteLine($"[Offline] Студента '{studentName}' зараховано на курс \"{Title}\".");
            Console.WriteLine($"          Місце проведення занять: {CampusAddress}, ауд. {RoomNumber}");
        }

        public override string GetFormatDetails() =>
            $"Очний формат ({DurationWeeks} тижнів), адреса: {CampusAddress}, ауд. {RoomNumber}";
    }

    public abstract class CourseFactory
    {
        public abstract Course CreateCourse(string courseType);
    }

    public class UniversityCourseFactory : CourseFactory
    {
        public override Course CreateCourse(string courseType)
        {
            return courseType.Trim().ToLower() switch
            {
                "online" => new OnlineCourse(
                    "Об'єктно-орієнтований аналіз та проєктування (C#)",
                    12,
                    "https://lms.edu.ua/courses/ooad-csharp"),

                "offline" => new OfflineCourse(
                    "Адміністрування баз даних та сховищ даних",
                    14,
                    "вул. Солом'янська, 7",
                    "304-А"),

                _ => throw new ArgumentException($"Невідомий тип курсу: '{courseType}'. Очікується 'online' або 'offline'.")
            };
        }
    }
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CourseFactory factory = new UniversityCourseFactory();

            Course onlineCourse = factory.CreateCourse("online");
            Console.WriteLine($"Створено курс: {onlineCourse.GetFormatDetails()}");
            onlineCourse.EnrollStudent("Березін Дмитро");

            Console.WriteLine();

            Course offlineCourse = factory.CreateCourse("offline");
            Console.WriteLine($"Створено курс: {offlineCourse.GetFormatDetails()}");
            offlineCourse.EnrollStudent("Коваленко Олена");
        }
    }
}