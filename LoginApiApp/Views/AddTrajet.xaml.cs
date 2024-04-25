using LoginApiApp.Dtos;
using LoginApiApp.Services;
using System.Globalization;

namespace LoginApiApp.Views;

public partial class AddTrajet : ContentPage
{
    private readonly ApiService _apiService;
    public AddTrajet(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;

    }

    private async void OnAddTrajetClicked(object sender, EventArgs e)
    {
        // Parse the inputs
        bool parseDateSuccess = DateTime.TryParseExact(dateEntry.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
        bool parseKilometrageSuccess = int.TryParse(kilometrageEntry.Text, out var kilometrage);
        bool parseDureeSuccess = int.TryParse(dureeEntry.Text, out var duree);
        bool parseVehiculeIdSuccess = int.TryParse(vehiculeIdEntry.Text, out var vehiculeId);

        if (!parseDateSuccess || !parseKilometrageSuccess || !parseDureeSuccess || !parseVehiculeIdSuccess)
        {
            await DisplayAlert("Validation Error", "Please check your input and try again.", "OK");
            return;
        }

        var trajetDto = new TrajetDto
        {
            Date = date,
            Kilometrage = kilometrage,
            Duree = duree,
            Gains = gainsEntry.Text,
            VehiculeId = vehiculeId
        };

        var isSuccess = await _apiService.AddTrajetAsync(trajetDto);
        if (isSuccess)
        {
            await DisplayAlert("Success", "Trajet added successfully", "OK");
            // Optionally clear the form or navigate away
            await Shell.Current.GoToAsync("//Trajet");
        }
        else
        {
            await DisplayAlert("Error", "Failed to add trajet", "OK");
        }
    }


    private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}