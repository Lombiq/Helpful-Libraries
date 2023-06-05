using System;
using System.Net.Mail;

namespace Lombiq.HelpfulLibraries.Common.Validation;

/// <summary>
/// Helpers to validate email addresses.
/// </summary>
public static class EmailValidationHelpers
{
    /// <summary>
    /// Determines whether the provided string is a valid email address.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the given string is a valid email address, <see langword="false"/> otherwise.
    /// </returns>
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
    /// Determines whether the provided string is a valid list of comma-separated email addresses.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the given string is a valid list of comma-separated email addresses,
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool IsValidCommaSeparatedEmailAddressList(string emailAddresses) =>
        emailAddresses.SplitByCommas().TrueForAll(IsValidEmailAddress);
}
