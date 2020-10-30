//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Text;

namespace Berdiev.Storage.SqlStatements
{
    public class ColumnToUpdate
    {
        public ColumnToUpdate(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }

        public string ColumnName { get; }

        public Object Value { get; }
    }
}
