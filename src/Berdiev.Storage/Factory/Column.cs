//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Factory
{
    /// <summary>
    /// Represents the column of a <see cref="Table"/>.
    /// </summary>
    public class Column
    {
        internal Column(string columnName, Type columnType, bool isPrimaryKey, bool isForeignKey, bool isAutoIncrement)
        {
            ColumnName = columnName;
            ColumnType = columnType;
            IsPrimaryKey = isPrimaryKey;
            IsForeignKey = isForeignKey;
            IsAutoIncrement = isAutoIncrement;
        }

        /// <summary>
        /// Represents column name.
        /// </summary>
        public String ColumnName { get; }

        /// <summary>
        /// Represents column type.
        /// </summary>
        public Type ColumnType { get; }

        /// <summary>
        /// Sets the column as primary key.
        /// </summary>
        public Boolean IsPrimaryKey { get; }

        /// <summary>
        /// Sets the column as foreign key.
        /// </summary>
        public Boolean IsForeignKey { get; }

        /// <summary>
        /// Auto increment for the column. Column must be an INTEGER.
        /// </summary>
        public Boolean IsAutoIncrement { get; }

        /// <summary>
        /// Creates a default column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <typeparam name="T">Type of the column.</typeparam>
        public static Column Default<T>(string columnName)
        {
            return new Column(columnName, typeof(T), false, false, false);
        }

        /// <summary>
        /// Creates a column with all supported constraints.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="primaryKey">Sets the column as primary key.</param>
        /// <param name="foreignKey">Sets the column as foreign key.</param>
        /// <param name="autoIncrement">Defines the column as autoIncrement.</param>
        /// <typeparam name="T">Type of the column.</typeparam>
        public static Column From<T>(string columnName, bool primaryKey, bool foreignKey, bool autoIncrement)
        {
            return new Column(columnName, typeof(T), primaryKey, foreignKey, autoIncrement);
        }
    }
}
