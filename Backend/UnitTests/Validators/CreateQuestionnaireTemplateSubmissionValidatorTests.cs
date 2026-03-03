namespace UnitTests.Validators
{
    public class CreateQuestionnaireTemplateSubmissionValidatorTests
    {
        private readonly CreateQuestionnaireTemplateSubmissionValidator _validator = new();

        [Fact]
        public void Validate_WithDuplicateQuestionPrompts_ShouldFail()
        {
            // Arrange
            var request = new QuestionnaireTemplateAdd
            {
                Title = "Template",
                Questions = new List<QuestionnaireQuestionAdd>
                {
                    new() { Prompt = "Same Prompt", AllowCustom = false, SortOrder = 1 },
                    new() { Prompt = "Same Prompt", AllowCustom = false, SortOrder = 2 }
                }
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Questions must be unique by Prompt.");
        }

        [Fact]
        public void Validate_WithDuplicateOptionTextsInQuestion_ShouldFail()
        {
            // Arrange
            var request = new QuestionnaireTemplateAdd
            {
                Title = "Template",
                Questions = new List<QuestionnaireQuestionAdd>
                {
                    new()
                    {
                        Prompt = "Unique Prompt",
                        AllowCustom = false,
                        SortOrder = 1,
                        Options = new List<QuestionnaireOptionAdd>
                        {
                            new() { DisplayText = "Same Option", OptionValue = 1, SortOrder = 1 },
                            new() { DisplayText = "Same Option", OptionValue = 2, SortOrder = 2 }
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Options inside a question must be unique by Text.");
        }

        [Fact]
        public void Validate_WithValidTemplate_ShouldPass()
        {
            // Arrange
            var request = new QuestionnaireTemplateAdd
            {
                Title = "Template",
                Questions = new List<QuestionnaireQuestionAdd>
                {
                    new()
                    {
                        Prompt = "First Question",
                        AllowCustom = false,
                        SortOrder = 1,
                        Options = new List<QuestionnaireOptionAdd>
                        {
                            new() { DisplayText = "Option A", OptionValue = 1, SortOrder = 1 },
                            new() { DisplayText = "Option B", OptionValue = 2, SortOrder = 2 }
                        }
                    },
                    new()
                    {
                        Prompt = "Second Question",
                        AllowCustom = true,
                        SortOrder = 2,
                        Options = new List<QuestionnaireOptionAdd>
                        {
                            new() { DisplayText = "Option C", OptionValue = 1, SortOrder = 1 }
                        }
                    }
                }
            };

            // Act
            var result = _validator.Validate(request);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
