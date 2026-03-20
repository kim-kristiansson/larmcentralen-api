using Indiko.Maui.Controls.Markdown;
using Indiko.Maui.Controls.Markdown.Theming;
using Larmcentralen.Maui.Converters;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class SolutionDetailPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly int _solutionId;
    private readonly string _severity;

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
        var theme = new MarkdownTheme();

        theme.Palette.TextPrimary = Color.FromArgb("#1E293B");
        theme.Palette.H1Color = Color.FromArgb("#1E293B");
        theme.Palette.H2Color = Color.FromArgb("#334155");
        theme.Palette.H3Color = Color.FromArgb("#475569");
        theme.Palette.HyperlinkColor = Color.FromArgb("#C41E2A");
        theme.Palette.CodeBlockBackground = Color.FromArgb("#F1F5F9");
        theme.Palette.CodeBlockText = Color.FromArgb("#334155");
        theme.Palette.BlockQuoteBackground = Color.FromArgb("#FEF3C7");
        theme.Palette.BlockQuoteBorder = Color.FromArgb("#D97706");
        theme.Palette.TableHeaderBackground = Color.FromArgb("#F1F5F9");
        theme.Palette.TableHeaderText = Color.FromArgb("#1E293B");
        theme.Palette.TableBorder = Color.FromArgb("#E2E8F0");

        theme.Typography.H1FontSize = 26;
        theme.Typography.H2FontSize = 22;
        theme.Typography.H3FontSize = 18;
        theme.Typography.BodyFontSize = 15;
        theme.Typography.LineHeight = 1.6;
        theme.Typography.ParagraphSpacing = 2;

        MarkdownContent.Theme = theme;
    }

    private async void LoadSolution()
    {
        try
        {
            var solution = await _api.GetSolutionAsync(_solutionId);
            if (solution is null) return;

            TitleLabel.Text = solution.Title;
            TimeLabel.Text = solution.EstimatedTime != null ? $"Uppskattad tid: {solution.EstimatedTime}" : "";
            MarkdownContent.MarkdownText = solution.Content ?? "";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta lösning: {ex.Message}", "OK");
        }
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}