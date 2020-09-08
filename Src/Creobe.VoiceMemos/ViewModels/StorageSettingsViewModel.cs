using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Core.Extensions;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Resources;
using System.Windows;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class StorageSettingsViewModel : ViewModelBase
    {
        #region Commands

        public ICommand ResetDataCommand { get; private set; }
        public ICommand RecoverCommand { get; private set; }

        #endregion

        #region Properties

        private string _availableStorage;

        public string AvailableStorage
        {
            get { return _availableStorage; }
            set
            {
                _availableStorage = value;
                NotifyPropertyChanged("AvailableStorage");
            }
        }


        #endregion

        #region Constructors

        public StorageSettingsViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            AvailableStorage = string.Format(AppResources.AvailableStorageText, StorageHelper.GetAvailableFreeSpace().ToSizeString());
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            ResetDataCommand = new DelegateCommand(ResetData);
            RecoverCommand = new DelegateCommand(Recover);
        }

        #endregion

        #region Command Handlers

        public void ResetData(object arg)
        {
            if (ShowMessage(AppResources.ResetDataWarningMessageText, AppResources.ResetDataWarningMessageCaption,
                System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
            {
                StorageHelper.DeleteIsolatedStorage();
                Uow.DeleteDatabase();
                TileHelper.ClearPrimaryTileCount();
                TileHelper.UnPinSecondaryTiles();
                Application.Current.Terminate();
            }
        }

        public void Recover(object arg)
        {
#if BETA
            ShowMessage(AppResources.NotImplementedMessageText, AppResources.NotImplementedMessageCaption,
                System.Windows.MessageBoxButton.OK);
#else
            NavigationService.Navigate(ViewLocator.View("Recover"));
#endif
        }

        #endregion
    }
}
