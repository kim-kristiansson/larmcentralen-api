using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class AlarmDetailPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly int _alarmId;

    public AlarmDetailPage(ApiClient api, int alarmId)
    {
        InitializeComponent();
        _api = api;
        _alarmId = alarmId;
        LoadAlarm();
    }

    private async void LoadAlarm()
    {
        try
        {
            var alarm = await _api.GetAlarmAsync(_alarmId);
            if (alarm is null) return;

            TitleLabel.Text = alarm.Title;
            AlarmCodeLabel.Text = alarm.AlarmCode ?? "";
            SeverityLabel.Text = alarm.Severity;
            EquipmentLabel.Text = $"{alarm.AreaTitle} → {alarm.EquipmentTitle}";
            DescriptionLabel.Text = alarm.Description ?? "";
            SolutionsList.ItemsSource = alarm.Solutions;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta alarm: {ex.Message}", "OK");
        }
    }

    private async void OnSolutionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Models.SolutionDto solution)
        {
            await Navigation.PushAsync(new SolutionDetailPage(_api, solution.Id));
            SolutionsList.SelectedItem = null;
        }
    }
}