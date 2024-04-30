using LoginApiApp.Services;

namespace LoginApiApp;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;

    public RegisterPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }
    private async void btnRegister_Clicked(object sender, EventArgs e)
    {
        var username = usernameEntry.Text;
        var password = passwordEntry.Text;
        var email = emailEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Error", "Please enter a username, email and password.", "OK");
            return;
        }

        var success = await _apiService.RegisterAsync(username, password,email);
        if (success)
        {
            await DisplayAlert("Success", "Registration successful", "OK");
            // Optionally navigate to the login page
            await Shell.Current.GoToAsync("//LoginPage");
        }
        else
        {
            await DisplayAlert("Error", "Registration Failed, Please try again.", "OK");
        }
    }
   

}