using System;
using System.Collections.Generic;

namespace Practical11_2_Composite
{

    public abstract class CourseComponent
    {
        public string Title { get; }

        protected CourseComponent(string title)
        {
            Title = title;
        }

        public virtual void Add(CourseComponent component)
        {
            throw new NotSupportedException("Неможливо додати підкомпонент до окремого уроку.");
        }

        public abstract void Display(string indent = "");
    }


    public class Lesson : CourseComponent
    {
        public int DurationMinutes { get; }

        public Lesson(string title, int durationMinutes) : base(title)
        {
            DurationMinutes = durationMinutes;
        }

        public override void Display(string indent = "")
        {
            Console.WriteLine($"{indent}- Урок: \"{Title}\" ({DurationMinutes} хв)");
        }
    }

    public class Module : CourseComponent
    {
        private readonly List<CourseComponent> _components = new();

        public Module(string title) : base(title) { }

        public override void Add(CourseComponent component)
        {
            _components.Add(component);
        }

        public override void Display(string indent = "")
        {
            Console.WriteLine($"{indent}+ [Модуль] {Title}");
            foreach (var component in _components)
            {
                component.Display(indent + "   ");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var mainCourse = new Module("Проєктування програмного забезпечення на C#");

            var module1 = new Module("Модуль 1. Принципи SOLID");
            module1.Add(new Lesson("Single Responsibility та Open/Closed", 25));
            module1.Add(new Lesson("Liskov Substitution, ISP та DIP", 35));

            var module2 = new Module("Модуль 2. Шаблони проєктування GoF");

            var creationalSubModule = new Module("Підмодуль 2.1. Породжувальні патерни");
            creationalSubModule.Add(new Lesson("Singleton та Builder", 30));
            creationalSubModule.Add(new Lesson("Factory Method та Abstract Factory", 40));

            var structuralSubModule = new Module("Підмодуль 2.2. Структурні патерни");
            structuralSubModule.Add(new Lesson("Adapter та Composite", 30));
            structuralSubModule.Add(new Lesson("Proxy та Decorator", 35));

            module2.Add(creationalSubModule);
            module2.Add(structuralSubModule);

            var finalLesson = new Lesson("Підсумковий практичний проєкт", 60);

            mainCourse.Add(module1);
            mainCourse.Add(module2);
            mainCourse.Add(finalLesson);

            mainCourse.Display();
        }
    }
}