using Creobe.VoiceMemos.Core.Data;
using System;
using System.Data.Linq.Mapping;

namespace Creobe.VoiceMemos.Models.Legacy
{
    [Table(Name = "Tags")]
    public class Tag : EntityBase, IEntity
    {
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set 
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        

        #region IEntity Members

        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity")]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private DateTime? _createdDate;

        [Column]
        public DateTime? CreatedDate
        {
            get { return _createdDate; }
            set
            {
                _createdDate = value;
                NotifyPropertyChanged("CreatedDate");
            }
        }

        private DateTime? _modifiedDate;

        [Column]
        public DateTime? ModifiedDate
        {
            get { return _modifiedDate; }
            set
            {
                _modifiedDate = value;
                NotifyPropertyChanged("ModifiedDate");
            }
        }

        #endregion
    }
}
