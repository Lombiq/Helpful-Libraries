using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class CultureHelpers
{
    /// <summary>
    /// Gets all available <see cref="Country"/> objects and orders them by <see cref="Country.DisplayText"/>.
    /// </summary>
    public static IEnumerable<Country> GetCountries() =>
        CultureInfo
            .GetCultures(CultureTypes.SpecificCultures)
            .Select(culture =>
            {
                // This sometimes throws "CultureNotFoundException: Culture is not supported." exception on Linux, or
                // "ArgumentException: Customized cultures cannot be passed by ID, only by name." on Windows.
                try { return new RegionInfo(culture.LCID); } // #spell-check-ignore-line
                catch { return null; }
            })
            .Where(region => region is { TwoLetterISORegionName.Length: 2 } && !string.IsNullOrEmpty(region.EnglishName))
            .Distinct()
            .Select(regionInfo => new Country
            {
                TwoLetterIsoCode = regionInfo.TwoLetterISORegionName,
                DisplayText = regionInfo.EnglishName,
            })
            .OrderBy(country => country.DisplayText);

    /// <summary>
    /// Gets all available <see cref="Language"/> objects.
    /// </summary>
    public static IEnumerable<Language> GetLanguages() =>
        CultureInfo
            .GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
            .Select(culture =>
                new Language
                {
                    Code = culture.Name,
                    DisplayText = culture.EnglishName,
                });
}

public sealed class Country : IEquatable<Country>
{
    public string TwoLetterIsoCode { get; set; }
    public string DisplayText { get; set; }

    public bool Equals(Country other) => other != null && TwoLetterIsoCode == other.TwoLetterIsoCode;

    public override bool Equals(object obj) => Equals(obj as Country);

    public override int GetHashCode() => HashCode.Combine(TwoLetterIsoCode);
}

public sealed class Language : IEquatable<Language>
{
    public string Code { get; set; }
    public string DisplayText { get; set; }

    public bool Equals(Language other) => other != null && Code == other.Code;

    public override bool Equals(object obj) => Equals(obj as Language);

    public override int GetHashCode() => HashCode.Combine(Code);
}
