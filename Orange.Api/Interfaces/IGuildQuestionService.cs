using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Models;

namespace Orange.Api.Interfaces;

public interface IGuildQuestionService
{
    /// <summary>
    /// Gets all guild questions from the database
    /// </summary>
    /// <returns>a collection of guild questions</returns>
    Task<GuildQuestionGetBatchDto> GetGuildQuestionsPerGuildAsync(GuildIdDTO guildIdDto);

    /// <summary>
    /// Gets a guild question by its id
    /// </summary>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>a guild question</returns>
    Task<GuildQuestionGetDto> GetGuildQuestionByIdAsync(ulong guildId, int questionId);

    /// <summary>
    /// Creates a new guild question in the database
    /// </summary>
    /// <param name="guildQuestion">the guild question to create</param>
    /// <returns>the created guild question</returns>
    Task<GuildQuestionGetDto> CreateGuildQuestionAsync(GuildQuestionCreateDto guildQuestion);

    /// <summary>
    /// Updates an existing guild question in the database
    /// </summary>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="dto">The updated question data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>the updated guild question</returns>
    Task<GuildQuestionGetDto> UpdateGuildQuestionAsync(
        ulong guildId,
        int questionId,
        GuildQuestionUpdateDto dto,
        CancellationToken cancellationToken = default
        );

    /// <summary>
    /// Deletes a guild question from the database
    /// </summary>
    /// <param name="guildQuestionOrderDeleteDto">The information of the question to delete</param>
    /// <returns>true if the question was deleted, false otherwise</returns>
    Task<bool> DeleteGuildQuestionAsync(GuildQuestionOrderDeleteDto guildQuestionOrderDeleteDto);
    
    Task<IActionResult> ReorderGuildQuestionsAsync(GuildQuestionsReorderDto guildQuestionReorderDto);
}