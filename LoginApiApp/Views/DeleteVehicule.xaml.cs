using LoginApiApp.Services;

namespace LoginApiApp;

public partial class DeleteVehicule : ContentPage
{
    private readonly ApiService _apiService;
    public DeleteVehicule(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }
    private async void DeleteVehiculeButton_Clicked(object sender, EventArgs e)
    {
        if (!int.TryParse(EntryVehiculeId.Text, out var vehiculeId))
        {
            await DisplayAlert("Error", "Please enter a valid numeric ID.", "OK");
            return;
        }

        bool isSuccess = await _apiService.DeleteVehiculeAsync(vehiculeId);
        if (isSuccess)
        {
            await DisplayAlert("Success", "The vehicule has been successfully deleted.", "OK");
            // Optionally navigate back or reset the page
            await Shell.Current.GoToAsync("//Vehicule");
        }
        else
        {
            await DisplayAlert("Error", "Failed to delete the vehicule.", "OK");
        }
    }

}