using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using Shouldly;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Lombiq.HelpfulLibraries.Tests.UnitTests.ValidationTests;

public class DataAnnotationValidatorTests
{
    [Fact]
    public static void ToContentPartTypeShouldReturnCorrectType()
    {
        var services = new ServiceCollection();
        var builder = services.AddContentPart<TestContentPart>();

        var type = typeof(TextFieldDataAnnotationValidationHandler<>);
        var result = builder.ToContentPartType(type);
        var expected = typeof(TextFieldDataAnnotationValidationHandler<TestContentPart>);

        result.ShouldNotBe(type);
        result.ShouldBe(expected);
    }

    // Note: intentionally not using "nameof" in the InlineData, both for readability and to validate a constant result.
    [Theory]
    [InlineData("MinLength", "123", "TestContentPart.MinLength.Text", "The field MinLength must be a string with a minimum length of 10.")]
    [InlineData("MaxLength", "123", "TestContentPart.MaxLength.Text", "The field MaxLength must be a string with a maximum length of 2.")]
    [InlineData("StringLength", "123", "TestContentPart.StringLength.Text", "The field StringLength must be a string with a maximum length of 2.")]
    [InlineData("Required", null, "TestContentPart.Required.Text", "The Required field is required.")]
    [InlineData("Required", "  \n\t\n  ", "TestContentPart.Required.Text", "The Required field is required.")]
    public static async Task ValidationShouldFail(string name, string text, string expectedKey, string expectedMessage)
    {
        var services = new ServiceCollection();
        services
            .AddContentPart<TestContentPart>()
            .AddTextFieldDataAnnotationValidationHandler();
        services.AddSingleton(new Mock<IUpdateModelAccessor>().Object);
        services.AddLogging();
        services.AddLocalization();

        var context = new ValidateContentFieldContext(new ContentItem())
        {
            ContentPartFieldDefinition = new ContentPartFieldDefinition(
                new ContentFieldDefinition(nameof(TextField)),
                name,
                JsonObject.Create(JsonDocument.Parse("{}").RootElement))
            {
                PartDefinition = new ContentPartDefinition(nameof(TestContentPart)),
            },
        };
        var provider = services.BuildServiceProvider();
        var validator = provider.GetRequiredService<TextFieldDataAnnotationValidationHandler<TestContentPart>>();
        await validator.ValidatingAsync(context, new TextField { Text = text });

        context.ContentValidateResult.Succeeded.ShouldBeFalse();
        var errors = context.ContentValidateResult.Errors;
        errors
            .Where(result => result.MemberNames.Single().EndsWithOrdinal(expectedKey))
            .ShouldHaveSingleItem()
            .ErrorMessage.ShouldBe(expectedMessage);
    }

    public class TestContentPart : ContentPart
    {
        [MinLength(10)]
        public TextField MinLength { get; set; } = new();

        [MaxLength(2)]
        public TextField MaxLength { get; set; } = new();

        [StringLength(2)]
        public TextField StringLength { get; set; } = new();

        [Required]
        public TextField Required { get; set; } = new();
    }
}
