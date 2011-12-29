using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                return _workContextAccessor.GetContext().Resolve<T>();
            }
        }
    }
}
