using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orange.Api.DTO.GuildQuestion;

namespace Orange.Bot.API;

public class QuestionApiClient(HttpClient httpClient, ILogger<QuestionApiClient> logger) : ApiClientBase(httpClient, logger)
{
    public async Task<GuildQuestionCreateDto> CreateQuestionAsync(GuildQuestionCreateDto question, CancellationToken ct = default)
    {
        var path = $"/api/v1/guilds/{Uri.EscapeDataString(question.GuildId.ToString())}/questions";
        return await PostAsync<GuildQuestionCreateDto, GuildQuestionCreateDto>(path, question, ct);
    }
}