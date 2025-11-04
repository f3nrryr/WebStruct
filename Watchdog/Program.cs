using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static TelegramBotClient _botClient;
    private static string _botToken = "YOUR_BOT_TOKEN";
    private static string _chatId = "YOUR_CHAT_ID";
    private static string _healthCheckUrl = "https://your-service-url/hc";

    // Храним предыдущий статус для сравнения
    private static string _lastOverallStatus = null;
    private static Dictionary<string, string> _lastUnhealthyServices = new Dictionary<string, string>();

    static async Task Main(string[] args)
    {
        _botClient = new TelegramBotClient(_botToken);

        Console.WriteLine("🐕 SMART WATCHDOG STARTED - Press Ctrl+C to stop");
        Console.WriteLine("📊 Will only send alerts when status CHANGES");

        while (true)
        {
            try
            {
                await CheckHealthAndNotify();
                await Task.Delay(TimeSpan.FromMinutes(1)); // Проверяем каждую минуту
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                SendTelegramAlert($"🔥 WATCHDOG ERROR: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }

    static async Task CheckHealthAndNotify()
    {
        var response = await _httpClient.GetStringAsync(_healthCheckUrl);
        var healthData = JsonSerializer.Deserialize<HealthCheckResponse>(response);

        if (healthData?.Entries != null)
        {
            var currentUnhealthyServices = new Dictionary<string, string>();
            var currentOverallStatus = healthData.Status;

            // Собираем текущие unhealthy сервисы
            foreach (var entry in healthData.Entries)
            {
                if (entry.Value.Status != "Healthy")
                {
                    var description = string.IsNullOrEmpty(entry.Value.Description)
                        ? "No description"
                        : entry.Value.Description;

                    currentUnhealthyServices[entry.Key] = description;
                }
            }

            // Проверяем, изменился ли статус
            bool statusChanged = HasStatusChanged(currentOverallStatus, currentUnhealthyServices);

            if (statusChanged)
            {
                var message = BuildStatusMessage(currentOverallStatus, currentUnhealthyServices, healthData.TotalDuration);
                SendTelegramAlert(message);

                // Обновляем предыдущий статус
                _lastOverallStatus = currentOverallStatus;
                _lastUnhealthyServices = new Dictionary<string, string>(currentUnhealthyServices);

                Console.WriteLine($"📢 STATUS CHANGED - Alert sent at {DateTime.Now:HH:mm:ss}");
            }
            else
            {
                Console.WriteLine($"⚡ Status unchanged - {DateTime.Now:HH:mm:ss}");
            }
        }
    }

    static bool HasStatusChanged(string currentOverallStatus, Dictionary<string, string> currentUnhealthyServices)
    {
        // Первая проверка - отправляем всё
        if (_lastOverallStatus == null)
            return true;

        // Изменился общий статус (Healthy/Unhealthy/Degraded)
        if (_lastOverallStatus != currentOverallStatus)
            return true;

        // Изменилось количество unhealthy сервисов
        if (_lastUnhealthyServices.Count != currentUnhealthyServices.Count)
            return true;

        // Изменился состав unhealthy сервисов
        foreach (var service in currentUnhealthyServices)
        {
            if (!_lastUnhealthyServices.ContainsKey(service.Key) ||
                _lastUnhealthyServices[service.Key] != service.Value)
            {
                return true;
            }
        }

        // Проверяем, не исчезли ли какие-то сервисы
        foreach (var service in _lastUnhealthyServices)
        {
            if (!currentUnhealthyServices.ContainsKey(service.Key))
            {
                return true;
            }
        }

        return false;
    }

    static string BuildStatusMessage(string overallStatus, Dictionary<string, string> unhealthyServices, string totalDuration)
    {
        if (unhealthyServices.Count > 0)
        {
            var servicesList = unhealthyServices.Select(s => $"🔴 {s.Key}: {s.Value}").ToArray();

            return $"🚨 HEALTH CHECK STATUS CHANGED 🚨\n\n" +
                   $"📊 Overall Status: {overallStatus}\n" +
                   $"⏱️ Total Duration: {totalDuration}\n\n" +
                   $"🔴 UNHEALTHY SERVICES ({unhealthyServices.Count}):\n{string.Join("\n", servicesList)}\n\n" +
                   $"🕒 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
        else
        {
            return $"✅ ALL SERVICES RECOVERED ✅\n\n" +
                   $"📊 Overall Status: {overallStatus}\n" +
                   $"⏱️ Total Duration: {totalDuration}\n\n" +
                   $"🎉 All services are now healthy!\n\n" +
                   $"🕒 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
    }

    static void SendTelegramAlert(string message)
    {
        try
        {
            _botClient.SendMessage(_chatId, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TELEGRAM SEND ERROR: {ex.Message}");
        }
    }
}

// Модели для парсинга JSON
public class HealthCheckResponse
{
    public string Status { get; set; }
    public string TotalDuration { get; set; }
    public Dictionary<string, HealthCheckEntry> Entries { get; set; }
}

public class HealthCheckEntry
{
    public string Status { get; set; }
    public string Description { get; set; }
    public string Duration { get; set; }
    public Dictionary<string, object> Data { get; set; }
}