using FCG.Game.Application.Interfaces;

namespace FCG.Game.Application.Services;

public class UserService(HttpClient httpClient) : IUserService
{
    public async Task<bool> UserExistAsync(string userId)
    {
        try
        {
            var response = await httpClient.GetAsync($"api/Users/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
