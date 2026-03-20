using Larmcentralen.Maui.Converters;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class AlarmDetailPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly int _alarmId;
    private string _severity = "";

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

            _severity = alarm.Severity;
            
            TitleLabel.Text = alarm.Title;
            AlarmCodeLabel.Text = alarm.AlarmCode ?? "";
            EquipmentLabel.Text = $"{alarm.AreaTitle} → {alarm.EquipmentTitle}";
            DescriptionLabel.Text = alarm.Description ?? "";
            SolutionsList.ItemsSource = alarm.Solutions;

            // Severity badge
            var sevBg = new SeverityToBackgroundConverter();
            var sevText = new SeverityToTextColorConverter();
            var sevIcon = new SeverityToIconConverter();
            SeverityBadge.BackgroundColor = (Color)sevBg.Convert(alarm.Severity, typeof(Color), null, null)!;
            SeverityLabel.TextColor = (Color)sevText.Convert(alarm.Severity, typeof(Color), null, null)!;
            SeverityLabel.Text = $"{sevIcon.Convert(alarm.Severity, typeof(string), null, null)} {alarm.Severity}";
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
            await Navigation.PushAsync(new SolutionDetailPage(_api, solution.Id, _severity));
            SolutionsList.SelectedItem = null;
        }
    }
    
    private async void OnAddSolution(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new SolutionEditorPage(_api, _alarmId));
    }
    
    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}