using System.Net.Http;
using System.Text.Json;

namespace LoginApiApp.Views;

public partial class Trajet : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();

    public Trajet()
	{
        InitializeComponent();
	}

    private async void OnTrajetClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();
    }
    private async Task LoadDataAsync()
    {
        var url = "http://192.168.4.230:5178/api/Trajet";
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                // Convert the JSON response to a list of objects
                var trajetsList = JsonSerializer.Deserialize<List<LoginApiApp.Models.Trajet>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (trajetsList != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TrajetGrid.Children.Clear();
                        TrajetGrid.RowDefinitions.Clear();
                        TrajetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        AddHeaderToGrid("Trajet ID", 0);
                        AddHeaderToGrid("Date", 1);
                        AddHeaderToGrid("Kilometrage", 2);
                        AddHeaderToGrid("Duree", 3);
                        AddHeaderToGrid("Gains ", 4);
                        AddHeaderToGrid("Vehicule ID ", 5);

                        // Add rows for the data
                        foreach (var trajet in trajetsList)
                        {
                            AddRowToGrid(trajet);
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
        TrajetGrid.Children.Add(headerLabel);
    }

    private void AddRowToGrid(LoginApiApp.Models.Trajet trajet)
    {
        int rowIndex = TrajetGrid.RowDefinitions.Count;
        TrajetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        AddCellToGrid(trajet.Id.ToString(), 0, rowIndex);
        AddCellToGrid(trajet.Date.ToString("g"), 1, rowIndex);
        AddCellToGrid(trajet.Kilometrage.ToString(), 2, rowIndex);
        AddCellToGrid(trajet.Duree.ToString(), 3, rowIndex);
        AddCellToGrid(trajet.Gains.ToString(), 4, rowIndex);
        AddCellToGrid(trajet.VehiculeId.ToString(), 5, rowIndex);
    }

    private void AddCellToGrid(string text, int column, int row)
    {
        var cellLabel = new Label { Text = text };
        Grid.SetColumn(cellLabel, column);
        Grid.SetRow(cellLabel, row);
        TrajetGrid.Children.Add(cellLabel);
    }

    private void btnEditTrajet_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(EditTrajet));
    }
    private void btnAddTrajet_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AddTrajet));

    }

    private void DeleteTrajetButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(DeleteTrajet));

    }

}