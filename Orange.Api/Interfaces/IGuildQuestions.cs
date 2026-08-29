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
    /// <param name="id">the id of the question to retrieve</param>
    /// <returns>a guild question</returns>
    Task<GuildQuestion?> GetGuildQuestionByIdAsync(int id);

    /// <summary>
    /// Creates a new guild question in the database
    /// </summary>
    /// <param name="guildQuestion">the guild question to create</param>
    /// <returns>the created guild question</returns>
    Task<GuildQuestion> CreateGuildQuestionAsync(GuildQuestionUpdateDto guildQuestion);

    /// <summary>
    /// Updates an existing guild question in the database
    /// </summary>
    /// <param name="guildQuestion">the guild question to update</param>
    /// <returns>the updated guild question</returns>
    Task<GuildQuestion> UpdateGuildQuestionAsync(GuildQuestionUpdateDto guildQuestion);

    /// <summary>
    /// Deletes a guild question from the database
    /// </summary>
    /// <param name="id">the id of the question to delete</param>
    /// <returns>true if the question was deleted, false otherwise</returns>
    Task<bool> DeleteGuildQuestionAsync(int id);
}