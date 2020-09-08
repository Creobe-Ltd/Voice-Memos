using Creobe.VoiceMemos.Core.Data;
using Creobe.VoiceMemos.Core.Extensions;
using Lex.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Data.Repositories
{
    public class Repository<T> : INotifyPropertyChanged, IRepository<T> where T : class, IEntity, new ()
    {
        internal DbInstance instance;

        internal ObservableCollection<T> _all;

        public Repository(DbInstance dbInstance)
        {
            instance = dbInstance;
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
                //return dbConnection.Table<T>().Count<T>();
                return instance.Table<T>().Count();
            }

        }

        public virtual T Find(int id)
        {
            //return dbConnection.Table<T>()
            //    .Cast<T>()
            //    .Where(e => e.Id == id)
            //    .FirstOrDefault();

            return instance.Table<T>()
                .Cast<T>()
                .Where(e => e.Id == id)
                .FirstOrDefault();
        }

        public virtual void Add(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;

            //dbContext.GetTable<T>()
            //    .InsertOnSubmit(entity);

            //dbConnection.Insert(entity);
            instance.Table<T>().Save(entity);

            _all.AddToCollection(entity);
        }

        public virtual void Update(T entity)
        {
            entity.ModifiedDate = DateTime.Now;

            //dbConnection.Update(entity);
            instance.Table<T>().Save(entity);
        }

        public virtual void Delete(T entity)
        {
            //dbContext.GetTable<T>().DeleteOnSubmit(entity);

            //dbConnection.Delete(entity);
            instance.Table<T>().Delete(entity);

            _all.RemoveFromCollection(entity);
        }

        public virtual void Delete(int id)
        {
            Delete(Find(id));
        }

        public virtual void DeleteMany(IEnumerable<T> collection)
        {
            //foreach (T entity in collection)
            //{
            //    Delete(entity);
            //}
            instance.Table<T>().Delete(collection);
        }

        public virtual async Task LoadCollectionsAsync()
        {
            await Task.Run(() =>
            {
                //_all = new ObservableCollection<T>(dbContext.GetTable<T>());
                _all = new ObservableCollection<T>(instance.Table<T>().LoadAll());
            });
        }

        //public virtual void Save()
        //{
        //    //dbContext.SubmitChanges();
        //    instance.su
        //}

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
