//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Berdiev.Storage.Factory;
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

            var id = Column.From<int>("Id", true, false, true);
            var name = Column.Default<String>("Name");
            var birthday = Column.Default<DateTime>("Buruthday");
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

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Buruthday = DateTime.UtcNow;

            var res = connection.Insert(p1);
            Assert.IsTrue(res);
        }

        [Test]
        public async Task CanInsertAsync()
        {
            var connection = ConnectionFactory.CreateSqLite(_path);

            var p1 = new Person();
            p1.Name = "Hugo";
            p1.Buruthday = DateTime.UtcNow;

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

            [Column("Buruthday")]
            public DateTime Buruthday { get; set; }

            [Column("Data")]
            public String Data { get; set; }

            [Column("DoubleValue")]
            public double DoubleValue { get; set; }

            public int Count { get; set; }
        }
    }
}