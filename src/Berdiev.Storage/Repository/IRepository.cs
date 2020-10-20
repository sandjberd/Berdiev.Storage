//Copyright by Sandjar Berdiev

using System.Threading.Tasks;

namespace Berdiev.Storage.Repository
{
    public interface IRepository<TItem, TId>
    {
        Task Insert(TItem item);

        Task Update(TItem item);

        Task Delete(TId id);

        Task<TItem> GetById(TId id);
    }
}
