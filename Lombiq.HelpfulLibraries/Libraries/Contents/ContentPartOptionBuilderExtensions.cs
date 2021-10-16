using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    public static class ContentPartOptionBuilderExtensions
    {
        public static ContentPartOptionBuilder WithMigration<TMigration>(this ContentPartOptionBuilder builder)
            where TMigration : IDataMigration
        {
            builder.Services.AddScoped(typeof(IDataMigration), typeof(TMigration));
            return builder;
        }
    }
}
