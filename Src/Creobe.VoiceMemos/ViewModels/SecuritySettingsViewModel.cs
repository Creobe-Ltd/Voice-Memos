using Creobe.VoiceMemos.Core.Commands;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class SecuritySettingsViewModel : ViewModelBase
    {
        #region Commands

        public ICommand RequirePasswordCheckedCommand { get; private set; }
        public ICommand RequirePasswordUncheckedCommand { get; private set; }
        public ICommand ChangePasswordCommand { get; private set; }

        #endregion

        #region Private Members

        private bool _isNewInstance;

        #endregion

        #region Properties

        private bool _requirePassword;

        public bool RequirePassword
        {
            get { return _requirePassword; }
            set
            {
                _requirePassword = value;
                NotifyPropertyChanged("RequirePassword");
            }
        }

        private bool _isPasswordEnabled;

        public bool IsPasswordEnabled
        {
            get { return _isPasswordEnabled; }
            set
            {
                _isPasswordEnabled = value;
                NotifyPropertyChanged("IsPasswordEnabled");
            }
        }


        #endregion

        #region Constructors

        public SecuritySettingsViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _isNewInstance = e.NavigationMode == System.Windows.Navigation.NavigationMode.New ? true : false;
            RequirePassword = App.SettingsHelper.RequirePassword;
            IsPasswordEnabled = App.SettingsHelper.RequirePassword;
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            RequirePasswordCheckedCommand = new DelegateCommand(RequirePasswordChecked);
            RequirePasswordUncheckedCommand = new DelegateCommand(RequirePasswordUnchecked);
            ChangePasswordCommand = new DelegateCommand(ChangePassword);
        }

        #endregion

        #region Command Handlers

        public void RequirePasswordChecked(object arg)
        {
            if (!App.SettingsHelper.RequirePassword)
                NavigationService.Navigate(ViewLocator.View("Password"));
        }

        public void RequirePasswordUnchecked(object arg)
        {
            if (App.SettingsHelper.RequirePassword)
                NavigationService.Navigate(ViewLocator.View("Password", new { Mode = "Off" }));
        }

        public void ChangePassword(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Password", new { Mode = "Change" }));
        }

        #endregion

    }
}
