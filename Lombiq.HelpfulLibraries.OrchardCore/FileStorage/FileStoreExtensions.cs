using System.Threading.Tasks;

namespace OrchardCore.FileStorage;

public static class FileStoreExtensions
{
    /// <summary>
    /// Checks the existence of a file in the file store.
    /// </summary>
    /// <param name="path">Relative path to the file.</param>
    public static async Task<bool> FileExistsAsync(this IFileStore fileStore, string path) =>
        (await fileStore.GetFileInfoAsync(path)) != null;

    /// <summary>
    /// Checks the existence of a directory in the file store.
    /// </summary>
    /// <param name="path">Relative path to the directory.</param>
    public static async Task<bool> DirectoryExistsAsync(this IFileStore fileStore, string path) =>
        (await fileStore.GetDirectoryInfoAsync(path)) != null;
}
