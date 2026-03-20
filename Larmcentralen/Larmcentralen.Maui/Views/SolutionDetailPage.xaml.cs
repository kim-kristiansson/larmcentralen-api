using Indiko.Maui.Controls.Markdown;
using Indiko.Maui.Controls.Markdown.Theming;
using Larmcentralen.Maui.Converters;
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
        var theme = new MarkdownTheme
        {
            Palette =
            {
                // Palette
                TextPrimary = Color.FromArgb("#1E293B"),
                H1Color = Color.FromArgb("#1E293B"),
                H2Color = Color.FromArgb("#334155"),
                H3Color = Color.FromArgb("#475569"),
                HyperlinkColor = Color.FromArgb("#C41E2A"),
                CodeBlockBackground = Color.FromArgb("#F1F5F9"),
                CodeBlockText = Color.FromArgb("#334155"),
                CodeBlockBorder = Color.FromArgb("#E2E8F0"),
                BlockQuoteBackground = Color.FromArgb("#FEF3C7"),
                BlockQuoteBorder = Color.FromArgb("#D97706"),
                BlockQuoteText = Color.FromArgb("#92400E"),
                TableHeaderBackground = Color.FromArgb("#F1F5F9"),
                TableHeaderText = Color.FromArgb("#1E293B"),
                TableBorder = Color.FromArgb("#E2E8F0"),
                DividerColor = Color.FromArgb("#E2E8F0")
            },
            Typography =
            {
                // Typography — using only real property names
                H1FontSize = 26,
                H2FontSize = 22,
                H3FontSize = 18,
                BodyFontSize = 15,
                CodeFontSize = 13,
                LineHeight = 1.6,
                ParagraphSpacing = 1.2,
                ListItemSpacing = 6,
                ListIndent = 24
            }
        };

        MarkdownContent.Theme = theme;
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