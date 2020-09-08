using Creobe.VoiceMemos.Controls;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Core.Extensions;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class ListViewModel : ViewModelBase
    {
        #region Private Members

        private BindableApplicationBarIconButton _deleteButton;
        private BindableApplicationBarIconButton _favoriteButton;
        private BindableApplicationBarIconButton _selectButton;
        private BindableApplicationBarIconButton _searchButton;

        private BindableApplicationBarMenuItem _pinTagMenuItem;
        private BindableApplicationBarMenuItem _saveToMediaMenuItem;

        private Tag _selectedtag;
        private List<Memo> _selectedItems;

        #endregion

        #region Commands

        public ICommand AddToFavoritesCommand { get; private set; }
        public ICommand MemosEnableSelectionCommand { get; private set; }
        public ICommand MemosSelectionChangedCommand { get; private set; }
        public ICommand PivotSelectionChangedCommand { get; private set; }
        public ICommand MemoTapCommand { get; private set; }
        public ICommand TagTapCommand { get; private set; }
        public ICommand PinTagCommand { get; private set; }
        public ICommand SaveToMediaLibraryCommand { get; private set; }
        public ICommand MemoDeleteCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }

        #endregion

        #region Properties

        private ObservableCollection<Memo> _all;

        public ObservableCollection<Memo> All
        {
            get { return _all; }
            set
            {
                _all = value;
                NotifyPropertyChanged("All");
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get
            {
                return Uow.TagRepository.All;
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

        private string _memoListHeading;

        public string MemoListHeading
        {
            get { return _memoListHeading; }
            set
            {
                _memoListHeading = value;
                NotifyPropertyChanged("MemoListHeading");
            }
        }

        private bool _isPivotLocked;

        public bool IsPivotLocked
        {
            get { return _isPivotLocked; }
            set
            {
                _isPivotLocked = value;
                NotifyPropertyChanged("IsPivotLocked");
            }
        }

        #endregion

        #region Constructors

        public ListViewModel()
        {
            InitializeCommands();
            InitializeAppBar();

            _selectedItems = new List<Memo>();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Filter"))
            {
                string filter = NavigationContext.QueryString["Filter"];

                switch (filter)
                {
                    case "Tag":
                        if (NavigationContext.QueryString.ContainsKey("Id"))
                        {
                            int tagId = int.Parse(NavigationContext.QueryString["Id"]);
                            _selectedtag = Uow.TagRepository.Find(tagId);

                            AddAppBarItem(_pinTagMenuItem);

                            MemoListHeading = _selectedtag.Name;
                            IsPivotLocked = true;

                            if (TileHelper.IsTagPinnned(_selectedtag))
                                _pinTagMenuItem.Text = AppResources.UnPinTagMenuItemText;

                            All = new ObservableCollection<Memo>(Uow.MemoRepository.ByTag(tagId));
                        }
                        break;
                    case "Untagged":
                        MemoListHeading = "untagged";
                        IsPivotLocked = true;
                        All = new ObservableCollection<Memo>(Uow.MemoRepository.Untagged());
                        break;
                }
            }
            else
            {
                MemoListHeading = AppResources.MemoListPivotHeadingAllText;
                IsPivotLocked = false;

                All = Uow.MemoRepository.All;
            }

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

        private void InitializeCommands()
        {
            AddToFavoritesCommand = new DelegateCommand(AddToFavorites);
            MemosEnableSelectionCommand = new DelegateCommand(MemosEnableSelection);
            MemosSelectionChangedCommand = new DelegateCommand(MemosSelectionChanged);
            PivotSelectionChangedCommand = new DelegateCommand(PivotSelectionChanged);
            MemoTapCommand = new DelegateCommand(MemoTap);
            TagTapCommand = new DelegateCommand(TagTap);
            PinTagCommand = new DelegateCommand(PinTag);
            SaveToMediaLibraryCommand = new DelegateCommand(SaveToMediaLibrary);
            MemoDeleteCommand = new DelegateCommand(MemoDelete);
            SearchCommand = new DelegateCommand(Search);
        }

        private void InitializeAppBar()
        {
            _selectButton = new BindableApplicationBarIconButton();
            _searchButton = new BindableApplicationBarIconButton();
            _pinTagMenuItem = new BindableApplicationBarMenuItem();
            _saveToMediaMenuItem = new BindableApplicationBarMenuItem();

            _favoriteButton = new BindableApplicationBarIconButton();
            _favoriteButton.Command = AddToFavoritesCommand;
            _favoriteButton.IconUri = new Uri("/Assets/AppBar/appbar.star.add.png", UriKind.RelativeOrAbsolute);
            _favoriteButton.Text = AppResources.AddToFavoriteButtonText;

            _deleteButton = new BindableApplicationBarIconButton();
            _deleteButton.Command = MemoDeleteCommand;
            _deleteButton.IconUri = new Uri("/Assets/AppBar/delete.png", UriKind.RelativeOrAbsolute);
            _deleteButton.Text = AppResources.DeleteButtonText;

            _selectButton.IconUri = new Uri("/Assets/AppBar/appbar.list.check.png", UriKind.RelativeOrAbsolute);
            _searchButton.IconUri = new Uri("/Assets/AppBar/feature.search.png", UriKind.RelativeOrAbsolute);

            _selectButton.Text = AppResources.SelectButtonText;
            _searchButton.Text = AppResources.SearchButtonText;
            _pinTagMenuItem.Text = AppResources.PinTagMenuItemText;
            _saveToMediaMenuItem.Text = AppResources.SaveToMediaMenuItemText;

            _selectButton.Command = MemosEnableSelectionCommand;
            _searchButton.Command = SearchCommand;
            _pinTagMenuItem.Command = PinTagCommand;
            _saveToMediaMenuItem.Command = SaveToMediaLibraryCommand;

            AddAppBarItem(_selectButton);
            AddAppBarItem(_searchButton);
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

                if (_selectedtag != null)
                    AddAppBarItem(_pinTagMenuItem);
            }
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

        public void MemosEnableSelection(object arg)
        {
            IsSelectionEnabled = !IsSelectionEnabled;

            UpdateAppBar();
        }

        public void MemoTap(object arg)
        {
            var memo = arg as Memo;

            if (memo != null)
            {
                NavigationService.Navigate(ViewLocator.View("Memo", new { Id = memo.Id }));
            }
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

                    if (_selectedtag != null)
                        All.RemoveRangeFromCollection(memosToDelete);
                }
            }
        }

        public void Search(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Search"));
        }

        public void TagTap(object arg)
        {
            var tag = arg as Tag;

            if (tag != null)
            {
                NavigationService.Navigate(ViewLocator.View("List", new { Filter = "Tag", Id = tag.Id }));
            }
            else
            {
                NavigationService.Navigate(ViewLocator.View("List", new { Filter = "Untagged" }));
            }
        }

        public void PivotSelectionChanged(object arg)
        {
            var eventArgs = (arg as SelectionChangedEventArgs);

            if (eventArgs != null)
            {
                var pivotItem = eventArgs.AddedItems[0] as PivotItem;

                switch (pivotItem.Name)
                {
                    case "MemosPivotItem":
                        IsAppBarVisible = true;
                        break;
                    case "TagsPivotItem":
                        IsAppBarVisible = false;
                        break;
                    default:
                        IsAppBarVisible = true;
                        break;
                }
            }
        }

        public void PinTag(object arg)
        {
            if (TileHelper.IsTagPinnned(_selectedtag))
            {
                TileHelper.UnPinTag(_selectedtag);
                _pinTagMenuItem.Text = AppResources.PinTagMenuItemText;
            }
            else
            {
                TileHelper.PinTag(_selectedtag);
                _pinTagMenuItem.Text = AppResources.UnPinTagMenuItemText;
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
