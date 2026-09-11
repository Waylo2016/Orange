using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

public class GuildQuestionService(ApplicationDbContext _context, ILogger<GuildQuestionService> _logger) : IGuildQuestionService
{
    /// <summary>
    /// This method is invoked wanting to see all guild questions for a specific guild
    /// </summary>
    /// <param name="guildIdDto">DTO carrying the guild ID</param>
    /// <returns></returns>
    public async Task<GuildQuestionGetBatchDto> GetGuildQuestionsPerGuildAsync(GuildIdDTO guildIdDto)
    {
        var guildQuestions = await _context.GuildQuestions
            .Where(gq => gq.GuildId == guildIdDto.GuildId)
            .ToListAsync();

        var orderedQuestions = guildQuestions.OrderBy(gq => gq.QuestionOrder)
            .Select(gq => new GuildQuestionGetDto
            {
                GuildId = gq.GuildId,
                QuestionOrder = gq.QuestionOrder,
                Question = gq.Question
            }).ToList();

        return new GuildQuestionGetBatchDto()
        {
            GuildId = guildIdDto.GuildId,
            Questions = orderedQuestions
        };
    }

    /// <summary>
    /// This method is invoked wanting to see a specific guild question by its ID
    /// </summary>
    /// <param name="guildId"></param>
    /// <param name="questionOrder"></param>
    /// <returns>the single question asked</returns>
    /// <exception cref="NotFoundException">thrown when no question is found</exception>
    public async Task<GuildQuestionGetDto> GetGuildQuestionByOrderAsync(ulong guildId, int questionOrder)
    {
        var guildQuestion = await _context.GuildQuestions
            .FirstOrDefaultAsync(gq => gq.QuestionOrder == questionOrder && gq.GuildId == guildId);

        if (guildQuestion == null)
        {
            _logger.LogWarning("Guild question with guild ID {Id} and question order {QuestionId} not found.", guildId, questionOrder);
            throw new NotFoundException($"Guild question with guild ID {guildId} and question order {questionOrder} not found.");
        }

        return new GuildQuestionGetDto()
        {
            GuildId = guildQuestion.GuildId,
            QuestionOrder = guildQuestion.QuestionOrder,
            Question = guildQuestion.Question
        };
    }

    /// <summary>
    /// This method is invoked wanting to create a new guild question
    /// </summary>
    /// <param name="guildQuestion">The question to create</param>
    /// <returns>The created question</returns>
    public async Task<GuildQuestionGetDto> CreateGuildQuestionAsync(GuildQuestionCreateDto guildQuestion)
    {
        var newGuildQuestion = new GuildQuestion
        {
            GuildId = guildQuestion.GuildId,
            Question = guildQuestion.Question,
            QuestionOrder = guildQuestion.QuestionOrder
        };

        _context.GuildQuestions.Add(newGuildQuestion);
        await _context.SaveChangesAsync();

        return new GuildQuestionGetDto()
        {
            GuildId = newGuildQuestion.GuildId,
            QuestionOrder = newGuildQuestion.QuestionOrder,
            Question = newGuildQuestion.Question
        };
    }

    /// <summary>
    /// This method is invoked wanting to update an existing guild question
    /// </summary>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="dto">The updated question data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated question</returns>
    public async Task<GuildQuestionGetDto> UpdateGuildQuestionAsync(
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

        question.Question = dto.Question;

        await _context.SaveChangesAsync(cancellationToken);
        return new GuildQuestionGetDto()
        {
            GuildId = question.GuildId,
            QuestionOrder = question.QuestionOrder,
            Question = question.Question
        };
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
        throw new NotImplementedException();
    }
}