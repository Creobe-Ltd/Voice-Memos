using Creobe.SoundCloud;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Resources;
using Microsoft.Live;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class AccountSettingsViewModel : ViewModelBase
    {
        #region Commands

        public ICommand SkyDriveConnectCommand { get; private set; }
        public ICommand SoundCloudConnectCommand { get; private set; }

        #endregion

        #region Private Members

        LiveConnectSessionStatus _skyDriveSessionStatus;
        SoundCloudSessionStatus _soundCloudSessionStatus;

        #endregion

        #region Properties

        private string _skyDriveStatusText;

        public string SkyDriveStatusText
        {
            get { return _skyDriveStatusText; }
            set
            {
                _skyDriveStatusText = value;
                NotifyPropertyChanged("SkyDriveStatusText");
            }
        }

        private string _soundCloudStatusText;

        public string SoundCloudStatusText
        {
            get { return _soundCloudStatusText; }
            set
            {
                _soundCloudStatusText = value;
                NotifyPropertyChanged("SoundCloudStatusText");
            }
        }

        private bool _isSkyDriveButtonEnabled;

        public bool IsSkyDriveButtonEnabled
        {
            get { return _isSkyDriveButtonEnabled; }
            set
            {
                _isSkyDriveButtonEnabled = value;
                NotifyPropertyChanged("IsSkyDriveButtonEnabled");
            }
        }

        private bool _isSoundCloudButtonEnabled;

        public bool IsSoundCloudButtonEnabled
        {
            get { return _isSoundCloudButtonEnabled; }
            set
            {
                _isSoundCloudButtonEnabled = value;
                NotifyPropertyChanged("IsSoundCloudButtonEnabled");
            }
        }

        #endregion

        #region Constructors

        public AccountSettingsViewModel()
        {
            InitializeCommands();

            _skyDriveStatusText = AppResources.AccountStatusCheckingText;
            _soundCloudStatusText = AppResources.AccountStatusCheckingText;
            _isSkyDriveButtonEnabled = false;
            _isSoundCloudButtonEnabled = false;
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            GetSkyDriveSessionStatus();
            GetSoundCloudSessionStatus();
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            SkyDriveConnectCommand = new DelegateCommand(SkyDriveConnect);
            SoundCloudConnectCommand = new DelegateCommand(SoundCloudConnect);
        }

        private async void GetSkyDriveSessionStatus()
        {
            try
            {
                IsSkyDriveButtonEnabled = false;

                _skyDriveSessionStatus = await SkyDriveHelper.GetSessionStatusAsync();

                switch (_skyDriveSessionStatus)
                {
                    case LiveConnectSessionStatus.Connected:
                        SkyDriveStatusText = AppResources.AccountConnectedStatusText;
                        break;
                    case LiveConnectSessionStatus.NotConnected:
                        SkyDriveStatusText = AppResources.AccountDisconnectedStatusText;
                        break;
                    case LiveConnectSessionStatus.Unknown:
                        SkyDriveStatusText = AppResources.AccountUnknownStatusText;
                        break;
                }
            }
            catch(LiveAuthException)
            {
                SkyDriveStatusText = AppResources.AccountErrorStatusText;
            }
            finally
            {
                IsSkyDriveButtonEnabled = true;
            }
        }

        private async void SkyDriveLogin()
        {
            try
            {
                _skyDriveSessionStatus = await SkyDriveHelper.LoginAsync();

                switch (_skyDriveSessionStatus)
                {
                    case LiveConnectSessionStatus.Connected:
                        SkyDriveStatusText = AppResources.AccountConnectedStatusText;
                        break;
                    case LiveConnectSessionStatus.NotConnected:
                        SkyDriveStatusText = AppResources.AccountDisconnectedStatusText;
                        break;
                    case LiveConnectSessionStatus.Unknown:
                        SkyDriveStatusText = AppResources.AccountUnknownStatusText;
                        break;
                }
            }
            catch (LiveAuthException)
            {
                SkyDriveStatusText = AppResources.AccountErrorStatusText;
            }
        }

        private void SkyDriveLogout()
        {
            SkyDriveHelper.Logout();
            _skyDriveSessionStatus = LiveConnectSessionStatus.NotConnected;
            SkyDriveStatusText = AppResources.AccountDisconnectedStatusText;
        }

        private async void GetSoundCloudSessionStatus()
        {
            try
            {
                IsSoundCloudButtonEnabled = false;

                _soundCloudSessionStatus = await SoundCloudHelper.GetSessionStatusAsync();

                switch (_soundCloudSessionStatus)
                {
                    case SoundCloudSessionStatus.Connected:
                        SoundCloudStatusText = AppResources.AccountConnectedStatusText;
                        break;
                    case SoundCloudSessionStatus.NotConnected:
                        SoundCloudStatusText = AppResources.AccountDisconnectedStatusText;
                        break;
                    case SoundCloudSessionStatus.Unknown:
                        SoundCloudStatusText = AppResources.AccountUnknownStatusText;
                        break;
                }
            }
            catch
            {
                SoundCloudStatusText = AppResources.AccountErrorStatusText;
            }
            finally
            {
                IsSoundCloudButtonEnabled = true;
            }
        }

        private async void SoundCloudLogin()
        {
            try
            {
                _soundCloudSessionStatus = await SoundCloudHelper.LoginAsync();

                switch (_soundCloudSessionStatus)
                {
                    case SoundCloudSessionStatus.Connected:
                        SoundCloudStatusText = AppResources.AccountConnectedStatusText;
                        break;
                    case SoundCloudSessionStatus.NotConnected:
                        SoundCloudStatusText = AppResources.AccountDisconnectedStatusText;
                        break;
                    case SoundCloudSessionStatus.Unknown:
                        SoundCloudStatusText = AppResources.AccountUnknownStatusText;
                        break;
                }
            }
            catch
            {
                SkyDriveStatusText = AppResources.AccountErrorStatusText;
            }
        }

        private void SoundCloudLogout()
        {
            SoundCloudHelper.Logout();
            _soundCloudSessionStatus = SoundCloudSessionStatus.NotConnected;
            SoundCloudStatusText = AppResources.AccountDisconnectedStatusText;
        }

        #endregion

        #region Command Handlers

        public void SkyDriveConnect(object arg)
        {
            if (_skyDriveSessionStatus == LiveConnectSessionStatus.Connected)
            {
                if (ShowMessage(AppResources.AccountDisconnectConfirmMessageText, AppResources.AccountDisconnectConfirmMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    SkyDriveLogout();
                }
            }
            else
            {
                SkyDriveLogin();
            }
        }

        public void SoundCloudConnect(object arg)
        {
            if (_soundCloudSessionStatus == SoundCloudSessionStatus.Connected)
            {
                if (ShowMessage(AppResources.AccountDisconnectConfirmMessageText, AppResources.AccountDisconnectConfirmMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    SoundCloudLogout();
                }
            }
            else
            {
                SoundCloudLogin();
            }
        }

        #endregion

    }
}
