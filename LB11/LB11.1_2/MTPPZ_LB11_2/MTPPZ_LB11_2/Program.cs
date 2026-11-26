using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab11_Singleton
{
    public sealed class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _instance =
            new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        private readonly Dictionary<string, string> _settings;
        private readonly string _configFilePath = "appsettings.json";

        public static ConfigurationManager Instance => _instance.Value;

        private ConfigurationManager()
        {
            Console.WriteLine("[ConfigurationManager] Ініціалізація єдиного екземпляра та завантаження конфігурації...");
            _settings = LoadConfiguration();
        }

        private Dictionary<string, string> LoadConfiguration()
        {
            if (!File.Exists(_configFilePath))
            {
                var defaultSettings = new Dictionary<string, string>
                {
                    { "AppName", "VendingAndShopSystem" },
                    { "Environment", "Production" },
                    { "DbConnection", "Host=localhost;Database=shop_db;Username=admin" },
                    { "MaxRetryCount", "3" }
                };

                string json = JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configFilePath, json);
            }

            string fileContent = File.ReadAllText(_configFilePath);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent)
                   ?? new Dictionary<string, string>();
        }

        public string GetSetting(string key)
        {
            return _settings.TryGetValue(key, out string? value)
                ? value
                : $"[Параметр '{key}' не знайдено]";
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ConfigurationManager config1 = ConfigurationManager.Instance;
            ConfigurationManager config2 = ConfigurationManager.Instance;

            Console.WriteLine($"AppName: {config1.GetSetting("AppName")}");
            Console.WriteLine($"DbConnection: {config2.GetSetting("DbConnection")}");
            Console.WriteLine($"UnknownKey: {config1.GetSetting("Timeout")}");

            bool areSameReference = ReferenceEquals(config1, config2);
            Console.WriteLine($"\nconfig1 та config2 вказують на один і той самий об'єкт: {areSameReference}");
            Console.WriteLine($"HashCode config1: {config1.GetHashCode()} | HashCode config2: {config2.GetHashCode()}");

            Console.WriteLine("\nПеревірка доступу з 5 паралельних потоків:");
            Parallel.For(0, 5, i =>
            {
                var threadInstance = ConfigurationManager.Instance;
                Console.WriteLine($"Потік #{i}: HashCode екземпляра = {threadInstance.GetHashCode()}");
            });
        }
    }
}