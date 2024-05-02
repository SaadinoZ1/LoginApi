using LoginApiApp.Dtos;
using LoginApiApp.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace LoginApiApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiServiceClient");
        }

        private async Task AttachAuthorizationHeader()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<bool> RegisterAsync(string username, string password, string email, string role)
        {
            var registerDto = new UserDto
            {
                UserName = username,
                Password = password,
                Email = email,
                Role = role,
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://192.168.4.230:5178/api/Auth/register", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            try
            {

                var loginRequest = new { Username = username, Password = password };
                var loginUrl = "http://192.168.4.230:5178/api/Auth/login";
                var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(loginUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var user = JsonConvert.DeserializeObject<UserDto>(jsonResponse);
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginAsync: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> RefreshTokenAsync(string refreshToken)
        {
            var refreshDto = new { Token = refreshToken };
            var content = new StringContent(JsonConvert.SerializeObject(refreshDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://192.168.4.230:5178/api/Auth/refresh", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var newTokenDetails = JsonConvert.DeserializeObject<dynamic>(json);
                var newToken = (string)newTokenDetails?.token;
                var newRefreshToken = (string)newTokenDetails?.refreshToken;
                if(! string.IsNullOrWhiteSpace(newToken) && ! string.IsNullOrWhiteSpace(newRefreshToken))
                {
                    await SecureStorage.SetAsync("jwt_token", newToken);
                    await SecureStorage.SetAsync("refreshToken",newRefreshToken);

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",newToken);
                    return true;

                }
            }

            return false;
        }


        public async Task<List<Trajet>> GetTrajetsAsync()
        {
            var url = "http://192.168.4.230:5178/api/Trajet";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                await AttachAuthorizationHeader();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Trajet>>(json);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refresheToken = await SecureStorage.GetAsync("refreshToken");
            if (refresheToken != null)
                {
                    var isRefreshed = await RefreshTokenAsync(refresheToken);
                    if (isRefreshed)
                    {
                        return await GetTrajetsAsync();
                    }
                    }
               
                    await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            else
            {
                Console.WriteLine($"Error fetching data : {response.StatusCode}");
            }
            return new List<Trajet>();
        }
        public async Task<TrajetDto?> GetTrajetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"http://192.168.4.230:5178/api/Trajet/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TrajetDto>(jsonResponse);
            }
            return null;
        }

        public async Task<bool> AddTrajetAsync(TrajetDto trajetDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(trajetDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://192.168.4.230:5178/api/Trajet", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EditTrajetAsync(int id, TrajetDto trajetDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(trajetDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"http://192.168.4.230:5178/api/Trajet/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTrajetAsync(int trajetId)
        {
            var response = await _httpClient.DeleteAsync($"http://192.168.4.230:5178/api/Trajet/{trajetId}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<Vehicule>> GetVehiculesAsync()
        {
            var url = "http://192.168.4.230:5178/api/Vehicule";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<Vehicule>>(json);
            }

            return new List<Vehicule>();
        }
        public async Task<VehiculeDto?> GetVehiculeAsync(int id)
        {
            var response = await _httpClient.GetAsync($"http://192.168.4.230:5178/api/Vehicule/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<VehiculeDto>(jsonResponse);
            }
            return null;
        }
        public async Task<bool> AddVehiculeAsync(VehiculeDto vehiculeDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(vehiculeDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://192.168.4.230:5178/api/Vehicule", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> EditVehiculeAsync(int id, VehiculeDto vehiculeDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(vehiculeDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"http://192.168.4.230:5178/api/Vehicule/{id}", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteVehiculeAsync(int vehiculeId)
        {
            var response = await _httpClient.DeleteAsync($"http://192.168.4.230:5178/api/Vehicule/{vehiculeId}");
            return response.IsSuccessStatusCode;
        }
    }
}
