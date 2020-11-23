//Copyright by Sandjar Berdiev

using System;

namespace Berdiev.Storage.Factory
{
    public class ColumnConstraint
    {
        internal ColumnConstraint(bool isPrimaryKey, bool isNotNull, bool isUnique, bool isAutoIncrement)
        {
            IsPrimaryKey = isPrimaryKey;
            IsNotNull = isNotNull;
            IsUnique = isUnique;
            IsAutoIncrement = isAutoIncrement;
        }

        /// <summary>
        /// Sets the column as primary key.
        /// </summary>
        public Boolean IsPrimaryKey { get; }

        /// <summary>
        /// Sets the column as not null constraint.
        /// </summary>
        public Boolean IsNotNull { get; }

        /// <summary>
        /// Sets the column as isUnique constraint.
        /// </summary>
        public Boolean IsUnique { get; }

        /// <summary>
        /// Auto increment for the column. Column must be an INTEGER.
        /// </summary>
        public Boolean IsAutoIncrement { get; }

        public static ColumnConstraint Default()
        {
            return new ColumnConstraint(false, false, false, false);
        }

        public static ColumnConstraint PrimaryKey()
        {
            return new ColumnConstraint(true, false, false, false);
        }

        public static ColumnConstraint PrimaryKeyNotNull()
        {
            return new ColumnConstraint(true, true, false, false);
        }

        public static ColumnConstraint PrimaryKeyUnique()
        {
            return new ColumnConstraint(true, false, true, false);
        }

        public static ColumnConstraint PrimaryKeyNotNullUnique()
        {
            return new ColumnConstraint(true, true, true, false);
        }

        public ColumnConstraint AsPrimaryKey()
        {
            return new ColumnConstraint(true, IsNotNull, IsUnique, IsAutoIncrement);
        }

        public ColumnConstraint AsUnique()
        {
            return new ColumnConstraint(IsPrimaryKey, IsNotNull, true, IsAutoIncrement);
        }

        public ColumnConstraint AsNotNull()
        {
            return new ColumnConstraint(IsPrimaryKey, true, IsUnique, IsAutoIncrement);
        }

        public ColumnConstraint AsAutoIncrement()
        {
            return new ColumnConstraint(IsPrimaryKey, IsNotNull, IsUnique, IsAutoIncrement);
        }
    }
}
