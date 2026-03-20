using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class SolutionDetailPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly int _solutionId;

    public SolutionDetailPage(ApiClient api, int solutionId)
    {
        InitializeComponent();
        _api = api;
        _solutionId = solutionId;
        LoadSolution();
    }

    private async void LoadSolution()
    {
        try
        {
            var solution = await _api.GetSolutionAsync(_solutionId);
            if (solution is null) return;

            TitleLabel.Text = solution.Title;
            TimeLabel.Text = solution.EstimatedTime ?? "";
            MarkdownContent.MarkdownText = solution.Content ?? "";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta lösning: {ex.Message}", "OK");
        }
    }
}