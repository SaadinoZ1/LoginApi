using LoginApiApp.Dtos;
using LoginApiApp.Services;

namespace LoginApiApp.Views;

public partial class AddVehicule : ContentPage
{
    private readonly ApiService _apiService;
    public AddVehicule(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }
    private async void OnAddVehiculeClicked(object sender, EventArgs e)
    {
        var modèle = modèleEntry.Text;
        var plaqueImmatriculation = plaqueImmatriculationEntry.Text;

        if (string.IsNullOrWhiteSpace(modèle) || string.IsNullOrWhiteSpace(plaqueImmatriculation))
        {
            await DisplayAlert("Error", "Please enter all fields", "OK");
            return;
        }

        var vehiculeDto = new VehiculeDto
        {
            Modèle = modèle,
            PlaqueImmatriculation = plaqueImmatriculation
        };

        bool isSuccess = await _apiService.AddVehiculeAsync(vehiculeDto);
        if (isSuccess)
        {
            await DisplayAlert("Success", "Vehicule added successfully", "OK");
            // Optionally navigate back or refresh the list
            await Shell.Current.GoToAsync("//Vehicule");
        }
        else
        {
            await DisplayAlert("Error", "Failed to add vehicule", "OK");
        }
    }
    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}