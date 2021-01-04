using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Lombiq.HelpfulLibraries.Libraries.Mvc
{
    public static class ActionResultHelpers
    {
        /// <summary>
        /// Compresses the <paramref name="files"/> into a zip archive ready to be served by a controller.
        /// </summary>
        /// <param name="files">The file collection where key is the zip entry file name and value is the data.</param>
        /// <param name="zipFileName">The filename of the archive, can be with or without the ".zip" at the end.</param>
        public static FileResult ZipFile(IDictionary<string, byte[]> files, string zipFileName)
        {
            // The MemoryStream gets disposed by the FileStreamResult. see: https://stackoverflow.com/a/51655063
            using var outStream = new MemoryStream();

            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach ((string fileName, byte[] fileBytes) in files)
                {
                    var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    using var fileToCompressStream = new MemoryStream(fileBytes);
                    fileToCompressStream.CopyTo(entryStream);
                }
            }

            var compressedBytes = outStream.ToArray();

            if (!zipFileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)) zipFileName += ".zip";
            return new FileContentResult(compressedBytes, MimeTypes.Zip) { FileDownloadName = zipFileName };
        }
    }
}
