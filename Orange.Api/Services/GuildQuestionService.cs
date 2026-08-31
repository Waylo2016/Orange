using System.Collections.Generic;
using System.Linq;
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

public class GuildQuestionService(ApplicationDbContext _context, ILogger<GuildQuestionService> _logger) : IGuildQuestions
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
    public async Task<GuildQuestion> GetGuildQuestionByIdAsync(GuildQuestionOrderDeleteDto guildQuestionOrderDeleteDto)
    {
        var guildQuestion = await _context.GuildQuestions
            .FirstOrDefaultAsync(gq => gq.GuildId == guildQuestionOrderDeleteDto.GuildId && gq.QuestionOrder == guildQuestionOrderDeleteDto.QuestionOrder);

        if (guildQuestion == null)
        {
            _logger.LogWarning("Guild question with guild ID {Id} and question order {Order} not found.", guildQuestionOrderDeleteDto.GuildId, guildQuestionOrderDeleteDto.QuestionOrder);
            throw new NotFoundException($"Guild question with ID {guildQuestionOrderDeleteDto.GuildId} not found.");
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
    /// <param name="guildQuestion">The updated question</param>
    /// <returns>The updated question</returns>
    public async Task<GuildQuestion> UpdateGuildQuestionAsync(GuildQuestionUpdateDto guildQuestion)
    {
        var existingGuildQuestion = await _context.GuildQuestions
            .FirstOrDefaultAsync(gq => gq.GuildId == guildQuestion.GuildId && gq.QuestionOrder == guildQuestion.OldQuestionOrder);

        if (existingGuildQuestion == null)
        {
            _logger.LogWarning("Guild question with guild ID {Id} and question order {Order} not found.", guildQuestion.GuildId, guildQuestion.OldQuestionOrder);
            throw new NotFoundException($"Guild question with ID {guildQuestion.GuildId} not found.");
        }

        var newGuildQuestion = new GuildQuestion
        {
            GuildId = guildQuestion.GuildId,
            Question = guildQuestion.NewQuestion,
            QuestionOrder = guildQuestion.NewQuestionOrder
        };

        _context.GuildQuestions.Update(newGuildQuestion);
        await _context.SaveChangesAsync();

        return newGuildQuestion;
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
}