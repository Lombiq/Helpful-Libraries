using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Piedone.HelpfulLibraries.Libraries.Helpers
{
    /// <summary>
    /// File Cleanser Helper Class
    /// </summary>
    public static class FileCleanserHelper
    {
        /// <summary>
        /// Gets a cleansed file name and path.
        /// </summary>
        /// <param name="fileNameToValidate">File Name to Validate</param>
        /// <returns></returns>
        public static string GetSafeFilePath(string fileNameToValidate)
        {
            if (string.IsNullOrEmpty(fileNameToValidate))
            {
                throw new ArgumentException("Parameter 'fileNameToValidate' can not be null.", nameof(fileNameToValidate));
            }

            if (fileNameToValidate.StartsWith(".") || fileNameToValidate.StartsWith("/"))
            {
                throw new ApplicationException($"Invalid file path '{fileNameToValidate}'.");
            }

            char[] blackListChars = Path.GetInvalidPathChars();

            //Replace Black-listed Characters
            foreach (var invalidChar in blackListChars)
            {
                if (fileNameToValidate.Contains(invalidChar))
                {
                    fileNameToValidate = fileNameToValidate.Replace(invalidChar, ' ').Trim();
                }
            }

            string fullPath = Path.GetFullPath(fileNameToValidate);
            string directoryName = Path.GetDirectoryName(fullPath);
            string fileName = Path.GetFileName(fullPath);

            fileName = GetSafeFileName(fileName);

            //Validated Path
            string finalPath = Path.Combine(directoryName, fileName);

            return finalPath;
        }

        /// <summary>
        /// Gets cleansed file name.
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <returns></returns>
        public static string GetSafeFileName(string fileName)
        {
            char[] blackListFilename = Path.GetInvalidFileNameChars();

            //Replace Black-listed File Names
            foreach (var invalidChar in blackListFilename)
            {
                if (fileName.Contains(invalidChar))
                {
                    fileName = fileName.Replace(invalidChar, ' ').Trim();
                }
            }

            return fileName;
        }
    }

}
