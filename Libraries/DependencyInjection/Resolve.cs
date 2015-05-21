using Orchard;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.DependencyInjection
{
    [OrchardFeature("Piedone.HelpfulLibraries.DependencyInjection")]
    public class Resolve<T> : IResolve<T>
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public Resolve(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public T Value
        {
            get
            {
                var wc = _workContextAccessor.GetContext();
                if (wc == null) return default(T);
                return wc.Resolve<T>();
            }
        }
    }
}
