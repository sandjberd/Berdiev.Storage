//Copyright by Sandjar Berdiev

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

            return stringBuilder.Append(");").ToString();
        }

        public static string ToSql(this Column column)
        {
            return ColumnToSqlConverter.ConvertToSql(column);
        }
    }
}
