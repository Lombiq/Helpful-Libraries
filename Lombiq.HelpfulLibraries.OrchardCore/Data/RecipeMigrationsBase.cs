using OrchardCore.Data.Migration;
using OrchardCore.Modules.Manifest;
using OrchardCore.Recipes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Data;

/// <summary>
/// A base class for recipe migrations that automatically calls the <c>{module-or-theme-id}.UpdateFrom0.recipe.json</c>
/// recipe on creation and makes it easier to call update recipes in a similar format using the <see
/// cref="ExecuteAsync"/> helper method.
/// </summary>
public abstract class RecipeMigrationsBase : DataMigration
{
    protected virtual string BaseName => GetType()
        .Assembly
        .GetCustomAttributes<FeatureAttribute>()
        .Select(feature => feature.Id)
        .WhereNot(string.IsNullOrEmpty)
        .FirstOrDefault();

    protected readonly IRecipeMigrator _recipeMigrator;

    protected RecipeMigrationsBase(IRecipeMigrator recipeMigrator) => _recipeMigrator = recipeMigrator;

    public virtual Task<int> CreateAsync() => ExecuteAsync(0);

    protected async Task<int> ExecuteAsync(int version)
    {
        var baseName = BaseName ?? throw new InvalidOperationException("Failed to get the current assembly's feature ID.");
        var recipeName = $"UpdateFrom{version.ToTechnicalString()}";
        await _recipeMigrator.ExecuteAsync($"{baseName}.{recipeName}.recipe.json", this);
        return version + 1;
    }
}
