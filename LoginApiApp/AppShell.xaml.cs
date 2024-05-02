using LoginApiApp.Views;
using System.ComponentModel;
using System.Windows.Input;

namespace LoginApiApp
{
    public partial class AppShell : Shell
    {
        public string WelcomeMessage { get; set; }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    WelcomeMessage = $"Bienvenue, {_userName}";
                    OnPropertyChanged(nameof(UserName));
                    OnPropertyChanged(nameof(WelcomeMessage));
                }
            }
        }

        public ICommand SignOutCommand { get; }
        public AppShell()
        {
            InitializeComponent();
            UserName = Preferences.Get("UserName", "Utilisateur");
            SignOutCommand = new Command(OnSignOut);
            // Assurez-vous que le binding contexte est défini si vous l'utilisez dans le xaml
            this.BindingContext = this;

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(Trajet), typeof(Trajet));
            Routing.RegisterRoute(nameof(Vehicule), typeof(Vehicule));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(EditTrajet), typeof(EditTrajet));
            Routing.RegisterRoute(nameof(AddTrajet), typeof(AddTrajet));
            Routing.RegisterRoute(nameof(DeleteTrajet), typeof(DeleteTrajet));
            Routing.RegisterRoute(nameof(AddVehicule), typeof(AddVehicule));
            Routing.RegisterRoute(nameof(EditVehicule), typeof(EditVehicule));
            Routing.RegisterRoute(nameof(DeleteVehicule), typeof(DeleteVehicule));

           
        }
    

        private async void OnSignOut()
        {
            // Fermez le Flyout
            this.FlyoutIsPresented = false;
            // Effacez toutes les informations de connexion stockées
            Preferences.Remove("UserName");
            UserName = string.Empty;
            // Naviguez vers la page de connexion
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

}

