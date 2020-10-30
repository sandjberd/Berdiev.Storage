//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Dynamic;
using System.Linq;
using System.Text;
using Berdiev.Storage.ConnectionBridge;
using Dapper;
using FluentMigrator.Infrastructure.Extensions;

namespace Berdiev.Storage.SqlStatements
{
    public static class QueryExtensions
    {
        private const string _columnNameWhereClausePostfix = "WhereToUpdate";

        public static IEnumerable<T> Get<T>(this SQLiteConnection cnn)
        {
            _VerifyClassAsTable<T>();

            var sql = $"SELECT * FROM {typeof(T).Name};";
            
            var result = cnn.Query<T>(sql);

            return result;
        }

        public static IEnumerable<T> Select<T>(this SQLiteConnection cnn, Predicate<T> selectionCondition)
        {
            _VerifyClassAsTable<T>();

            var sql = $"SELECT * FROM {typeof(T).Name};";

            if (selectionCondition == null)
            {
                return cnn.Get<T>();
            }

            var reader = cnn.ExecuteReader(sql);

            var specifiedValues = new List<T>();

            while (reader.Read())
            {
                var f =reader.GetRowParser<T>();

                var item = f(reader);

                if (selectionCondition(item))
                {
                    specifiedValues.Add(item);
                }
            }

            reader.Dispose();

            return specifiedValues;
        }

        public static IEnumerable<T> Select<T>(this SQLiteConnection cnn, IReadOnlyList<WhereClause> whereClauses)
        {
            _VerifyClassAsTable<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);

            var sql = $"SELECT * FROM {typeof(T).Name} {sqlWhereClause};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = cnn.Query<T>(sql, whereClausesObject);

            return query;
        }

        public static bool Update<T>(this SQLiteConnection cnn, T itemToUpdate, WhereClause whereClause)
        {
            var sql = _CreateUpdateSqlStatement<T>(whereClause);

            cnn.Execute(sql, new object[]
            {
                itemToUpdate
            });

            return true;
        }

        public static bool Update<T>(this SQLiteConnection cnn, IReadOnlyList<ColumnToUpdate> columnsToUpdate, WhereClause whereClause)
        {
            var sql = _CreateUpdateSqlStatement<T>(columnsToUpdate, whereClause);

            var t = _CreateObjectForUpdate<T>(columnsToUpdate, whereClause);

            cnn.Execute(sql, t);

            return true;
        }

        private static object _CreateObjectForUpdate<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, WhereClause whereClause)
        {
            var objectMappings = new Dictionary<string, object>();

            foreach (var columnToUpdate in columnsToUpdate)
            {
                objectMappings.Add(columnToUpdate.ColumnName, columnToUpdate.Value);
            }

            objectMappings.Add(whereClause.ColumnName+_columnNameWhereClausePostfix, whereClause.ColumnValue);

            return new DynamicParameters(objectMappings);
        }

        private static object _CreateObjectForWhereClauses(IReadOnlyList<WhereClause> whereClauses)
        {
            var objectMappings = new Dictionary<string, object>();

            foreach (var clause in whereClauses)
            {
                objectMappings.Add(clause.ColumnName + _columnNameWhereClausePostfix, clause.ColumnValue);
            }

            return new DynamicParameters(objectMappings);
        }

        public static bool Insert<T>(this SQLiteConnection cnn, T item)
        {
            _VerifyClassAsTable<T>();

            var sql = _CreateInsertSqlStatement<T>();

            var cmdDefinition = new CommandDefinition(sql, item);

            cnn.Execute(cmdDefinition);

            return true;
        }

        public static bool InsertMany<T>(this SQLiteConnection cnn, IReadOnlyList<T> item)
        {
            _VerifyClassAsTable<T>();

            var sql = _CreateInsertSqlStatement<T>();

            var cmdDefinition = new CommandDefinition(sql, item);

            cnn.Execute(cmdDefinition);

            return true;
        }

        private static void _VerifyClassAsTable<T>()
        {
            var tableAttribute = typeof(T).GetOneAttribute<TableAttribute>();

            if (tableAttribute == null)
                throw new ArgumentException($"{nameof(T)} must contain {nameof(TableAttribute)}");
        }

        private static string _CreateInsertSqlStatement<T>()
        {
            var columnDescriptions = _GetColumnDescriptions(typeof(T));

            var columnNameBuilder = new StringBuilder();
            var columnValueBuilder = new StringBuilder();

            columnNameBuilder.Append('(');
            columnValueBuilder.Append('(');

            var prefix = string.Empty;
            foreach (var columnDescription in columnDescriptions)
            {
                columnNameBuilder.Append(prefix);
                columnValueBuilder.Append(prefix);

                columnNameBuilder.Append(columnDescription.Name);
                columnValueBuilder.Append('@').Append(columnDescription.Name);

                prefix = ", ";
            }

            columnNameBuilder.Append(')');
            columnValueBuilder.Append(')');
            var columnNames = columnNameBuilder.ToString();
            var columnValues = columnValueBuilder.ToString();

            var sql = $"INSERT INTO {typeof(T).Name} {columnNames} VALUES {columnValues};";
            return sql;
        }

        private static string _CreateUpdateSqlStatement<T>(IReadOnlyList<ColumnToUpdate> columnToUpdates, WhereClause whereClause)
        {
            var updateSetDefinition = new StringBuilder();

            var prefix = string.Empty;
            foreach (var columnDescription in columnToUpdates)
            {
                updateSetDefinition
                    .Append(prefix)
                    .Append(columnDescription.ColumnName)
                    .Append(" = ")
                    .Append('@')
                    .Append(columnDescription.ColumnName);

                prefix = ", ";
            }

            var sql = $"UPDATE {typeof(T).Name} SET {updateSetDefinition.ToString()};";

            if (whereClause != null)
            {
                sql = $"UPDATE {typeof(T).Name} SET {updateSetDefinition.ToString()} WHERE {whereClause.ColumnName} = @{whereClause.ColumnName}{_columnNameWhereClausePostfix};";
            }

            return sql;
        }

        private static string _CreateUpdateSqlStatement<T>(WhereClause whereClause)
        {
            var columnDescriptions = _GetColumnDescriptions(typeof(T));

            var updateSetDefinition = new StringBuilder();

            var prefix = string.Empty;
            foreach (var columnDescription in columnDescriptions)
            {
                updateSetDefinition
                    .Append(prefix)
                    .Append(columnDescription.Name)
                    .Append(" = ")
                    .Append('@')
                    .Append(columnDescription.Name);

                prefix = ", ";
            }
            
            var sql = $"UPDATE {typeof(T).Name} SET {updateSetDefinition.ToString()}";

            if (whereClause != null)
            {
                sql = $"UPDATE {typeof(T).Name} SET {updateSetDefinition.ToString()} WHERE {whereClause.ColumnName} = @{whereClause.ColumnName}";
            }

            return sql;
        }

        private static string _CreateWhereSqlStatement(IReadOnlyList<WhereClause> whereClauses)
        {
            if (!whereClauses.Any())
                return string.Empty;

            var whereClauseBuilder = new StringBuilder();

            whereClauseBuilder.Append("WHERE ");
            var prefix = string.Empty;
            foreach (var whereClause in whereClauses)
            {
                whereClauseBuilder
                    .Append(prefix)
                    .Append(whereClause.ColumnName)
                    .Append(" = ")
                    .Append('@')
                    .Append(whereClause.ColumnName).Append(_columnNameWhereClausePostfix);

                prefix = " AND ";
            }

            return whereClauseBuilder.ToString();
        }

        private static IReadOnlyList<ColumnDescription> _GetColumnDescriptions(Type type)
        {
            var columnDescriptions = new List<ColumnDescription>();

            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.HasAttribute<ColumnAttribute>())
                    continue;

                var attr = propertyInfo.GetOneAttribute<ColumnAttribute>();
                columnDescriptions.Add(ColumnDescription.FromAttribute(attr, propertyInfo.PropertyType));
            }

            return columnDescriptions;
        }


    }
}
