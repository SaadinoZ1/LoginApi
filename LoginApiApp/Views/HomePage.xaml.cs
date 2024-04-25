using LoginApiApp.Services;
using System.Net.Http;
using System.Text.Json;

namespace LoginApiApp.Views;

public partial class HomePage : ContentPage
{

    public HomePage()
	{
        InitializeComponent();
    
	}
    private void OnGetTrajetClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(Trajet));
    }

    private  void OnGetVehiculeClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(Vehicule));
    }
}