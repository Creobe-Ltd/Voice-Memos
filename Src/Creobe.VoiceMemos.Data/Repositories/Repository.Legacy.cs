using Creobe.VoiceMemos.Core.Data;
using Creobe.VoiceMemos.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Legacy.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        internal DataContext dbContext;

        internal ObservableCollection<T> _all;

        public Repository(DataContext context)
        {
            dbContext = context;
        }

        #region IRepository Members

        public virtual ObservableCollection<T> All
        {
            get
            {
                return _all;
            }
        }

        public virtual int Count
        {
            get
            {
                return dbContext.GetTable<T>().Count();
            }

        }

        public virtual T Find(int id)
        {
            return dbContext.GetTable<T>()
                .Cast<T>()
                .Where(e => e.Id == id)
                    .FirstOrDefault();
        }

        public virtual void Add(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;

            dbContext.GetTable<T>()
                .InsertOnSubmit(entity);

            _all.AddToCollection(entity);
        }

        public virtual void Update(T entity)
        {
            entity.ModifiedDate = DateTime.Now;
        }

        public virtual void Delete(T entity)
        {
            dbContext.GetTable<T>().DeleteOnSubmit(entity);

            _all.RemoveFromCollection(entity);
        }

        public virtual void Delete(int id)
        {
            Delete(Find(id));
        }

        public virtual void DeleteMany(IEnumerable<T> collection)
        {
            foreach (T entity in collection)
            {
                Delete(entity);
            }
        }

        public virtual async Task LoadCollectionsAsync()
        {
            await Task.Run(() =>
            {
                _all = new ObservableCollection<T>(dbContext.GetTable<T>());
            });
        }

        public virtual void Save()
        {
            dbContext.SubmitChanges();
        }

        #endregion
    }
}
