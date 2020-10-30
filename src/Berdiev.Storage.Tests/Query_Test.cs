//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Berdiev.Storage.Factory;
using Berdiev.Storage.SqlStatements;
using NUnit.Framework;

namespace Berdiev.Storage.Tests
{
    [TestFixture]
    public class Query_Test
    {
        private string _path;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory("Huhu");
            _path = Path.Combine("Huhu", Guid.NewGuid().ToString("N") + ".db3");

            DatabaseFactory.CreateDatabase(DatabaseType.SQLite, _path);

            var columns = new List<Column>();

            var id = Column.Default<int>("Id");
            var name = Column.Default<String>("Name");
            var birthday = Column.Default<DateTime>("Birthday");

            columns.Add(id);
            columns.Add(name);
            columns.Add(birthday);

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
            var connection = ConnectionFactory.CreateSqLiteConnection(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Birthday = DateTime.UtcNow;

            var res = connection.Insert<Person>(p1);
            Assert.IsTrue(res);
        }

        [Test]
        public void CanGet()
        {
            var connection = ConnectionFactory.CreateSqLiteConnection(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Birthday = DateTime.UtcNow;

            var res = connection.Insert<Person>(p1);

            var person = connection.Get<Person>();

            Assert.AreEqual(p1.Name, person.First().Name);
            Assert.IsTrue(res);
        }

        [Test]
        public void CanSelect()
        {
            var connection = ConnectionFactory.CreateSqLiteConnection(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Birthday = DateTime.UtcNow;

            var p2 = new Person();
            p2.Name = "Blubu";
            p2.Birthday = DateTime.UtcNow;

            var p3 = new Person();
            p3.Name = "Blubu_2";
            p3.Birthday = DateTime.UtcNow;

            var res1 = connection.Insert<Person>(p1);
            var res2 = connection.Insert<Person>(p2);
            var res3 = connection.Insert<Person>(p3);

            var persons = connection.Select<Person>((person) => person.Name.Equals("Blubu"));

            Assert.AreEqual(1, persons.Count());
            Assert.AreEqual("Blubu", persons.First().Name);

            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
        }

        [Test]
        public void CanSelect_FastWay()
        {
            var connection = ConnectionFactory.CreateSqLiteConnection(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Birthday = DateTime.UtcNow;

            var p2 = new Person();
            p2.Name = "Blubu";
            p2.Birthday = DateTime.UtcNow;

            var p3 = new Person();
            p3.Name = "Blubu_2";
            p3.Birthday = DateTime.UtcNow;

            var res1 = connection.Insert<Person>(p1);
            var res2 = connection.Insert<Person>(p2);
            var res3 = connection.Insert<Person>(p3);

            var whereClause = new WhereClause("Name", "Blubu");

            var persons = connection.Select<Person>(new List<WhereClause>
            {
                whereClause
            });

            Assert.AreEqual(1, persons.Count());
            Assert.AreEqual("Blubu", persons.First().Name);

            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
        }

        [Test]
        public void CanInsertMany()
        {
            var connection = ConnectionFactory.CreateSqLiteConnection(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Birthday = DateTime.UtcNow;

            var p2 = new Person();
            p2.Name = "HHHH";
            p2.Birthday = DateTime.UtcNow;

            var res = connection.InsertMany<Person>(new List<Person>{p1, p2});
            Assert.IsTrue(res);
        }

        [Test]
        public void CanUpate()
        {
            var connection = ConnectionFactory.CreateSqLiteConnection(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Birthday = DateTime.UtcNow;

            var p2 = new Person();
            p2.Name = "HHHH";
            p2.Birthday = DateTime.UtcNow;

            connection.InsertMany<Person>(new List<Person> { p1, p2 });

            var columnsToUpdate = new List<ColumnToUpdate>();

            columnsToUpdate.Add(new ColumnToUpdate("Name", "DummKopf"));

            var res = connection.Update<Person>(columnsToUpdate, new WhereClause("Name", "HHHH"));

            Assert.IsTrue(res);
        }

        [Table("Person")]
        private class Person
        {
            [Column("Name")]
            public string Name { get; set; }

            [Column("Birthday")]
            public DateTime Birthday { get; set; }

            public int Count { get; set; }
        }
    }
}