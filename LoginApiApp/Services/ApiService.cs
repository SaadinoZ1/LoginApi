using LoginApiApp.Dtos;
using LoginApiApp.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace LoginApiApp.Services
{
    public class ApiService
    {
        private readonly RestClient _client;

        public ApiService()
        {
            _client = new RestClient("http://192.168.4.230:5178/api");
        }

        private async Task<RestRequest> CreateRequest(string resource, Method method, object body = null)
        {
            var request = new RestRequest(resource, method);
            var token = await SecureStorage.GetAsync("jwt_token");
            Console.WriteLine($"Using JWT token :{token}");
            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }
            else
            {
                Console.WriteLine("Jwt token is null or empty.");
            }

            if (body != null)
            {
                request.AddJsonBody(body);
            }
            return request;
        }


        public async Task<bool> RegisterAsync(string username, string password, string email, string role)
        {
            var body = new
            {
                UserName = username,
                Password = password,
                Email = email,
                Role = role
            };
            var request = await CreateRequest("/Auth/register", Method.Post, body);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            var body = new { Username = username, Password = password };
            var request = await CreateRequest("/Auth/login", Method.Post, body);
            var response = await _client.ExecuteAsync<UserDto>(request);
            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                Dictionary<string, string> _Token = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
                await SecureStorage.SetAsync("jwt_token", _Token["token"]);
                await SecureStorage.SetAsync("refreshToken", _Token["refreshToken"]);
                return response.Data;
            }
            return null;
        }


        public async Task<bool> RefreshTokenAsync(string refreshToken)
        {
            var body = new { Token = refreshToken };
            var request = await CreateRequest("/Auth/refresh", Method.Post, body);
            var response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var updatedToken = JsonConvert.DeserializeObject<dynamic>(response.Content);
                await SecureStorage.SetAsync("jwt_token", updatedToken.token);
                await SecureStorage.SetAsync("refreshToken", updatedToken.refreshToken);
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to refresh token : {response.ErrorMessage}");
                return false;
            }
            
        }


        public async Task<List<Trajet>> GetTrajetsAsync()
        {
            try
            {
                var request = await CreateRequest("/Trajet", Method.Get);
                var response = await _client.ExecuteAsync<List<Trajet>>(request);
                Console.WriteLine($"Status Code: {response.StatusCode}, Response: {response.Content}");

                if (response.IsSuccessful)
                {
                    return response.Data ?? new List<Trajet>();
                }
                else
                {
                    Console.WriteLine($"Error fetching trajets: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetTrajetsAsync: {ex.Message}");
            }

            return new List<Trajet>();
        }

        public async Task<TrajetDto> GetTrajetAsync(int id)
        {
            var request = await CreateRequest($"/Trajet/{id}", Method.Get);
            var response = await _client.ExecuteAsync<TrajetDto>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            return null;
        }


        public async Task<bool> AddTrajetAsync(TrajetDto trajetDto)
        {
            var request = await CreateRequest("/Trajet", Method.Post, trajetDto);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

        public async Task<bool> EditTrajetAsync(int id, TrajetDto trajetDto)
        {
            var request = await CreateRequest($"/Trajet/{id}", Method.Put, trajetDto);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

        public async Task<bool> DeleteTrajetAsync(int trajetId)
        {
            var request = await CreateRequest($"/Trajet/{trajetId}", Method.Delete);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

         public async Task<List<Vehicule>> GetVehiculesAsync()
        {
            var request = await CreateRequest("/Vehicule", Method.Get);
            var response = await _client.ExecuteAsync<List<Vehicule>>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            return new List<Vehicule>();
        }

        public async Task<VehiculeDto?> GetVehiculeAsync(int id)
        {
            var request = await CreateRequest($"/Vehicule/{id}", Method.Get);
            var response = await _client.ExecuteAsync<VehiculeDto>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            return null;
        }

        public async Task<bool> AddVehiculeAsync(VehiculeDto vehiculeDto)
        {
            var request = await CreateRequest("/Vehicule", Method.Post, vehiculeDto);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        public async Task<bool> EditVehiculeAsync(int id, VehiculeDto vehiculeDto)
        {
            var request = await CreateRequest($"/Vehicule/{id}", Method.Put, vehiculeDto);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }

        public async Task<bool> DeleteVehiculeAsync(int vehiculeId)
        {
            var request = await CreateRequest($"/Vehicule/{vehiculeId}", Method.Delete);
            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}
