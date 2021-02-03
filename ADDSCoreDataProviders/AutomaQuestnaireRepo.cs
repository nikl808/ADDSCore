using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void Delete(int id)
        {
            var find = connection.db.AutomaQuestnaire.Find(id);
            if (find != null) connection.db.AutomaQuestnaire.Remove(find);
        }

        public async Task<IEnumerable<AutomaSysQuestnaire>> GetEntitiesListAsync() => await connection.db.AutomaQuestnaire.ToListAsync();
        
        public AutomaSysQuestnaire GetEntity(int id) => connection.db.AutomaQuestnaire.Local.FirstOrDefault(t => t.Id == id);//return null, if id not found

        public void Save() => connection.db.SaveChanges();

        public void Update(int id, AutomaSysQuestnaire entity)
        {
            var list = connection.db.AutomaQuestnaire.Find(id);

            if (list != null)
            {
                list.ListName = entity.ListName;
                list.ObjName = entity.ObjName;
                list.ControlAnalog = entity.ControlAnalog;
                list.ControlStruct = entity.ControlStruct;
                list.Network = entity.Network;
                list.Software = entity.Software;
                list.Document = entity.Document;
                list.Extra = entity.Extra;
                list.Cabinet = entity.Cabinet;
                list.Parameter = entity.Parameter;
            }
        }

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