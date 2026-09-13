using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Orange.Bot.Exceptions;

namespace Orange.Bot.API;

public abstract class ApiClientBase(HttpClient httpClient, ILogger logger)
{
    
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    
    protected async Task<TResponse?> GetAsync<TResponse>(string path, CancellationToken ct)
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


    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken ct)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(path, body, JsonOptions, ct);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                logger.LogError("POST {Url} failed with status code {StatusCode}. Response: {Response}", path, response.StatusCode, errorContent);
                 
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        throw new ApiException($"POST {path} failed with status code 400 Bad Request. Response: {errorContent}", System.Net.HttpStatusCode.BadRequest, errorContent);
                    case System.Net.HttpStatusCode.Conflict:
                        throw new ApiException($"POST {path} failed with status code 409 Conflict. Response: {errorContent}", System.Net.HttpStatusCode.Conflict, errorContent);
                    default:
                        throw new ApiException($"POST {path} failed with status code {response.StatusCode}. Response: {errorContent}", response.StatusCode, errorContent);
                }
            }
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, ct);

            if (result is null)
            {
                throw new InvalidOperationException($"POST {path} returned an empty body.");
            }


            
            return result;
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Failed to Post {Url}", path);
            throw;
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e, "Invalid operation");
            throw;
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, "Failed to Post {Url}", path);
            throw;
        }
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken ct)
    {
        try
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
        catch (ApiException e)
        {
            logger.LogError(e, "Failed to Put {Url}", path);
            throw;
        }
        catch (InvalidOperationException e)
        {
            logger.LogError(e, "Invalid operation");
            throw;
        }
    }
    
    protected async Task DeleteAsync(string path, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.DeleteAsync(path, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (ApiException e)
        {
            logger.LogError(e, "Failed to Delete {Url}", path);
            throw;
        }
    }
}