using Larmcentralen.Maui.Models;
using Larmcentralen.Maui.Services;

namespace Larmcentralen.Maui.Views;

public partial class AlarmEditorPage : ContentPage
{
    private readonly ApiClient _api;
    private List<AreaDto> _areas = [];
    private List<EquipmentDto> _equipment = [];
    private string? _selectedSeverity;

    public AlarmEditorPage(ApiClient api)
    {
        InitializeComponent();
        _api = api;
        _selectedSeverity = "Låg";
        SeverityLabel.Text = "Låg";
        SeverityLabel.TextColor = (Color)Application.Current!.Resources["Primary"];
        LoadAreas();
    }

    private async void LoadAreas()
    {
        try
        {
            _areas = await _api.GetAreasAsync();
            AreaPicker.ItemsSource = _areas.Select(a => a.DisplayName ?? a.Title).ToList();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta områden: {ex.Message}", "OK");
        }
    }

    private async void OnAreaChanged(object? sender, EventArgs e)
    {
        if (AreaPicker.SelectedIndex < 0) return;

        try
        {
            var area = _areas[AreaPicker.SelectedIndex];
            _equipment = await _api.GetEquipmentAsync(area.Id);
            EquipmentPicker.ItemsSource = _equipment.Select(eq => eq.DisplayName ?? eq.Title).ToList();
            EquipmentPicker.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Kunde inte hämta utrustning: {ex.Message}", "OK");
        }
    }

    private async void OnSaveTapped(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlertAsync("Fel", "Titel krävs", "OK");
            return;
        }

        if (string.IsNullOrEmpty(_selectedSeverity))
        {
            await DisplayAlertAsync("Fel", "Välj allvarlighetsgrad", "OK");
            return;
        }

        if (AreaPicker.SelectedIndex < 0)
        {
            await DisplayAlertAsync("Fel", "Välj område", "OK");
            return;
        }

        if (EquipmentPicker.SelectedIndex < 0)
        {
            await DisplayAlertAsync("Fel", "Välj utrustning", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Sparar...";

            var dto = new CreateAlarmDto
            {
                Title = TitleEntry.Text,
                AlarmCode = string.IsNullOrWhiteSpace(AlarmCodeEntry.Text) ? null : AlarmCodeEntry.Text,
                Severity = _selectedSeverity ?? "Låg",
                Description = string.IsNullOrWhiteSpace(DescriptionEditor.Text) ? null : DescriptionEditor.Text,
                EquipmentId = _equipment[EquipmentPicker.SelectedIndex].Id
            };

            await _api.CreateAlarmAsync(dto);
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
        if (!string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            var confirm = await DisplayAlertAsync("Avbryt", "Vill du lämna utan att spara?", "Ja", "Nej");
            if (!confirm) return;
        }
        await Navigation.PopAsync();
    }
    
    [Obsolete("Obsolete")]
    private async void OnSeverityTapped(object? sender, TappedEventArgs e)
    {
        var result = await DisplayActionSheet("Allvarlighetsgrad", "Avbryt", null, "Låg", "Medel", "Hög", "Kritisk");
        if (result is not null and not "Avbryt")
        {
            _selectedSeverity = result;
            SeverityLabel.Text = result;
            SeverityLabel.TextColor = (Color)Application.Current!.Resources["Primary"];
        }
    }
}