using Lombiq.HelpfulLibraries.OrchardCore.Data;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using YesSql.Indexes;

namespace Lombiq.HelpfulLibraries.Samples.Migrations;

// Here we demonstrate a more tightly coupled way of declaring parts and fields. We won't do anything with the part and
// index so they can be a local classes. This way it's easier to see the whole thing in context too.
public class ExpressionSampleMigration : DataMigration
{
    // The content type
    private const string ExpressionContent = nameof(ExpressionContent);

    private readonly IContentDefinitionManager _contentDefinitionManager;

    public ExpressionSampleMigration(IContentDefinitionManager contentDefinitionManager)
        => _contentDefinitionManager = contentDefinitionManager;

    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1114:Parameter list should follow declaration",
        Justification = "Needed for commenting first arguments.")]
    public int Create()
    {
        _contentDefinitionManager.AlterTypeDefinition(ExpressionContent, type => type
            // The point of using SetAbilities instead of individual extensions is that this way you can be explicit
            // about whether you want to refuse some feature or you don't care. This improves maintainability.
            .SetAbilities(
                creatable: true,
                listable: true)
            .WithTitlePart()
            .WithPart(
                // The new generic AlterPartDefinition overload returns the type name, so you can create a part
                // definition "in-line".
                _contentDefinitionManager.AlterPartDefinition<ExpressionPart>(partBuilder => partBuilder
                    // Even when you don't configure anything you get the correct "OfType" and the property name as
                    // display name by default.
                    .WithField(part => part.SomeText)
                    // Besides that the configuration action works exactly the same as the stock one.
                    .WithField(part => part.OtherText, field => field.WithDisplayName("Some Other Text"))
                    .WithField(part => part.SomeNumber, field => field.
                        WithSettings(new NumericFieldSettings { DefaultValue = "10", Minimum = 0 })))));

        // This generic overload lets you create an index automatically, using property attributes to set up common
        // constraints.
        SchemaBuilder.CreateMapIndexTable<ExpressionIndex>();

        return 1;
    }

    public class ExpressionPart : ContentPart
    {
        public TextField SomeText { get; set; } = new();
        public TextField OtherText { get; set; } = new();
        public NumericField SomeNumber { get; set; } = new();
    }

    public class ExpressionIndex : MapIndex
    {
        // This calls WithCommonUniqueIdLength to set the length to 26.
        [ContentItemIdColumn]
        public string ContentItem { get; set; }

        [UnlimitedLength]
        public string SomeText { get; set; }

        // This is coming from DataAnnotations. We only use the length property.
        [MaxLength(length: 150)]
        public string OtherText { get; set; }

        public int SomeNumber { get; set; }
    }
}
