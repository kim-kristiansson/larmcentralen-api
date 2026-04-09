using Larmcentralen.Maui.Converters;
using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class SolutionDetailPage : ContentPage
{
    private readonly ApiClient _api;
    private readonly WebViewPool _pool;
    private readonly int _solutionId;
    private readonly string _severity;
    private SolutionDto? _solution;

    public SolutionDetailPage(ApiClient api, WebViewPool pool, int solutionId, string severity)
    {
        InitializeComponent();
        _api = api;
        _pool = pool;
        _solutionId = solutionId;
        _severity = severity;
        ApplySeverity();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _pool.DetachViewer();
        ContentContainer.Children.Add(_pool.ViewerView);
    
        _pool.ViewerView.Navigating += OnViewerNavigating;
        
        _pool.ViewerView.InputTransparent = true;
    
        await LoadSolution();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pool.ViewerView.Navigating -= OnViewerNavigating;
        _pool.DetachViewer();
        _pool.ViewerView.InputTransparent = false;
    }

    private void OnViewerNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("invoke://resize/"))
        {
            e.Cancel = true;
            var heightStr = e.Url.Replace("invoke://resize/", "");
            if (double.TryParse(heightStr, out var height))
            {
                _pool.ViewerView.HeightRequest = height;
            }
        }
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

    private async Task LoadSolution()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            _solution = await _api.GetSolutionAsync(_solutionId);
            if (_solution is null) return;

            TitleLabel.Text = _solution.Title;
            TimeLabel.Text = _solution.EstimatedTime != null ? $"Uppskattad tid: {_solution.EstimatedTime}" : "";

            await _pool.WaitForViewer();
            var content = _solution.Content ?? "";
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content));
            await _pool.ViewerView.EvaluateJavaScriptAsync($"setHtmlBase64('{base64}')");
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
        await Navigation.PushAsync(new SolutionEditorPage(_api, _pool, _solution.AlarmId, _solution));
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