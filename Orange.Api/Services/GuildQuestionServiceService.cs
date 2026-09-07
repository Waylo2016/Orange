using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Exceptions;
using Orange.Api.Interfaces;
using Orange.Api.Models;
using Orange.Api.utils;

namespace Orange.Api.Services;

public class GuildQuestionServiceService(ApplicationDbContext _context, ILogger<GuildQuestionServiceService> _logger) : IGuildQuestionService
{
    /// <summary>
    /// This method is invoked wanting to see all guild questions for a specific guild
    /// </summary>
    /// <param name="guildIdDto">DTO carrying the guild ID</param>
    /// <returns></returns>
    public async Task<List<GuildQuestion>> GetGuildQuestionsPerGuildAsync(GuildIdDTO guildIdDto)
    {
        var guildQuestions = await _context.GuildQuestions
            .Where(gq => gq.GuildId == guildIdDto.GuildId)
            .ToListAsync();

        return guildQuestions;
    }

    /// <summary>
    /// This method is invoked wanting to see a specific guild question by its ID
    /// </summary>
    /// <param name="guildQuestionOrderDeleteDto">DTO containing guild ID and question Order</param>
    /// <returns>the single question asked</returns>
    /// <exception cref="NotFoundException">thrown when no question is found</exception>
    public async Task<GuildQuestion> GetGuildQuestionByIdAsync(ulong guildId, int questionId)
    {
        var guildQuestion = await _context.GuildQuestions
            .FirstOrDefaultAsync(gq => gq.GuildId == guildId && gq.Id == questionId);

        if (guildQuestion == null)
        {
            _logger.LogWarning("Guild question with guild ID {Id} and question ID {QuestionId} not found.", guildId, questionId);
            throw new NotFoundException($"Guild question with ID {guildId} not found.");
        }

        return guildQuestion;
    }

    /// <summary>
    /// This method is invoked wanting to create a new guild question
    /// </summary>
    /// <param name="guildQuestion">The question to create</param>
    /// <returns>The created question</returns>
    public async Task<GuildQuestion> CreateGuildQuestionAsync(GuildQuestionCreateDto guildQuestion)
    {
        var newGuildQuestion = new GuildQuestion
        {
            GuildId = guildQuestion.GuildId,
            Question = guildQuestion.Question,
            QuestionOrder = guildQuestion.QuestionOrder
        };

        _context.GuildQuestions.Add(newGuildQuestion);
        await _context.SaveChangesAsync();

        return newGuildQuestion;
    }

    /// <summary>
    /// This method is invoked wanting to update an existing guild question
    /// </summary>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="dto">The updated question data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated question</returns>
    public async Task<GuildQuestion> UpdateGuildQuestionAsync(
        ulong guildId,
        int questionId,
        GuildQuestionUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var question = await _context.GuildQuestions
            .FirstOrDefaultAsync(q => q.Id == questionId && q.GuildId == guildId, cancellationToken);

        if (question is null)
        {
            _logger.LogWarning("Guild question {QuestionId} not found in guild {GuildId}.", questionId, guildId);
            throw new NotFoundException($"Question {questionId} was not found.");
        }

        // The entity is tracked, so assigning the property is enough. No Update() call needed.
        question.Question = dto.Question;

        await _context.SaveChangesAsync(cancellationToken);
        return question;
    }

    /// <summary>
    /// This method is invoked wanting to delete a specific guild question by its ID
    /// </summary>
    /// <param name="guildQuestionOrderDeleteDto">The data of the guild question to delete</param>
    /// <returns>True if the guild question was deleted, false otherwise</returns>
    public async Task<bool> DeleteGuildQuestionAsync(GuildQuestionOrderDeleteDto guildQuestionOrderDeleteDto)
    {
        var existingGuildQuestion = await _context.GuildQuestions
            .FirstOrDefaultAsync(gq => gq.GuildId == guildQuestionOrderDeleteDto.GuildId && gq.QuestionOrder == guildQuestionOrderDeleteDto.QuestionOrder);

        if (existingGuildQuestion == null)
        {
            return false;
        }

        _context.GuildQuestions.Remove(existingGuildQuestion);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IActionResult> ReorderGuildQuestionsAsync(GuildQuestionsReorderDto guildQuestionReorderDto)
    {
        throw new System.NotImplementedException();
    }
}