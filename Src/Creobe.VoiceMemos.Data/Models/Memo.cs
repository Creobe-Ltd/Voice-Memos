using Creobe.VoiceMemos.Core.Data;
using System;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Xml.Serialization;

namespace Creobe.VoiceMemos.Data.Models
{
    public partial class Memo : EntityBase, IEntity
    {
        private string _title;

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

        public string ImageFile
        {
            get { return _imageFile; }
            set
            {
                _imageFile = value;
                NotifyPropertyChanged("ImageFile");
            }
        }


        private int[] _tagsFK;

        public int[] TagsFK
        {
            get { return _tagsFK; }
            set 
            {
                _tagsFK = value;
                NotifyPropertyChanged("TagsFK");
            }
        }

        private int[] _markersFK;

        public int[] MarkersFK
        {
            get { return _markersFK; }
            set 
            { 
                _markersFK = value;
                NotifyPropertyChanged("MarkersFK");
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
