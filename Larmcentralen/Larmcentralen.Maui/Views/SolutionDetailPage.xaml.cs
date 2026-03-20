using Indiko.Maui.Controls.Markdown;
using Indiko.Maui.Controls.Markdown.Theming;
using Larmcentralen.Maui.Converters;
using Larmcentralen.Maui.Helpers;
using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class SolutionDetailPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly int _solutionId;
    private readonly string _severity;
    private SolutionDto? _solution;
    
    public SolutionDetailPage(ApiClient api, int solutionId, string severity)
    {
        InitializeComponent();
        _api = api;
        _solutionId = solutionId;
        _severity = severity;
        ApplySeverity();
        ApplyMarkdownTheme();
        LoadSolution();
    }

    private void ApplySeverity()
    {
        var sevBg = new SeverityToBackgroundConverter();
        var sevText = new SeverityToTextColorConverter();
        var sevIcon = new SeverityToIconConverter();
        SeverityBadge.BackgroundColor = (Color)sevBg.Convert(_severity, typeof(Color), null, null)!;
        SeverityLabel.TextColor = (Color)sevText.Convert(_severity, typeof(Color), null, null)!;
        SeverityLabel.Text = $"{sevIcon.Convert(_severity, typeof(string), null, null)} {_severity}";
    }

    private void ApplyMarkdownTheme()
    {
        MarkdownContent.Theme = MarkdownThemeHelper.Create();
    }

    private async void LoadSolution()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            _solution = await _api.GetSolutionAsync(_solutionId);
            if (_solution is null) return;

            TitleLabel.Text = _solution.Title;
            TimeLabel.Text = _solution.EstimatedTime != null ? $"Uppskattad tid: {_solution.EstimatedTime}" : "";
            MarkdownContent.MarkdownText = _solution.Content ?? "";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta lösning: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
    
    private async void OnEditTapped(object? sender, EventArgs e)
    {
        if (_solution is null) return;
        await Navigation.PushAsync(new SolutionEditorPage(_api, _solution.AlarmId, _solution));
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSolution();
    }
    
    private async void OnDeleteTapped(object? sender, EventArgs e)
    {
        if (_solution is null) return;

        var confirm = await DisplayAlertAsync(
            "Ta bort lösning", 
            $"Vill du ta bort \"{_solution.Title}\"? Detta kan inte ångras.", 
            "Ta bort", 
            "Avbryt");

        if (!confirm) return;

        try
        {
            await _api.DeleteSolutionAsync(_solution.Id);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte ta bort: {ex.Message}", "OK");
        }
    }
}