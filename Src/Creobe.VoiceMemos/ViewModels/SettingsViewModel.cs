using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Resources;
using System.Windows;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Commands

        public ICommand RecordingCommand { get; private set; }
        public ICommand AccountsCommand { get; private set; }
        public ICommand SecurityCommand { get; private set; }
        public ICommand StorageCommand { get; private set; }
        public ICommand GeneralCommand { get; private set; }

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Overrides

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            RecordingCommand = new DelegateCommand(Recording);
            AccountsCommand = new DelegateCommand(Accounts);
            SecurityCommand = new DelegateCommand(Security);
            StorageCommand = new DelegateCommand(Storage);
            GeneralCommand = new DelegateCommand(General);
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

        public void Recording(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("RecordingSettings"));
        }

        public void Accounts(object arg)
        {
            if (LicenseHelper.IsPremium)
                NavigationService.Navigate(ViewLocator.View("AccountSettings"));
            else
                UpgradeToPremium();
        }

        public void Security(object arg)
        {
            if (LicenseHelper.IsPremium)
                NavigationService.Navigate(ViewLocator.View("SecuritySettings"));
            else
                UpgradeToPremium();
        }

        public void Storage(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("StorageSettings"));
        }

        public void General(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("GeneralSettings"));
        }

        #endregion
    }
}
