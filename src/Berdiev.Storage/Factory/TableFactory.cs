//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Factory
{
    /// <summary>
    /// This factory should be used to create relational database tables.
    /// </summary>
    public class TableFactory
    {
        /// <summary>
        /// Creates a table of an existing database.
        /// </summary>
        /// <param name="type">Type of the existing SQL database.</param>
        /// <param name="path">Path of the existing database.</param>
        /// <param name="table">Table configuration for the table creation.</param>
        /// <exception cref="NotSupportedException">Thrown if the type is not supported.</exception>
        /// <exception cref="NotSupportedException">If the database does not exist.</exception>
        public static bool CreateTable(DatabaseType type, string path, Table table)
        {
            if (type != DatabaseType.SQLite)
                throw new NotSupportedException($"This type '{type}' is not supported!");

            var factory = new SqLiteFactory();

            return factory.CreateTable(path, table);
        }
    }
}
