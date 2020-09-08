using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Creobe.VoiceMemos.TileTemplates
{
    public partial class MemoMediumWithImage : UserControl, INotifyPropertyChanged
    {

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

        private Brush _backgrundBrush;

        public Brush BackgroundBrush
        {
            get { return _backgrundBrush; }
            set 
            { 
                _backgrundBrush = value;
                NotifyPropertyChanged("BackgroundBrush");
            }
        }
        
        
        public MemoMediumWithImage()
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
