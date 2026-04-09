using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class SolutionEditorPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly WebViewPool _pool;
    private readonly int _alarmId;
    private readonly int? _solutionId;
    private string? _initialContent;

    // Create new
    public SolutionEditorPage(ApiClient api, WebViewPool pool, int alarmId)
    {
        InitializeComponent();
        _api = api;
        _pool = pool;
        _alarmId = alarmId;
        _solutionId = null;
    }

    // Edit existing
    public SolutionEditorPage(ApiClient api, WebViewPool pool, int alarmId, SolutionDto existing)
    {
        InitializeComponent();
        _api = api;
        _pool = pool;
        _alarmId = alarmId;
        _solutionId = existing.Id;

        TitleEntry.Text = existing.Title;
        TimeEntry.Text = existing.EstimatedTime ?? "";
        _initialContent = existing.Content ?? "";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _pool.DetachEditor();
        EditorContainer.Children.Add(_pool.EditorView);

        await _pool.WaitForEditor();
        await _pool.EditorView.EvaluateJavaScriptAsync("setHtml('')");

        if (!string.IsNullOrEmpty(_initialContent))
        {
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_initialContent));
            await _pool.EditorView.EvaluateJavaScriptAsync($"setHtmlBase64('{base64}')");
        }
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pool.DetachEditor();
    }

    private async Task<string> GetEditorHtml()
    {
        var base64 = await _pool.EditorView.EvaluateJavaScriptAsync("getHtmlBase64()");
        if (base64 is null) return "";

        base64 = base64.Trim('"');
        var bytes = Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(bytes);
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
            var content = await GetEditorHtml();

            if (_solutionId.HasValue)
            {
                await _api.UpdateSolutionAsync(_solutionId.Value, new UpdateSolutionDto
                {
                    Title = TitleEntry.Text,
                    Content = content,
                    SortOrder = 10,
                    EstimatedTime = string.IsNullOrWhiteSpace(TimeEntry.Text) ? null : TimeEntry.Text
                });
            }
            else
            {
                await _api.CreateSolutionAsync(new CreateSolutionDto
                {
                    Title = TitleEntry.Text,
                    Content = content,
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
        var content = await GetEditorHtml();
        if (!string.IsNullOrWhiteSpace(content) && content != "<p><br></p>")
        {
            var confirm = await DisplayAlertAsync("Avbryt", "Vill du lämna utan att spara?", "Ja", "Nej");
            if (!confirm) return;
        }
        await Navigation.PopAsync();
    }
}