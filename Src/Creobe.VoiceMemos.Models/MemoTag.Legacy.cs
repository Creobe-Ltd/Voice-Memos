using Creobe.VoiceMemos.Core.Data;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Creobe.VoiceMemos.Models.Legacy
{
    [Table(Name="MemoTags")]
    public class MemoTag : EntityBase, IEntity
    {
        [Column]
        private int _memoId;

        private EntityRef<Memo> _memo;

        [Association(Storage = "_memo", ThisKey = "_memoId", OtherKey = "Id", IsForeignKey = true, DeleteOnNull=true)]
        public Memo Memo
        {
            get { return _memo.Entity; }
            set
            {
                _memo.Entity = value;

                if (value != null)
                {
                    _memoId = value.Id;
                }

                NotifyPropertyChanged("Memo");
            }
        }

        [Column]
        private int _tagId;

        private EntityRef<Tag> _tag;

        [Association(Storage = "_tag", ThisKey = "_tagId", OtherKey = "Id", IsForeignKey = true, DeleteOnNull=true)]
        public Tag Tag
        {
            get { return _tag.Entity; }
            set
            {
                _tag.Entity = value;

                if (value != null)
                {
                    _tagId = value.Id;
                }

                NotifyPropertyChanged("Tag");
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
