using System;
using System.IO;
using Berdiev.Storage.Factory;
using NUnit.Framework;

namespace Berdiev.Storage.Tests
{
    public class Tests
    {
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            _filePath = Guid.NewGuid().ToString("N") + ".db3";
        }

        [Test]
        public void Can_CreateDatabase()
        {
            Assert.DoesNotThrow(() => DatabaseFactory.CreateDatabase(DatabaseType.SQLite, _filePath));
        }

        [Test]
        public void Can_Not_CreateDatabase_Duplicate()
        {
            Assert.DoesNotThrow(() => DatabaseFactory.CreateDatabase(DatabaseType.SQLite, _filePath));
            Assert.Throws<InvalidOperationException>(() =>
                DatabaseFactory.CreateDatabase(DatabaseType.SQLite, _filePath));
        }

        [Test]
        public void Can_Not_CreateDatabase_WrongFilePath()
        {
            Assert.Throws<ArgumentException>(() =>
                DatabaseFactory.CreateDatabase(DatabaseType.SQLite, "Foo"));

            Assert.Throws<ArgumentException>(() =>
                DatabaseFactory.CreateDatabase(DatabaseType.SQLite, null));

            Assert.Throws<ArgumentException>(() =>
                DatabaseFactory.CreateDatabase(DatabaseType.SQLite, string.Empty));
        }

        [Test]
        public void Can_Not_CreateDatabase_ReadOnly()
        {
            var dir = Directory.CreateDirectory("Huhu");
            dir.Attributes = FileAttributes.ReadOnly;

            Assert.Throws<InvalidOperationException>(() =>
                DatabaseFactory.CreateDatabase(DatabaseType.SQLite, Path.Combine(dir.FullName, _filePath)));

            dir.Attributes = FileAttributes.Normal;

            Directory.Delete(dir.FullName, true);
        }

        [Test]
        public void CanNotSupportOtherTypes()
        {
            Assert.Throws<NotSupportedException>(() => DatabaseFactory.CreateDatabase(DatabaseType.Postgres, _filePath));
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(_filePath);
        }
    }
}