using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    public static class CultureHelpers
    {
        public static IEnumerable<Country> GetCountries() =>
            CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(culture =>
                {
                    var regionInfo = new RegionInfo(culture.LCID);
                    return new Country
                    {
                        TwoLetterIsoCode = regionInfo.TwoLetterISORegionName,
                        CountryName = regionInfo.EnglishName,
                    };
                })
                .Distinct()
                .OrderBy(country => country.CountryName);
    }

    public class Country
    {
        public string TwoLetterIsoCode { get; set; }
        public string CountryName { get; set; }
    }
}