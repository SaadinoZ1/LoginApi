using System.Text.Json;

namespace LoginApiApp.Views;

public partial class Vehicule : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();

    public Vehicule()
	{
		InitializeComponent();
	}
    private async void OnLoadDataClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();

    }
    private async Task LoadDataAsync()
    {
        var url = "http://192.168.4.222:5178/api/Vehicule";
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Convert the JSON response to a list of objects
                var vehiculesList = JsonSerializer.Deserialize<List<LoginApiApp.Models.Vehicule>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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


                        // Add rows for the data
                        foreach (var vehicule in vehiculesList)
                        {
                            AddRowToGrid(vehicule);
                        }
                    });
                }
            }
            else
            {
                Console.WriteLine($"Error fetching data: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception fetching data: {ex.Message}");
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