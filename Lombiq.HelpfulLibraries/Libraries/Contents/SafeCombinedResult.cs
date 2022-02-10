using OrchardCore.DisplayManagement.Views;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Libraries.Contents
{
    /// <summary>
    /// A version of <see cref="CombinedResult"/> where the constructor's result list can contain <see langword="null"/>
    /// that gets filtered out. This way items can easily be made conditional using a ternary operator where one arm is
    /// <see langword="null"/>.
    /// </summary>
    public class SafeCombinedResult : CombinedResult
    {
        public SafeCombinedResult(params IDisplayResult[] results)
            : this(results.AsEnumerable())
        {
        }

        public SafeCombinedResult(IEnumerable<IDisplayResult> results)
            : base(results?.Where(item => item != null).ToList() ?? Enumerable.Empty<IDisplayResult>())
        {
        }
    }
}
