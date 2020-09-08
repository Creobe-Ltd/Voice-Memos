using Creobe.SoundCloud;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class ShareViewModel : ViewModelBase
    {
        #region Commands

        public ICommand AccountSelectedCommand { get; private set; }
        public ICommand MessagingShareCommand { get; private set; }
        public ICommand EmailShareCommand { get; private set; }
        public ICommand SocialShareCommand { get; private set; }
        public ICommand UploadCommand { get; private set; }

        #endregion

        #region Private Members

        private Memo _memo;
        private CancellationTokenSource _cts;
        private Progress<SoundCloudUploadProgressChangedEventArgs> _scProgress;
        private Progress<LiveOperationProgress> _sdProgress;

        #endregion

        #region Properties

        private string _account;

        public string Account
        {
            get { return _account; }
            set
            {
                _account = value;
                NotifyPropertyChanged("Account");
            }
        }

        private bool _isUploading;

        public bool IsUploading
        {
            get { return _isUploading; }
            set
            {
                _isUploading = value;
                NotifyPropertyChanged("IsUploading");
            }
        }

        private double _percentCompleted;

        public double PercentCompleted
        {
            get { return _percentCompleted; }
            set
            {
                _percentCompleted = value;
                NotifyPropertyChanged("PercentCompleted");
            }
        }

        private IEnumerable<string> _accountList;

        public IEnumerable<string> AccountList
        {
            get { return _accountList; }
            set
            {
                _accountList = value;
                NotifyPropertyChanged("AccountList");
            }
        }

        #endregion

        #region Constructors

        public ShareViewModel()
        {
            InitializeCommands();
            PopulateSelectionLists();
        }

        #endregion

        #region Overrides

        public override void OnNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Id"))
            {
                int Id = int.Parse(NavigationContext.QueryString["Id"]);
                _memo = Uow.MemoRepository.Find(Id);
            }
        }

        public override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested && _isUploading)
            {
                e.Cancel = true;

                if (ShowMessage(AppResources.UploadCancelMessageText, AppResources.UploadCancelMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    _cts.Cancel();
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            AccountSelectedCommand = new DelegateCommand(AccountSelected);
            MessagingShareCommand = new DelegateCommand(MessagingShare);
            EmailShareCommand = new DelegateCommand(EmailShare);
            SocialShareCommand = new DelegateCommand(SocialShare);
            UploadCommand = new DelegateCommand(Upload);
        }

        private void PopulateSelectionLists()
        {
            AccountList = new string[] { AppResources.SkyDriveNameText, AppResources.SoundCloudNameText };
            _account = GetLocalizedAccountName(App.SettingsHelper.PreferredAccount);
        }

        private async Task<string> UploadToSkyDriveAsync(bool getLink)
        {
            _cts = new CancellationTokenSource();
            _cts.Token.Register(OnUploadCanceled);
            _sdProgress = new Progress<LiveOperationProgress>(OnSkyDriveUploadProgress);

            _cts.Token.ThrowIfCancellationRequested();

            var status = await SkyDriveHelper.GetSessionStatusAsync();

            if (status != LiveConnectSessionStatus.Connected)
                throw new InvalidOperationException("Not connected.");

            var fileId = await SkyDriveHelper.UploadFileAsync(_memo.AudioFile, GetUploadFileName(), _cts.Token, _sdProgress);

            if (getLink && !string.IsNullOrWhiteSpace(fileId))
            {
                var link = await SkyDriveHelper.GetLinkAsync(fileId, _cts.Token);
                return link;
            }

            return null;
        }

        private async Task<string> UploadToSoundCloudAsync()
        {
            _cts = new CancellationTokenSource();
            _cts.Token.Register(OnUploadCanceled);
            _scProgress = new Progress<SoundCloudUploadProgressChangedEventArgs>(OnSoundCloudUploadProgress);

            _cts.Token.ThrowIfCancellationRequested();

            var status = await SoundCloudHelper.GetSessionStatusAsync();

            if (status != SoundCloudSessionStatus.Connected)
                throw new InvalidOperationException("Not connected.");

            var link = await SoundCloudHelper.UploadFileAsync(_memo.Title, _memo.AudioFile, _memo.AudioFormat == "MP3" ? "audio/x-mpeg" : "audio/x-wav", _cts.Token, _scProgress);

            return link;
        }

        private void OnSkyDriveUploadProgress(LiveOperationProgress progress)
        {
            PercentCompleted = progress.ProgressPercentage;
        }

        private void OnSoundCloudUploadProgress(SoundCloudUploadProgressChangedEventArgs progress)
        {
            PercentCompleted = progress.ProgressPercentage;
        }

        private void OnUploadCanceled()
        {
            IsUploading = false;
        }

        private string GetUploadFileName()
        {
            return _memo.Title + (_memo.AudioFormat.Equals("MP3") ? ".mp3" : ".wav");
        }

        private string GetNeutralAccountName(string localizedName)
        {
            string neutralName = string.Empty;

            if (localizedName.Equals(AppResources.SkyDriveNameText))
                neutralName = "SkyDrive";

            if (localizedName.Equals(AppResources.SoundCloudNameText))
                neutralName = "SoundCloud";

            return neutralName;
        }

        private string GetLocalizedAccountName(string neutralName)
        {
            string localizedName = string.Empty;

            if (neutralName.Equals("SkyDrive"))
                localizedName = AppResources.SkyDriveNameText;

            if (neutralName.Equals("SoundCloud"))
                localizedName = AppResources.SoundCloudNameText;

            return localizedName;
        }

        #endregion

        #region Command Handlers

        public void AccountSelected(object arg)
        {
            var eventArgs = arg as SelectionChangedEventArgs;

            var account = eventArgs.AddedItems[0].ToString();

            App.SettingsHelper.PreferredAccount = GetNeutralAccountName(account);
            App.SettingsHelper.SaveSettings();
        }

        public async void MessagingShare(object arg)
        {
            try
            {
                IsUploading = true;
                string link = null;

                if (Account.Equals(AppResources.SkyDriveNameText))
                    link = await UploadToSkyDriveAsync(true);

                if (Account.Equals(AppResources.SoundCloudNameText))
                    link = await UploadToSoundCloudAsync();

                if (!string.IsNullOrWhiteSpace(link))
                    TaskHelper.ShareMessaging(link);

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();

            }
            catch (InvalidOperationException)
            {
                if (ShowMessage(string.Format(AppResources.AccountDisconnectedMessageText,
                    Account), AppResources.AccountDisconnectedMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    NavigationService.Navigate(ViewLocator.View("AccountSettings"));
                }

                IsUploading = false;
            }
            catch
            {
                IsUploading = false;
                ShowToast(AppResources.UploadFailedMessageCaption, string.Format(AppResources.UploadFailedMessageText, Account));
            }
        }

        public async void EmailShare(object arg)
        {
            try
            {
                IsUploading = true;
                string link = null;

                if (Account.Equals(AppResources.SkyDriveNameText))
                    link = await UploadToSkyDriveAsync(true);

                if (Account.Equals(AppResources.SoundCloudNameText))
                    link = await UploadToSoundCloudAsync();

                if (!string.IsNullOrWhiteSpace(link))
                    TaskHelper.ShareEmail(link);

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();

            }
            catch (InvalidOperationException)
            {
                if (ShowMessage(string.Format(AppResources.AccountDisconnectedMessageText,
                    Account), AppResources.AccountDisconnectedMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    NavigationService.Navigate(ViewLocator.View("AccountSettings"));
                }

                IsUploading = false;
            }
            catch
            {
                IsUploading = false;
                ShowToast(AppResources.UploadFailedMessageCaption, string.Format(AppResources.UploadFailedMessageText, Account));
            }
        }

        public async void SocialShare(object arg)
        {
            try
            {
                IsUploading = true;
                string link = null;

                if (Account.Equals(AppResources.SkyDriveNameText))
                    link = await UploadToSkyDriveAsync(true);

                if (Account.Equals(AppResources.SoundCloudNameText))
                    link = await UploadToSoundCloudAsync();

                if (!string.IsNullOrWhiteSpace(link))
                    TaskHelper.ShareSocial(link, _memo.Title);

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();

            }
            catch (InvalidOperationException)
            {
                if (ShowMessage(string.Format(AppResources.AccountDisconnectedMessageText,
                    Account), AppResources.AccountDisconnectedMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    NavigationService.Navigate(ViewLocator.View("AccountSettings"));
                }

                IsUploading = false;
            }
            catch
            {
                IsUploading = false;
                ShowToast(AppResources.UploadFailedMessageCaption, string.Format(AppResources.UploadFailedMessageText, Account));
            }
        }

        public async void Upload(object arg)
        {
            try
            {
                IsUploading = true;

                if (Account.Equals(AppResources.SkyDriveNameText))
                    await UploadToSkyDriveAsync(false);

                if (Account.Equals(AppResources.SoundCloudNameText))
                    await UploadToSoundCloudAsync();

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();

            }
            catch (InvalidOperationException)
            {
                if (ShowMessage(string.Format(AppResources.AccountDisconnectedMessageText,
                    Account), AppResources.AccountDisconnectedMessageCaption,
                    System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.OK)
                {
                    NavigationService.Navigate(ViewLocator.View("AccountSettings"));
                }

                IsUploading = false;
            }
            catch
            {
                IsUploading = false;
                ShowToast(AppResources.UploadFailedMessageCaption, string.Format(AppResources.UploadFailedMessageText, Account));
            }
        }

        #endregion

    }
}
