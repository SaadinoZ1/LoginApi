using LoginApiApp.Services;

namespace LoginApiApp;

public partial class DeleteTrajet : ContentPage
{
    private readonly ApiService _apiService;

    public DeleteTrajet(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
	}
    private async void DeleteTrajetButton_Clicked(object sender, EventArgs e)
    {
        if (!int.TryParse(EntryTrajetId.Text, out var trajetId))
        {
            await DisplayAlert("Error", "Please enter a valid numeric ID.", "OK");
            return;
        }

        bool isSuccess = await _apiService.DeleteTrajetAsync(trajetId);
        if (isSuccess)
        {
            await DisplayAlert("Success", "The trajet has been successfully deleted.", "OK");
            // Optionally navigate back or reset the page
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Error", "Failed to delete the trajet.", "OK");
        }
    }


}