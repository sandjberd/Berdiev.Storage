﻿//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Berdiev.Storage.SqlStatements;
using Dapper;
using FluentMigrator.Infrastructure.Extensions;

namespace Berdiev.Storage.ConnectionBridge
{
    internal class SqLiteConnectionBridge : IConnectionBridge
    {
        private readonly SQLiteConnection _connection;
        private const string ColumnNameWhereClausePostfix = "WhereToUpdate";

        private readonly object _lock;

        public SqLiteConnectionBridge(SQLiteConnection connection)
        {
            _connection = connection;
            _lock = new Object();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public IEnumerable<T> GetAll<T>()
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = $"SELECT * FROM {typeof(T).Name};";

            var result = _connection.Query<T>(sql);
            
            Monitor.Exit(_lock);

            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = $"SELECT * FROM {typeof(T).Name};";

            var result = await _connection.QueryAsync<T>(sql).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return result;
        }

        public IEnumerable<T> SelectRecords<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);

            var sql = $"SELECT * FROM {typeof(T).Name} {sqlWhereClause};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = _connection.Query<T>(sql, whereClausesObject);

            Monitor.Exit(_lock);

            return query;
        }

        public async Task<IEnumerable<T>> SelectRecordsAsync<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);

            var sql = $"SELECT * FROM {typeof(T).Name} {sqlWhereClause};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = await _connection.QueryAsync<T>(sql, whereClausesObject).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return query;
        }

        public bool Update<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var sql = _CreateUpdateSqlStatement<T>(columnsToUpdate, whereClauses);

            var t = _CreateObjectForUpdate<T>(columnsToUpdate, whereClauses);

            var rowsAffected = _connection.Execute(sql, t);

            Monitor.Exit(_lock);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var sql = _CreateUpdateSqlStatement<T>(columnsToUpdate, whereClauses);

            var t = _CreateObjectForUpdate<T>(columnsToUpdate, whereClauses);

            await _connection.ExecuteAsync(sql, t).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return true;
        }

        public bool Insert<T>(T item)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = _CreateInsertSqlStatement<T>();

            var cmdDefinition = new CommandDefinition(sql, item);

            var affectedRows = _connection.Execute(cmdDefinition);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public async Task<bool> InsertAsync<T>(T item)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = _CreateInsertSqlStatement<T>();

            var cmdDefinition = new CommandDefinition(sql, item);

            var affectedRows = await _connection.ExecuteAsync(cmdDefinition).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public bool InsertMany<T>(IEnumerable<T> items)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = _CreateInsertSqlStatement<T>();

            var cmdDefinition = new CommandDefinition(sql, items);

            var affectedRows = _connection.Execute(cmdDefinition);

            Monitor.Exit(_lock);

            return affectedRows == items.Count();
        }

        public async Task<bool> InsertManyAsync<T>(IEnumerable<T> items)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = _CreateInsertSqlStatement<T>();

            var cmdDefinition = new CommandDefinition(sql, items);

            var affectedRows = await _connection.ExecuteAsync(cmdDefinition).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public bool Delete<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = _CreateDeleteSqlStatement<T>(whereClauses);

            var whereObject = _CreateObjectForWhereClauses(whereClauses);

            var cmdDefinition = new CommandDefinition(sql, whereObject);

            var affectedRows = _connection.Execute(cmdDefinition);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            _VerifyClassAsTable<T>();

            var sql = _CreateDeleteSqlStatement<T>(whereClauses);

            var whereObject = _CreateObjectForWhereClauses(whereClauses);

            var cmdDefinition = new CommandDefinition(sql, whereObject);

            var affectedRows = await _connection.ExecuteAsync(cmdDefinition).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        private static void _VerifyClassAsTable<T>()
        {
            var tableAttribute = typeof(T).GetOneAttribute<TableAttribute>();

            if (tableAttribute == null)
                throw new ArgumentException($"{nameof(T)} must contain {nameof(TableAttribute)}");
        }

        private static object _CreateObjectForUpdate<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses)
        {
            var objectMappings = new Dictionary<string, object>();

            foreach (var columnToUpdate in columnsToUpdate)
            {
                objectMappings.Add(columnToUpdate.ColumnName, columnToUpdate.Value);
            }

            foreach (var whereClause in whereClauses)
            {
                objectMappings.Add(whereClause.ColumnName + ColumnNameWhereClausePostfix, whereClause.ColumnValue);
            }

            return new DynamicParameters(objectMappings);
        }

        private static object _CreateObjectForWhereClauses(IReadOnlyList<WhereClause> whereClauses)
        {
            var objectMappings = new Dictionary<string, object>();

            foreach (var clause in whereClauses)
            {
                objectMappings.Add(clause.ColumnName + ColumnNameWhereClausePostfix, clause.ColumnValue);
            }

            return new DynamicParameters(objectMappings);
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

        private static string _CreateDeleteSqlStatement<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            var sql = $"DELETE FROM {typeof(T).Name};";

            if (whereClauses.Any())
            {
                var whereClausesSql = _CreateWhereSqlStatement(whereClauses);
                sql = $"DELETE FROM {typeof(T).Name} {whereClausesSql};";
            }

            return sql;
        }

        private static string _CreateUpdateSqlStatement<T>(IReadOnlyList<ColumnToUpdate> columnToUpdates, IReadOnlyList<WhereClause> whereClauses)
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

            var sql = $"UPDATE {typeof(T).Name} SET {updateSetDefinition};";

            if (whereClauses.Any())
            {
                var whereClausesSql = _CreateWhereSqlStatement(whereClauses);
                sql = $"UPDATE {typeof(T).Name} SET {updateSetDefinition} {whereClausesSql};";
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
                    .Append(whereClause.ColumnName).Append(ColumnNameWhereClausePostfix);

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