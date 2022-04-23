using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class CultureHelpers
{
    public static IEnumerable<Country> GetCountries() =>
        CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(culture =>
            {
                var regionInfo = new RegionInfo(culture.LCID);
                return new Country
                {
                    TwoLetterIsoCode = regionInfo.TwoLetterISORegionName,
                    DisplayText = regionInfo.EnglishName,
                };
            })
            .Distinct()
            .OrderBy(country => country.DisplayText);

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
