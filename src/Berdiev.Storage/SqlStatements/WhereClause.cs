//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.SqlStatements
{
    public class WhereClause
    {
        public WhereClause(string columnName, object columnValue)
        {
            ColumnName = columnName;
            ColumnType = columnValue?.GetType();
            ColumnValue = columnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type ColumnType { get; }

        /// <summary>
        /// 
        /// </summary>
        public Object ColumnValue { get; }
    }
}
