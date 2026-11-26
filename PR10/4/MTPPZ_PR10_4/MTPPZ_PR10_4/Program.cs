using System;

namespace Task4_InformationHiding
{
    public abstract class Vehicle
    {
        private bool _isMoving;
        public bool IsMoving => _isMoving;
        protected string Model { get; }

        protected Vehicle(string model)
        {
            Model = model;
        }

        protected void SetMovingState(bool state) => _isMoving = state;

        public abstract void Start();
        public abstract void Stop();
    }

    public class Car : Vehicle
    {
        private double _fuelLevel;
        private int _engineTemperature;

        public double FuelLevel => _fuelLevel;

        public Car(string model, double initialFuel) : base(model)
        {
            _fuelLevel = initialFuel > 0 ? initialFuel : 0;
            _engineTemperature = 20;
        }
        private bool CheckEngineDiagnostics() => _fuelLevel > 0.5 && _engineTemperature < 110;

        public void Refuel(double liters)
        {
            if (liters <= 0)
                throw new ArgumentException("Об'єм пального має бути більшим за нуль.");
            _fuelLevel += liters;
        }

        public override void Start()
        {
            if (!CheckEngineDiagnostics())
            {
                Console.WriteLine($"Автомобіль {Model}: запуск заблоковано (недостатньо пального або перегрів).");
                return;
            }

            _fuelLevel -= 0.5;
            _engineTemperature = 85;
            SetMovingState(true);
            Console.WriteLine($"Автомобіль {Model}: рух розпочато. Залишок пального: {_fuelLevel:F1} л.");
        }

        public override void Stop()
        {
            SetMovingState(false);
            Console.WriteLine($"Автомобіль {Model}: зупинено.");
        }
    }

    public class Bicycle : Vehicle
    {
        private int _currentGear = 1;
        public int CurrentGear => _currentGear;

        public Bicycle(string model) : base(model) { }

        public void ShiftGear(int gear)
        {
            if (gear < 1 || gear > 21)
                throw new ArgumentOutOfRangeException(nameof(gear), "Передача має бути в межах від 1 до 21.");
            _currentGear = gear;
        }

        public override void Start()
        {
            SetMovingState(true);
            Console.WriteLine($"Велосипед {Model}: рух розпочато на {_currentGear}-й передачі.");
        }

        public override void Stop()
        {
            SetMovingState(false);
            Console.WriteLine($"Велосипед {Model}: зупинено.");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var car = new Car("Mazda 6", 15.0);
            car.Start();
            car.Stop();

            var bike = new Bicycle("Giant Talon");
            bike.ShiftGear(5);
            bike.Start();
            bike.Stop();
        }
    }
}