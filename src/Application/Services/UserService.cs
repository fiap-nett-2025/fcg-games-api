using Application.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> UserExistAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Users/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
