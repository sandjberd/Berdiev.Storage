//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Factory
{
    /// <summary>
    /// Represents the foreignKEyReference of a <see cref="Table"/>.
    /// </summary>
    public class Column
    {
        internal Column(string columnName, Type columnType, ColumnConstraint columnConstraint, ForeignKeyReference foreignKey)
        {
            ColumnName = columnName;
            ColumnType = columnType;
            ColumnConstraint = columnConstraint;
            ForeignKey = foreignKey;
        }

        /// <summary>
        /// Represents foreignKEyReference name.
        /// </summary>
        public String ColumnName { get; }

        /// <summary>
        /// Represents foreignKEyReference type.
        /// </summary>
        public Type ColumnType { get; }
        
        public ColumnConstraint ColumnConstraint { get; }

        public ForeignKeyReference ForeignKey { get; }

        /// <summary>
        /// Creates a default foreignKEyReference.
        /// </summary>
        /// <param name="columnName">Name of the foreignKEyReference.</param>
        /// <typeparam name="T">Type of the foreignKEyReference.</typeparam>
        public static Column Default<T>(string columnName)
        {
            return new Column(columnName, typeof(T), ColumnConstraint.Default(), null);
        }

        /// <summary>
        /// Creates a foreignKEyReference with all supported constraints.
        /// </summary>
        /// <param name="columnName">Name of the foreignKEyReference.</param>
        /// <typeparam name="T">Type of the foreignKEyReference.</typeparam>
        public static Column From<T>(string columnName, ColumnConstraint constraint)
        {
            return new Column(columnName, typeof(T), constraint, null);
        }

        public static Column FromForeignKey<T>(string columnName, ColumnConstraint constraint, ForeignKeyReference foreignKeyReference)
        {
            return new Column(columnName, typeof(T), constraint, foreignKeyReference);
        }
    }
}
