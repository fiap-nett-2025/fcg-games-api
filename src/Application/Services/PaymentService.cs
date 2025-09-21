using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infra.Repository;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;

namespace Application.Services;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;

    public PaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ProcessPaymentAsync(string userId, List<int> gameIds, decimal totalAmount)
    {
        {
            var paymentRequest = new
            {
                UserId = userId,
                GameIds = gameIds,
                TotalAmount = totalAmount,
                TransactionDate = DateTime.UtcNow
            };

            var response = await _httpClient.PostAsJsonAsync("payments/process", paymentRequest);
            return response.IsSuccessStatusCode;
        }
    }
}
