using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class LoadingPage : ContentPage
{
    private readonly WebViewPool _pool;

    public LoadingPage(WebViewPool pool)
    {
        InitializeComponent();
        _pool = pool;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Attach WebViews to trigger loading
        PreloadContainer.Children.Add(_pool.EditorView);
        PreloadContainer.Children.Add(_pool.ViewerView);

        // Wait for both to be ready
        await Task.WhenAll(_pool.WaitForEditor(), _pool.WaitForViewer());

        // Detach before navigating
        _pool.DetachEditor();
        _pool.DetachViewer();

        // Navigate to main page
        Application.Current!.Windows[0].Page = new AppShell();
    }
}