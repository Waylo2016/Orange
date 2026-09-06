using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orange.Blazor;

public class OrangeApiClient(HttpClient httpClient, ILogger<OrangeApiClient> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    
    
    private async Task<T?> GetAsync<T>(string path, CancellationToken ct)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<T>(path, JsonOptions, ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to GET {Url}", path);
            throw;
        }
    }
    
    private async Task<IReadOnlyList<T>> GetListAsync<T>(string path, CancellationToken ct)
    {
        var items = await httpClient.GetFromJsonAsync<List<T>>(path, ct);
        logger.LogInformation("GET {Url} returned {Count} items", path, items?.Count ?? 0);
        return items ?? [];
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync(path, body, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(ct);

        if (result is null)
        {
            throw new InvalidOperationException($"POST {path} returned an empty body.");
        }

        return result;
    }
}