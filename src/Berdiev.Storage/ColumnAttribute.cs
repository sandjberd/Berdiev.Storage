//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Text;

namespace Berdiev.Storage
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public bool IsIdentity { get; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
            IsIdentity = false;
        }

        public ColumnAttribute(string columnName, bool isIdentity)
        {
            ColumnName = columnName;
            IsIdentity = isIdentity;
        }
    }
}
