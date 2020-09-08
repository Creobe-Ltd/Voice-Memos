using Creobe.VoiceMemos.Controls;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Resources;
using System;
using System.Security.Cryptography;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class PasswordViewModel : ViewModelBase
    {
        #region Commands

        public ICommand CancelCommand { get; private set; }
        public ICommand DoneCommand { get; private set; }

        #endregion

        #region Private Members

        private BindableApplicationBarIconButton _doneButton;
        private BindableApplicationBarIconButton _cancelButton;

        private string _mode;

        #endregion

        #region Properties

        private string _currentPassword;

        public string CurrentPassword
        {
            get { return _currentPassword; }
            set
            {
                _currentPassword = value;
                NotifyPropertyChanged("CurrentPassword");
            }
        }

        private string _newPassword;

        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                _newPassword = value;
                NotifyPropertyChanged("NewPassword");
            }
        }

        private string _confirmPassword;

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                NotifyPropertyChanged("ConfirmPassword");
            }
        }

        private string _viewTitle;

        public string ViewTitle
        {
            get { return _viewTitle; }
            set
            {
                _viewTitle = value;
                NotifyPropertyChanged("ViewTitle");
            }
        }

        private bool _allowCurrentPassword;

        public bool AllowCurrentPassword
        {
            get { return _allowCurrentPassword; }
            set
            {
                _allowCurrentPassword = value;
                NotifyPropertyChanged("AllowCurrentPassword");
            }
        }


        private bool _allowNewPassword;

        public bool AllowNewPassword
        {
            get { return _allowNewPassword; }
            set
            {
                _allowNewPassword = value;
                NotifyPropertyChanged("AllowNewPassword");
            }
        }

        private bool _allowConfirmPassword;

        public bool AllowConfirmPassword
        {
            get { return _allowConfirmPassword; }
            set
            {
                _allowConfirmPassword = value;
                NotifyPropertyChanged("AllowConfirmPassword");
            }
        }


        #endregion

        #region Constructors

        public PasswordViewModel()
        {
            InitializeCommands();
            InitializeAppBar();

            _mode = "New";
            _allowCurrentPassword = false;
            _allowNewPassword = true;
            _allowConfirmPassword = true;
            _viewTitle = AppResources.SetPasswordViewTitle;
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Mode"))
            {
                _mode = NavigationContext.QueryString["Mode"];

                switch (_mode)
                {
                    case "Off":
                        AllowCurrentPassword = true;
                        AllowNewPassword = false;
                        AllowConfirmPassword = false;
                        ViewTitle = AppResources.RemovePasswordViewTitle;
                        break;
                    case "Change":
                        AllowCurrentPassword = true;
                        ViewTitle = AppResources.ChangePasswordViewTitle;
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            CancelCommand = new DelegateCommand(Cancel);
            DoneCommand = new DelegateCommand(Done);
        }

        private void InitializeAppBar()
        {
            _doneButton = new BindableApplicationBarIconButton();
            _cancelButton = new BindableApplicationBarIconButton();

            _doneButton.IconUri = new Uri("/Assets/AppBar/check.png", UriKind.RelativeOrAbsolute);
            _cancelButton.IconUri = new Uri("/Assets/AppBar/cancel.png", UriKind.RelativeOrAbsolute);

            _doneButton.Text = AppResources.DoneButtonText;
            _cancelButton.Text = AppResources.CancelButtonText;

            _doneButton.Command = DoneCommand;
            _cancelButton.Command = CancelCommand;

            AddAppBarItem(_doneButton);
            AddAppBarItem(_cancelButton);
        }

        private string ComputeHash(string value)
        {
            var shar256 = new SHA256Managed();
            return Convert.ToBase64String(shar256.ComputeHash(new System.Text.UTF8Encoding().GetBytes(value)));
        }

        #endregion

        #region Command Handlers

        public void Cancel(object arg)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        public void Done(object arg)
        {
            if (!_mode.Equals("New") && (CurrentPassword == null || !ComputeHash(CurrentPassword).Equals(App.SettingsHelper.PasswordHash)))
            {
                ShowMessage(AppResources.PasswordIncorrectMessageText, AppResources.PasswordIncorrectMessageCaption, System.Windows.MessageBoxButton.OK);
                return;
            }

            if (!_mode.Equals("Off") && (NewPassword == null || NewPassword.Length < 4))
            {
                ShowMessage(AppResources.PasswordShortMessageText, AppResources.PasswordShortMessageCaption, System.Windows.MessageBoxButton.OK);
                return;
            }

            if (!_mode.Equals("Off") && NewPassword != null && (ConfirmPassword == null || !NewPassword.Equals(ConfirmPassword)))
            {
                ShowMessage(AppResources.PasswordNoMatchMessageText, AppResources.PasswordNoMatchMessageCaption, System.Windows.MessageBoxButton.OK);
                return;
            }

            if (_mode.Equals("Off"))
            {
                App.SettingsHelper.RequirePassword = false;
                App.SettingsHelper.PasswordHash = null;
                App.SettingsHelper.PasswordLength = 0;
            }
            else
            {
                App.SettingsHelper.RequirePassword = true;
                App.SettingsHelper.PasswordHash = ComputeHash(NewPassword);
                App.SettingsHelper.PasswordLength = NewPassword.Length;
            }

            App.SettingsHelper.SaveSettings();
            App.IsAuthenticated = true;

            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        #endregion

    }
}
