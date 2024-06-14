using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using OrchardCore.Email;
using OrchardCore.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Validation;

public class EmailAndPasswordValidator : IEmailAndPasswordValidator
{
    private readonly IEmailAddressValidator _emailAddressValidator;
    private readonly UserManager<IUser> _userManager;
    private readonly IStringLocalizer<EmailAndPasswordValidator> T;
    public EmailAndPasswordValidator(
        IEmailAddressValidator emailAddressValidator,
        UserManager<IUser> userManager,
        IStringLocalizer<EmailAndPasswordValidator> stringLocalizer)
    {
        _emailAddressValidator = emailAddressValidator;
        _userManager = userManager;
        T = stringLocalizer;
    }

    public Task<IEnumerable<LocalizedString>> ValidateEmailAsync(string email) =>
        Task.FromResult(_emailAddressValidator.Validate(email)
            ? Enumerable.Empty<LocalizedString>()
            : [T["Invalid email address."]]);

    public async Task<IEnumerable<LocalizedString>> ValidatePasswordAsync(string password)
    {
        var errors = new List<LocalizedString>();
        if (password == null) return errors;

        foreach (var passwordValidator in _userManager.PasswordValidators)
        {
            var result = await passwordValidator.ValidateAsync(_userManager, user: null, password);

            if (result.Succeeded) continue;
            errors.AddRange(result.Errors.Select(error => T[error.Description]));
        }

        return errors;
    }
}
