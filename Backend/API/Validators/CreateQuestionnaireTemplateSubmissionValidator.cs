using Database.DTO.QuestionnaireTemplate;
using FluentValidation;

namespace API.Validators;

public class CreateQuestionnaireTemplateSubmissionValidator : AbstractValidator<QuestionnaireTemplateAdd>
{
    public CreateQuestionnaireTemplateSubmissionValidator()
    {
        // Check that all questions have unique prompts
        RuleFor(x => x.Questions).Must(q => q.DistinctBy(q => q.Prompt).Count() == q.Count)
            .WithMessage("Questions must be unique by Prompt.");
        
        // Check that all options inside a question have unique texts
        RuleForEach(x => x.Questions).ChildRules(q =>
        {
            q.RuleFor(x => x.Options).Must(o => o.DistinctBy(o => o.DisplayText).Count() == o.Count)
                .WithMessage("Options inside a question must be unique by Text.");
        });
    }
}