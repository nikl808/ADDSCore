using System;
using System.ComponentModel;

namespace ADDSCore.DataProviders
{
    interface IRepository<T>:IDisposable
    {
        BindingList<T> GetEntitiesList();
        T GetEntity(int id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
