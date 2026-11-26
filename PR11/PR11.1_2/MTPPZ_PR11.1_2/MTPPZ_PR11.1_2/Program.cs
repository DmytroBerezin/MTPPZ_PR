using System;

namespace Practical11_AbstractFactory
{

    public interface IButton
    {
        void Render();
        void OnClick();
    }

    public interface ICheckbox
    {
        void Render();
        void OnClick();
    }

    public class WindowsButton : IButton
    {
        public void Render() => Console.WriteLine("[Windows UI] Відмальовано кнопку у стилі Fluent Design (прямокутна рамка).");
        public void OnClick() => Console.WriteLine("[Windows UI] Натиснуто кнопку Windows: виконано системну дію.");
    }

    public class WindowsCheckbox : ICheckbox
    {
        private bool _isChecked;

        public void Render() => Console.WriteLine($"[Windows UI] Відмальовано прапорець Windows (стан: {(_isChecked ? "Увімкнено" : "Вимкнено")}).");

        public void OnClick()
        {
            _isChecked = !_isChecked;
            Console.WriteLine($"[Windows UI] Перемкнуто прапорець Windows -> новий стан: {(_isChecked ? "[✓]" : "[ ]")}");
        }
    }

    public class MacButton : IButton
    {
        public void Render() => Console.WriteLine("[macOS UI] Відмальовано кнопку у стилі Aqua / SwiftUI (заокруглені краї).");
        public void OnClick() => Console.WriteLine("[macOS UI] Натиснуто кнопку macOS: виконано дію в середовищі Cocoa.");
    }

    public class MacCheckbox : ICheckbox
    {
        private bool _isChecked;

        public void Render() => Console.WriteLine($"[macOS UI] Відмальовано перемикач macOS (стан: {(_isChecked ? "Активний" : "Неактивний")}).");

        public void OnClick()
        {
            _isChecked = !_isChecked;
            Console.WriteLine($"[macOS UI] Перемкнуто прапорець macOS -> новий стан: {(_isChecked ? "(•)" : "( )")}");
        }
    }

    public interface IUIFactory
    {
        IButton CreateButton();
        ICheckbox CreateCheckbox();
    }

    public class WindowsFactory : IUIFactory
    {
        public IButton CreateButton() => new WindowsButton();
        public ICheckbox CreateCheckbox() => new WindowsCheckbox();
    }

    public class MacFactory : IUIFactory
    {
        public IButton CreateButton() => new MacButton();
        public ICheckbox CreateCheckbox() => new MacCheckbox();
    }

    public class GuiApplication
    {
        private readonly IButton _button;
        private readonly ICheckbox _checkbox;

        public GuiApplication(IUIFactory factory)
        {
            _button = factory.CreateButton();
            _checkbox = factory.CreateCheckbox();
        }

        public void RenderUI()
        {
            _button.Render();
            _checkbox.Render();
        }

        public void SimulateUserInteraction()
        {
            _button.OnClick();
            _checkbox.OnClick();
        }
    }

    class Program
    {
        static IUIFactory GetFactoryByPlatform(string platformName)
        {
            return platformName.Trim().ToLower() switch
            {
                "windows" => new WindowsFactory(),
                "mac" or "macos" => new MacFactory(),
                _ => throw new NotSupportedException($"Платформа '{platformName}' не підтримується.")
            };
        }

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string[] platforms = { "Windows", "Mac" };

            foreach (var os in platforms)
            {
                Console.WriteLine($"=== Ініціалізація GUI-тюнера для платформи: {os} ===");
                IUIFactory factory = GetFactoryByPlatform(os);
                var app = new GuiApplication(factory);

                app.RenderUI();
                app.SimulateUserInteraction();
                Console.WriteLine();
            }
        }
    }
}