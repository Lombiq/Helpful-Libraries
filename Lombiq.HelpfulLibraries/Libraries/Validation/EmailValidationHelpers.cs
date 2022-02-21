using System;
using System.Linq;
using System.Net.Mail;

namespace Lombiq.HelpfulLibraries.Libraries.Validation
{
    public static class EmailValidationHelpers
    {
        public static bool IsValidEmailAddress(string email)
        {
            try
            {
#pragma warning disable S1481
                var mailAddress = new MailAddress(email);
#pragma warning restore S1481

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
}
