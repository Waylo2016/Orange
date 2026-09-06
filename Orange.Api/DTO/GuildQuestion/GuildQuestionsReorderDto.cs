using System.Collections.Generic;

namespace Orange.Api.DTO.GuildQuestion;

public class GuildQuestionsReorderDto
{
    // TODO: no reorder service exists yet - confirm this shape (ordered list of question IDs)
    // once GuildQuestionService supports reordering. See TODO.md.
    public required List<int> QuestionIds { get; set; }
}
