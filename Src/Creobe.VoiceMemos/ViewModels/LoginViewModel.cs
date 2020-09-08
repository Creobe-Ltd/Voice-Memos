using Creobe.VoiceMemos.Core.Commands;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Commands

        public ICommand TestCommand { get; private set; }
        public ICommand KeyPressCommand { get; private set; }

        #endregion

        #region Private Members

        private Uri _returnUri;
        private string _enteredPassword = string.Empty;
        private string _passwordCharacter = "\uE236";

        #endregion

        #region Properties

        public bool ShowWaterMark
        {
            get { return string.IsNullOrWhiteSpace(_enteredPassword); }
        }

        public string MaskedPassword
        {
            get { return Regex.Replace(_enteredPassword, @".", _passwordCharacter); }
        }

        #endregion

        #region Constructors

        public LoginViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Url"))
                _returnUri = new Uri(Uri.UnescapeDataString(NavigationContext.QueryString["Url"]), UriKind.RelativeOrAbsolute);
            else
                _returnUri = ViewLocator.View("Main");
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            TestCommand = new DelegateCommand(Test);
            KeyPressCommand = new DelegateCommand(KeyPress);
        }

        private string ComputeHash(string value)
        {
            var shar256 = new SHA256Managed();
            return Convert.ToBase64String(shar256.ComputeHash(new System.Text.UTF8Encoding().GetBytes(value)));
        }

        #endregion

        #region Command Handlers

        public void Test(object arg)
        {
            App.IsAuthenticated = true;

            NavigationService.Navigate(_returnUri);
            NavigationService.RemoveBackEntry();
        }

        public void KeyPress(object arg)
        {
            var key = arg as string;

            if (arg != null)
            {
                if (!key.Equals("Back"))
                {
                    _enteredPassword += key;
                }
                else
                {
                    if (_enteredPassword.Length > 0)
                        _enteredPassword = _enteredPassword.Substring(0, _enteredPassword.Length - 1);
                }

                NotifyPropertyChanged("MaskedPassword");
                NotifyPropertyChanged("ShowWaterMark");

                if (_enteredPassword.Length == App.SettingsHelper.PasswordLength)
                {
                    if (ComputeHash(_enteredPassword).Equals(App.SettingsHelper.PasswordHash))
                    {
                        App.IsAuthenticated = true;

                        NavigationService.Navigate(_returnUri);
                        NavigationService.RemoveBackEntry();
                    }
                    else
                    {
                        _enteredPassword = string.Empty;
                        NotifyPropertyChanged("MaskedPassword");
                        NotifyPropertyChanged("ShowWaterMark");
                    }
                }
            }
        }

        #endregion

    }
}
