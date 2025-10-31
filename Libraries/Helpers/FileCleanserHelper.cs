using System;
using System.IO;
using System.Linq;

namespace Piedone.HelpfulLibraries.Libraries.Helpers
{
    /// <summary>
    /// The purpose of this class is to cleanse file names and file paths. This helps prevent security flaw CWE 73
    /// outlined here: https://cwe.mitre.org/data/definitions/73.html.
    /// </summary>
    public static class FileCleanserHelper
    {
        /// <summary>
        /// Gets a cleansed file name and path.
        /// </summary>
        /// <param name="filePath">File path to cleanse.</param>
        /// <returns>The cleansed file path.</returns>
        public static string GetSafeFilePath(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (filePath.StartsWith(".") || filePath.StartsWith("/") || filePath.StartsWith("\\"))
            {
                var error = $"Invalid file path '{filePath}': must be absolute, or not start with '.', '/', and '\\'.";
                throw new ArgumentException(error);
            }

            // Replaces black-listed characters.
            filePath = Path.GetInvalidPathChars()
                .Aggregate(filePath, (current, invalidChar) => current.Replace(invalidChar, ' '))
                .Trim();

            if (filePath == string.Empty)
                throw new ArgumentException($"Invalid file path '{filePath}': contains no valid characters.");

            var fullPath = Path.GetFullPath(filePath);
            var directoryName = Path.GetDirectoryName(fullPath);
            var fileName = Path.GetFileName(fullPath);

            fileName = GetSafeFileName(fileName);

            return Path.Combine(directoryName, fileName);
        }

        /// <summary>
        /// Gets cleansed file name.
        /// </summary>
        /// <param name="fileName">File name to cleanse.</param>
        /// <returns>The cleansed file name.</returns>
        public static string GetSafeFileName(string fileName) =>
            // Replaces black-listed characters.
            Path.GetInvalidFileNameChars()
                .Aggregate(fileName, (current, invalidChar) => current.Replace(invalidChar, ' '))
                .Trim();
    }
}
