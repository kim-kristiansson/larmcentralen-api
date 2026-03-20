using CommunityToolkit.Maui;
using Larmcentralen.Maui.Services;
using Microsoft.Extensions.Logging;

namespace Larmcentralen.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
		builder.Services.AddSingleton(sp => new HttpClient
		{
			BaseAddress = new Uri("http://localhost:5270/")
		});
		builder.Services.AddSingleton<ApiClient>();
		builder.Services.AddTransient<MainPage>();
		
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

#if WINDOWS
		void StripBorder(Microsoft.UI.Xaml.Controls.Control control, string prefix)
		{
			control.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
			control.Background = null;
			control.FocusVisualMargin = new Microsoft.UI.Xaml.Thickness(0);
			control.Resources[$"{prefix}BorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
			control.Resources[$"{prefix}BorderThemeThicknessPointerOver"] = new Microsoft.UI.Xaml.Thickness(0);
		}

		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (h, _) => StripBorder(h.PlatformView, "TextControl"));
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("Borderless", (h, _) => StripBorder(h.PlatformView, "TextControl"));
		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (h, _) => StripBorder(h.PlatformView, "ComboBox"));
#endif
		
		return builder.Build();
	}
}
