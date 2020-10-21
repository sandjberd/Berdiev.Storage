﻿//Copyright by Sandjar Berdiev

using System;
using System.Data.SQLite;
using System.IO;

namespace Berdiev.Storage.Factory
{
    internal class SqLiteFactory
    {
        public void CreateDatabase(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path must not be empty or null!");

            var fileName = Path.GetFileName(path);
            var dirName = Path.GetDirectoryName(path);

            var dirInfo = new DirectoryInfo(path);
            if (!string.IsNullOrEmpty(dirName))
                dirInfo = new DirectoryInfo(dirName);

            if (string.IsNullOrEmpty(fileName) || !Path.HasExtension(path))
                throw new ArgumentException("Invalid filepath for database creation!");

            if (File.Exists(path))
                throw new InvalidOperationException($"Database '{fileName}' exists already!");

            var dirReadOnly = FileAttributes.ReadOnly | FileAttributes.Directory;

            if (dirInfo.Attributes == dirReadOnly)
                throw new InvalidOperationException("Directory is in read only mode, can not write!");

            SQLiteConnection.CreateFile(path);
        }
    }
}
