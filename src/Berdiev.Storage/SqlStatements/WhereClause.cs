//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.SqlStatements
{
    /// <summary>
    /// This is used as standard SQL where clause for basic CRUD operations.
    /// </summary>
    public class WhereClause
    {
        /// <summary>
        /// Instantiates the where clause.
        /// </summary>
        public WhereClause(string columnName, object columnValue)
        {
            ColumnName = columnName;
            ColumnType = columnValue?.GetType();
            ColumnValue = columnValue;
        }

        /// <summary>
        /// Column name of the table.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// The table column type.
        /// </summary>
        public Type ColumnType { get; }

        /// <summary>
        /// The value of the table column.
        /// </summary>
        public Object ColumnValue { get; }
    }
}
