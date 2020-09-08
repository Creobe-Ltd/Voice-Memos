using Creobe.VoiceMemos.Core.Data;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Creobe.VoiceMemos.Models.Legacy
{
    [Table(Name = "Memos")]
    public class Memo : EntityBase, IEntity
    {
        private string _title;

        [Column]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string _description;

        [Column]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        private int _duration;

        [Column]
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        private bool _isPlayed;

        [Column]
        public bool IsPlayed
        {
            get { return _isPlayed; }
            set
            {
                _isPlayed = value;
                NotifyPropertyChanged("IsPlayed");
            }
        }

        private double? _longitude;

        [Column]
        public double? Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                NotifyPropertyChanged("Longitude");
            }
        }

        private double? _latitude;

        [Column]
        public double? Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                NotifyPropertyChanged("Latitude");
            }
        }

        private string _audioFile;

        [Column]
        public string AudioFile
        {
            get { return _audioFile; }
            set
            {
                _audioFile = value;
                NotifyPropertyChanged("AudioFile");
            }
        }

        private string _audioFormat;

        [Column]
        public string AudioFormat
        {
            get { return _audioFormat; }
            set
            {
                _audioFormat = value;
                NotifyPropertyChanged("AudioFormat");
            }
        }

        private int? _sampleRate;

        [Column]
        public int? SampleRate
        {
            get { return _sampleRate; }
            set
            {
                _sampleRate = value;
                NotifyPropertyChanged("SampleRate");
            }
        }

        private int? _bitRate;

        [Column]
        public int? BitRate
        {
            get { return _bitRate; }
            set
            {
                _bitRate = value;
                NotifyPropertyChanged("BitRate");
            }
        }

        private int? _channels;

        [Column]
        public int? Channels
        {
            get { return _channels; }
            set
            {
                _channels = value;
                NotifyPropertyChanged("Channels");
            }
        }

        private string _imageFile;

        [Column]
        public string ImageFile
        {
            get { return _imageFile; }
            set
            {
                _imageFile = value;
                NotifyPropertyChanged("ImageFile");
            }
        }

        private EntitySet<MemoTag> _tags;

        [Association(Storage = "_tags", OtherKey = "_memoId", ThisKey = "Id", IsForeignKey = false)]
        public EntitySet<MemoTag> Tags
        {
            get { return _tags; }
            set
            {
                _tags.Assign(value);
                NotifyPropertyChanged("Tags");
            }
        }

        private EntitySet<Marker> _markers;

        [Association(Storage = "_markers", OtherKey = "_memoId", ThisKey = "Id", IsForeignKey = false)]
        public EntitySet<Marker> Markers
        {
            get { return _markers; }
            set
            {
                _markers.Assign(value);
                NotifyPropertyChanged("Markers");
            }
        }

        public Memo()
        {
            _tags = new EntitySet<MemoTag>(
                new Action<MemoTag>(attach_tag),
                new Action<MemoTag>(detach_tag));

            _markers = new EntitySet<Marker>(
                new Action<Marker>(attach_marker),
                new Action<Marker>(detach_marker));
        }

        private void attach_tag(MemoTag tag)
        {
            tag.Memo = this;
        }

        private void detach_tag(MemoTag tag)
        {
            tag.Memo = null;
        }

        private void attach_marker(Marker marker)
        {
            marker.Memo = this;
        }

        private void detach_marker(Marker marker)
        {
            marker.Memo = null;
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
