using LoginApiApp.Dtos;
using LoginApiApp.Services;
using System.Globalization;

namespace LoginApiApp.Views;

public partial class EditTrajet : ContentPage
{
    private readonly ApiService _apiService;

    public EditTrajet(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }
    private async void OnLoadTrajetClicked(object sender, EventArgs e)
    {
        if (int.TryParse(trajetIdEntry.Text, out int trajetId))
        {
            var trajet = await _apiService.GetTrajetAsync(trajetId);
            if (trajet != null)
            {
                dateEntry.Text = trajet.Date.ToString("yyyy-MM-dd");
                kilometrageEntry.Text = trajet.Kilometrage.ToString();
                dureeEntry.Text = trajet.Duree.ToString();
                gainsEntry.Text = trajet.Gains;
                vehiculeIdEntry.Text = trajet.VehiculeId.ToString();
            }
            else
            {
                await DisplayAlert("Error", "Trajet not found.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Invalid Trajet ID", "OK");
        }
    }
    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (int.TryParse(trajetIdEntry.Text, out int trajetId))
        {
            var updatedTrajet = new TrajetDto
            {
                Id = trajetId,
                Date = DateTime.ParseExact(dateEntry.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Kilometrage = int.Parse(kilometrageEntry.Text),
                Duree = int.Parse(dureeEntry.Text),
                Gains = gainsEntry.Text,
                VehiculeId = int.Parse(vehiculeIdEntry.Text)
            };

            var success = await _apiService.EditTrajetAsync(trajetId, updatedTrajet);
            if (success)
            {
                await DisplayAlert("Success", "Trajet updated successfully.", "OK");
                // Optionally navigate back or to another page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Error", "Failed to update trajet.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Invalid Trajet ID", "OK");
        }
    }

        private void btnCancel_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}