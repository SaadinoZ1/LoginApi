using LoginApiApp.Dtos;
using LoginApiApp.Services;

namespace LoginApiApp.Views;

public partial class EditVehicule : ContentPage
{
    private readonly ApiService _apiService;

    public EditVehicule(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void OnLoadVehiculeClicked(object sender, EventArgs e)
    {
        if (int.TryParse(vehiculeIdEntry.Text, out int vehiculeId))
        {
            var vehicule = await _apiService.GetVehiculeAsync(vehiculeId);
            if (vehicule != null)
            {
                mod�leEntry.Text = vehicule.Mod�le.ToString();
                plaqueImmatriculationEntry.Text = vehicule.PlaqueImmatriculation;
            }
            else
            {
                await DisplayAlert("Error", "Vehicule not found.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Invalid Vehicule ID", "OK");
        }
    }
    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (int.TryParse(vehiculeIdEntry.Text, out int vehiculeId))
        {
            var updatedVehicule = new VehiculeDto
            {
                Id = vehiculeId,
                Mod�le = mod�leEntry.Text,
                PlaqueImmatriculation = plaqueImmatriculationEntry.Text
            };

            var success = await _apiService.EditVehiculeAsync(vehiculeId, updatedVehicule);
            if (success)
            {
                await DisplayAlert("Success", "Vehicule updated successfully.", "OK");
                // Optionally navigate back or to another page
                await Shell.Current.GoToAsync("//Vehicule");
            }
            else
            {
                await DisplayAlert("Error", "Failed to update vehicule.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Invalid Vehicule ID", "OK");
        }
    }
    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}