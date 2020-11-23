//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage
{
    /// <summary>
    /// This attribute is used to mark a table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Represents the table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Instantiates new attribute.
        /// </summary>
        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
