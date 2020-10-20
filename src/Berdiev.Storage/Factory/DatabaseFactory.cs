//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Text;

namespace Berdiev.Storage.Factory
{
    public class DatabaseFactory
    {
        public void CreateDatabase(DatabaseType type, string path)
        {
            if (type == DatabaseType.SQLite)
            {
                var sqliteFactory = new SqLiteFactory();
                sqliteFactory.CreateDatabase("dd");
            }
        }
    }
}
