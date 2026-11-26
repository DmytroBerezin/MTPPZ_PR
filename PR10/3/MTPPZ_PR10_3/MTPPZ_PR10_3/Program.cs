using System;

namespace Task3_Abstraction
{
    public abstract class Vehicle
    {
        public string Model { get; }

        protected Vehicle(string model)
        {
            Model = model;
        }

        public abstract void Start();
        public abstract void Stop();
    }

    public class Car : Vehicle
    {
        public Car(string model) : base(model) { }

        public override void Start() => Console.WriteLine($"Автомобіль {Model}: двигун запущено, рух розпочато.");
        public override void Stop() => Console.WriteLine($"Автомобіль {Model}: натиснуто гальма, двигун зупинено.");
    }

    public class Bicycle : Vehicle
    {
        public Bicycle(string model) : base(model) { }

        public override void Start() => Console.WriteLine($"Велосипед {Model}: крутимо педалі, рух розпочато.");
        public override void Stop() => Console.WriteLine($"Велосипед {Model}: натиснуто ручні гальма, зупинка.");
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Vehicle[] vehicles = { new Car("Toyota Corolla"), new Bicycle("Trek Marlin 7") };
            foreach (var v in vehicles)
            {
                v.Start();
                v.Stop();
            }
        }
    }
}