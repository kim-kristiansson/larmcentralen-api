using System.Text.RegularExpressions;
using Larmcentralen.Application.Services;
using Larmcentralen.Domain.Interfaces;
using Microsoft.Extensions.Options;
namespace Larmcentralen.Api.Services;

public class EmailOptions
{
    public string ElksUsername { get; set; } = "";
    public string ElksPassword { get; set; } = "";
    public string AlertRecipient { get; set; } = "";
}

public class SmsAlarmService(
    IAlarmRepository alarmRepo,
    IOptions<EmailOptions> emailOptions,
    IOptions<SharePointOptions> spOptions,
    ILogger<SmsAlarmService> logger)
{
    private readonly EmailOptions _email = emailOptions.Value;
    private readonly SharePointOptions _sp = spOptions.Value;

    public async Task ProcessSmsAsync(string from, string message)
    {
        logger.LogInformation("SMS received from {From}: {Message}", from, message);

        var parsed = ParseSms(message);
        if (parsed is null)
        {
            logger.LogWarning("Could not parse SMS: {Message}", message);
            return;
        }

        var (equipment, alarmCode, description) = parsed.Value;

        // Look up alarm in database
        var alarms = await alarmRepo.SearchAsync(alarmCode, null, null, null, 0, 5);
        var alarm = alarms.FirstOrDefault(a =>
            a.AlarmCode == alarmCode &&
            a.Equipment.Title.Equals(equipment, StringComparison.OrdinalIgnoreCase));

        if (alarm is null)
        {
            logger.LogInformation("Alarm {Code} on {Equipment} not found in database", alarmCode, equipment);
            return;
        }

        if (alarm.Severity is not ("Hög" or "Kritisk"))
        {
            logger.LogInformation("Alarm {Code} on {Equipment} is {Severity} — ignoring", alarmCode, equipment, alarm.Severity);
            return;
        }

        // Send alert email
        await SendAlertSmsAsync(alarm.Equipment.Title, alarm.AlarmCode!, alarm.Title, alarm.Severity, description);
        logger.LogInformation("Alert email sent for {Severity} alarm: {Code} on {Equipment}", alarm.Severity, alarmCode, equipment);
    }

    private static (string Equipment, string AlarmCode, string Description)? ParseSms(string message)
    {
        // Format: "06:56 / T10-T7 Line : T102 : M206.6 Ingen svarssignal tandbrännare ytterlucka"
        var match = Regex.Match(message, @"Line\s*:\s*(\w+)\s*:\s*(\S+)\s*(.*)");
        if (!match.Success) return null;

        return (
            match.Groups[1].Value.Trim(),
            match.Groups[2].Value.Trim(),
            match.Groups[3].Value.Trim()
        );
    }

    private async Task SendAlertSmsAsync(string equipment, string alarmCode, string title, string severity, string description)
    {
        using var client = new HttpClient();
        var auth = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{_email.ElksUsername}:{_email.ElksPassword}"));

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "from", "Larmcentral" },
            { "to", _email.AlertRecipient },
            { "message", $"🚨 {severity}: {alarmCode} på {equipment}\n{title}\n{description}" }
        });

        client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
        var response = await client.PostAsync("https://api.46elks.com/a1/sms", content);
        var body = await response.Content.ReadAsStringAsync();
        logger.LogInformation("46elks response: {Status} {Body}", response.StatusCode, body);
    }
}