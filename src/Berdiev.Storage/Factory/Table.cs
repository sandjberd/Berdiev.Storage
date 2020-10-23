//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;

namespace Berdiev.Storage.Factory
{
    /// <summary>
    /// Represents the table of an relational database.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Name of the database.
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Configured columns of the current table
        /// </summary>
        public IReadOnlyList<Column> Columns { get; }

        /// <summary>
        /// Instantiates new table.
        /// </summary>
        /// <param name="name">Name of the table.</param>
        /// <param name="columns">Column s of the table.</param>
        public Table(string name, IReadOnlyList<Column> columns)
        {
            Name = name;
            Columns = columns;
        }
    }
}
