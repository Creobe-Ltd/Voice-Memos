using Creobe.VoiceMemos.Core.Data;
using Lex.Db;
using System;
using System.Data.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data
{
    public class UnitOfWorkBase : IUnitOfWork
    {
        //protected DbInstance instance;

        public UnitOfWorkBase() { }

        //public UnitOfWorkBase(string fileOrConnection, System.Data.Linq.Mapping.MappingSource mapping)
        //{
        //    DbContext = CreateContext(fileOrConnection, mapping);
        //}

        //protected TConnection CreateConnection(string connectionString)
        //{
        //    return Activator.CreateInstance(typeof(TConnection), connectionString) as TConnection;
        //}

        //protected TConnection CreateConnection(string fileOrConnection, System.Data.Linq.Mapping.MappingSource mapping)
        //{
        //    return Activator.CreateInstance(typeof(TContext), fileOrConnection, mapping) as TContext;
        //}

        public virtual void CreateDatabase()
        {
            throw new NotImplementedException();
        }

        public virtual Task LoadCollectionsAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteDatabase()
        {
            throw new NotImplementedException();
        }

        //public virtual void Save()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
