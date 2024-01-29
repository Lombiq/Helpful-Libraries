using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using OrchardCore.Email;
using OrchardCore.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Validation;

public class EmailAndPasswordValidator(
    IEmailAddressValidator emailAddressValidator,
    UserManager<IUser> userManager,
    IStringLocalizer<EmailAndPasswordValidator> stringLocalizer) : IEmailAndPasswordValidator
{
    private readonly IStringLocalizer T = stringLocalizer;

    public Task<IEnumerable<LocalizedString>> ValidateEmailAsync(string email) =>
        Task.FromResult(emailAddressValidator.Validate(email)
            ? Enumerable.Empty<LocalizedString>()
            : new[] { T["Invalid email address."] });

    public async Task<IEnumerable<LocalizedString>> ValidatePasswordAsync(string password)
    {
        var errors = new List<LocalizedString>();
        if (password == null) return errors;

        foreach (var passwordValidator in userManager.PasswordValidators)
        {
            var result = await passwordValidator.ValidateAsync(userManager, user: null, password);

            if (result.Succeeded) continue;
            errors.AddRange(result.Errors.Select(error => T[error.Description]));
        }

        return errors;
    }
}
