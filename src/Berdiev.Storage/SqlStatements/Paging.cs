//Copyright by Sandjar Berdiev

namespace Berdiev.Storage.SqlStatements
{
    public class Paging
    {
        public Paging(int limit, int offset)
        {
            Limit = limit;
            Offset = offset;
        }

        public int Limit { get; }

        public int Offset { get; }
    }
}
