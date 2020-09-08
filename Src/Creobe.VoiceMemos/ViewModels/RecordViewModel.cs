using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Core.Media;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Media;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.Shell;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;

namespace Creobe.VoiceMemos.ViewModels
{
    public class RecordViewModel : ViewModelBase
    {
        #region Private Members

        private string _fileName;
        private IRecorder _recorder;
        private bool _isRecording;
        private Geoposition _location;
        private bool _isCanceling;

        #endregion

        #region Commands

        public ICommand RecordCommand { get; private set; }
        public ICommand PauseResumeCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand SkipSilenceCheckedCommand { get; private set; }
        public ICommand SkipSilenceUncheckedCommand { get; private set; }


        #endregion

        #region Properties

        private string _statusText;

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                NotifyPropertyChanged("StatusText");
            }
        }

        private string _pauseResumeButtonText;

        public string PauseResumeButtonText
        {
            get { return _pauseResumeButtonText; }
            set
            {
                _pauseResumeButtonText = value;
                NotifyPropertyChanged("PauseResumeButtonText");
            }
        }

        private string _durationText;

        public string DurationText
        {
            get { return _durationText; }
            set
            {
                _durationText = value;
                NotifyPropertyChanged("DurationText");
            }
        }

        private int _microphoneLevel;

        public int MicrophoneLevel
        {
            get { return _microphoneLevel; }
            set
            {
                _microphoneLevel = value;
                NotifyPropertyChanged("MicrophoneLevel");
            }
        }


        private string _locationText;

        public string LocationText
        {
            get { return _locationText; }
            set
            {
                _locationText = value;
                NotifyPropertyChanged("LocationText");
            }
        }

        private bool _isLocationEnabled;

        public bool IsLocationEnabled
        {
            get { return _isLocationEnabled; }
            set
            {
                _isLocationEnabled = value;
                NotifyPropertyChanged("IsLocationEnabled");
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

        #endregion

        #region Constructors

        public RecordViewModel()
        {
            SkipSilence = App.SettingsHelper.SkipSilence;

            InitializeCommands();

            PhoneApplicationService.Current.Activated += OnAppActivated;
            PhoneApplicationService.Current.Deactivated += OnAppDeactivated;
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {

            RecordCommand = new DelegateCommand(Record);
            PauseResumeCommand = new DelegateCommand(PauseResume);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            SkipSilenceCheckedCommand = new DelegateCommand(SkipSilenceChecked);
            SkipSilenceUncheckedCommand = new DelegateCommand(SkipSilenceUnchecked);
        }

        private async Task InitializeRecorder()
        {
            var sampleRate = ParseSampleRate(App.SettingsHelper.Quality);
            var channels = ParseChannels(App.SettingsHelper.Channels);
            var encodingFormat = (EncodingFormat)Enum.Parse(typeof(EncodingFormat), App.SettingsHelper.EncodingFormat);
            var encodingPreset = App.SettingsHelper.EncodingQuality;

            _fileName = Guid.NewGuid().ToString();

            if (encodingFormat == EncodingFormat.Wave)
                _fileName += ".wav";

            if (encodingFormat == EncodingFormat.MP3)
                _fileName += ".mp3";

            var stream = await StorageHelper.CreateFileForWriteAsync(_fileName);

            _recorder = new WasapiRecorder(stream.AsStream(), sampleRate, 16, channels, encodingFormat, encodingPreset);
            _recorder.SkipSilence = SkipSilence;
            _recorder.SampleReceived += OnSampleReceived;
        }

        private void ReleaseRecorder()
        {
            if (_recorder != null && _recorder.State != RecorderState.Stopped && _recorder.State != RecorderState.Unknown)
                _recorder.Stop();

            _recorder.SampleReceived -= OnSampleReceived;
            _recorder = null;
        }

        private void OnAppDeactivated(object sender, DeactivatedEventArgs e)
        {
            if (_isRecording)
            {
                if (_recorder != null && _recorder.IsCapturing)
                {
                    _recorder.PauseCapture();
                }
            }
        }

        private void OnAppActivated(object sender, ActivatedEventArgs e)
        {
            if (_isRecording)
            {
                if (_recorder != null && !_recorder.IsCapturing)
                {
                    _recorder.ResumeCapture();
                }
            }
        }

        private void OnSampleReceived(object sender, SampleReceivedEventArgs e)
        {
            if (_recorder != null && _recorder.IsCapturing && !App.RunningInBackground)
            {
                DurationText = TimeSpan.FromSeconds(e.Duration).ToString();
                MicrophoneLevel = (int)(e.Level * 100);
            }
        }

        private int ParseSampleRate(string value)
        {
            int sampleRate = 16000;

            switch (value)
            {
                case "High":
                    sampleRate = 48000;
                    break;
                case "Medium":
                    sampleRate = 44100;
                    break;
                case "Low":
                    sampleRate = 16000;
                    break;
            }

            return sampleRate;
        }

        private int ParseChannels(string value)
        {
            int channels = 1;

            switch (value)
            {
                case "Mono":
                    channels = 1;
                    break;
                case "Stereo":
                    channels = 2;
                    break;
            }

            return channels;
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
                Record(null);
        }

        public override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (_isRecording)
            {
                e.Cancel = true;

                BeginInvoke(() =>
                {
                    if (ShowMessage(AppResources.CancelMessageText, AppResources.CancelMessageCaption,
                        System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                    {
                        _isCanceling = true;
                        Cancel(null);
                    }
                });
            }
        }

        public override void OnObscured(object sender, Microsoft.Phone.Controls.ObscuredEventArgs e)
        {
            if (!e.IsLocked && _isRecording)
            {
                if (_recorder != null && _recorder.IsCapturing)
                {
                    _recorder.PauseCapture();
                }
            }

        }

        public override void OnUnobscured(object sender, EventArgs e)
        {
            if (_isRecording)
            {
                if (_recorder != null && !_recorder.IsCapturing)
                {
                    _recorder.ResumeCapture();
                }
            }

        }

        public override async void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_isRecording && Reset &&
                e.Uri.OriginalString.Contains("Action"))
            {
                ReleaseRecorder();
                _isRecording = false;

                await StorageHelper.DeleteFileAsync(_fileName);

                StatusText = AppResources.ReadyStatusText;
            }

            base.OnNavigating(sender, e);
        }

        #endregion

        #region Command Handlers

        public async void Record(object arg)
        {
            await InitializeRecorder();

            if (App.SettingsHelper.RecordInBackground)
                LocationHelper.StartTracking();

            _recorder.Start();
            _isRecording = true;


            StatusText = AppResources.RecordingStatusText;
            PauseResumeButtonText = AppResources.PauseButtonText;


            if (App.SettingsHelper.AddLocation)
            {
                IsLocationEnabled = true;

                LocationText = AppResources.LocationFindingText;
                _location = await LocationHelper.GetLocationAsync();

                if (_location != null)
                {
                    LocationText = AppResources.AddressFindingText;
                    var address = await LocationHelper.GetAddressAsync(_location);

                    LocationText = !string.IsNullOrWhiteSpace(address) ? address : AppResources.AddressUnidentifiedText;
                }
                else
                {
                    LocationText = AppResources.LocationNotFoundText;
                }
            }
        }

        public void PauseResume(object arg)
        {
            if (_recorder.State == RecorderState.Recording)
            {
                _recorder.Pause();
                StatusText = AppResources.PausedStatusText;
                PauseResumeButtonText = AppResources.ResumeButtonText;
            }
            else
            {
                _recorder.Resume();
                StatusText = AppResources.RecordingStatusText;
                PauseResumeButtonText = AppResources.PauseButtonText;
            }
        }

        public void Save(object arg)
        {
            long timestamp = (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;

            var newMemo = new Memo
            {
                AudioFile = _fileName,
                Title = string.Format(AppResources.MemoNameTemplate, timestamp.ToString()),
                Duration = _recorder.Duration,
                SampleRate = _recorder.SampleRate,
                Channels = _recorder.Channels,
                BitRate = _recorder.BitsRate,
                AudioFormat = _recorder.EncodingFormat.ToString(),
                Latitude = _location != null ? _location.Coordinate.Latitude : new Nullable<double>(),
                Longitude = _location != null ? _location.Coordinate.Longitude : new Nullable<double>()
            };


            LocationHelper.StopTracking();
            _isRecording = false;
            ReleaseRecorder();

            Uow.MemoRepository.Add(newMemo);
            //Uow.Save();

            StatusText = AppResources.ReadyStatusText;

            //if (newMemo.AudioFormat.Equals("MP3"))
            //    StorageHelper.WriteId3Tag(newMemo.AudioFile, null, newMemo.Title, "Voice Memos", "Voice Memos", null);

            if (App.SettingsHelper.EditDetails)
            {
                NavigationService.Navigate(ViewLocator.View("Edit", new { Id = newMemo.Id, SourceView = "Record" }));
            }
            else
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }

        }

        public async void Cancel(object arg)
        {
            if (_isCanceling || !App.SettingsHelper.ConfirmCancel || ShowMessage(AppResources.ConfirmCancelMessageText,
                AppResources.ConfirmCancelMessageCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (App.SettingsHelper.RecordInBackground)
                    LocationHelper.StopTracking();

                ReleaseRecorder();
                _isRecording = false;

                await StorageHelper.DeleteFileAsync(_fileName);

                StatusText = AppResources.ReadyStatusText;

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        public void SkipSilenceChecked(object arg)
        {
            if (_recorder != null)
                _recorder.SkipSilence = true;
        }

        public void SkipSilenceUnchecked(object arg)
        {
            if (_recorder != null)
                _recorder.SkipSilence = false;
        }

        #endregion
    }
}
