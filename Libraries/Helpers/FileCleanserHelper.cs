using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Piedone.HelpfulLibraries.Libraries.Helpers
{
    /// <summary>
    /// File Cleanser Helper Class
    /// 
    /// The purpose of this class is to cleanse either a file name or a path to a file. 
    /// This helps prevent a CWE 73 security flaw outlined here: https://cwe.mitre.org/data/definitions/73.html
    /// </summary>
    public static class FileCleanserHelper
    {
        /// <summary>
        /// Gets a cleansed file name and path.
        /// </summary>
        /// <param name="filePath">File path to cleanse.</param>
        /// <returns></returns>
        public static string GetSafeFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("Parameter 'filePath' can not be null.", nameof(filePath));
            }

            if (filePath.StartsWith(".") || filePath.StartsWith("/"))
            {
                throw new ApplicationException($"Invalid file path '{filePath}'.");
            }

            var blackListChars = Path.GetInvalidPathChars();

            // Replaces black-listed characters.
            foreach (var invalidChar in blackListChars)
            {
                if (filePath.Contains(invalidChar))
                {
                    filePath = filePath.Replace(invalidChar, ' ');
                }
            }

            filePath = filePath.Trim();

            var fullPath = Path.GetFullPath(filePath);
            var directoryName = Path.GetDirectoryName(fullPath);
            var fileName = Path.GetFileName(fullPath);

            fileName = GetSafeFileName(fileName);

            var finalPath = Path.Combine(directoryName, fileName);

            return finalPath;
        }

        /// <summary>
        /// Gets cleansed file name.
        /// </summary>
        /// <param name="fileName">File name to cleanse.</param>
        /// <returns></returns>
        public static string GetSafeFileName(string fileName)
        {
            var blackListFilename = Path.GetInvalidFileNameChars();

            // Replaces black-listed characters.
            foreach (var invalidChar in blackListFilename)
            {
                if (fileName.Contains(invalidChar))
                {
                    fileName = fileName.Replace(invalidChar, ' ');
                }
            }

            fileName = fileName.Trim();

            return fileName;
        }
    }

}
