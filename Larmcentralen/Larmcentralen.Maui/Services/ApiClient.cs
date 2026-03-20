using System.Net.Http.Json;
using Larmcentralen.Maui.Models;

namespace Larmcentralen.Maui.Services;

public class ApiClient(HttpClient http)
{
    // Alarms
    public async Task<List<AlarmListDto>> SearchAlarmsAsync(string? search = null)
    {
        var url = "api/alarms";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"?search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<List<AlarmListDto>>(url) ?? [];
    }

    public async Task<AlarmDto?> GetAlarmAsync(int id)
    {
        return await http.GetFromJsonAsync<AlarmDto>($"api/alarms/{id}");
    }

    // Solutions
    public async Task<List<SolutionDto>> GetSolutionsAsync(int alarmId)
    {
        return await http.GetFromJsonAsync<List<SolutionDto>>($"api/solutions/by-alarm/{alarmId}") ?? [];
    }
}