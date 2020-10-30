//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage
{
    /// <summary>
    /// This is used to mark a column of a table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Name of the column.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Instantiates new column attribute.
        /// </summary>
        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
