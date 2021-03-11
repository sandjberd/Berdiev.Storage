//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Berdiev.Storage.Factory;
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
        private SQLiteTransaction _transaction = default;

        public SqLiteConnectionBridge(SQLiteConnection connection)
        {
            _connection = connection;
            _lock = new Object();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public void BeginTransaction()
        {
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction == default)
                return;

            _transaction.Commit();
            _connection.Close();
        }

        public void RollbackTransaction()
        {
            if (_transaction == default)
                return;

            _transaction.Rollback();
            _connection.Close();
        }

        public IEnumerable<T> GetAll<T>()
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sql = $"SELECT * FROM {tableName};";

            var result = _connection.Query<T>(sql);
            
            Monitor.Exit(_lock);

            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sql = $"SELECT * FROM {tableName};";

            var result = await _connection.QueryAsync<T>(sql).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return result;
        }

        public IEnumerable<T> Get<T>(Paging paging, OrderByClause orderByClause, IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);
            var sqlOrderByClause = _CreateOrderBySqlStatement(orderByClause);
            
            var sql = $"SELECT * FROM {tableName} {sqlWhereClause} {sqlOrderByClause} LIMIT {paging.Limit} OFFSET {paging.Offset};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = _connection.Query<T>(sql, whereClausesObject);

            Monitor.Exit(_lock);

            return query;
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Paging paging, OrderByClause orderByClause, IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);
            var sqlOrderByClause = _CreateOrderBySqlStatement(orderByClause);

            var sql = $"SELECT * FROM {tableName} {sqlWhereClause} {sqlOrderByClause} LIMIT {paging.Limit} OFFSET {paging.Offset};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = await _connection.QueryAsync<T>(sql, whereClausesObject);

            Monitor.Exit(_lock);

            return query;
        }

        public int GetRowCount<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();
            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);

            var sql = $"SELECT COUNT(*) FROM {tableName} {sqlWhereClause};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = _connection.QueryFirst<int>(sql, whereClausesObject);

            Monitor.Exit(_lock);

            return query;
        }

        public IEnumerable<T> SelectRecords<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);

            var sql = $"SELECT * FROM {tableName} {sqlWhereClause};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = _connection.Query<T>(sql, whereClausesObject);

            Monitor.Exit(_lock);

            return query;
        }

        public async Task<IEnumerable<T>> SelectRecordsAsync<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sqlWhereClause = _CreateWhereSqlStatement(whereClauses);

            var sql = $"SELECT * FROM {tableName} {sqlWhereClause};";

            var whereClausesObject = _CreateObjectForWhereClauses(whereClauses);

            var query = await _connection.QueryAsync<T>(sql, whereClausesObject).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return query;
        }

        public bool Update<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var sql = _CreateUpdateSqlStatement<T>(columnsToUpdate, whereClauses);

            var t = _CreateObjectForUpdate(columnsToUpdate, whereClauses);

            var rowsAffected = _connection.Execute(sql, t);

            Monitor.Exit(_lock);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAsync<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

            var sql = _CreateUpdateSqlStatement<T>(columnsToUpdate, whereClauses);

            var t = _CreateObjectForUpdate(columnsToUpdate, whereClauses);

            await _connection.ExecuteAsync(sql, t).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return true;
        }

        public bool Insert<T>(T item)
        {
            Monitor.Enter(_lock);

            var sql = _CreateInsertSqlStatement<T>();
            var objectToInsert = _CreateObjectForInsert(item);

            var cmdDefinition = new CommandDefinition(sql, objectToInsert);

            var affectedRows = _connection.Execute(cmdDefinition);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }



        public async Task<bool> InsertAsync<T>(T item)
        {
            Monitor.Enter(_lock);

            var sql = _CreateInsertSqlStatement<T>();
            var objectToInsert = _CreateObjectForInsert(item);

            var cmdDefinition = new CommandDefinition(sql, objectToInsert);

            var affectedRows = await _connection.ExecuteAsync(cmdDefinition).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public bool InsertMany<T>(IEnumerable<T> items)
        {
            Monitor.Enter(_lock);

            var sql = _CreateInsertSqlStatement<T>();

            var objects = items.Select(_CreateObjectForInsert);

            var cmdDefinition = new CommandDefinition(sql, objects);

            var affectedRows = _connection.Execute(cmdDefinition);

            Monitor.Exit(_lock);

            return affectedRows == items.Count();
        }

        public async Task<bool> InsertManyAsync<T>(IEnumerable<T> items)
        {
            Monitor.Enter(_lock);

            var sql = _CreateInsertSqlStatement<T>();
            var objects = items.Select(_CreateObjectForInsert);

            var cmdDefinition = new CommandDefinition(sql, objects);

            var affectedRows = await _connection.ExecuteAsync(cmdDefinition).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public bool Delete<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            Monitor.Enter(_lock);

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

            var sql = _CreateDeleteSqlStatement<T>(whereClauses);

            var whereObject = _CreateObjectForWhereClauses(whereClauses);

            var cmdDefinition = new CommandDefinition(sql, whereObject);

            var affectedRows = await _connection.ExecuteAsync(cmdDefinition).ConfigureAwait(false);

            Monitor.Exit(_lock);

            return affectedRows > 0;
        }

        public bool CreateIndex<T>(string name, string columnName)
        {
            Monitor.Enter(_lock);

            var tableName = _GetTableName<T>();

            var sql = $"CREATE INDEX {name} ON {tableName} ({columnName})";

            var res = false;

            try
            {
                _connection.Execute(new CommandDefinition(sql));

                res = true;
            }
            catch (Exception e)
            {
                res = false;
            }

            Monitor.Exit(_lock);

            return res;
        }

        private string _GetTableName<T>()
        {
            var tableAttribute = typeof(T).GetOneAttribute<TableAttribute>();

            if (tableAttribute == null)
                throw new ArgumentException($"{nameof(T)} must contain {nameof(TableAttribute)}");

            return tableAttribute.TableName;
        }

        private object _CreateObjectForUpdate(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses)
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

        private object _CreateObjectForWhereClauses(IReadOnlyList<WhereClause> whereClauses)
        {
            var objectMappings = new Dictionary<string, object>();

            foreach (var clause in whereClauses)
            {
                objectMappings.Add(clause.ColumnName + ColumnNameWhereClausePostfix, clause.ColumnValue);
            }

            return new DynamicParameters(objectMappings);
        }

        private string _CreateInsertSqlStatement<T>()
        {
            var tableName = _GetTableName<T>();

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

            var sql = $"INSERT INTO {tableName} {columnNames} VALUES {columnValues};";
            return sql;
        }

        private string _CreateDeleteSqlStatement<T>(IReadOnlyList<WhereClause> whereClauses)
        {
            var tableName = _GetTableName<T>();

            var sql = $"DELETE FROM {tableName};";

            if (whereClauses.Any())
            {
                var whereClausesSql = _CreateWhereSqlStatement(whereClauses);
                sql = $"DELETE FROM {tableName} {whereClausesSql};";
            }

            return sql;
        }

        private string _CreateUpdateSqlStatement<T>(IReadOnlyList<ColumnToUpdate> columnToUpdates, IReadOnlyList<WhereClause> whereClauses)
        {
            var updateSetDefinition = new StringBuilder();
            var tableName = _GetTableName<T>();

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

            var sql = $"UPDATE {tableName} SET {updateSetDefinition};";

            if (whereClauses.Any())
            {
                var whereClausesSql = _CreateWhereSqlStatement(whereClauses);
                sql = $"UPDATE {tableName} SET {updateSetDefinition} {whereClausesSql};";
            }

            return sql;
        }

        private string _CreateWhereSqlStatement(IReadOnlyList<WhereClause> whereClauses)
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
                    .Append(whereClause.SqlOperator.ToSqlString())
                    .Append('@')
                    .Append(whereClause.ColumnName).Append(ColumnNameWhereClausePostfix);

                prefix = " AND ";
            }

            return whereClauseBuilder.ToString();
        }

        private string _CreateOrderBySqlStatement(OrderByClause clause)
        {
            var clauseBuilder = new StringBuilder();

            if (!clause.OrderByColumns.Any())
                return string.Empty;

            clauseBuilder.Append("ORDER BY ");
            var prefix = string.Empty;
            foreach (var clauseOrderByColumn in clause.OrderByColumns)
            {
                clauseBuilder
                    .Append(prefix)
                    .Append(clauseOrderByColumn);

                prefix = ", ";
            }

            clauseBuilder.Append(clause.Desc ? " DESC" : " ASC");

            return clauseBuilder.ToString();
        }

        private IReadOnlyList<ColumnDescription> _GetColumnDescriptions(Type type)
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

        private object _CreateObjectForInsert<T>(T item)
        {
            var objectMappings = new Dictionary<string, object>();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (!propertyInfo.HasAttribute<ColumnAttribute>())
                    continue;

                var attr = propertyInfo.GetOneAttribute<ColumnAttribute>();
                objectMappings.Add(attr.ColumnName, propertyInfo.GetValue(item));
            }

            return new DynamicParameters(objectMappings);
        }
    }
}
