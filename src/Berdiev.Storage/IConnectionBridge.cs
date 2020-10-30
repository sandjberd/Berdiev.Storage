//Copyright by Sandjar Berdiev

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

        bool Insert<T>(T item);

        Task<bool> InsertAsync<T>(T item);

        bool InsertMany<T>(IEnumerable<T> items);

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
    }
}
