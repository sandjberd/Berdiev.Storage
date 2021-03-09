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
            SqlOperator = SqlOperator.Equals;
        }

        /// <summary>
        /// Instantiates the where clause.
        /// </summary>
        public WhereClause(string columnName, object columnValue, SqlOperator sqlOperator)
        {
            ColumnName = columnName;
            ColumnType = columnValue?.GetType();
            ColumnValue = columnValue;
            SqlOperator = sqlOperator;
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

        /// <summary>
        /// Operator of the sql where clause for values.
        /// </summary>
        public SqlOperator SqlOperator { get; }
    }
}
