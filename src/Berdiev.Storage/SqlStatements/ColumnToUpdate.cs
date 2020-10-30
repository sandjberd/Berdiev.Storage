//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.SqlStatements
{
    /// <summary>
    /// This represents a column of a specific table for update.
    /// </summary>
    public class ColumnToUpdate
    {
        /// <summary>
        /// Instantiates new column.
        /// </summary>
        public ColumnToUpdate(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }

        /// <summary>
        /// Column name of the table.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// The new value that is used to update.
        /// </summary>
        public Object Value { get; }
    }
}
