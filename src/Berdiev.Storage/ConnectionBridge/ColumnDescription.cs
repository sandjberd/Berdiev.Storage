//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.ConnectionBridge
{
    internal class ColumnDescription
    {
        internal ColumnDescription(string name,  Type columnType)
        {
            Name = name;
            ColumnType = columnType;
        }

        public String Name { get; }

        public Type ColumnType { get; }

        public static ColumnDescription FromAttribute(ColumnAttribute attribute, Type columnType)
        {
            return new ColumnDescription(attribute.ColumnName, columnType);
        }
    }
}
