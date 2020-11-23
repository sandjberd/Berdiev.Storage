//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Factory
{
    public class ForeignKeyReference
    {
        public ForeignKeyReference(string referenceTableName, string referenceColumnName)
        {
            ReferenceTableName = referenceTableName;
            ReferenceColumnName = referenceColumnName;
        }

        public String ReferenceTableName { get; }

        public String ReferenceColumnName { get; }
    }
}
