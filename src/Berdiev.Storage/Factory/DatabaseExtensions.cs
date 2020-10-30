//Copyright by Sandjar Berdiev

using System;
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
            var type = _GetSqLiteType(column);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(column.ColumnName).Append(' ').Append(type);

            if (column.IsPrimaryKey)
            {
                stringBuilder.Append(' ').Append("PRIMARY KEY");
            }

            if (column.IsAutoIncrement)
            {
                stringBuilder.Append(' ').Append("AUTOINCREMENT");
            }

            return stringBuilder.ToString();
        }

        private static string _GetSqLiteType(Column column)
        {
            var type = string.Empty;

            if (column.ColumnType == typeof(short) ||
                column.ColumnType == typeof(int) ||
                column.ColumnType == typeof(long) ||
                column.ColumnType == typeof(IntPtr) ||
                column.ColumnType == typeof(ushort) ||
                column.ColumnType == typeof(uint) ||
                column.ColumnType == typeof(ulong))
            {
                type = "INTEGER";
            }

            if (column.ColumnType == typeof(string))
            {
                type = "VARCHAR(255)";
            }

            if (column.ColumnType == typeof(double) ||
                column.ColumnType == typeof(float) ||
                column.ColumnType == typeof(decimal))
            {
                type = "REAL";
            }

            if (column.ColumnType == typeof(bool))
            {
                type = "BOOLEAN";
            }

            if (column.ColumnType == typeof(DateTime))
            {
                type = "TEXT";
            }

            if (string.IsNullOrEmpty(type))
                type = "BLOB";

            return type;
        }
    }
}
