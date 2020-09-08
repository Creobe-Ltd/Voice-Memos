using Creobe.VoiceMemos.Core.Data;
using System;
using System.Data.Linq.Mapping;

namespace Creobe.VoiceMemos.Data.Models
{
    public partial class Tag : EntityBase, IEntity
    {
        private string _name;

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
