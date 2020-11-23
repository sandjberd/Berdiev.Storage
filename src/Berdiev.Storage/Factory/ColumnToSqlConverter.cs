//Copyright by Sandjar Berdiev

using System;
using System.Text;

namespace Berdiev.Storage.Factory
{
    internal static class ColumnToSqlConverter
    {
        public static string ConvertToSql(Column column)
        {
            var type = _GetSqLiteType(column);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(column.ColumnName).Append(' ').Append(type);

            _ConvertConstraints(column, stringBuilder);

            if (column.ForeignKey != null)
            {
                stringBuilder
                    .Append(", ")
                    .Append($"FOREIGN KEY ({column.ColumnName}) ")
                    .Append("REFERENCES ")
                    .Append(column.ForeignKey.ReferenceTableName)
                    .Append($" ({column.ForeignKey.ReferenceColumnName})");
            }

            return stringBuilder.ToString();
        }

        private static void _ConvertConstraints(Column column, StringBuilder stringBuilder)
        {
            if (column.ColumnConstraint.IsPrimaryKey)
            {
                stringBuilder.Append(' ').Append("PRIMARY KEY");
            }

            if (column.ColumnConstraint.IsAutoIncrement)
            {
                stringBuilder.Append(' ').Append("AUTOINCREMENT");
            }

            if (column.ColumnConstraint.IsNotNull)
            {
                stringBuilder.Append(' ').Append("NOT NULL");
            }

            if (column.ColumnConstraint.IsUnique)
            {
                stringBuilder.Append(' ').Append("UNIQUE");
            }
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

            if (column.ColumnType == typeof(DateTimeOffset))
            {
                type = "TEXT";
            }

            if (column.ColumnType == typeof(Guid))
            {
                type = "TEXT";
            }

            if (string.IsNullOrEmpty(type))
                type = "BLOB";

            return type;
        }
    }
}
