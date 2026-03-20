using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui;

public partial class MainPage : ContentPage
{
    private readonly ApiClient _api;
    private CancellationTokenSource? _debounce;
    private string? _activeSeverityFilter;
    private bool _hasSearched;

    public MainPage(ApiClient api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void LoadAlarms(string? search = null, string? severity = null)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            ResultCount.Text = "";

            var alarms = await _api.SearchAlarmsAsync(search, severity);
            
            if (_hasSearched && alarms.Count == 0)
            {
                EmptyTitle.Text = "Inga larm matchade din sökning";
                EmptySubtitle.IsVisible = false;
            }
            else
            {
                EmptyTitle.Text = "Sök efter larmkod, utrustning eller beskrivning";
                EmptySubtitle.IsVisible = true;
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

    private void OnSearch(object? sender, EventArgs e)
    {
        LoadAlarms(SearchEntry.Text, _activeSeverityFilter);
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ClearButton.IsVisible = !string.IsNullOrWhiteSpace(e.NewTextValue);

        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;

        Task.Delay(300, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LoadAlarms(e.NewTextValue, _activeSeverityFilter);
                });
            }
        }, TaskScheduler.Default);
    }

    private void OnClearSearch(object? sender, TappedEventArgs e)
    {
        SearchEntry.Text = "";
        _activeSeverityFilter = null;
        LoadAlarms();
    }

    private void OnFilterAll(object? sender, TappedEventArgs e)
    {
        _activeSeverityFilter = null;
        LoadAlarms(SearchEntry.Text);
    }

    private void OnFilterKritisk(object? sender, TappedEventArgs e)
    {
        _activeSeverityFilter = "Kritisk";
        LoadAlarms(SearchEntry.Text, "Kritisk");
    }

    private void OnFilterHög(object? sender, TappedEventArgs e)
    {
        _activeSeverityFilter = "Hög";
        LoadAlarms(SearchEntry.Text, "Hög");
    }

    private void OnFilterMedel(object? sender, TappedEventArgs e)
    {
        _activeSeverityFilter = "Medel";
        LoadAlarms(SearchEntry.Text, "Medel");
    }

    private void OnFilterLåg(object? sender, TappedEventArgs e)
    {
        _activeSeverityFilter = "Låg";
        LoadAlarms(SearchEntry.Text, "Låg");
    }

    private async void OnAlarmSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is AlarmListDto alarm)
        {
            await Navigation.PushAsync(new Views.AlarmDetailPage(_api, alarm.Id));
            AlarmList.SelectedItem = null;
        }
    }
}