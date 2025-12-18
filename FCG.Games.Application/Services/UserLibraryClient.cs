using FCG.Games.Application.Exceptions;
using FCG.Games.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FCG.Games.Application.Services;

public class UserLibraryClient(IHttpClientFactory httpClientFactory) : IUserLibraryClient
{
    public async Task<List<string>> GetOwnedGameIdsAsync(Guid userId, string jwt)
    {
		try
		{
			var httpClient = httpClientFactory.CreateClient("UserApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var response = await httpClient.GetAsync($"api/UserGameLibrary/{userId}");

            if (!response.IsSuccessStatusCode)
                throw new BusinessErrorDetailsException("Não foi possível buscar a biblioteca do usuário");

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            return doc.RootElement
                .GetProperty("data")
                .EnumerateArray()
                .Select(x => x.GetProperty("gameId").GetString()!)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new BusinessErrorDetailsException($"Erro ao buscar biblioteca do usuário: {ex.Message}");
        }
    }
}
