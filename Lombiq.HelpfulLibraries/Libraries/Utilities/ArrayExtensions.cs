using System.Linq;

namespace System
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Converts an <see cref="object"/> array to a <see cref="string"/> array.
        /// </summary>
        /// <param name="includeNulls">If <see langword="false"/>, the converted array will exclude nulls (i.e., the
        /// that are not <see cref="string"/>s).</param>
        /// <returns>The created <see cref="string"/> array.</returns>
        public static string[] ToStringArray(this object[] array, bool includeNulls = false)
        {
            var stringArray = Array.ConvertAll(array, item => item?.ToString());

            return includeNulls ? stringArray : stringArray.Where(item => item != null).ToArray();
        }
    }
}
