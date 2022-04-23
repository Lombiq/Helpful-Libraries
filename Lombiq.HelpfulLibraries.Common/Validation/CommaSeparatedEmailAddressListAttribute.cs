using System;
using System.ComponentModel.DataAnnotations;

namespace Lombiq.HelpfulLibraries.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class CommaSeparatedEmailAddressListAttribute : DataTypeAttribute
{
    public CommaSeparatedEmailAddressListAttribute()
        : base(DataType.Text)
    {
    }

    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is not string valueAsString)
        {
            return false;
        }

        return EmailValidationHelpers.IsValidCommaSeparatedEmailAddressList(valueAsString);
    }
}
