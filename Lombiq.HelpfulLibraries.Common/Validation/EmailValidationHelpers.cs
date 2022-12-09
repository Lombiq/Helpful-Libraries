using System;
using System.Linq;
using System.Net.Mail;

namespace Lombiq.HelpfulLibraries.Common.Validation;

/// <summary>
/// Helpers to validate email addresses.
/// </summary>
public static class EmailValidationHelpers
{
    /// <summary>
    /// Determines whether the provided string is a valid email address and returns <see langword="true"/> if so.
    /// </summary>
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

    /// <summary>
    /// Determines whether the provided string is a valid list of comma-separated email addresses and returns
    /// <see langword="true"/> if so.
    /// </summary>
    public static bool IsValidCommaSeparatedEmailAddressList(string emailAddresses) =>
        emailAddresses.SplitByCommas().All(IsValidEmailAddress);
}
