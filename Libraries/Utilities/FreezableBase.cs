using Orchard.Environment.Extensions;
using System.Data;

namespace Piedone.HelpfulLibraries.Utilities
{
    /// <summary>
    /// Base for classes implementing the interface <see cref="IFreezable"/>.
    /// </summary>
    /// <remarks>
    /// This is a lightweight approach to the Freezable class from WPF.
    /// </remarks>
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public abstract class FreezableBase : IFreezable
    {
        /// <summary>
        /// Checks if the object can be modified or not
        /// </summary>
        public bool IsFrozen { get; private set; }

        /// <summary>
        /// Prevents further modification of the object
        /// </summary>
        public void Freeze()
        {
            IsFrozen = true;
        }

        /// <summary>
        /// Throws an ReadOnlyException if the object's state is "frozen"
        /// </summary>
        protected void ThrowIfFrozen()
        {
            if (IsFrozen) throw new ReadOnlyException("A frozen IFreezable object can't be modified.");
        }
    }
}
