using LoginApiApp.Services;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Input;

namespace LoginApiApp.Views;

public partial class HomePage : ContentPage
{
    public ObservableCollection<FrameItem> Frames { get; set; }
   
    public HomePage()
    {
        InitializeComponent();
        Frames = new ObservableCollection<FrameItem>
        {
            new FrameItem
            {
                Image = "trajet.jpg",
                Text = "Trajets",
                TapCommand = new Command(async () => await  OnGetTrajetClicked())

            },
             new FrameItem
            {
                Image = "bus.jpg",
                Text = "Véhicules",
                TapCommand = new Command(async () =>await OnGetVehiculeClicked())

            }
        };
        BindingContext = this;

    }
    private async Task  OnGetTrajetClicked()
    {
     await    Shell.Current.GoToAsync(nameof(Trajet));
    }

    private async Task  OnGetVehiculeClicked()
    {
     await   Shell.Current.GoToAsync(nameof(Vehicule));
    }
    public class FrameItem
    {
        public string Image { get; set; }
        public string Text { get; set; }
        public ICommand TapCommand { get; set; }
    }
}