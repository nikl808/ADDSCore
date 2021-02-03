using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADDSCore.DataProviders
{
    interface IRepository<T>:IDisposable
    {
        Task<IEnumerable<T>> GetEntitiesListAsync();
        T GetEntity(int id);
        void Create(T entity);
        void Update(int id, T entity);
        void Delete(int id);
        void Save();
    }
}
