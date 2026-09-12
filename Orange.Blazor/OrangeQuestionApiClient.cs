using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Orange.Blazor.DTO.Questions;

namespace Orange.Blazor;

public class OrangeQuestionApiClient(HttpClient httpClient, ILogger<OrangeQuestionApiClient> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<GuildQuestionGetBatchDto?> GetGuildQuestionsAsync(string guildId, CancellationToken ct)
    {
        var path = $"/api/v1/guilds/{Uri.EscapeDataString(guildId)}/questions";
        return await GetAsync<GuildQuestionGetBatchDto?>(path, ct);
    }


    private async Task<TResponse?> GetAsync<TResponse>(string path, CancellationToken ct)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<TResponse>(path, JsonOptions, ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to GET {Url}", path);
            throw;
        }
    }


    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken ct)
    {
        var response = await httpClient.PostAsJsonAsync(path, body, JsonOptions, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, ct);

        if (result is null)
        {
            throw new InvalidOperationException($"POST {path} returned an empty body.");
        }

        return result;
    }

    private async Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken ct)
    {
        var response = await httpClient.PutAsJsonAsync(path, body, JsonOptions, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, ct);

        if (result is null)
        {
            throw new InvalidOperationException($"PUT {path} returned an empty body.");
        }

        return result;
    }
}