namespace Larmcentralen.Maui.Services;

public class WebViewPool
{
    public WebView EditorView { get; }
    public WebView ViewerView { get; }

    private bool _editorReady;
    private bool _viewerReady;

    public WebViewPool()
    {
        EditorView = new WebView
        {
            Source = "quill/editor.html",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        EditorView.Navigated += (_, _) => _editorReady = true;

        ViewerView = new WebView
        {
            Source = "quill/viewer.html",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        ViewerView.Navigated += (_, _) => _viewerReady = true;
    }

    public async Task WaitForEditor()
    {
        while (!_editorReady)
            await Task.Delay(50);
    }

    public async Task WaitForViewer()
    {
        while (!_viewerReady)
            await Task.Delay(50);
    }

    public void DetachEditor()
    {
        if (EditorView.Parent is Layout parent)
            parent.Children.Remove(EditorView);
    }

    public void DetachViewer()
    {
        if (ViewerView.Parent is Layout parent)
            parent.Children.Remove(ViewerView);
    }
}