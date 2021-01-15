//Copyright by Sandjar Berdiev

using System.Linq;
using System.Text;

namespace Berdiev.Storage.Factory
{
    internal static class DatabaseExtensions
    {
        public static string ToSql(this Table table)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("CREATE TABLE")
                .Append(' ')
                .Append(table.Name)
                .Append(' ')
                .Append('(');

            var prefix = string.Empty;
            foreach (var tableColumn in table.Columns)
            {
                stringBuilder.Append(prefix);
                stringBuilder.Append(tableColumn.ToSql());
                prefix = ", ";
            }

            var foreignKeyColumns = table.Columns.Where(x => x.ForeignKey != null);

            foreach (var foreignKeyColumn in foreignKeyColumns)
            {
                stringBuilder
                    .Append(", ")
                    .Append($"FOREIGN KEY ({foreignKeyColumn.ColumnName}) ")
                    .Append("REFERENCES ")
                    .Append(foreignKeyColumn.ForeignKey.ReferenceTableName)
                    .Append($" ({foreignKeyColumn.ForeignKey.ReferenceColumnName})");
            }

            return stringBuilder.Append(");").ToString();
        }

        public static string ToSql(this Column column)
        {
            return ColumnToSqlConverter.ConvertToSql(column);
        }
    }
}
