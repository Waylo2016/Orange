using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orange.Api.DTO.Guild;
using Orange.Api.DTO.GuildQuestion;
using Orange.Api.Exceptions;
using Orange.Api.Interfaces;

namespace Orange.Api.Controllers.V1;

[ApiController]
[Route("api/v{version:apiVersion}/guilds/{guildId:ulong}/questions")]
[ApiVersion("1.0")]
[Tags("Guild - Questions")]
[Produces("application/json")]
public class GuildQuestionsController(IGuildQuestionService guildQuestionService) : ControllerBase
{
    /// <summary>
    /// invoked wanting to see all questions for a guild
    /// </summary>
    /// <remarks>Auth: bot + mod (not yet enforced - see TODO.md)</remarks>
    /// <param name="guildId">The ID of the guild for which to retrieve questions</param>
    /// <returns>A list of guild questions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GuildQuestionGetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GuildQuestionGetDto>>> GetGuildQuestions([FromRoute] ulong guildId)
    {
        var questions = await guildQuestionService.GetGuildQuestionsPerGuildAsync(new GuildIdDTO { GuildId = guildId });
        return Ok(questions);
    }

    /// <summary>
    /// invoked wanting to create a new question for a guild
    /// </summary>
    /// <remarks>Auth: bot + mod (not yet enforced - see TODO.md)</remarks>
    /// <param name="guildId">The ID of the guild the question belongs to</param>
    /// <param name="guildQuestion">The question to create</param>
    /// <returns>The created question</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GuildQuestionGetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuildQuestionGetDto>> CreateGuildQuestion(
        [FromRoute] ulong guildId,
        [FromBody] GuildQuestionCreateDto guildQuestion)
    {
        guildQuestion.GuildId = guildId;

        try
        {
            var createdQuestion = await guildQuestionService.CreateGuildQuestionAsync(guildQuestion);
            return CreatedAtAction(
                nameof(GetGuildQuestionById),
                new
                {
                    guildId = createdQuestion.GuildId,
                    questionId = createdQuestion.Id
                },
                createdQuestion);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// invoked wanting to get a single question by its ID
    /// </summary>
    /// <remarks>
    /// Auth: bot + mod (not yet enforced - see TODO.md).
    /// TODO: IGuildQuestionService has no lookup by question ID yet, only by question order - see TODO.md.
    /// </remarks>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>the specified question</returns>
    [HttpGet("{questionId:int}")]
    [ProducesResponseType(typeof(GuildQuestionGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuildQuestionGetDto>> GetGuildQuestionById([FromRoute] ulong guildId, [FromRoute] int questionId)
    {
        try
        {
            var question = await guildQuestionService.GetGuildQuestionByIdAsync(guildId, questionId);

            return Ok(question);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// invoked wanting to update an existing question
    /// </summary>
    /// <remarks>Auth: bot + mod (not yet enforced - see TODO.md)</remarks>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <param name="guildQuestion">The updated question data</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated question</returns>
    [HttpPut("{questionId:int}")]
    [ProducesResponseType(typeof(GuildQuestionUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuildQuestionUpdateDto>> UpdateGuildQuestion(
        [FromRoute] ulong guildId,
        [FromRoute] int questionId,
        [FromBody] GuildQuestionUpdateDto guildQuestion,
        CancellationToken cancellationToken)
    {
        try
        {
            var updatedQuestion = await guildQuestionService.UpdateGuildQuestionAsync(guildId, questionId, guildQuestion, cancellationToken);
            return Ok(updatedQuestion);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    /// <summary>
    /// invoked wanting to delete an existing question
    /// </summary>
    /// <remarks>
    /// Auth: bot + mod (not yet enforced - see TODO.md).
    /// TODO: IGuildQuestionService has no delete-by-question-ID method yet, only by question order - see TODO.md.
    /// </remarks>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="questionId">The ID of the question</param>
    /// <returns>no content</returns>
    [HttpDelete("{questionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteGuildQuestion([FromRoute] ulong guildId, [FromRoute] int questionId)
    {
        return Problem("not yet implemented", statusCode: StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// invoked wanting to reorder the questions of a guild
    /// </summary>
    /// <remarks>
    /// Auth: bot + mod (not yet enforced - see TODO.md).
    /// TODO: no reorder service method exists yet - see TODO.md.
    /// </remarks>
    /// <param name="guildId">The ID of the guild</param>
    /// <param name="reorderDto">The new question order</param>
    /// <returns>no content</returns>
    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReorderGuildQuestions([FromRoute] ulong guildId, [FromBody] GuildQuestionsReorderDto reorderDto)
    {
        return Problem("not yet implemented", statusCode: StatusCodes.Status501NotImplemented);
    }
}
