using System.Collections.Generic;
using System.Threading.Tasks;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Models;

namespace Orange.Api.Interfaces;

public interface IGuildQuestions
{
    /// <summary>
    /// Gets all guild questions from the database
    /// </summary>
    /// <returns>a collection of guild questions</returns>
    Task<List<GuildQuestion>> GetGuildQuestionsPerGuildAsync(GuildIdDTO guildIdDto);

    /// <summary>
    /// Gets a guild question by its id
    /// </summary>
    /// <param name="guildQuestionOrderDeleteDto">The order of the question within the guild</param>
    /// <returns>a guild question</returns>
    Task<GuildQuestion> GetGuildQuestionByIdAsync(GuildQuestionOrderDeleteDto guildQuestionOrderDeleteDto);

    /// <summary>
    /// Creates a new guild question in the database
    /// </summary>
    /// <param name="guildQuestion">the guild question to create</param>
    /// <returns>the created guild question</returns>
    Task<GuildQuestion> CreateGuildQuestionAsync(GuildQuestionCreateDto guildQuestion);

    /// <summary>
    /// Updates an existing guild question in the database
    /// </summary>
    /// <param name="guildQuestion">the data to update the question</param>
    /// <returns>the updated guild question</returns>
    Task<GuildQuestion> UpdateGuildQuestionAsync(GuildQuestionUpdateDto guildQuestion);

    /// <summary>
    /// Deletes a guild question from the database
    /// </summary>
    /// <param name="guildQuestionOrderDeleteDto">The information of the question to delete</param>
    /// <returns>true if the question was deleted, false otherwise</returns>
    Task<bool> DeleteGuildQuestionAsync(GuildQuestionOrderDeleteDto guildQuestionOrderDeleteDto);
}