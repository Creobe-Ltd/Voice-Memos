using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Core.Media;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class RecordingSettingsViewModel : ViewModelBase
    {
        #region Commands

        public ICommand QualitySelectedCommand { get; private set; }
        public ICommand ChannelsSelectedCommand { get; private set; }
        public ICommand FormatSelectedCommand { get; private set; }
        public ICommand BitRateSelectedCommand { get; private set; }
        public ICommand SkipSilenceChangedCommand { get; private set; }
        public ICommand RecordUnderLockChangedCommand { get; private set; }
        public ICommand RecordInBackgroundChangedCommand { get; private set; }
        public ICommand AddLocationChangedCommand { get; private set; }
        public ICommand EditDetailsChangedCommand { get; private set; }

        #endregion

        #region Properties

        private string _quality;

        public string Quality
        {
            get { return _quality; }
            set
            {
                _quality = value;
                NotifyPropertyChanged("Quality");
            }
        }

        private string _channels;

        public string Channels
        {
            get { return _channels; }
            set
            {
                _channels = value;
                NotifyPropertyChanged("Channels");
            }
        }

        private string _format;

        public string Format
        {
            get { return _format; }
            set
            {
                _format = value;
                NotifyPropertyChanged("Format");
            }
        }

        private string _bitRate;

        public string BitRate
        {
            get { return _bitRate; }
            set
            {
                _bitRate = value;
                NotifyPropertyChanged("BitRate");
            }
        }

        private IEnumerable<string> _qualityList;

        public IEnumerable<string> QualityList
        {
            get { return _qualityList; }
            set
            {
                _qualityList = value;
                NotifyPropertyChanged("QualityList");
            }
        }

        private IEnumerable<string> _channelList;

        public IEnumerable<string> ChannelList
        {
            get { return _channelList; }
            set
            {
                _channelList = value;
                NotifyPropertyChanged("ChannelList");
            }
        }

        private IEnumerable<string> _formatList;

        public IEnumerable<string> FormatList
        {
            get { return _formatList; }
            set
            {
                _formatList = value;
                NotifyPropertyChanged("FormatList");
            }
        }

        private IEnumerable<string> _bitRateList;

        public IEnumerable<string> BitRateList
        {
            get { return _bitRateList; }
            set
            {
                _bitRateList = value;
                NotifyPropertyChanged("BitRateList");
            }
        }


        private bool _skipSilence;

        public bool SkipSilence
        {
            get { return _skipSilence; }
            set
            {
                _skipSilence = value;
                NotifyPropertyChanged("SkipSilence");
            }
        }

        private bool _recordUnderLock;

        public bool RecordUnderLock
        {
            get { return _recordUnderLock; }
            set
            {
                _recordUnderLock = value;
                NotifyPropertyChanged("RecordUnderLock");
            }
        }

        private bool _recordInBackground;

        public bool RecordInBackground
        {
            get { return _recordInBackground; }
            set
            {
                _recordInBackground = value;
                NotifyPropertyChanged("RecordInBackground");
            }
        }


        private bool _addLocation;

        public bool AddLocation
        {
            get { return _addLocation; }
            set
            {
                _addLocation = value;
                NotifyPropertyChanged("AddLocation");
            }
        }

        private bool _showBitRate;

        public bool ShowBitRate
        {
            get { return _showBitRate; }
            set
            {
                _showBitRate = value;
                NotifyPropertyChanged("ShowBitRate");
            }
        }

        private bool _editDetails;

        public bool EditDetails
        {
            get { return _editDetails; }
            set
            {
                _editDetails = value;
                NotifyPropertyChanged("EditDetails");
            }
        }


        #endregion

        #region Constructors

        public RecordingSettingsViewModel()
        {
            InitializeCommands();
            PopulateSelectionLists();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Quality = GetNameFromQuality((RecordingQuality)Enum.Parse(typeof(RecordingQuality),
                App.SettingsHelper.Quality));

            Channels = GetNameFromChannels((RecordingChannels)Enum.Parse(typeof(RecordingChannels),
                App.SettingsHelper.Channels));

            Format = App.SettingsHelper.EncodingFormat;

            BitRate = string.Format("{0}kbps", App.SettingsHelper.EncodingQuality);

            ShowBitRate = Format.Equals("MP3") ? true : false;

            SkipSilence = App.SettingsHelper.SkipSilence;
            RecordUnderLock = App.SettingsHelper.RecordUnderLock;
            RecordInBackground = App.SettingsHelper.RecordInBackground;
            AddLocation = App.SettingsHelper.AddLocation;
            EditDetails = App.SettingsHelper.EditDetails;
        }

        public override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.SettingsHelper.SaveSettings();
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            QualitySelectedCommand = new DelegateCommand(QualitySelected);
            ChannelsSelectedCommand = new DelegateCommand(ChannelsSelected);
            FormatSelectedCommand = new DelegateCommand(FormatSelected);
            BitRateSelectedCommand = new DelegateCommand(BitRateSelected);
            SkipSilenceChangedCommand = new DelegateCommand(SkipSilenceChanged);
            RecordUnderLockChangedCommand = new DelegateCommand(RecordUnderLockChanged);
            RecordInBackgroundChangedCommand = new DelegateCommand(RecordInBackgroundChanged);
            AddLocationChangedCommand = new DelegateCommand(AddLocationChanged);
            EditDetailsChangedCommand = new DelegateCommand(EditDetailsChanged);
        }

        private void PopulateSelectionLists()
        {
            QualityList = Enum.GetValues(typeof(RecordingQuality))
                .Cast<RecordingQuality>()
                .Select(q => GetNameFromQuality(q));

            ChannelList = Enum.GetValues(typeof(RecordingChannels))
                .Cast<RecordingChannels>()
                .Select(c => GetNameFromChannels(c));

            FormatList = Enum.GetNames(typeof(EncodingFormat));

            BitRateList = new string[] { "64kbps", "128kbps", "320kbps" };
        }

        private RecordingQuality GetQualityFromName(string name)
        {
            if (name.Equals(AppResources.QualityLowText))
                return RecordingQuality.Low;

            if (name.Equals(AppResources.QualityMediumText))
                return RecordingQuality.Medium;

            if (name.Equals(AppResources.QualityHighText))
                return RecordingQuality.High;

            throw new ArgumentOutOfRangeException("name parameter did not match any known qualities.");
        }

        private string GetNameFromQuality(RecordingQuality quality)
        {
            switch (quality)
            {
                case RecordingQuality.Low:
                    return AppResources.QualityLowText;
                case RecordingQuality.Medium:
                    return AppResources.QualityMediumText;
                case RecordingQuality.High:
                    return AppResources.QualityHighText;
                default:
                    return null;
            }
        }

        private RecordingChannels GetChannelsFromName(string name)
        {
            if (name.Equals(AppResources.ChannelsMonoText))
                return RecordingChannels.Mono;

            if (name.Equals(AppResources.ChannelsStereoText))
                return RecordingChannels.Stereo;

            throw new ArgumentOutOfRangeException("name parameter did not match any known channels.");
        }

        private string GetNameFromChannels(RecordingChannels quality)
        {
            switch (quality)
            {
                case RecordingChannels.Mono:
                    return AppResources.ChannelsMonoText;
                case RecordingChannels.Stereo:
                    return AppResources.ChannelsStereoText;
                default:
                    return null;
            }
        }

        private void UpgradeToPremium()
        {
            BeginInvoke(async () =>
            {
                if (ShowMessage(AppResources.FeatureNotAvailableMessageText, AppResources.FeatureNotAvailableMessageCaption,
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (await LicenseHelper.UpgradeToPremiumAsync())
                    {
                        ShowMessage(AppResources.UpgradeSuccessMessageText, AppResources.UpgradeSuccessMessageCaption,
                            MessageBoxButton.OK);
                    }
                }
            });
        }

        #endregion

        #region Command Handlers

        public void QualitySelected(object arg)
        {
            var eventArgs = arg as SelectionChangedEventArgs;

            var quality = GetQualityFromName(eventArgs.AddedItems[0].ToString());

            if (!LicenseHelper.IsPremium && quality != RecordingQuality.Low)
            {
                Quality = GetNameFromQuality(RecordingQuality.Low);
                UpgradeToPremium();
            }

            App.SettingsHelper.Quality = quality.ToString();
        }

        public void ChannelsSelected(object arg)
        {
            var eventArgs = arg as SelectionChangedEventArgs;

            var channels = GetChannelsFromName(eventArgs.AddedItems[0].ToString());

            if (!LicenseHelper.IsPremium && channels != RecordingChannels.Mono)
            {
                Channels = GetNameFromChannels(RecordingChannels.Mono);
                UpgradeToPremium();
            }

            App.SettingsHelper.Channels = channels.ToString();
        }

        public void FormatSelected(object arg)
        {
            var eventArgs = arg as SelectionChangedEventArgs;

            var format = eventArgs.AddedItems[0].ToString();

            if (!LicenseHelper.IsPremium && format.Equals("MP3"))
            {
                Format = format = EncodingFormat.Wave.ToString();
                UpgradeToPremium();
            }

            ShowBitRate = format.Equals("MP3") ? true : false;
            App.SettingsHelper.EncodingFormat = format;
        }

        public void BitRateSelected(object arg)
        {
            var eventArgs = arg as SelectionChangedEventArgs;

            if (eventArgs.AddedItems.Count > 0)
            {
                var bitRate = eventArgs.AddedItems[0].ToString();

                switch (bitRate)
                {
                    case "64kbps":
                        App.SettingsHelper.EncodingQuality = 64;
                        break;
                    case "128kbps":
                        App.SettingsHelper.EncodingQuality = 128;
                        break;
                    case "320kbps":
                        App.SettingsHelper.EncodingQuality = 320;
                        break;
                    default:
                        App.SettingsHelper.EncodingQuality = 128;
                        break;
                }
            }
        }

        public void SkipSilenceChanged(object arg)
        {
            App.SettingsHelper.SkipSilence = (arg != null && arg is bool) ? (bool)arg : false;
        }

        public void RecordUnderLockChanged(object arg)
        {
            App.SettingsHelper.RecordUnderLock = (arg != null && arg is bool) ? (bool)arg : false;
        }

        public void RecordInBackgroundChanged(object arg)
        {
            var recordInBackground = (arg != null && arg is bool) ? (bool)arg : false;

            if (!LicenseHelper.IsPremium && recordInBackground)
            {
                RecordInBackground = false;
                UpgradeToPremium();
            }

            App.SettingsHelper.RecordInBackground = recordInBackground;
        }

        public void AddLocationChanged(object arg)
        {
            App.SettingsHelper.AddLocation = (arg != null && arg is bool) ? (bool)arg : false;
        }

        public void EditDetailsChanged(object arg)
        {
            App.SettingsHelper.EditDetails = (arg != null && arg is bool) ? (bool)arg : false;
        }

        #endregion

    }
}
