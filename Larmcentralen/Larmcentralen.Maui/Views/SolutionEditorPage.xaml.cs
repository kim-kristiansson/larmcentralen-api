using Larmcentralen.Maui.Helpers;
using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class SolutionEditorPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly int _alarmId;
    private readonly int? _solutionId;
    private CancellationTokenSource? _previewDebounce;
    
    private void Init()
    {
        InitializeComponent();
        Preview.Theme = MarkdownThemeHelper.Create();
    }

    // Create new solution
    public SolutionEditorPage(ApiClient api, int alarmId)
    {
        Init();
        _api = api;
        _alarmId = alarmId;
        _solutionId = null;
    }

    // Edit existing solution
    public SolutionEditorPage(ApiClient api, int alarmId, SolutionDto existing)
    {
        Init();
        _api = api;
        _alarmId = alarmId;
        _solutionId = existing.Id;

        TitleEntry.Text = existing.Title;
        TimeEntry.Text = existing.EstimatedTime ?? "";
        MarkdownEditor.Text = existing.Content ?? "";
        Preview.MarkdownText = existing.Content ?? "";
    }

    private void OnMarkdownChanged(object? sender, TextChangedEventArgs e)
    {
        _previewDebounce?.Cancel();
        _previewDebounce = new CancellationTokenSource();
        var token = _previewDebounce.Token;

        Task.Delay(400, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Preview.MarkdownText = e.NewTextValue ?? "";
                });
            }
        }, TaskScheduler.Default);
    }

    private async void OnSaveTapped(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlertAsync("Fel", "Titel krävs", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Sparar...";

            if (_solutionId.HasValue)
            {
                await _api.UpdateSolutionAsync(_solutionId.Value, new UpdateSolutionDto
                {
                    Title = TitleEntry.Text,
                    Content = MarkdownEditor.Text,
                    SortOrder = 10,
                    EstimatedTime = string.IsNullOrWhiteSpace(TimeEntry.Text) ? null : TimeEntry.Text
                });
            }
            else
            {
                await _api.CreateSolutionAsync(new CreateSolutionDto
                {
                    Title = TitleEntry.Text,
                    Content = MarkdownEditor.Text,
                    SortOrder = 10,
                    EstimatedTime = string.IsNullOrWhiteSpace(TimeEntry.Text) ? null : TimeEntry.Text,
                    AlarmId = _alarmId
                });
            }

            StatusLabel.Text = "Sparat!";
            await Task.Delay(500);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "";
            await DisplayAlertAsync("Fel", $"Kunde inte spara: {ex.Message}", "OK");
        }
    }

    private async void OnCancelTapped(object? sender, TappedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(MarkdownEditor.Text))
        {
            var confirm = await DisplayAlertAsync("Avbryt", "Vill du lämna utan att spara?", "Ja", "Nej");
            if (!confirm) return;
        }
        await Navigation.PopAsync();
    }
}