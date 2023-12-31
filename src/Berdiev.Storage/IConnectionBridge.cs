﻿//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Berdiev.Storage.SqlStatements;

namespace Berdiev.Storage
{
    /// <summary>
    /// This represents a connection between a database and the domain level.
    /// The owner of this has the control of CRUD operations of the desired database.
    ///
    /// <para>
    /// Note that the owner must dispose the instance if it's not needed anymore.
    /// </para>
    /// 
    /// <para>
    /// Every operation is thread safe. That means that CRUD operations are executed sequentially of different threads.
    /// </para>
    /// </summary>
    public interface IConnectionBridge : IDisposable
    {
        /// <summary>
        /// Begins a transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits a current transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollbacks a current transaction.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Gets all records of a table. (SELECT * FROM [TABLE-NAME];)
        /// </summary>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>All records of the table.</returns>
        IEnumerable<T> GetAll<T>();

        /// <summary>
        /// Gets all records of a table in async way. (SELECT * FROM [TABLE-NAME];)
        /// </summary>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>All records of the table.</returns>
        Task<IEnumerable<T>> GetAllAsync<T>();

        /// <summary>
        /// Gets all limited records of a table. (SELECT * FROM [TABLE-NAME];)
        /// </summary>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>All limited records of the table.</returns>
        IEnumerable<T> Get<T>(Paging paging, OrderByClause orderByClause, IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Gets all limited records of a table. (SELECT * FROM [TABLE-NAME];)
        /// </summary>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>All limited records of the table.</returns>
        Task<IEnumerable<T>> GetAsync<T>(Paging paging, OrderByClause orderByClause, IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Counts rows of the table.
        /// </summary>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>Number of rows.</returns>
        int GetRowCount<T>(IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Selects specific records that are filtered by a 'where clause'. (SELECT * FROM [TABLE-NAME] WHERE ...;)
        /// </summary>
        /// <param name="whereClauses">Where clauses for filtering the records.</param>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>Filtered records of the table.</returns>
        IEnumerable<T> SelectRecords<T>(IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Selects specific records that are filtered by a 'where clause' in async way. (SELECT * FROM [TABLE-NAME] WHERE ...;)
        /// </summary>
        /// <param name="whereClauses">Where clauses for filtering the records.</param>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>Filtered records of the table.</returns>
        Task<IEnumerable<T>> SelectRecordsAsync<T>(IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Updates specific columns of a table. (UPDATE [TABLE-NAME] SET [columns] = [columnValues] WHERE ...;)
        /// </summary>
        /// <param name="columnsToUpdate">Defines column names and column values. These values represents the future values of the table.</param>
        /// <param name="whereClauses">Where clauses for updating the values.</param>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>True, if the update was successful, otherwise false.</returns>
        bool Update<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Updates specific columns of a table in async way. (UPDATE [TABLE-NAME] SET [columns] = [columnValues] WHERE ...;)
        /// </summary>
        /// <param name="columnsToUpdate">Defines column names and column values. These values represents the future values of the table.</param>
        /// <param name="whereClauses">Where clauses for updating the values.</param>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>True, if the update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync<T>(IReadOnlyList<ColumnToUpdate> columnsToUpdate, IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Inserts an item into table.
        /// </summary>
        /// <param name="item">Item to insert in the table.</param>
        /// <typeparam name="T">The type represents the table.</typeparam>
        /// <returns>True, if insertion was successful, otherwise false.</returns>
        bool Insert<T>(T item);

        /// <summary>
        /// Inserts an item into table in async way.
        /// </summary>
        /// <param name="item">Item to insert in the table.</param>
        /// <typeparam name="T">The type represents the table.</typeparam>
        /// <returns>True, if insertion was successful, otherwise false.</returns>
        Task<bool> InsertAsync<T>(T item);

        /// <summary>
        /// Inserts many items into table.
        /// </summary>
        /// <param name="items">Items to insert in the table.</param>
        /// <typeparam name="T">The type represents the table.</typeparam>
        /// <returns>True, if insertion was successful, otherwise false.</returns>
        bool InsertMany<T>(IEnumerable<T> items);

        /// <summary>
        /// Inserts many items into table in async way.
        /// </summary>
        /// <param name="items">Items to insert in the table.</param>
        /// <typeparam name="T">The type represents the table.</typeparam>
        /// <returns>True, if insertion was successful, otherwise false.</returns>
        Task<bool> InsertManyAsync<T>(IEnumerable<T> items);

        /// <summary>
        /// Deletes records of a specific table. (DELETE FROM [TABLE-NAME] WHERE ....;)
        /// </summary>
        /// <param name="whereClauses">Where clauses for deleting records.</param>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>True, if deletion was successful, otherwise false.</returns>
        bool Delete<T>(IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Deletes records of a specific table in async way. (DELETE FROM [TABLE-NAME] WHERE ....;)
        /// </summary>
        /// <param name="whereClauses">Where clauses for deleting records.</param>
        /// <typeparam name="T">Type of the table. This must contain the <see cref="TableAttribute"/>.</typeparam>
        /// <returns>True, if deletion was successful, otherwise false.</returns>
        Task<bool> DeleteAsync<T>(IReadOnlyList<WhereClause> whereClauses);

        /// <summary>
        /// Creates an index on given column.
        /// </summary>
        /// <returns>True, if the operation was successful.</returns>
        bool CreateIndex<T>(string name, string columnName);
    }
}
