//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.IO;
using Berdiev.Storage.Factory;
using NUnit.Framework;

namespace Berdiev.Storage.Tests
{
    [TestFixture]
    public class TableFactory_Test
    {
        private string _path;

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory("Huhu");
            _path = Path.Combine("Huhu", Guid.NewGuid().ToString("N") + ".db3");

            DatabaseFactory.CreateDatabase(DatabaseType.SQLite, _path);
        }

        [TearDown]
        public void TearDown()
        {
            var directoryInfo = new DirectoryInfo(_path);
            directoryInfo.Parent.Delete(true);
        }

        [Test]
        public void CanCreateTable()
        {
            var id = Column.From<int>("Id", ColumnConstraint.PrimaryKey());
            var name = Column.Default<string>("Name");
            var birthDate = Column.Default<DateTime>("Börsday");
            var money = Column.Default<double>("Moneyyyyy");
            var complex = Column.From<ComplexThing>("Foool", ColumnConstraint.Default().AsNotNull().AsUnique());

            var table = new Table("TestTable", new List<Column>
            {
                id, name, birthDate, money, complex
            });

            var res = TableFactory.CreateTable(DatabaseType.SQLite, _path, table);

            Assert.IsTrue(res);
        }

        [Test]
        public void CanCreateTable_ForeignKeys()
        {
            var id = Column.From<int>("Id", ColumnConstraint.PrimaryKey());
            var name = Column.FromForeignKey<string>("Name", ColumnConstraint.Default(), new ForeignKeyReference("NamId", "Names"));
            var birthDate = Column.Default<DateTime>("Börsday");
            var money = Column.FromForeignKey<double>("Moneyyyyy", ColumnConstraint.Default(), new ForeignKeyReference("MoneyId", "Moneys"));
            var complex = Column.From<ComplexThing>("Foool", ColumnConstraint.Default().AsNotNull().AsUnique());

            var table = new Table("TestTable", new List<Column>
            {
                id, name, birthDate, money, complex
            });

            var res = TableFactory.CreateTable(DatabaseType.SQLite, _path, table);

            Assert.IsTrue(res);
        }

        private class ComplexThing
        {
            [Column("Thing")]
            public String Thing { get; set; }

            [Column("Id")]
            public int Id { get; set; }
        }
    }
}