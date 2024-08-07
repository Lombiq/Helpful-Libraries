using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public abstract class JsonSectionDisplayDriver<TSection, TAdditionalData> : SiteDisplayDriver<TSection>
    where TSection : class, new()
{
    protected abstract string GroupId { get; }
    protected virtual Permission Permission => null;
    protected virtual string ShapeType => $"{typeof(TSection).Name}_Edit";
    protected virtual string Location => $"{CommonLocationNames.Content}:1";

    protected readonly IAuthorizationService _authorizationService;
    protected readonly IHttpContextAccessor _hca;

    protected JsonSectionDisplayDriver(
        IAuthorizationService authorizationService,
        IHttpContextAccessor hca)
    {
        _authorizationService = authorizationService;
        _hca = hca;
    }

    public async override Task<IDisplayResult> EditAsync(ISite model, TSection section, BuildEditorContext context) =>
        await AuthorizeAsync()
            ? Initialize<JsonViewModel<TAdditionalData>>(
                    ShapeType,
                    async settings =>
                    {
                        settings.Json = JsonSerializer.Serialize(section);
                        settings.AdditionalData = await GetAdditionalDataAsync(section, context);
                    })
                .Location(Location)
                .OnGroup(GroupId)
            : null;

    public override async Task<IDisplayResult> UpdateAsync(ISite model, TSection section, UpdateEditorContext context)
    {
        if (await context.CreateModelMaybeAsync<JsonViewModel<TAdditionalData>>(Prefix, AuthorizeAsync) is { } viewModel &&
            TryParseJson(viewModel.Json, out var result))
        {
            await UpdateAsync(section, context, result);
        }

        return await EditAsync(model, section, context);
    }

    protected abstract Task UpdateAsync(TSection section, BuildEditorContext context, TSection viewModel);

    protected virtual Task<TAdditionalData> GetAdditionalDataAsync(TSection section, BuildEditorContext context) =>
        Task.FromResult<TAdditionalData>(default);

    private Task<bool> AuthorizeAsync() =>
        Permission == null
            ? Task.FromResult(true)
            : _authorizationService.AuthorizeCurrentUserAsync(_hca.HttpContext, Permission);

    private static bool TryParseJson(string json, out TSection result)
    {
        result = null;

        try
        {
            if (string.IsNullOrEmpty(json)) return false;

            result = JsonSerializer.Deserialize<TSection>(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class JsonViewModel<TAdditionalData>
{
    public string Json { get; set; }

    [BindNever]
    public TAdditionalData AdditionalData { get; internal set; }
}
