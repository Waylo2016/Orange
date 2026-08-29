using System.Collections.Generic;
using System.Threading.Tasks;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Interfaces;
using Orange.Api.Models;

namespace Orange.Api.Services;

public class GuildQuestionService : IGuildQuestions
{
    public async Task<List<GuildQuestion>> GetGuildQuestionsPerGuildAsync(GuildIdDTO guildIdDto)
    {
        throw new System.NotImplementedException();
    }

    public async Task<GuildQuestion?> GetGuildQuestionByIdAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public async Task<GuildQuestion> CreateGuildQuestionAsync(GuildQuestionUpdateDto guildQuestion)
    {
        throw new System.NotImplementedException();
    }

    public async Task<GuildQuestion> UpdateGuildQuestionAsync(GuildQuestionUpdateDto guildQuestion)
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> DeleteGuildQuestionAsync(int id)
    {
        throw new System.NotImplementedException();
    }
}