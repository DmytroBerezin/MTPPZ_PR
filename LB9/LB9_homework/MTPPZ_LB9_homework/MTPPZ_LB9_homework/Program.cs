using System;
using System.Collections.Generic;

namespace ItCompanyDeploymentModule
{
    public class TelemetryServerNode
    {
        private readonly List<string> _incidentLogs = new();

        public void SaveIncident(string sourceNode, string ticketTitle, string severity)
        {
            string record = $"[{DateTime.Now:HH:mm:ss}] Вузол: {sourceNode} | Тікет: '{ticketTitle}' | Рівень: {severity}";
            _incidentLogs.Add(record);
            Console.WriteLine($"  [TelemetryServer] Збережено в БД: {record}");
        }

        public void PrintAllRecords()
        {
            Console.WriteLine("\n=== ЖУРНАЛ ІНЦИДЕНТІВ НА ТЕЛЕМЕТРИЧНОМУ СЕРВЕРІ ===");
            foreach (var log in _incidentLogs)
            {
                Console.WriteLine(log);
            }
        }
    }

    public class MonitoringAgentContainer
    {
        private readonly TelemetryServerNode _telemetryDb;

        public MonitoringAgentContainer(TelemetryServerNode telemetryDb)
        {
            _telemetryDb = telemetryDb;
        }

        public void CheckClientServerHealth(string clientServerName, int cpuLoadPercent)
        {
            Console.WriteLine($"[Docker: MonitoringAgent.exe] Перевірка '{clientServerName}': CPU = {cpuLoadPercent}%");
            if (cpuLoadPercent > 85)
            {
                _telemetryDb.SaveIncident("MonitoringAgent.exe", $"Критичне навантаження CPU ({cpuLoadPercent}%) на {clientServerName}", "HIGH");
            }
        }
    }

    public class ApiGatewayContainer
    {
        private readonly TelemetryServerNode _telemetryDb;

        public ApiGatewayContainer(TelemetryServerNode telemetryDb)
        {
            _telemetryDb = telemetryDb;
        }

        public void ReceiveClientTicket(string clientName, string issueDescription)
        {
            Console.WriteLine($"[Docker: ItService.Gateway.dll] HTTPS запит від клієнта '{clientName}'");
            _telemetryDb.SaveIncident("ServiceDesk.WebPortal", $"{clientName}: {issueDescription}", "NORMAL");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var telemetryServer = new TelemetryServerNode();
            var apiGateway = new ApiGatewayContainer(telemetryServer);
            var monitoringAgent = new MonitoringAgentContainer(telemetryServer);

            apiGateway.ReceiveClientTicket("ТОВ 'АгроТрейд'", "Потрібно налаштувати доступ новим співробітникам");
            monitoringAgent.CheckClientServerHealth("PROD-DB-CLIENT-01", 94);

            telemetryServer.PrintAllRecords();
        }
    }
}