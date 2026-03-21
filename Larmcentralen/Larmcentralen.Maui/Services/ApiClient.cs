using System.Net.Http.Json;
using Larmcentralen.Maui.Models;

namespace Larmcentralen.Maui.Services;

public class ApiClient(HttpClient http)
{
    // Alarms
    public async Task<List<AlarmListDto>> SearchAlarmsAsync(string? search = null, string? severity = null, int? areaId = null, int? equipmentId = null)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            parts.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(severity))
            parts.Add($"severity={Uri.EscapeDataString(severity)}");
        if (areaId.HasValue)
            parts.Add($"areaId={areaId}");
        if (equipmentId.HasValue)
            parts.Add($"equipmentId={equipmentId}");

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
    
    // Create solution
    public async Task<SolutionDto?> CreateSolutionAsync(CreateSolutionDto dto)
    {
        var response = await http.PostAsJsonAsync("api/solutions", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SolutionDto>();
    }

    // Update solution
    public async Task UpdateSolutionAsync(int id, UpdateSolutionDto dto)
    {
        var response = await http.PutAsJsonAsync($"api/solutions/{id}", dto);
        response.EnsureSuccessStatusCode();
    }
    
    // Export alarms
    public async Task<(byte[] Bytes, string FileName)?> ExportAlarmAsync(int alarmId)
    {
        var response = await http.GetAsync($"api/alarms/{alarmId}/export");
        if (!response.IsSuccessStatusCode) return null;

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName
                       ?? $"alarm_{alarmId}.docx";

        return (bytes, fileName.Trim('"'));
    }
    
    public async Task DeleteSolutionAsync(int id)
    {
        var response = await http.DeleteAsync($"api/solutions/{id}");
        response.EnsureSuccessStatusCode();
    }
    
    public async Task DeleteAlarmAsync(int id)
    {
        var response = await http.DeleteAsync($"api/alarms/{id}");
        response.EnsureSuccessStatusCode();
    }
    
    public async Task<List<AreaDto>> GetAreasAsync()
    {
        return await http.GetFromJsonAsync<List<AreaDto>>("api/areas") ?? [];
    }

    public async Task<List<EquipmentDto>> GetEquipmentAsync(int? areaId = null)
    {
        var url = areaId.HasValue ? $"api/equipment?areaId={areaId}" : "api/equipment";
        return await http.GetFromJsonAsync<List<EquipmentDto>>(url) ?? [];
    }

    public async Task<AlarmDto?> CreateAlarmAsync(CreateAlarmDto dto)
    {
        var response = await http.PostAsJsonAsync("api/alarms", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AlarmDto>();
    }
    
    public async Task<string?> UploadToSharePointAsync(int alarmId)
    {
        var response = await http.PostAsync($"api/alarms/{alarmId}/upload-sharepoint", null);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<SharePointUploadResult>();
        return result?.Url;
    }

    private class SharePointUploadResult
    {
        public string? Url { get; set; }
    }
    
    
    public async Task<List<AlarmListDto>> GetAlarmsByIdsAsync(List<int> ids)
    {
        if (ids.Count == 0) return [];
        
        var query = string.Join("&", ids.Select(id => $"ids={id}"));
        return await http.GetFromJsonAsync<List<AlarmListDto>>($"api/alarms/by-ids?{query}") ?? [];
    }
}