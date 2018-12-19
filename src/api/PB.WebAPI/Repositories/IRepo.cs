using System.Collections.Generic;
using System.Threading.Tasks;
using PB.WebAPI.Models;

namespace PB.WebAPI.Repositories
{
    public interface IRepo<TModel>
        where TModel : IDbModel, new()
    {
        Task CreateAsync(TModel model);
        Task<TModel> ReadAsync(long id);
        Task<IList<TModel>> ReadListAsync();
        Task UpdateAsync(TModel model);
        Task DeleteAsync(long model);
    }
}