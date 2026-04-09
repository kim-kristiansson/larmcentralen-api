using Larmcentralen.Maui.Services;
using Larmcentralen.Maui.Views;

namespace Larmcentralen.Maui;

public partial class App : Application
{
    public App(WebViewPool pool)
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Light;
        _pool = pool;
    }

    private readonly WebViewPool _pool;

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new LoadingPage(_pool));
    }
}