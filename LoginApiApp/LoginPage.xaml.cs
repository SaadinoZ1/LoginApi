using LoginApiApp.Dtos;
using LoginApiApp.Services;
using LoginApiApp.Views;

namespace LoginApiApp;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    public LoginPage( )
    {
        InitializeComponent();
    }
    public LoginPage( ApiService apiService ) : this() 
    {
           _apiService = apiService;
    }
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        string userName = usernameEntry.Text;
        string password = passwordEntry.Text;
        if (userName == null || password == null)
        {
            await DisplayAlert("Warning", "Please Input UserName & Password", "OK");
            return;
        }
        if (_apiService == null)
        {
            await DisplayAlert("Error", "API Service is not initialized", "OK");
            return;
        }

        try
        {
           var  UserDto  = await _apiService.LoginAsync(userName, password);
            if (UserDto != null)
            {
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await DisplayAlert("Warning", "UserName or Password is Incorrect", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }

    }
    private void btnRegister_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}
