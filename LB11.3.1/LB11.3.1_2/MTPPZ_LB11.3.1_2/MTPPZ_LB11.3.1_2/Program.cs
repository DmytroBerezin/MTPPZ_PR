using System;
using System.Collections.Generic;

namespace ItCompanyCommandPatternModule
{
    public interface IAdminCommand
    {
        void Execute();
        void Undo();
    }
    public class CloudServerReceiver
    {
        public string ServerName { get; }
        public bool IsRunning { get; private set; } = true;
        public int AllocatedRamGb { get; private set; }

        public CloudServerReceiver(string serverName, int initialRamGb)
        {
            ServerName = serverName;
            AllocatedRamGb = initialRamGb;
        }

        public void Restart()
        {
            Console.WriteLine($"  [Сервер {ServerName}] Виконується плановий перезапуск служб...");
            IsRunning = true;
        }

        public int ResizeRam(int newRamGb)
        {
            int previousRam = AllocatedRamGb;
            AllocatedRamGb = newRamGb;
            Console.WriteLine($"  [Сервер {ServerName}] Обсяг RAM змінено: {previousRam} ГБ -> {AllocatedRamGb} ГБ");
            return previousRam;
        }
    }

    public class RestartServerCommand : IAdminCommand
    {
        private readonly CloudServerReceiver _server;

        public RestartServerCommand(CloudServerReceiver server)
        {
            _server = server;
        }

        public void Execute() => _server.Restart();

        public void Undo()
        {
            Console.WriteLine($"  [Rollback] Перезапуск сервера '{_server.ServerName}' не потребує зворотного вимкнення (зафіксовано в журналі).");
        }
    }

    public class ScaleRamCommand : IAdminCommand
    {
        private readonly CloudServerReceiver _server;
        private readonly int _targetRamGb;
        private int _previousRamGb;

        public ScaleRamCommand(CloudServerReceiver server, int targetRamGb)
        {
            _server = server;
            _targetRamGb = targetRamGb;
        }

        public void Execute()
        {
            _previousRamGb = _server.ResizeRam(_targetRamGb);
        }

        public void Undo()
        {
            Console.WriteLine($"  [Rollback] Відновлення попередньої конфігурації RAM для '{_server.ServerName}':");
            _server.ResizeRam(_previousRamGb);
        }
    }

    public class ServiceDeskInvoker
    {
        private readonly Queue<IAdminCommand> _taskQueue = new();
        private readonly Stack<IAdminCommand> _executedHistory = new();

        public void EnqueueCommand(IAdminCommand command)
        {
            _taskQueue.Enqueue(command);
        }

        public void ProcessQueue()
        {
            Console.WriteLine("=== ВИКОНАННЯ ЧЕРГИ ЗАЯВОК ІТ-ПІДТРИМКИ ===");
            while (_taskQueue.Count > 0)
            {
                var cmd = _taskQueue.Dequeue();
                cmd.Execute();
                _executedHistory.Push(cmd);
            }
        }

        public void RollbackLastOperation()
        {
            if (_executedHistory.Count > 0)
            {
                Console.WriteLine("\n=== ВІДКАТ ОСТАННЬОЇ ОПЕРАЦІЇ ===");
                var last = _executedHistory.Pop();
                last.Undo();
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var clientServer = new CloudServerReceiver("CLIENT-CRM-PROD", 16);
            var dispatcher = new ServiceDeskInvoker();

            dispatcher.EnqueueCommand(new RestartServerCommand(clientServer));
            dispatcher.EnqueueCommand(new ScaleRamCommand(clientServer, 64));

            dispatcher.ProcessQueue();

            dispatcher.RollbackLastOperation();
        }
    }
}