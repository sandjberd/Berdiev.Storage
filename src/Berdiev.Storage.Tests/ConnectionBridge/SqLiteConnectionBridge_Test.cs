//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Berdiev.Storage.Factory;
using Berdiev.Storage.SqlStatements;
using NUnit.Framework;

namespace Berdiev.Storage.Tests.ConnectionBridge
{
    [TestFixture]
    public class SqLiteConnectionBridge_Test
    {
        private string _path;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory("Huhu");
            _path = Path.Combine("Huhu", Guid.NewGuid().ToString("N") + ".db3");

            DatabaseFactory.CreateDatabase(DatabaseType.SQLite, _path);

            var columns = new List<Column>();

            var id = Column.From<int>("Id", ColumnConstraint.PrimaryKeyNotNull().AsAutoIncrement());
            var name = Column.Default<String>("Name");
            var birthday = Column.Default<DateTime>("Birthday");
            var data = Column.Default<String>("Data");
            var doubleValue = Column.Default<double>("DoubleValue");

            columns.Add(id);
            columns.Add(name);
            columns.Add(birthday);
            columns.Add(data);
            columns.Add(doubleValue);

            TableFactory.CreateTable(DatabaseType.SQLite, _path, new Table("Person", columns));
        }

        [TearDown]
        public void TearDown()
        {
            var directoryInfo = new DirectoryInfo(_path);
            directoryInfo.Parent.Delete(true);
        }

        [Test]
        public void CanInsert()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var p1 = new Person {Name = "Hugo", Buruthday = DateTime.UtcNow};

            var res = connection.Insert(p1);
            Assert.IsTrue(res);
        }

        [Test]
        public void CanCommitTransaction()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var p1 = new Person { Name = "Hugo", Buruthday = DateTime.UtcNow };

            connection.BeginTransaction();

            var res = connection.Insert(p1);

            connection.CommitTransaction();

            Assert.IsTrue(res);
        }

        [Test]
        public void CanRollbackTransaction()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var p1 = new Person { Name = "Hugo", Buruthday = DateTime.UtcNow };

            connection.BeginTransaction();

            var res = connection.Insert(p1);

            connection.RollbackTransaction();

            Assert.IsTrue(res);
        }

        [Test]
        public async Task CanInsertAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var p1 = new Person {Name = "Hugo", Buruthday = DateTime.UtcNow};

            var res = await connection.InsertAsync(p1);
            Assert.IsTrue(res);
        }

        [Test]
        public void CanInsertMany()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(100);

            var res = connection.InsertMany(persons);
            
            Assert.IsTrue(res);
        }

        [Test]
        public async Task CanInsertManyAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(100);

            var res = await connection.InsertManyAsync(persons).ConfigureAwait(false);

            Assert.IsTrue(res);
        }

        [Test]
        public void CanGetAll()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var returnedPersons = connection.GetAll<Person>().ToList();

            Assert.AreEqual(30, returnedPersons.Count);
            Assert.AreEqual("foo 29", returnedPersons.Last().Name);
            Assert.AreEqual(30, returnedPersons.Last().Id);
            Assert.NotNull(returnedPersons.Last().Buruthday);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public async Task CanGetAllAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var returnedPersons = await connection.GetAllAsync<Person>().ConfigureAwait(false);

            Assert.AreEqual(30, returnedPersons.ToList().Count);
            Assert.AreEqual("foo 29", returnedPersons.Last().Name);
            Assert.AreEqual(30, returnedPersons.Last().Id);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public void CanGetPaging()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var orderbyClause = new OrderByClause(new List<string> {
                "Name"
            });

            var returnedPersons = connection.Get<Person>(new Paging(3, 4), orderbyClause, new List<WhereClause>()).ToList();

            Assert.AreEqual(3, returnedPersons.Count);
            Assert.AreEqual("foo 5", returnedPersons.First().Name);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public async Task CanGetPagingAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var orderbyClause = new OrderByClause(new List<string> {
                "Name"
            });

            var returnedPersons =
                (await connection.GetAsync<Person>(new Paging(3, 4), orderbyClause, new List<WhereClause>())).ToList();

            Assert.AreEqual(3, returnedPersons.Count);
            Assert.AreEqual("foo 5", returnedPersons.First().Name);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public void CanGetCount()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            connection.InsertMany(persons);

            var count30 = connection.GetRowCount<Person>(new List<WhereClause>());

            var count25 = connection.GetRowCount<Person>(new List<WhereClause> {
                new WhereClause("Id", 5, SqlOperator.GreaterThan)
            });

            var count26 = connection.GetRowCount<Person>(new List<WhereClause> {
                new WhereClause("Id", 5, SqlOperator.GreaterEqual)
            });

            Assert.AreEqual(30, count30);
            Assert.AreEqual(25, count25);
            Assert.AreEqual(26, count26);
        }

        [Test]
        public void CanSelect()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var whereClause1 = new WhereClause("Name", "foo 15");
            var whereClause2 = new WhereClause("Id", 16);

            var selectedRows = connection.SelectRecords<Person>(new List<WhereClause>
            {
                whereClause1,
                whereClause2
            });

            Assert.AreEqual(1, selectedRows.ToList().Count);
            Assert.AreEqual("foo 15", selectedRows.First().Name);
            Assert.AreEqual(16, selectedRows.First().Id);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public async Task CanSelectAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var whereClause1 = new WhereClause("Name", "foo 15");
            var whereClause2 = new WhereClause("Id", 16);

            var selectedRows = await connection.SelectRecordsAsync<Person>(new List<WhereClause>
            {
                whereClause1,
                whereClause2
            }).ConfigureAwait(false);

            Assert.AreEqual(1, selectedRows.ToList().Count);
            Assert.AreEqual("foo 15", selectedRows.First().Name);
            Assert.AreEqual(16, selectedRows.First().Id);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public void CanUpdate()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var whereClause1 = new WhereClause("Name", "foo 15");
            var whereClause2 = new WhereClause("Id", 16);

            var columnToUpdate = new ColumnToUpdate("Name", "You crazy mother******");

            var updateRes = connection.Update<Person>(new List<ColumnToUpdate>
            {
                columnToUpdate
            }, new List<WhereClause>
            {
                whereClause1,
                whereClause2
            });

            var newWhereClause = new WhereClause("Name", "You crazy mother******");

            var selectedRows = connection.SelectRecords<Person>(new List<WhereClause>
            {
                newWhereClause
            });

            Assert.IsTrue(insertManyRes);
            Assert.IsTrue(updateRes);
            Assert.AreEqual(1, selectedRows.ToList().Count);
            Assert.AreEqual("You crazy mother******", selectedRows.First().Name);
            Assert.AreEqual(16, selectedRows.First().Id);
        }

        [Test]
        public async Task CanUpdateAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var whereClause1 = new WhereClause("Name", "foo 15");
            var whereClause2 = new WhereClause("Id", 16);

            var columnToUpdate = new ColumnToUpdate("Name", "You crazy mother******");

            var updateRes = await connection.UpdateAsync<Person>(new List<ColumnToUpdate>
            {
                columnToUpdate
            }, new List<WhereClause>
            {
                whereClause1,
                whereClause2
            }).ConfigureAwait(false);

            var newWhereClause = new WhereClause("Name", "You crazy mother******");

            var selectedRows = connection.SelectRecords<Person>(new List<WhereClause>
            {
                newWhereClause
            });

            Assert.IsTrue(insertManyRes);
            Assert.IsTrue(updateRes);
            Assert.AreEqual(1, selectedRows.ToList().Count);
            Assert.AreEqual("You crazy mother******", selectedRows.First().Name);
            Assert.AreEqual(16, selectedRows.First().Id);
            Assert.NotNull(selectedRows.First().Buruthday);
        }

        [Test]
        public void CanDelete()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var whereClause = new WhereClause("Id", 16);

            var deleteResult = connection.Delete<Person>(new List<WhereClause>
            {
                whereClause
            });

            var selectedRows = connection.SelectRecords<Person>(new List<WhereClause>
            {
                whereClause
            });

            Assert.AreEqual(0, selectedRows.ToList().Count);
            Assert.IsTrue(deleteResult);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public async Task CanDeleteAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var whereClause = new WhereClause("Id", 16);

            var deleteResult = await connection.DeleteAsync<Person>(new List<WhereClause>
            {
                whereClause
            });

            var selectedRows = connection.SelectRecords<Person>(new List<WhereClause>
            {
                whereClause
            });

            Assert.AreEqual(0, selectedRows.ToList().Count);
            Assert.IsTrue(deleteResult);
            Assert.IsTrue(insertManyRes);
        }

        [Test]
        public void CanDeleteAll()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var persons = _CreateManyPersons(30);

            var insertManyRes = connection.InsertMany(persons);

            var deleteResult = connection.Delete<Person>(new List<WhereClause>());

            var selectedRows = connection.GetAll<Person>();

            Assert.AreEqual(0, selectedRows.ToList().Count);
            Assert.IsTrue(deleteResult);
            Assert.IsTrue(insertManyRes);
        }

        private List<Person> _CreateManyPersons(int count)
        {
            var persons = new List<Person>();

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < 250; i++)
            {
                stringBuilder.Append('F');
            }

            for (var i = 0; i < count; i++)
            {
                persons.Add(new Person
                {
                    Buruthday = DateTime.UtcNow,
                    Name = "foo " + i,
                    DoubleValue = 13567.3334554545 + i,
                    Data = stringBuilder.ToString()
                });
            }

            return persons;
        }

        [Table("Person")]
        private class Person
        {
            [Column("Name")]
            public string Name { get; set; }

            [Column("Birthday")]
            public DateTime Buruthday { get; set; }

            [Column("Data")]
            public String Data { get; set; }

            [Column("DoubleValue")]
            public double DoubleValue { get; set; }

            public int Count { get; set; }

            public int Id { get; set; }
        }
    }
}