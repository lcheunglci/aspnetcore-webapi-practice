using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EmployeeManagement.Business;

public class PromotionService(
    HttpClient httpClient,
    IEmployeeManagementRepository employeeManagementRepository) : IPromotionService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IEmployeeManagementRepository _employeeManagementRepository
        = employeeManagementRepository;

    public async Task<bool> PromoteInternalEmployeeAsync(
        InternalEmployee employee)
    {
        if (await CheckIfInternalEmployeeIsEligibleForPromotion(employee.Id))
        {
            employee.JobLevel++;
            await _employeeManagementRepository.SaveChangesAsync();
            return true;
        }
        return false;
    }

    private async Task<bool> CheckIfInternalEmployeeIsEligibleForPromotion(
        Guid employeeId)
    {
        var apiRoot = "http://localhost:5057";

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{apiRoot}/api/promotioneligibilities/{employeeId}");
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var promotionEligibility = JsonSerializer
            .Deserialize<PromotionEligibility>(content,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        return promotionEligibility != null
            && promotionEligibility.EligibleForPromotion;
    }
}
