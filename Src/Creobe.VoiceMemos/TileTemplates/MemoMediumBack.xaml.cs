using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Creobe.VoiceMemos.TileTemplates
{
    public partial class MemoMediumBack : UserControl, INotifyPropertyChanged
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

        private DateTime _createdDate;

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set 
            { 
                _createdDate = value;
                NotifyPropertyChanged("CreatedDate");
            }
        }

        private TimeSpan _duration;

        public TimeSpan Duration
        {
            get { return _duration; }
            set 
            { 
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        private BitmapImage _backgroundImage;

        public BitmapImage BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                NotifyPropertyChanged("BackgroundImage");
            }
        }

        
        public MemoMediumBack()
        {
            InitializeComponent();

            DataContext = this;
        }

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
