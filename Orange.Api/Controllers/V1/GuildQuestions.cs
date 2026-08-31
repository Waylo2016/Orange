using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Exceptions;
using Orange.Api.Interfaces;
using Orange.Api.Models;

namespace Orange.Api.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Tags("Guild - Questions")]
[Produces("application/json")]
public class GuildQuestionsController(IGuildQuestions guildQuestions) : ControllerBase
{

    /// <summary>
    /// invoked wanting to see all guild questions
    /// </summary>
    /// <param name="guildIdDto">The ID of the guild for which to retrieve questions</param>
    /// <returns>A list of guild questions</returns>
    [HttpGet("questions/guild")]
    [ProducesResponseType(typeof(List<GuildQuestion>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GuildQuestionGetDto>>> GetGuildQuestions([FromBody] GuildIdDTO guildIdDto)
    {
        try
        {
            var questions = await guildQuestions.GetGuildQuestionsPerGuildAsync(guildIdDto);
            return Ok(questions);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }

    }

    /// <summary>
    /// invoked wanting to get a specific guild question by its ID
    /// </summary>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="id">The order of the question within the guild</param>
    /// <returns>the specified question</returns>
    [HttpGet("questions/guild/{guildId:ulong}/questionOrder/{guestionOrder:int}")]
    [ProducesResponseType(typeof(GuildQuestionGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuildQuestionGetDto>> GetGuildQuestionById([FromRoute] ulong guildId, [FromRoute] int id)
    {
        var guildQuestionOrderGetDto = new GuildQuestionOrderDeleteDto
        {
            GuildId = guildId,
            QuestionOrder = id
        };
        
        try
        {
            var question = await guildQuestions.GetGuildQuestionByIdAsync(guildQuestionOrderGetDto);
            return Ok(question);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    
    /// <summary>
    /// invoked wanting to create a new question
    /// </summary>
    /// <param name="guildQuestion">The question to create</param>
    /// <returns>The created question</returns>
    [HttpPost("questions/create")]
    [ProducesResponseType(typeof(GuildQuestionUpdateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuildQuestionUpdateDto>> CreateGuildQuestion([FromBody] GuildQuestionCreateDto guildQuestion)
    {
        try
        {
            var createdQuestion = await guildQuestions.CreateGuildQuestionAsync(guildQuestion);
            return CreatedAtAction(
                nameof(GetGuildQuestionById), 
                new
                {
                    countOrder = createdQuestion.QuestionOrder,
                    guildId = createdQuestion.GuildId
                }, createdQuestion);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// invoked wanting to update an existing question
    /// </summary>
    /// <param name="guildQuestion">The updated question</param>
    /// <returns>The updated question</returns>
    [HttpPut("questions/update")]
    [ProducesResponseType(typeof(GuildQuestionUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuildQuestionUpdateDto>> UpdateGuildQuestion([FromBody] GuildQuestionUpdateDto guildQuestion)
    {
        try
        {
            var updatedQuestion = await guildQuestions.UpdateGuildQuestionAsync(guildQuestion);
            return Ok(updatedQuestion);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// invoked wanting to delete an existing question
    /// </summary>
    /// <param name="guildQuestionOrderDeleteDto">The information of the question to delete</param>
    /// <returns>True if the question was deleted, false otherwise</returns>
    [HttpDelete("questions/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<bool>> DeleteGuildQuestion([FromBody] GuildQuestionOrderDeleteDto guildQuestionOrderDeleteDto)
    {
        bool result = await guildQuestions.DeleteGuildQuestionAsync(guildQuestionOrderDeleteDto);
        return result ? NoContent() : Problem("An unexpected error occurred while trying to delete the guild question.");
        
    }
}