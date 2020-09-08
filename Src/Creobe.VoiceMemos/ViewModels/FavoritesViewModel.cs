using Creobe.VoiceMemos.Controls;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.BackgroundAudio;
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
    public class FavoritesViewModel : ViewModelBase
    {
        #region Private Members

        private BindableApplicationBarIconButton _deleteButton;
        private BindableApplicationBarIconButton _selectButton;
        private BindableApplicationBarIconButton _removeButton;

        private BindableApplicationBarMenuItem _pinFavoritesMenuItem;
        private BindableApplicationBarMenuItem _saveToMediaMenuItem;

        private List<Favorite> _selectedItems;

        #endregion

        #region Commands

        public ICommand DeleteCommand { get; private set; }
        public ICommand EnableSelectionCommand { get; private set; }
        public ICommand FavoriteTapCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand SaveToMediaLibraryCommand { get; private set; }
        public ICommand SelectionChangedCommand { get; private set; }
        public ICommand PinFavoritesCommand { get; private set; }

        #endregion

        #region Properties

        public ObservableCollection<Favorite> All
        {
            get
            {
                return Uow.FavoriteRepository.All;
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

        #endregion

        #region Constructors

        public FavoritesViewModel()
        {
            InitializeCommands();
            InitializeAppBar();

            _selectedItems = new List<Favorite>();
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (TileHelper.IsFavoritesPinnned())
                _pinFavoritesMenuItem.Text = AppResources.UnPinFavoritesMenuItemText;
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

        private void InitializeAppBar()
        {
            _deleteButton = new BindableApplicationBarIconButton();
            _deleteButton.Command = DeleteCommand;
            _deleteButton.IconUri = new Uri("/Assets/AppBar/delete.png", UriKind.RelativeOrAbsolute);
            _deleteButton.Text = AppResources.DeleteButtonText;

            _removeButton = new BindableApplicationBarIconButton();
            _removeButton.Command = RemoveCommand;
            _removeButton.IconUri = new Uri("/Assets/AppBar/appbar.star.minus.png", UriKind.RelativeOrAbsolute);
            _removeButton.Text = AppResources.RemoveFromFavoriteButtonText;

            _selectButton = new BindableApplicationBarIconButton();
            _selectButton.Command = EnableSelectionCommand;
            _selectButton.IconUri = new Uri("/Assets/AppBar/appbar.list.check.png", UriKind.RelativeOrAbsolute);
            _selectButton.Text = AppResources.SelectButtonText;

            _pinFavoritesMenuItem = new BindableApplicationBarMenuItem();
            _pinFavoritesMenuItem.Command = PinFavoritesCommand;
            _pinFavoritesMenuItem.Text = AppResources.PinFavoritesMenuItemText;

            _saveToMediaMenuItem = new BindableApplicationBarMenuItem();
            _saveToMediaMenuItem.Command = SaveToMediaLibraryCommand;
            _saveToMediaMenuItem.Text = AppResources.SaveToMediaMenuItemText;

            AddAppBarItem(_selectButton);
            AddAppBarItem(_pinFavoritesMenuItem);
        }

        private void InitializeCommands()
        {
            DeleteCommand = new DelegateCommand(Delete);
            EnableSelectionCommand = new DelegateCommand(EnableSelection);
            FavoriteTapCommand = new DelegateCommand(FavoriteTap);
            RemoveCommand = new DelegateCommand(Remove);
            SaveToMediaLibraryCommand = new DelegateCommand(SaveToMediaLibrary);
            SelectionChangedCommand = new DelegateCommand(SelectionChanged);
            PinFavoritesCommand = new DelegateCommand(PinFavorites);
        }

        private async Task RemoveFavoritesAsync(IEnumerable<Favorite> items)
        {
            var tasks = items.Select(f => Task.Run(() =>
            {
                BeginInvoke(() =>
                {
                    Uow.FavoriteRepository.Delete(f);
                    //Uow.Save();
                });

            }));

            await Task.WhenAll(tasks);
        }

        private void UpdateAppBar()
        {
            AppBarItems.Clear();

            if (IsSelectionEnabled)
            {
                AddAppBarItem(_removeButton);
                AddAppBarItem(_deleteButton);
                AddAppBarItem(_saveToMediaMenuItem);
            }
            else
            {
                AddAppBarItem(_selectButton);
                AddAppBarItem(_pinFavoritesMenuItem);
            }
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

        public async void Delete(object arg)
        {
            if (_selectedItems.Count > 0)
            {
                if (!App.SettingsHelper.ConfirmDelete || ShowMessage(AppResources.DeleteMultipleMessageText,
                    AppResources.DeleteMessageCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var memosToDelete = _selectedItems
                        .Select(f => f.Memo)
                        .ToList();

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

        public void EnableSelection(object arg)
        {
            IsSelectionEnabled = !IsSelectionEnabled;

            UpdateAppBar();
        }

        public void FavoriteTap(object arg)
        {
            var favorite = arg as Favorite;

            if (favorite != null)
            {
                NavigationService.Navigate(ViewLocator.View("Memo", new { Id = favorite.Memo.Id }));
            }
        }

        public async void Remove(object arg)
        {
            if (_selectedItems.Count > 0)
            {
                var favoritesToRemove = _selectedItems.ToList();

                BeginInvoke(() =>
                {
                    IsSelectionEnabled = false;
                    IsAppBarVisible = false;
                    IsBusy = true;
                });

                await RemoveFavoritesAsync(favoritesToRemove);

                BeginInvoke(() =>
                {
                    IsBusy = false;
                    IsAppBarVisible = true;
                    UpdateAppBar();
                });
            }
        }

        public async void SaveToMediaLibrary(object arg)
        {
            if (_selectedItems.Count > 0)
            {
                var memosToSave = _selectedItems
                    .Select(f => f.Memo)
                    .ToList();

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

        public void SelectionChanged(object arg)
        {
            var eventArgs = (arg as SelectionChangedEventArgs);

            if (eventArgs != null)
            {
                foreach (Favorite favorite in eventArgs.AddedItems)
                {
                    _selectedItems.Add(favorite);
                }

                foreach (Favorite favorite in eventArgs.RemovedItems)
                {
                    _selectedItems.Remove(favorite);
                }
            }

            UpdateAppBar();
        }

        public void PinFavorites(object arg)
        {
            if (TileHelper.IsFavoritesPinnned())
            {
                TileHelper.UnPinFavorites();
                _pinFavoritesMenuItem.Text = AppResources.PinFavoritesMenuItemText;
            }
            else
            {
                TileHelper.PinFavorites();
                _pinFavoritesMenuItem.Text = AppResources.UnPinFavoritesMenuItemText;
            }

        }

        #endregion

    }
}
