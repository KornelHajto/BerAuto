using System.Net.Http.Json;
using System.Text.Json;
using BerAuto.DTO;

namespace BerAuto_WEB.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    async public Task<AuthResponseDTO> Login(LoginDTO login)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:5001/api/Auth/login", login);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
                return authResponse ?? new AuthResponseDTO();
            }
            
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Authentication failed: {response.StatusCode} - {error}");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("Failed to process authentication request", ex);
        }
    }
}