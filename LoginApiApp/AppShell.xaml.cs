﻿using LoginApiApp.Views;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace LoginApiApp
{
    public partial class AppShell : Shell
    {
        public ICommand SignOutCommand { get; }
        public AppShell()
        {
            InitializeComponent();
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
            SecureStorage.RemoveAll();

            // Naviguez vers la page de connexion
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

}
