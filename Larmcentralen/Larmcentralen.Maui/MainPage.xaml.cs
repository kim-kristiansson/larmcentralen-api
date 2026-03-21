using Larmcentralen.Maui.Helpers;
using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui;

public partial class MainPage : ContentPage
{
    private readonly ApiClient _api;
    private CancellationTokenSource? _debounce;

    private List<AreaDto> _areas = [];
    private List<EquipmentDto> _allEquipment = [];

    private int? _selectedAreaId;
    private int? _selectedEquipmentId;
    private string? _selectedSeverity;

    public MainPage(ApiClient api)
    {
        InitializeComponent();
        _api = api;
        SeverityFilterPicker.SelectedIndex = 0;
        LoadFilters();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (string.IsNullOrWhiteSpace(SearchEntry.Text))
            LoadAlarms(null);
    }

    private async void LoadFilters()
    {
        try
        {
            _areas = await _api.GetAreasAsync();
            var areaNames = new List<string> { "Alla" };
            areaNames.AddRange(_areas.Select(a => a.DisplayName ?? a.Title));
            AreaPicker.ItemsSource = areaNames;
            AreaPicker.SelectedIndex = 0;

            _allEquipment = await _api.GetEquipmentAsync();
            UpdateEquipmentPicker();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta filter: {ex.Message}", "OK");
        }
    }

    private void UpdateEquipmentPicker()
    {
        var filtered = _selectedAreaId.HasValue
            ? _allEquipment.Where(e => e.AreaId == _selectedAreaId.Value).ToList()
            : _allEquipment;

        var names = new List<string> { "Alla" };
        names.AddRange(filtered.Select(e => e.DisplayName ?? e.Title));
        EquipmentPicker.ItemsSource = names;
        EquipmentPicker.SelectedIndex = 0;
        _selectedEquipmentId = null;
    }

    private void OnAreaFilterChanged(object? sender, EventArgs e)
    {
        if (AreaPicker.SelectedIndex <= 0)
        {
            _selectedAreaId = null;
        }
        else
        {
            _selectedAreaId = _areas[AreaPicker.SelectedIndex - 1].Id;
        }

        UpdateEquipmentPicker();
        TriggerSearchIfNeeded();
    }

    private void OnEquipmentFilterChanged(object? sender, EventArgs e)
    {
        if (EquipmentPicker.SelectedIndex <= 0)
        {
            _selectedEquipmentId = null;
        }
        else
        {
            var filtered = _selectedAreaId.HasValue
                ? _allEquipment.Where(eq => eq.AreaId == _selectedAreaId.Value).ToList()
                : _allEquipment;

            _selectedEquipmentId = filtered[EquipmentPicker.SelectedIndex - 1].Id;
        }

        TriggerSearchIfNeeded();
    }

    private void OnSeverityFilterChanged(object? sender, EventArgs e)
    {
        if (SeverityFilterPicker.SelectedIndex <= 0)
        {
            _selectedSeverity = null;
        }
        else
        {
            _selectedSeverity = SeverityFilterPicker.SelectedItem?.ToString();
        }

        TriggerSearchIfNeeded();
    }

    private void TriggerSearchIfNeeded()
    {
        if (!string.IsNullOrWhiteSpace(SearchEntry.Text))
        {
            LoadAlarms(SearchEntry.Text);
        }
    }

    private async void LoadAlarms(string? search = null)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            await LoadRecentAlarms();
            return;
        }

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            ResultCount.Text = "";

            var alarms = await _api.SearchAlarmsAsync(search, _selectedSeverity, _selectedAreaId, _selectedEquipmentId);

            if (alarms.Count == 0)
            {
                EmptyTitle.Text = "Inga larm matchade din sökning";
                EmptySubtitle.IsVisible = false;
            }

            AlarmList.ItemsSource = alarms;

            ResultCount.Text = alarms.Count switch
            {
                0 => "",
                1 => "1 larm",
                _ => $"{alarms.Count} larm"
            };
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta larm: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async Task LoadRecentAlarms()
    {
        var recentIds = RecentAlarmsHelper.Get();
        if (recentIds.Count == 0)
        {
            AlarmList.ItemsSource = null;
            ResultCount.Text = "";
            EmptyTitle.Text = "Sök efter larmkod, utrustning eller beskrivning";
            EmptySubtitle.IsVisible = true;
            return;
        }

        try
        {
            var alarms = await _api.GetAlarmsByIdsAsync(recentIds);
            AlarmList.ItemsSource = alarms;
            ResultCount.Text = $"Senast visade ({alarms.Count})";
            EmptyTitle.Text = "Sök efter larmkod, utrustning eller beskrivning";
            EmptySubtitle.IsVisible = true;
        }
        catch
        {
            AlarmList.ItemsSource = null;
            ResultCount.Text = "";
        }
    }

    private void OnSearch(object? sender, EventArgs e)
    {
        LoadAlarms(SearchEntry.Text);
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;

        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            LoadAlarms(null);
            return;
        }

        Task.Delay(300, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LoadAlarms(e.NewTextValue);
                });
            }
        }, TaskScheduler.Default);
    }

    private async void OnAddAlarm(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.AlarmEditorPage(_api));
    }
    
    private async void OnAlarmSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is AlarmListDto alarm)
        {
            RecentAlarmsHelper.Add(alarm.Id);
            await Navigation.PushAsync(new Views.AlarmDetailPage(_api, alarm.Id));
            AlarmList.SelectedItem = null;
        }
    }
}