//Copyright by Sandjar Berdiev

using System.Collections.Generic;

namespace Berdiev.Storage.SqlStatements
{
    public class OrderByClause
    {
        public OrderByClause(IReadOnlyList<string> orderByColumns, bool desc)
        {
            OrderByColumns = orderByColumns;
            Desc = desc;
        }

        public OrderByClause(IReadOnlyList<string> orderByColumns)
        {
            OrderByColumns = orderByColumns;
            Desc = true;
        }

        public IReadOnlyList<string> OrderByColumns { get; }

        public bool Desc { get; }
    }
}
