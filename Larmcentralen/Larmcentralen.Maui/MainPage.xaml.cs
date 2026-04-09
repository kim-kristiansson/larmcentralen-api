using Larmcentralen.Maui.Helpers;
using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui;

public partial class MainPage : ContentPage
{
    private List<AlarmListDto> _allResults = [];
    private bool _isLoadingMore;
    private bool _hasMoreResults = true;
    private string? _currentSearch;
    
    private readonly ApiClient _api;

    private List<AreaDto> _areas = [];
    private List<EquipmentDto> _allEquipment = [];

    private int? _selectedAreaId;
    private int? _selectedEquipmentId;
    private string? _selectedSeverity;
    
    private DateTime _lastSearch = DateTime.MinValue;
    private CancellationTokenSource? _debounce;
    
    private CancellationTokenSource? _searchCts;
    private readonly WebViewPool _pool;

    public MainPage(ApiClient api, WebViewPool pool)
    {
        InitializeComponent();
        _api = api;
        _pool = pool;
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

        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            ResultCount.Text = "";

            _currentSearch = search;
            _allResults = [];
            _hasMoreResults = true;

            var alarms = await _api.SearchAlarmsAsync(search, _selectedSeverity, _selectedAreaId, _selectedEquipmentId, 0, 10);

            if (token.IsCancellationRequested) return;

            _allResults = alarms;
            _hasMoreResults = alarms.Count == 10;

            if (alarms.Count == 0)
            {
                EmptyTitle.Text = "Inga larm matchade din sökning";
                EmptySubtitle.IsVisible = false;
            }

            AlarmList.ItemsSource = _allResults;

            ResultCount.Text = alarms.Count switch
            {
                0 => "",
                1 => "1 larm",
                _ => $"{alarms.Count} larm"
            };
            
            LoadMoreButton.IsVisible = _hasMoreResults;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta larm: {ex.Message}", "OK");
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }
    }

    private async void OnLoadMore(object? sender, EventArgs e)
    {
        if (_isLoadingMore || !_hasMoreResults || string.IsNullOrWhiteSpace(_currentSearch)) return;

        _isLoadingMore = true;
        LoadMoreButton.Text = "Laddar...";

        try
        {
            var more = await _api.SearchAlarmsAsync(_currentSearch, _selectedSeverity, _selectedAreaId, _selectedEquipmentId, _allResults.Count, 10);

            if (more.Count == 0)
            {
                _hasMoreResults = false;
                LoadMoreButton.IsVisible = false;
                return;
            }

            _allResults.AddRange(more);
            _hasMoreResults = more.Count == 10;
            AlarmList.ItemsSource = null;
            AlarmList.ItemsSource = _allResults;
            ResultCount.Text = $"{_allResults.Count} larm";
            LoadMoreButton.IsVisible = _hasMoreResults;
        }
        catch { }
        finally
        {
            _isLoadingMore = false;
            LoadMoreButton.Text = "Visa fler...";
        }
    }

    private async Task LoadRecentAlarms()
    {
        LoadMoreButton.IsVisible = false;
        
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

        var timeSinceLast = DateTime.UtcNow - _lastSearch;

        if (timeSinceLast.TotalMilliseconds > 1000)
        {
            // Fire immediately if it's been a while
            _lastSearch = DateTime.UtcNow;
            LoadAlarms(e.NewTextValue);
        }
        else
        {
            // Debounce subsequent keystrokes
            Task.Delay(600, token).ContinueWith(_ =>
            {
                if (!token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _lastSearch = DateTime.UtcNow;
                        LoadAlarms(e.NewTextValue);
                    });
                }
            }, TaskScheduler.Default);
        }
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
            await Navigation.PushAsync(new Views.AlarmDetailPage(_api, _pool, alarm.Id));
            AlarmList.SelectedItem = null;
        }
    }
}