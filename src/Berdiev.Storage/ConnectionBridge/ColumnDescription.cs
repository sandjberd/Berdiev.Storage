//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.ConnectionBridge
{
    internal class ColumnDescription
    {
        internal ColumnDescription(string name, bool isIdentity, Type columnType)
        {
            Name = name;
            IsIdentity = isIdentity;
            ColumnType = columnType;
        }

        public String Name { get; }

        public Boolean IsIdentity { get; }

        public Type ColumnType { get; }

        public static ColumnDescription FromAttribute(ColumnAttribute attribute, Type columnType)
        {
            return new ColumnDescription(attribute.ColumnName, attribute.IsIdentity, columnType);
        }
    }
}
