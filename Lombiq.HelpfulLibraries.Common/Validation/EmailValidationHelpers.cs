using System;
using System.Linq;
using System.Net.Mail;

namespace Lombiq.HelpfulLibraries.Common.Validation;

/// <summary>
/// Helpers to validate e-mail addresses.
/// </summary>
public static class EmailValidationHelpers
{
    public static bool IsValidEmailAddress(string email)
    {
        try
        {
            _ = new MailAddress(email);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool IsValidCommaSeparatedEmailAddressList(string emailAddresses) =>
        emailAddresses.SplitByCommas().All(IsValidEmailAddress);
}
