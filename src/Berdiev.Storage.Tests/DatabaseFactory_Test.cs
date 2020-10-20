using Berdiev.Storage.Factory;
using NUnit.Framework;

namespace Berdiev.Storage.Tests
{
    public class Tests
    {
        private DatabaseFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new DatabaseFactory();
        }

        [Test]
        public void Can_CreateDatabase()
        {
            _factory.CreateDatabase(DatabaseType.SQLite, "doo");
        }
    }
}