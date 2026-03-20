using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui;

public partial class MainPage : ContentPage
{
    private readonly ApiClient _api;

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
            await DisplayAlert("Fel", $"Kunde inte hämta larm: {ex.Message}", "OK");
        }
    }

    private void OnSearch(object sender, EventArgs e)
    {
        LoadAlarms(SearchBar.Text);
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
            LoadAlarms();
    }

    private async void OnAlarmSelected(object sender, SelectionChangedEventArgs e)
    {
        // We'll navigate to the detail page next
    }
}