//Copyright by Sandjar Berdiev

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Berdiev.Storage.Factory
{
    internal class SqLiteFactory
    {
        public void CreateDatabase(string path)
        {
            SQLiteConnection.CreateFile("databaseThingie.db");
        }
    }
}
