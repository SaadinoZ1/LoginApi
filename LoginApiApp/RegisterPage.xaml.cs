using LoginApiApp.Services;

namespace LoginApiApp;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private string selectedRole;


    public RegisterPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
        selectedRole = "Manager";
    }
    private void OnRoleChecked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = (RadioButton)sender;
            selectedRole = radioButton.Content.ToString();
        }
    }

    private async void btnRegister_Clicked(object sender, EventArgs e)
    {
        var username = usernameEntry.Text;
        var password = passwordEntry.Text;
        var email = emailEntry.Text;
        var role = selectedRole;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Error", "Please enter a username, email and password.", "OK");
            return;
        }

        var success = await _apiService.RegisterAsync(username, password,email,role);
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