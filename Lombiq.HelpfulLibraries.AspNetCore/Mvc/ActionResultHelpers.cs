using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using static System.Net.Mime.MediaTypeNames.Application;

namespace Lombiq.HelpfulLibraries.AspNetCore.Mvc;

public static class ActionResultHelpers
{
    /// <summary>
    /// Compresses the <paramref name="files"/> into a zip archive ready to be served by a controller.
    /// </summary>
    /// <param name="files">
    /// The file collection where key is the zip entry file name and value is the data stream.
    /// </param>
    /// <param name="zipFileName">The filename of the archive, can be with or without the ".zip" at the end.</param>
    public static FileResult ZipFile(IDictionary<string, Stream> files, string zipFileName)
    {
        using var outStream = new MemoryStream();

        using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach ((string fileName, var fileStream) in files)
            {
                var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                fileStream.CopyTo(entryStream);
                fileStream.Dispose();
            }
        }

        var compressedBytes = outStream.ToArray();

        if (!zipFileName.EndsWithOrdinalIgnoreCase(".zip")) zipFileName += ".zip";
        return new FileContentResult(compressedBytes, Zip) { FileDownloadName = zipFileName };
    }
}
