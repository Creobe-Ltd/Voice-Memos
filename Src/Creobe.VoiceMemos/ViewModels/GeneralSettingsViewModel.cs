using Creobe.VoiceMemos.Core.Commands;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class GeneralSettingsViewModel : ViewModelBase
    {
        #region Commands

        public ICommand ConfirmCancelChangedCommand { get; private set; }
        public ICommand ConfirmDeleteChangedCommand { get; private set; }
        public ICommand ShowUnplayedCountChangedCommand { get; private set; }
        public ICommand UseSystemPlayerChangedCommand { get; private set; }

        #endregion

        #region Properties        

        private bool _confirmCancel;

        public bool ConfirmCancel
        {
            get { return _confirmCancel; }
            set 
            { 
                _confirmCancel = value;
                NotifyPropertyChanged("ConfirmCancel");
            }
        }
        
        private bool _confirmDelete;

        public bool ConfirmDelete
        {
            get { return _confirmDelete; }
            set 
            {
                _confirmDelete = value;
                NotifyPropertyChanged("ConfirmDelete");
            }
        }

        private bool _showUnplayedCount;

        public bool ShowUnplayedCount
        {
            get { return _showUnplayedCount; }
            set 
            { 
                _showUnplayedCount = value;
                NotifyPropertyChanged("ShowUnplayedCount");
            }
        }

        private bool _useSystemPlayer;

        public bool UseSystemPlayer
        {
            get { return _useSystemPlayer; }
            set 
            { 
                _useSystemPlayer = value;
                NotifyPropertyChanged("UseSystemPlayer");
            }
        }        

        #endregion

        #region Constructors

        public GeneralSettingsViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ConfirmCancel = App.SettingsHelper.ConfirmCancel;
            ConfirmDelete = App.SettingsHelper.ConfirmDelete;
            ShowUnplayedCount = App.SettingsHelper.ShowUnplayedCount;
            UseSystemPlayer = App.SettingsHelper.UseSystemPlayer;
        }

        public override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            App.SettingsHelper.SaveSettings();
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            ConfirmCancelChangedCommand = new DelegateCommand(ConfirmCancelChanged);
            ConfirmDeleteChangedCommand = new DelegateCommand(ConfirmDeleteChanged);
            ShowUnplayedCountChangedCommand = new DelegateCommand(ShowUnplayedCountChanged);
            UseSystemPlayerChangedCommand = new DelegateCommand(UseSystemPlayerChanged);
        }


        #endregion

        #region Command Handlers

        public void ConfirmCancelChanged(object arg)
        {
            App.SettingsHelper.ConfirmCancel = (arg != null && arg is bool) ? (bool)arg : false;
        }

        public void ConfirmDeleteChanged(object arg)
        {
            App.SettingsHelper.ConfirmDelete = (arg != null && arg is bool) ? (bool)arg : false;
        }

        public void ShowUnplayedCountChanged(object arg)
        {
            App.SettingsHelper.ShowUnplayedCount = (arg != null && arg is bool) ? (bool)arg : false;
        }

        public void UseSystemPlayerChanged(object arg)
        {
            App.SettingsHelper.UseSystemPlayer = (arg != null && arg is bool) ? (bool)arg : false;
        }

        #endregion

    }
}
