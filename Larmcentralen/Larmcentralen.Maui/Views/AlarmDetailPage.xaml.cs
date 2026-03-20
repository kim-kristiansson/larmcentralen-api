using CommunityToolkit.Maui.Storage;
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
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadAlarm();
    }

    private async void LoadAlarm()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var alarm = await _api.GetAlarmAsync(_alarmId);
            if (alarm is null) return;

            _severity = alarm.Severity;
        
            TitleLabel.Text = alarm.Title;
            AlarmCodeLabel.Text = alarm.AlarmCode ?? "";
            EquipmentLabel.Text = $"{alarm.AreaTitle} → {alarm.EquipmentTitle}";
            DescriptionLabel.Text = alarm.Description ?? "";
            SolutionsList.ItemsSource = alarm.Solutions;

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
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
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
    
    private async void OnExport(object? sender, EventArgs e)
    {
        try
        {
            var result = await _api.ExportAlarmAsync(_alarmId);
            if (result is null)
            {
                await DisplayAlertAsync("Fel", "Kunde inte exportera alarm", "OK");
                return;
            }

            var (bytes, fileName) = result.Value;
            using var stream = new MemoryStream(bytes);
            var saveResult = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

            if (saveResult.IsSuccessful)
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(saveResult.FilePath)
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Export misslyckades: {ex.Message}", "OK");
        }
    }
    
    private async void OnDeleteAlarm(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlertAsync(
            "Ta bort larm",
            "Vill du ta bort detta larm och alla dess lösningar? Detta kan inte ångras.",
            "Ta bort",
            "Avbryt");

        if (!confirm) return;

        try
        {
            await _api.DeleteAlarmAsync(_alarmId);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte ta bort: {ex.Message}", "OK");
        }
    }
    private async void OnUploadSharePoint(object? sender, EventArgs e)
    {
        try
        {
            var url = await _api.UploadToSharePointAsync(_alarmId);
            if (url is null)
            {
                await DisplayAlertAsync("Fel", "Kunde inte ladda upp till SharePoint", "OK");
                return;
            }

            await DisplayAlertAsync("Uppladdad", $"Filen laddades upp till SharePoint", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Upload misslyckades: {ex.Message}", "OK");
        }
    }
}