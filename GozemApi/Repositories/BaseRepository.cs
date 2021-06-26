using System.Collections.Generic;
using System.Threading.Tasks;

using GozemApi.Models;

using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace GozemApi.Repositories {
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IBaseEntity
    {
        protected readonly IAsyncDocumentSession Session;

        protected BaseRepository(IAsyncDocumentSession session) => Session = session;

        public async Task<ICollection<TEntity>> GetAll() => await Session.Query<TEntity>().ToListAsync();

        public async Task<TEntity> GetOne(string key) => await Session.LoadAsync<TEntity>(key);

        public async Task AddOrUpdate(TEntity entity, string key = null) => await Session.StoreAsync(entity, key);

        public async Task Delete(string key) => await Task.Run(() => Session.Delete(key));

        public async Task Delete(TEntity entity) => await Task.Run(() => Session.Delete(entity));
    }
}