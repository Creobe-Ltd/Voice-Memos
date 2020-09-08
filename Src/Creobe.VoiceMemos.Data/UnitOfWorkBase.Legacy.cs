using Creobe.VoiceMemos.Core.Legacy.Data;
using System;
using System.Data.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Legacy
{
    public class UnitOfWorkBase<TContext> : IUnitOfWork where TContext: DataContext, new()
    {
        protected TContext DbContext;

        public UnitOfWorkBase(string fileOrConnection)
        {
            DbContext = CreateContext(fileOrConnection);
        }

        public UnitOfWorkBase(string fileOrConnection, System.Data.Linq.Mapping.MappingSource mapping)
        {
            DbContext = CreateContext(fileOrConnection, mapping);
        }

        protected TContext CreateContext(string fileOrConnection)
        {
            return Activator.CreateInstance(typeof(TContext), fileOrConnection) as TContext;
        }

        protected TContext CreateContext(string fileOrConnection, System.Data.Linq.Mapping.MappingSource mapping)
        {
            return Activator.CreateInstance(typeof(TContext), fileOrConnection, mapping) as TContext;
        }

        public virtual Task LoadCollectionsAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteDatabase()
        {
            DbContext.DeleteDatabase();
        }

        public virtual void Save()
        {
            DbContext.SubmitChanges();
        }
    }
}
