//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Factory
{
    /// <summary>
    /// Provides mechanism for database creations.
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// Creates a database, that can be used for further operations.
        /// </summary>
        /// <param name="type">Defines which database type should be created.</param>
        /// <param name="filePath">Path where the database should be created.</param>
        /// <exception cref="ArgumentException">If the file path is invalid</exception>
        /// <exception cref="InvalidOperationException">If the database already exists</exception>
        public static void CreateDatabase(DatabaseType type, string filePath)
        {
            if (type == DatabaseType.SQLite)
            {
                var sqliteFactory = new SqLiteFactory();
                sqliteFactory.CreateDatabase(filePath);
                return;
            }

            throw new NotSupportedException($"Other database types than '{type}' are not supported!");
        }
    }
}
