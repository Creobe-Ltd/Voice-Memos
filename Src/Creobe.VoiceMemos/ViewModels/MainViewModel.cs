using Creobe.VoiceMemos.Controls;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.BackgroundAudio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Creobe.VoiceMemos.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Private Members

        private BindableApplicationBarIconButton _searchButton;
        private BindableApplicationBarIconButton _mapButton;
        private BindableApplicationBarIconButton _selectButton;
        private BindableApplicationBarIconButton _deleteButton;
        private BindableApplicationBarIconButton _favoriteButton;

        private BindableApplicationBarMenuItem _pinRecordMenuItem;
        private BindableApplicationBarMenuItem _settingsMenuItem;
        private BindableApplicationBarMenuItem _upgradeMenuItem;
        private BindableApplicationBarMenuItem _aboutMenuItem;
        private BindableApplicationBarMenuItem _saveToMediaMenuItem;

#if BETA
        private BindableApplicationBarMenuItem _reportBugMenuItem;
#endif

#if (DEBUG && BETA) || DEBUG
        private BindableApplicationBarMenuItem _clearIapMenuItem;
#endif

        private int _askForRatingCount = 10;

        private List<Memo> _selectedItems;

        #endregion

        #region Commands

        public ICommand RecordCommand { get; private set; }
        public ICommand FavoritesCommand { get; private set; }
        public ICommand ListCommand { get; private set; }
        public ICommand ListMapCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand MemosEnableSelectionCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand SettingsCommand { get; private set; }
        public ICommand MemoTapCommand { get; private set; }
        public ICommand PinRecordCommad { get; private set; }
        public ICommand UpgradeCommand { get; private set; }
        public ICommand ReportBugCommand { get; private set; }
        public ICommand ClearIapCommand { get; private set; }
        public ICommand AddToFavoritesCommand { get; private set; }
        public ICommand SaveToMediaLibraryCommand { get; private set; }
        public ICommand MemoDeleteCommand { get; private set; }
        public ICommand MemosSelectionChangedCommand { get; private set; }

        #endregion

        #region Properties

        public ObservableCollection<Memo> Recent
        {
            get
            {
                return Uow.MemoRepository.Recent;
            }
        }

        private bool _isSelectionEnabled;

        public bool IsSelectionEnabled
        {
            get { return _isSelectionEnabled; }
            set
            {
                _isSelectionEnabled = value;
                NotifyPropertyChanged("IsSelectionEnabled");
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            InitializeCommands();
            InitializeAppBar();

            _selectedItems = new List<Memo>();
        }

        #endregion

        #region Overrides

        public override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (TileHelper.IsRecordPinnned())
                _pinRecordMenuItem.Text = AppResources.UnPinRecordMenuItemText;

            if (LicenseHelper.IsPremium)
                RemoveAppBarItem(_upgradeMenuItem);

            if (App.SettingsHelper.ShowUnplayedCount)
                TileHelper.SetPrimaryTileCount(Uow.MemoRepository.UnplayedCount);
            else
                TileHelper.SetPrimaryTileCount(0);


            if (e.NavigationMode == NavigationMode.New)
            {
                await MigrateDatabase();
                UpgradeToPremium();
#if !BETA
                AskForReview();
#endif
                HandleActions();
                HandleVoiceCommands();
            }

            UpdateVersion();
        }

        public override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (IsSelectionEnabled)
            {
                e.Cancel = true;
                IsSelectionEnabled = false;
                UpdateAppBar();
            }
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            RecordCommand = new DelegateCommand(Record);
            FavoritesCommand = new DelegateCommand(Favorites);
            ListCommand = new DelegateCommand(List);
            ListMapCommand = new DelegateCommand(ListMap);
            SearchCommand = new DelegateCommand(Search);
            AboutCommand = new DelegateCommand(About);
            SettingsCommand = new DelegateCommand(Settings);
            MemoTapCommand = new DelegateCommand(MemoTap);
            PinRecordCommad = new DelegateCommand(PinRecord);
            UpgradeCommand = new DelegateCommand(Upgrade);
            ReportBugCommand = new DelegateCommand(ReportBug);
            ClearIapCommand = new DelegateCommand(ClearIap);
            MemosEnableSelectionCommand = new DelegateCommand(MemosEnableSelection);
            MemosSelectionChangedCommand = new DelegateCommand(MemosSelectionChanged);
            AddToFavoritesCommand = new DelegateCommand(AddToFavorites);
            MemoDeleteCommand = new DelegateCommand(MemoDelete);
            SaveToMediaLibraryCommand = new DelegateCommand(SaveToMediaLibrary);
        }

        private void InitializeAppBar()
        {
            _aboutMenuItem = new BindableApplicationBarMenuItem();
            _aboutMenuItem.Command = AboutCommand;
            _aboutMenuItem.Text = AppResources.AboutMenuItemText;

            _mapButton = new BindableApplicationBarIconButton();
            _mapButton.Command = ListMapCommand;
            _mapButton.IconUri = new Uri("/Assets/AppBar/appbar.map.folds.png", UriKind.RelativeOrAbsolute);
            _mapButton.Text = AppResources.MapButtonText;

            _selectButton = new BindableApplicationBarIconButton();
            _selectButton.Command = MemosEnableSelectionCommand;
            _selectButton.IconUri = new Uri("/Assets/AppBar/appbar.list.check.png", UriKind.RelativeOrAbsolute);
            _selectButton.Text = AppResources.SelectButtonText;

            _favoriteButton = new BindableApplicationBarIconButton();
            _favoriteButton.Command = AddToFavoritesCommand;
            _favoriteButton.IconUri = new Uri("/Assets/AppBar/appbar.star.add.png", UriKind.RelativeOrAbsolute);
            _favoriteButton.Text = AppResources.AddToFavoriteButtonText;

            _deleteButton = new BindableApplicationBarIconButton();
            _deleteButton.Command = MemoDeleteCommand;
            _deleteButton.IconUri = new Uri("/Assets/AppBar/delete.png", UriKind.RelativeOrAbsolute);
            _deleteButton.Text = AppResources.DeleteButtonText;


            _pinRecordMenuItem = new BindableApplicationBarMenuItem();
            _pinRecordMenuItem.Command = PinRecordCommad;
            _pinRecordMenuItem.Text = AppResources.PinRecordMenuItemText;

            _settingsMenuItem = new BindableApplicationBarMenuItem();
            _settingsMenuItem.Command = SettingsCommand;
            _settingsMenuItem.Text = AppResources.SettingsMenuItemText;


            _searchButton = new BindableApplicationBarIconButton();
            _searchButton.Command = SearchCommand;
            _searchButton.IconUri = new Uri("/Assets/AppBar/feature.search.png", UriKind.RelativeOrAbsolute);
            _searchButton.Text = AppResources.SearchButtonText;

            _saveToMediaMenuItem = new BindableApplicationBarMenuItem();
            _saveToMediaMenuItem.Command = SaveToMediaLibraryCommand;
            _saveToMediaMenuItem.Text = AppResources.SaveToMediaMenuItemText;

            _upgradeMenuItem = new BindableApplicationBarMenuItem();
            _upgradeMenuItem.Command = UpgradeCommand;
            _upgradeMenuItem.Text = AppResources.UpgradeMenuItemText;

            AddAppBarItem(_selectButton);
            AddAppBarItem(_searchButton);
            AddAppBarItem(_mapButton);

            if (!LicenseHelper.IsPremium)
                AddAppBarItem(_upgradeMenuItem);

            AddAppBarItem(_pinRecordMenuItem);
            AddAppBarItem(_settingsMenuItem);
            AddAppBarItem(_aboutMenuItem);

#if BETA
            _reportBugMenuItem = new BindableApplicationBarMenuItem();
            _reportBugMenuItem.Text = AppResources.ReportBugMenuItemText;
            _reportBugMenuItem.Command = ReportBugCommand;
            AddAppBarItem(_reportBugMenuItem);
#endif

#if (DEBUG && BETA) || DEBUG
            _clearIapMenuItem = new BindableApplicationBarMenuItem();
            _clearIapMenuItem.Text = AppResources.ClearIapMenuItemText;
            _clearIapMenuItem.Command = ClearIapCommand;
            AddAppBarItem(_clearIapMenuItem);
#endif

        }

        private void UpdateAppBar()
        {
            AppBarItems.Clear();

            if (IsSelectionEnabled)
            {
                AddAppBarItem(_favoriteButton);
                AddAppBarItem(_deleteButton);
                AddAppBarItem(_saveToMediaMenuItem);
            }
            else
            {
                AddAppBarItem(_selectButton);
                AddAppBarItem(_searchButton);
                AddAppBarItem(_mapButton);

                if (!LicenseHelper.IsPremium)
                    AddAppBarItem(_upgradeMenuItem);

                AddAppBarItem(_pinRecordMenuItem);
                AddAppBarItem(_settingsMenuItem);
                AddAppBarItem(_aboutMenuItem);

#if (DEBUG && BETA) || DEBUG
                AddAppBarItem(_clearIapMenuItem);
#endif

            }
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version != null)
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            return "";
        }

        private bool IsFirstRun()
        {
            return !App.SettingsHelper.Version.Equals(GetVersion());
        }

        private void UpdateVersion()
        {
            App.SettingsHelper.Version = GetVersion();
            App.SettingsHelper.SaveSettings();
        }

        private async void UpgradeToPremium()
        {
#if !DEBUG
            if (IsFirstRun())
            {
#endif
                if (!LicenseHelper.IsPremium)
                {
                    string[] features = 
                { 
                    AppResources.PremiumFeature1Text, 
                    AppResources.PremiumFeature2Text, 
                    AppResources.PremiumFeature3Text, 
                    AppResources.PremiumFeature4Text,
                    AppResources.PremiumFeature5Text,
                    AppResources.PremiumFeature6Text
                };

                    StackPanel sp = new StackPanel();

                    for (int i = 0; i < features.Count(); i++)
                    {
                        TextBlock line = new TextBlock();
                        line.TextWrapping = System.Windows.TextWrapping.Wrap;
                        line.Margin = new System.Windows.Thickness(12, i == 0 ? 17 : 0, 12, 23);
                        line.FontSize = (double)Application.Current.Resources["PhoneFontSizeNormal"];

                        Run bullet = new Run();
                        bullet.Foreground = (Brush)Application.Current.Resources["PhoneAccentBrush"];
                        bullet.Text = "\u2605";

                        Run text = new Run();
                        text.Foreground = (Brush)Application.Current.Resources["PhoneForegroundBrush"];
                        text.Text = features[i];

                        line.Inlines.Add(bullet);
                        line.Inlines.Add("\u2003");
                        line.Inlines.Add(text);

                        sp.Children.Add(line);
                    }

                    TextBlock info = new TextBlock();
                    info.TextWrapping = System.Windows.TextWrapping.Wrap;
                    info.Margin = new System.Windows.Thickness(12, 0, 12, 23);
                    info.FontSize = (double)Application.Current.Resources["PhoneFontSizeNormal"];
                    info.Foreground = (Brush)Application.Current.Resources["PhoneSubtleBrush"];

#if BETA
                    info.Text = AppResources.UpgradeMessageBetaFooter;
#else
                    var formattedPrice = await LicenseHelper.GetPremiumPriceAsync();
                    info.Text = string.Format(AppResources.UpgradeMessageFooter, string.IsNullOrWhiteSpace(formattedPrice) ? "$0.99" : formattedPrice);
#endif

                    sp.Children.Add(info);

                    ScrollViewer sc = new ScrollViewer();
                    sc.Content = sp;

                    string result = await ShowCustomMessageAsync(AppResources.UpgradeMessageText, AppResources.UpgradeMessageCaption,
                        AppResources.UpgradeMessageUpgradeButtonText, AppResources.UpgradeMessageCancelButtonText, sc);

                    if (result.Equals(AppResources.UpgradeMessageUpgradeButtonText))
                    {
                        Upgrade(null);
                    }
                }
#if !DEBUG
            }
#endif
        }

        private async void AskForReview()
        {
#if !BETA
            if (!(NavigationContext.QueryString.ContainsKey("Action") && NavigationContext.QueryString["Action"].Equals("Record")) &&
                !(NavigationContext.QueryString.ContainsKey("VoiceCommand") && NavigationContext.QueryString["VoiceCommand"].Equals("Record")))
            {
                if (App.SettingsHelper.LaunchCount < 1 || App.SettingsHelper.LastLaunchDate < DateTime.Now.Date)
                {
                    if (!App.SettingsHelper.IsAppRated)
                    {
                        App.SettingsHelper.LaunchCount++;
                        App.SettingsHelper.LastLaunchDate = DateTime.Now;
                    }

                    if (App.SettingsHelper.LaunchCount == _askForRatingCount)
                    {
                        string result = null;

                        result = await ShowMessageAsync(AppResources.RatingMessageText, AppResources.RatingMessageCaption, new string[] { AppResources.RatingMessageRateButtonText, AppResources.RatingMessageCancelButtonText });

                        if (result != null && result.Equals(AppResources.RatingMessageRateButtonText))
                        {
                            App.SettingsHelper.IsAppRated = true;
                            TaskHelper.RateApp();
                        }
                        else
                        {
                            result = await ShowMessageAsync(AppResources.FeedbackMessageText, AppResources.FeedbackMessageCaption, new string[] { AppResources.FeedbackMessageGiveButtonText, AppResources.FeedbackMessageCancelButtonText });

                            if (result != null && result.Equals(AppResources.FeedbackMessageGiveButtonText))
                            {
                                TaskHelper.ContactSupport();
                            }
                        }

                        App.SettingsHelper.LaunchCount = 0;
                    }

                    App.SettingsHelper.SaveSettings();
                }
            }
#endif
        }

        private async Task MigrateDatabase()
        {
            if (!App.SettingsHelper.IsDbMigrated && StorageHelper.FileExists("VoiceMemos.sdf"))
            {
                IsSelectionEnabled = false;
                IsAppBarVisible = false;
                IsBusy = true;

                await DatabaseMigrator.MigrateAsync();

                App.SettingsHelper.IsDbMigrated = true;
                App.SettingsHelper.SaveSettings();

                Application.Current.Terminate();

            }
        }

        private void HandleActions()
        {
            if (NavigationContext.QueryString.ContainsKey("Action"))
            {
                string action = NavigationContext.QueryString["Action"];

                switch (action)
                {
                    case "Record":
                        NavigationService.Navigate(ViewLocator.View("Record"));
                        break;
                    case "Memo":
                        if (NavigationContext.QueryString.ContainsKey("Id"))
                            NavigationService.Navigate(ViewLocator.View("Memo", new { Id = NavigationContext.QueryString["Id"] }));
                        break;
                    case "Tag":
                        if (NavigationContext.QueryString.ContainsKey("Id"))
                            NavigationService.Navigate(ViewLocator.View("List", new { Filter = "Tag", Id = NavigationContext.QueryString["Id"] }));
                        break;
                    case "Favorites":
                        NavigationService.Navigate(ViewLocator.View("Favorites"));
                        break;
                }
            }
        }

        private void HandleVoiceCommands()
        {
            if (NavigationContext.QueryString.ContainsKey("VoiceCommand"))
            {
                string voiceCommand = NavigationContext.QueryString["VoiceCommand"];

                switch (voiceCommand)
                {
                    case "Record":
                        NavigationService.Navigate(ViewLocator.View("Record"));
                        break;
                    case "Show":
                        if (NavigationContext.QueryString.ContainsKey("views"))
                        {
                            string view = NavigationContext.QueryString["views"];

                            switch (view)
                            {
                                case "all":
                                    NavigationService.Navigate(ViewLocator.View("List"));
                                    break;
                                case "favorites":
                                    NavigationService.Navigate(ViewLocator.View("Favorites"));
                                    break;
                                case "map":
                                    NavigationService.Navigate(ViewLocator.View("ListMap"));
                                    break;
                            }
                        }
                        break;
                    case "Play":
                        if (NavigationContext.QueryString.ContainsKey("plays"))
                        {
                            var plays = NavigationContext.QueryString["plays"];

                            switch (plays)
                            {
                                case "last recorded":
                                    var memo = Recent.FirstOrDefault();

                                    if (memo != null)
                                        NavigationService.Navigate(ViewLocator.View("Memo", new { Id = memo.Id, Action = "Play" }));
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private async Task AddToFavoritesAsync(IEnumerable<Memo> items)
        {
            var tasks = items.Select(m => Task.Run(() =>
            {
                BeginInvoke(() =>
                {
                    if (!Uow.FavoriteRepository.IsMemoInFavorite(m))
                    {
                        Uow.FavoriteRepository.Add(new Favorite { Memo = m });
                        //Uow.Save();
                    }
                });

            }));

            await Task.WhenAll(tasks);
        }

        private async Task DeleteMemosAsync(IEnumerable<Memo> items)
        {
            var tasks = items.Select(m => Task.Run(async () =>
            {
                BeginInvoke(() =>
                {
                    Uow.MemoRepository.Delete(m);
                    //Uow.MemoTagRepository.DeleteByMemo(m);
                    Uow.FavoriteRepository.DeleteByMemo(m);
                    //Uow.Save();

                    TileHelper.UnPinMemo(m);
                });

                if (BackgroundAudioPlayer.Instance.Track != null &&
                    BackgroundAudioPlayer.Instance.Track.Source.OriginalString.Contains(m.AudioFile))
                {
                    BackgroundAudioPlayer.Instance.Close();
                }

                await StorageHelper.DeleteFileAsync(m.AudioFile);

                if (!string.IsNullOrWhiteSpace(m.ImageFile))
                    await StorageHelper.DeleteFileAsync(m.ImageFile);
            }));

            await Task.WhenAll(tasks);
        }

        private async Task<bool> SaveToMusicAsync(IEnumerable<Memo> items)
        {
            var tasks = items.Select(m => Task.Run<bool>(() =>
            {
                return MediaLibraryHelper.SaveToMediaLibrary(m.AudioFile, m.Title, m.Duration);
            }));

            var result = await Task.WhenAll<bool>(tasks);

            return result.Contains(false) ? false : true;
        }

        #endregion

        #region Command Handlers

        public void Record(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Record"));
        }

        public void Favorites(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Favorites"));
        }

        public void List(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("List"));
        }

        public void ListMap(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("ListMap"));
        }

        public void Search(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Search"));
        }

        public void About(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("About"));
        }

        public void Settings(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Settings"));
        }

        public void MemoTap(object arg)
        {
            var memo = arg as Memo;

            if (memo != null)
            {
                NavigationService.Navigate(ViewLocator.View("Memo", new { Id = memo.Id }));
            }
        }

        public void PinRecord(object arg)
        {
            if (TileHelper.IsRecordPinnned())
            {
                TileHelper.UnPinRecord();
                _pinRecordMenuItem.Text = AppResources.PinRecordMenuItemText;
            }
            else
            {
                TileHelper.PinRecord();
                _pinRecordMenuItem.Text = AppResources.UnPinRecordMenuItemText;
            }
        }

        public async void Upgrade(object arg)
        {
            if (await LicenseHelper.UpgradeToPremiumAsync())
            {
                RemoveAppBarItem(_upgradeMenuItem);
                ShowMessage(AppResources.UpgradeSuccessMessageText, AppResources.UpgradeSuccessMessageCaption, MessageBoxButton.OK);
            }
        }

        public void ReportBug(object arg)
        {
#if BETA
            TaskHelper.ReportBug();
#endif
        }

        public void ClearIap(object arg)
        {
#if (DEBUG && BETA) || DEBUG
            LicenseHelper.ClearMockIAP();
#endif
        }

        public void MemosEnableSelection(object arg)
        {
            IsSelectionEnabled = !IsSelectionEnabled;

            UpdateAppBar();
        }

        public void MemosSelectionChanged(object arg)
        {
            var eventArgs = (arg as SelectionChangedEventArgs);

            if (eventArgs != null)
            {
                foreach (Memo memo in eventArgs.AddedItems)
                {
                    _selectedItems.Add(memo);
                }

                foreach (Memo memo in eventArgs.RemovedItems)
                {
                    _selectedItems.Remove(memo);
                }
            }

            UpdateAppBar();
        }

        public async void SaveToMediaLibrary(object arg)
        {
            if (_selectedItems.Count > 0)
            {
                var memosToSave = _selectedItems.ToList();

                BeginInvoke(() =>
                {
                    IsSelectionEnabled = false;
                    IsAppBarVisible = false;
                    IsBusy = true;
                });

                var isSuccess = await SaveToMusicAsync(memosToSave);

                BeginInvoke(() =>
                {
                    IsBusy = false;
                    IsAppBarVisible = true;
                    UpdateAppBar();
                });

                if (isSuccess)
                {
                    ShowToast(AppResources.SaveToMediaSuccessMessageCaption, AppResources.SaveToMediaMultipleSuccessMessageText);
                }
                else
                {
                    ShowToast(AppResources.SaveToMediaFailedMessageCaption, AppResources.SaveToMediaMultipleFailedMessageText);
                }
            }
        }

        public async void MemoDelete(object arg)
        {
            if (_selectedItems.Count > 0)
            {
                if (!App.SettingsHelper.ConfirmDelete || ShowMessage(AppResources.DeleteMultipleMessageText,
                    AppResources.DeleteMessageCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var memosToDelete = _selectedItems.ToList();

                    BeginInvoke(() =>
                    {
                        IsSelectionEnabled = false;
                        IsAppBarVisible = false;
                        IsBusy = true;
                    });

                    await DeleteMemosAsync(memosToDelete);

                    BeginInvoke(() =>
                    {
                        IsBusy = false;
                        IsAppBarVisible = true;
                        UpdateAppBar();
                    });
                }
            }
        }

        public async void AddToFavorites(object arg)
        {
            if (_selectedItems.Count > 0)
            {
                var memosToAdd = _selectedItems.ToList();

                BeginInvoke(() =>
                {
                    IsSelectionEnabled = false;
                    IsAppBarVisible = false;
                    IsBusy = true;
                });

                await AddToFavoritesAsync(memosToAdd);

                BeginInvoke(() =>
                {
                    IsBusy = false;
                    IsAppBarVisible = true;
                    UpdateAppBar();
                });

                ShowToast(AppResources.AddToFavoriteSuccessMessageCaption, AppResources.AddToFavoriteMultipleSuccessMessageText);
            }

        }

        #endregion

    }
}
