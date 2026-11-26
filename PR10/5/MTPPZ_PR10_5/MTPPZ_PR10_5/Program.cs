using System;

namespace Task5_TDD
{
    public class CalculatorTests
    {
        public static void RunAllTests()
        {
            TestAdd();
            TestSubtract();
            TestMultiply();
            TestDivide();
            TestDivideByZero_ThrowsException();
            Console.WriteLine("Усі TDD-тести пройдено успішно (GREEN)!");
        }

        public static void TestAdd()
        {
            var calc = new Calculator();
            double result = calc.Add(4, 6);
            if (result != 10)
                throw new Exception($"TestAdd failed: expected 10, got {result}");
        }

        public static void TestSubtract()
        {
            var calc = new Calculator();
            double result = calc.Subtract(10, 3);
            if (result != 7)
                throw new Exception($"TestSubtract failed: expected 7, got {result}");
        }

        public static void TestMultiply()
        {
            var calc = new Calculator();
            double result = calc.Multiply(5, 4);
            if (result != 20)
                throw new Exception($"TestMultiply failed: expected 20, got {result}");
        }

        public static void TestDivide()
        {
            var calc = new Calculator();
            double result = calc.Divide(15, 3);
            if (result != 5)
                throw new Exception($"TestDivide failed: expected 5, got {result}");
        }

        public static void TestDivideByZero_ThrowsException()
        {
            var calc = new Calculator();
            try
            {
                calc.Divide(10, 0);
                throw new Exception("TestDivideByZero failed: exception was not thrown");
            }
            catch (DivideByZeroException)
            {
                // Очікувана поведінка
            }
        }
    }

    public class Calculator
    {
        public double Add(double a, double b) => a + b;

        public double Subtract(double a, double b) => a - b;

        public double Multiply(double a, double b) => a * b;

        public double Divide(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("Ділення на нуль заборонено.");
            return a / b;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CalculatorTests.RunAllTests();
        }
    }
}