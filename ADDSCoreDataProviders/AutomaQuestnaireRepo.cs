using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using ADDSCore.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace ADDSCore.DataProviders
{
    public class AutomaQuestnaireRepo : IRepository<AutomaSysQuestnaire>
    {
        private DbConnection connection;
        private bool disposedValue;

        public AutomaQuestnaireRepo() => connection = new DbConnection();

        public void Create(AutomaSysQuestnaire entity) => connection.db.Add(entity);

        public void Delete(AutomaSysQuestnaire entity) => connection.db.Entry(entity).State = EntityState.Deleted;

        public BindingList<AutomaSysQuestnaire> GetEntitiesList() => new BindingList<AutomaSysQuestnaire>(connection.db.AutomaQuestnaire.ToList());

        public AutomaSysQuestnaire GetEntity(int id) => connection.db.AutomaQuestnaire.FirstOrDefault(t=>t.Id==id);

        public void Save() => connection.db.SaveChanges();

        public void Update(AutomaSysQuestnaire entity) => connection.db.Entry(entity).State = EntityState.Modified;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}