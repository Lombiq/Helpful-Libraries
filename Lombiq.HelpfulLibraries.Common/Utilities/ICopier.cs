namespace Lombiq.HelpfulLibraries.Common.Utilities;

/// <summary>
/// Types implementing this interface can copy their values to a <typeparamref name="TTarget"/> object.
/// </summary>
/// <typeparam name="TTarget">The type of the object to copy to.</typeparam>
public interface ICopier<in TTarget>
{
    /// <summary>
    /// Copies the applicable contents of the current instance into <paramref name="target"/>, overwriting its original
    /// values.
    /// </summary>
    void CopyTo(TTarget target);
}
