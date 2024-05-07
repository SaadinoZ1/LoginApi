using LoginApiApp.Services;
using System.Text.Json;

namespace LoginApiApp.Views;

public partial class Vehicule : ContentPage
{
    private readonly ApiService _apiService;

    public Vehicule(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
	}
    private async void OnLoadDataClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();

    }
    private async Task LoadDataAsync()
    {
        var vehiculesList = await _apiService.GetVehiculesAsync();
        if (vehiculesList != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                VehiculeGrid.Children.Clear();
                VehiculeGrid.RowDefinitions.Clear();
                VehiculeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                AddHeaderToGrid("Vehicule ID", 0);
                AddHeaderToGrid("Modèle", 1);
                AddHeaderToGrid("PlaqueImmatriculation", 2);

                foreach (var vehicule in vehiculesList)
                {
                    AddRowToGrid(vehicule);
                }
            });
        }
        else
        {
            await DisplayAlert("Error", "Failed to fetch vehicles.", "OK");
        }
    }

    private void AddHeaderToGrid(string headerText, int columnIndex)
    {
        var headerLabel = new Label { Text = headerText, FontAttributes = FontAttributes.Bold };
        Grid.SetColumn(headerLabel, columnIndex);
        Grid.SetRow(headerLabel, 0);
        VehiculeGrid.Children.Add(headerLabel);
    }

    private void AddRowToGrid(LoginApiApp.Models.Vehicule vehicule)
    {
        int rowIndex = VehiculeGrid.RowDefinitions.Count;
        VehiculeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
       
        AddCellToGrid(vehicule.Id.ToString(), 0, rowIndex);
        AddCellToGrid(vehicule.Modèle.ToString(), 1, rowIndex);
        AddCellToGrid(vehicule.PlaqueImmatriculation.ToString(), 2, rowIndex);

    }

    private void AddCellToGrid(string text, int column, int row)
    {
        var cellLabel = new Label { Text = text };
        Grid.SetColumn(cellLabel, column);
        Grid.SetRow(cellLabel, row);
        VehiculeGrid.Children.Add(cellLabel);
    }
    private void btnEditVehicule_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(EditVehicule));
    }
    private void btnAddVehicule_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AddVehicule));

    }
    private void DeleteVehiculeButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DeleteVehicule));

    }

}