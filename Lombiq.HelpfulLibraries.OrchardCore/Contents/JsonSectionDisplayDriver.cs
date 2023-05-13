using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public abstract class JsonSectionDisplayDriver<TSection> : SectionDisplayDriver<ISite, TSection>
    where TSection : new()
{
    protected abstract string GroupId { get; }
    protected virtual Permission Permission { get; }
    protected virtual string ShapeType => $"{typeof(TSection).Name}_Edit";
    protected virtual string Location => "Content:1";

    protected readonly IAuthorizationService _authorizationService;
    protected readonly IHttpContextAccessor _hca;

    protected JsonSectionDisplayDriver(
        IAuthorizationService authorizationService,
        IHttpContextAccessor hca)
    {
        _authorizationService = authorizationService;
        _hca = hca;
    }

    public override async Task<IDisplayResult> EditAsync(TSection section, BuildEditorContext context) =>
        await AuthorizeAsync()
            ? Initialize<JsonViewModel>(
                    ShapeType,
                    settings => settings.Json = JsonConvert.SerializeObject(section))
                .Location(Location)
                .OnGroup(GroupId)
            : null;

    public override async Task<IDisplayResult> UpdateAsync(TSection section, BuildEditorContext context)
    {
        var viewModel = new JsonViewModel();

        if (context.GroupId == GroupId &&
            await AuthorizeAsync() &&
            await context.Updater.TryUpdateModelAsync(viewModel, Prefix) &&
            TryParseJson(viewModel.Json, out var result))
        {
            await UpdateAsync(section, context, result);
        }

        return await EditAsync(section, context);
    }

    protected abstract Task UpdateAsync(TSection section, BuildEditorContext context, TSection viewModel);

    private Task<bool> AuthorizeAsync() =>
        Permission == null
            ? Task.FromResult(true)
            : _authorizationService.AuthorizeCurrentUserAsync(_hca.HttpContext, Permission);

    private static bool TryParseJson(string json, out TSection result)
    {
        try
        {
            result = JsonConvert.DeserializeObject<TSection>(json);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public class JsonViewModel
    {
        public string Json { get; set; }
    }
}

