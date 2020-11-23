//Copyright by Sandjar Berdiev

using System;
using System.Data.SQLite;
using System.IO;
using Berdiev.Storage.ConnectionBridge;

namespace Berdiev.Storage.Factory
{
    /// <summary>
    /// Provides factory for database connections.
    /// </summary>
    public static class ConnectionFactory
    {
        /// <summary>
        /// Creates a sql lite connection.
        /// </summary>
        /// <param name="path">Path of the database.</param>
        /// <exception cref="InvalidOperationException">Throws if the database does not exist.</exception>
        public static SQLiteConnection CreateSqLiteConnection(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"Database '{path}' does not exist!");

            var connectionString = "Data Source=" + path;

            var sqlConnection = new SQLiteConnection(connectionString);

            return sqlConnection;
        }

        public static IConnectionBridge CreateSqLite(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"Database '{path}' does not exist!");

            var connectionString = "Data Source=" + path;
            
            var sqlConnection = new SQLiteConnection(connectionString);

            return new SqLiteConnectionBridge(sqlConnection);
        }
    }
}
