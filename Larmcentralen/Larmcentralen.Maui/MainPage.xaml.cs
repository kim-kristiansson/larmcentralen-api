using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui;

public partial class MainPage : ContentPage
{
    private readonly ApiClient _api;
    private CancellationTokenSource? _debounce;

    public MainPage(ApiClient api)
    {
        InitializeComponent();
        _api = api;
        LoadAlarms();
    }

    private async void LoadAlarms(string? search = null)
    {
        try
        {
            var alarms = await _api.SearchAlarmsAsync(search);
            AlarmList.ItemsSource = alarms;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta larm: {ex.Message}", "OK");
        }
    }

    private void OnSearch(object? sender, EventArgs e)
    {
        LoadAlarms(SearchBar.Text);
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        var token = _debounce.Token;

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

    private async void OnAlarmSelected(object? sender, SelectionChangedEventArgs e)
    {
        // Next step — navigate to detail
    }
}