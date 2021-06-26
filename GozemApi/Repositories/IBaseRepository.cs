using System.Collections.Generic;
using System.Threading.Tasks;

using GozemApi.Models;

namespace GozemApi.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<ICollection<TEntity>> GetAll();
        Task<TEntity> GetOne(string key);
        Task AddOrUpdate(TEntity entity, string key = null);
        Task Delete(string key);
        Task Delete(TEntity entity);
    }
}