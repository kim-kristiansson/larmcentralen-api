using System.Net.Http.Json;
using Larmcentralen.Maui.Models;

namespace Larmcentralen.Maui.Services;

public class ApiClient(HttpClient http)
{
    // Alarms
    public async Task<List<AlarmListDto>> SearchAlarmsAsync(string? search = null, string? severity = null)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            parts.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(severity))
            parts.Add($"severity={Uri.EscapeDataString(severity)}");

        var url = "api/alarms";
        if (parts.Count > 0)
            url += "?" + string.Join("&", parts);

        return await http.GetFromJsonAsync<List<AlarmListDto>>(url) ?? [];
    }

    public async Task<AlarmDto?> GetAlarmAsync(int id)
    {
        return await http.GetFromJsonAsync<AlarmDto>($"api/alarms/{id}");
    }

    // Solutions
    public async Task<SolutionDto?> GetSolutionAsync(int id)
    {
        return await http.GetFromJsonAsync<SolutionDto>($"api/solutions/{id}");
    }
}