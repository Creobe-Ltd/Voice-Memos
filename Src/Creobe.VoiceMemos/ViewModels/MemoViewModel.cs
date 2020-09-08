using Creobe.VoiceMemos.Controls;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Creobe.VoiceMemos.ViewModels
{
    public class MemoViewModel : ViewModelBase
    {
        #region Private Members

        private BindableApplicationBarIconButton _editButton;
        private BindableApplicationBarIconButton _favoriteButton;
        private BindableApplicationBarIconButton _playPauseButton;
        private BindableApplicationBarIconButton _deleteButton;

        private BindableApplicationBarMenuItem _pinMemoMenuItem;
        private BindableApplicationBarMenuItem _saveToMediaMenuItem;
        private BindableApplicationBarMenuItem _shareMenuItem;

        private BackgroundAudioPlayer _instance = BackgroundAudioPlayer.Instance;
        private Memo _memo;
        private StorageHelper _storage = new StorageHelper();
        private DispatcherTimer _timer;

        #endregion

        #region Commands

        public ICommand AddRemoveFavoriteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand PinMemoCommand { get; private set; }
        public ICommand PlayPauseCommand { get; private set; }
        public ICommand SaveToMediaCommand { get; private set; }
        public ICommand ShareCommand { get; private set; }
        public ICommand ShowHideNotesCommand { get; private set; }
        public ICommand ShowMapCommand { get; private set; }
        public ICommand TagTapCommand { get; private set; }

        #endregion

        #region Properties

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        private int _duration;

        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        private string _durationText;

        public string DurationText
        {
            get { return _durationText; }
            set
            {
                _durationText = value;
                NotifyPropertyChanged("DurationText");
            }
        }

        private string _imageFile;

        public string ImageFile
        {
            get { return _imageFile; }
            set
            {
                _imageFile = value;
                NotifyPropertyChanged("ImageFile");
            }
        }

        private double _position;

        public double Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyPropertyChanged("Position");
            }
        }

        private string _positionText;

        public string PositionText
        {
            get { return _positionText; }
            set
            {
                _positionText = value;
                NotifyPropertyChanged("PositionText");
            }
        }

        private ObservableCollection<Tag> _tags;

        public ObservableCollection<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                NotifyPropertyChanged("Tags");
            }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string _showHideNotesText;

        public string ShowHideNotesText
        {
            get { return _showHideNotesText; }
            set
            {
                _showHideNotesText = value;
                NotifyPropertyChanged("ShowHideNotesText");
            }
        }

        private bool _showNotes;

        public bool ShowNotes
        {
            get { return _showNotes; }
            set
            {
                _showNotes = value;
                NotifyPropertyChanged("ShowNotes");
            }
        }

        #endregion

        #region Constructors

        public MemoViewModel()
        {
            InitializeCommands();
            InitializeAppBar();

            _instance.PlayStateChanged += OnPlayStateChanged;

            _showHideNotesText = AppResources.ShowNotesButtonText;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1);
            _timer.Tick += OnTimerTick;
        }

        #endregion

        #region Overrides

        public override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("Id"))
            {
                int Id = int.Parse(NavigationContext.QueryString["Id"]);
                _memo = Uow.MemoRepository.Find(Id);

                if (_memo != null)
                {
                    Title = _memo.Title;
                    Description = _memo.Description;
                    Duration = (int)TimeSpan.FromSeconds(_memo.Duration).TotalMilliseconds;
                    DurationText = TimeSpan.FromSeconds(_memo.Duration).ToString();
                    PositionText = "00:00:00";
                    Tags = new ObservableCollection<Tag>(_memo.Tags);
                    ImageFile = _memo.ImageFile;

                    //if (_memo.Latitude.HasValue && _memo.Longitude.HasValue)
                    //    AddAppBarItem(_mapMenuItem);

                    if (Uow.FavoriteRepository.IsMemoInFavorite(_memo))
                    {
                        _favoriteButton.IconUri = new Uri("/Assets/AppBar/appbar.star.minus.png", UriKind.RelativeOrAbsolute);
                        _favoriteButton.Text = AppResources.RemoveFromFavoriteButtonText;
                    }

                    SetControlStates();

                    if (TileHelper.IsMemoPinned(_memo))
                        _pinMemoMenuItem.Text = AppResources.UnPinMemoMenuItemText;
                    else
                        _pinMemoMenuItem.Text = AppResources.PinMemoMenuItemText;

                    if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
                    {
                        if (NavigationContext.QueryString.ContainsKey("Action"))
                        {
                            var action = NavigationContext.QueryString["Action"];

                            switch (action)
                            {
                                case "Play":
                                    PlayPause(null);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeAppBar()
        {
            // Icon Buttons

            _editButton = new BindableApplicationBarIconButton();
            _editButton.Command = EditCommand;
            _editButton.IconUri = new Uri("/Assets/AppBar/edit.png", UriKind.RelativeOrAbsolute);
            _editButton.Text = AppResources.EditButtonText;

            _favoriteButton = new BindableApplicationBarIconButton();
            _favoriteButton.Command = AddRemoveFavoriteCommand;
            _favoriteButton.IconUri = new Uri("/Assets/AppBar/appbar.star.add.png", UriKind.RelativeOrAbsolute);
            _favoriteButton.Text = AppResources.AddToFavoriteButtonText;

            _playPauseButton = new BindableApplicationBarIconButton();
            _playPauseButton.Command = PlayPauseCommand;
            _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
            _playPauseButton.Text = AppResources.PlayButtonText;

            _deleteButton = new BindableApplicationBarIconButton();
            _deleteButton.Command = DeleteCommand;
            _deleteButton.IconUri = new Uri("/Assets/AppBar/delete.png", UriKind.RelativeOrAbsolute);
            _deleteButton.Text = AppResources.DeleteButtonText;

            // Menu Items

            _pinMemoMenuItem = new BindableApplicationBarMenuItem();
            _pinMemoMenuItem.Command = PinMemoCommand;
            _pinMemoMenuItem.Text = AppResources.PinMemoMenuItemText;

            _saveToMediaMenuItem = new BindableApplicationBarMenuItem();
            _saveToMediaMenuItem.Command = SaveToMediaCommand;
            _saveToMediaMenuItem.Text = AppResources.SaveToMediaMenuItemText;

            _shareMenuItem = new BindableApplicationBarMenuItem();
            _shareMenuItem.Command = ShareCommand;
            _shareMenuItem.Text = AppResources.ShareMenuItemText;

            // Add app bar items

            AddAppBarItem(_favoriteButton);
            AddAppBarItem(_playPauseButton);
            AddAppBarItem(_editButton);
            AddAppBarItem(_deleteButton);
            AddAppBarItem(_shareMenuItem);
            AddAppBarItem(_pinMemoMenuItem);
            AddAppBarItem(_saveToMediaMenuItem);
        }

        private void InitializeCommands()
        {
            PlayPauseCommand = new DelegateCommand(PlayPause);
            ShowMapCommand = new DelegateCommand(ShowMap);
            AddRemoveFavoriteCommand = new DelegateCommand(AddRemoveFavorite);
            EditCommand = new DelegateCommand(Edit);
            DeleteCommand = new DelegateCommand(Delete);
            SaveToMediaCommand = new DelegateCommand(SaveToMedia);
            ShareCommand = new DelegateCommand(Share);
            PinMemoCommand = new DelegateCommand(PinMemo);
            ShowHideNotesCommand = new DelegateCommand(ShowHideNotes);
            TagTapCommand = new DelegateCommand(TagTap);
        }

        private void OnPlayStateChanged(object sender, EventArgs e)
        {
            SetControlStates();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_instance.PlayerState == PlayState.Playing || _instance.PlayerState == PlayState.FastForwarding || _instance.PlayerState == PlayState.Rewinding)
            {
                BeginInvoke(() =>
                {
                    Position = (int)_instance.Position.TotalMilliseconds;
                    PositionText = TimeSpan.FromMilliseconds(Position).ToString(@"hh\:mm\:ss");
                });

            }
            else if (_instance.PlayerState == PlayState.Stopped || _instance.PlayerState == PlayState.Unknown)
            {
                _playPauseButton.Text = AppResources.PlayButtonText;
                _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
                _timer.Stop();
                Position = 0;
                PositionText = "00:00:00";
            }
        }

        private void SetAudioTrack(string fileName, string title)
        {
            if (_instance.Track != null)
            {
                _instance.Track = null;
            }

            _instance.Track = new AudioTrack(new Uri(fileName, UriKind.Relative), title, null, null, null);
        }

        private void SetControlStates()
        {
            if (_instance.Track != null && _instance.Track.Source.OriginalString.Contains(_memo.AudioFile))
            {
                switch (_instance.PlayerState)
                {
                    case PlayState.Playing:
                        _playPauseButton.Text = AppResources.PauseButtonText;
                        _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.pause.png", UriKind.RelativeOrAbsolute);
                        Position = _instance.Position.TotalMilliseconds;
                        PositionText = TimeSpan.FromMilliseconds(Position).ToString(@"hh\:mm\:ss");
                        _timer.Start();
                        break;
                    case PlayState.Paused:
                        _playPauseButton.Text = AppResources.PlayButtonText;
                        _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
                        _timer.Stop();
                        Position = _instance.Position.TotalMilliseconds;
                        PositionText = TimeSpan.FromMilliseconds(Position).ToString(@"hh\:mm\:ss");
                        break;
                    case PlayState.Stopped:
                        _playPauseButton.Text = AppResources.PlayButtonText;
                        _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
                        _timer.Stop();
                        Position = 0;
                        PositionText = "00:00:00";
                        break;
                    case PlayState.TrackEnded:
                        _playPauseButton.Text = AppResources.PlayButtonText;
                        _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
                        _timer.Stop();
                        Position = 0;
                        PositionText = "00:00:00";
                        break;
                    case PlayState.Unknown:
                        _playPauseButton.Text = AppResources.PlayButtonText;
                        _playPauseButton.IconUri = new Uri("/Assets/AppBar/transport.play.png", UriKind.RelativeOrAbsolute);
                        _timer.Stop();
                        Position = 0;
                        PositionText = "00:00:00";
                        break;
                }

            }
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

        public void AddRemoveFavorite(object arg)
        {
            if (!Uow.FavoriteRepository.IsMemoInFavorite(_memo))
            {
                Uow.FavoriteRepository.Add(new Favorite { Memo = _memo });
                _favoriteButton.IconUri = new Uri("/Assets/AppBar/appbar.star.minus.png", UriKind.RelativeOrAbsolute);
                _favoriteButton.Text = AppResources.RemoveFromFavoriteButtonText;
            }
            else
            {
                Uow.FavoriteRepository.DeleteByMemo(_memo);
                _favoriteButton.IconUri = new Uri("/Assets/AppBar/appbar.star.add.png", UriKind.RelativeOrAbsolute);
                _favoriteButton.Text = AppResources.AddToFavoriteButtonText;
            }

            //Uow.Save();
        }

        public async void Delete(object arg)
        {
            if (!App.SettingsHelper.ConfirmDelete || ShowMessage(AppResources.DeleteSingleMessageText,
                AppResources.DeleteMessageCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //Uow.MemoTagRepository.DeleteByMemo(_memo);
                Uow.FavoriteRepository.DeleteByMemo(_memo);
                Uow.MemoRepository.Delete(_memo);
                //Uow.Save();

                TileHelper.UnPinMemo(_memo);

                if (_instance.Track != null && _instance.Track.Source.OriginalString.Contains(_memo.AudioFile))
                {
                    _instance.Close();
                }

                await StorageHelper.DeleteFileAsync(_memo.AudioFile);

                if (!string.IsNullOrWhiteSpace(_memo.ImageFile))
                    await (StorageHelper.DeleteFileAsync(_memo.ImageFile));

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        public void Edit(object arg)
        {
            NavigationService.Navigate(ViewLocator.View("Edit", new { Id = _memo.Id, SourceView = "Memo" }));
        }

        public void PinMemo(object arg)
        {
            if (TileHelper.IsMemoPinned(_memo))
            {
                TileHelper.UnPinMemo(_memo);
                _pinMemoMenuItem.Text = AppResources.PinMemoMenuItemText;
            }
            else
            {
                TileHelper.PinMemo(_memo);
                _pinMemoMenuItem.Text = AppResources.UnPinMemoMenuItemText;
            }
        }

        public void PlayPause(object arg)
        {
            if (_memo != null)
            {
                if (App.SettingsHelper.UseSystemPlayer)
                {
                    MediaPlayerLauncher playerLauncher = new MediaPlayerLauncher();

                    playerLauncher.Controls = MediaPlaybackControls.FastForward |
                        MediaPlaybackControls.Rewind | MediaPlaybackControls.Pause | MediaPlaybackControls.Stop;

                    playerLauncher.Location = MediaLocationType.Data;
                    playerLauncher.Media = new Uri(_memo.AudioFile, UriKind.RelativeOrAbsolute);
                    playerLauncher.Orientation = MediaPlayerOrientation.Portrait;

                    if (!_memo.IsPlayed)
                    {
                        _memo.IsPlayed = true;
                        //Uow.Save();
                    }

                    playerLauncher.Show();
                }
                else
                {
                    if (_instance.Track == null)
                        SetAudioTrack(_memo.AudioFile, _memo.Title);

                    if (!_instance.Track.Source.OriginalString.Contains(_memo.AudioFile))
                        SetAudioTrack(_memo.AudioFile, _memo.Title);

                    if (_instance.PlayerState != PlayState.Playing)
                    {
                        _instance.Play();

                        if (!_memo.IsPlayed)
                        {
                            _memo.IsPlayed = true;
                            Uow.MemoRepository.Update(_memo);
                            //Uow.Save();
                        }
                    }
                    else
                    {
                        if (_instance.CanPause)
                            _instance.Pause();
                    }
                }
            }
        }

        public void SaveToMedia(object arg)
        {
            if (_memo != null)
            {
                if (MediaLibraryHelper.SaveToMediaLibrary(_memo.AudioFile, _memo.Title, _memo.Duration))
                {
                    ShowToast(AppResources.SaveToMediaSuccessMessageCaption, AppResources.SaveToMediaSingleSuccessMessageText);
                }
                else
                {
                    ShowToast(AppResources.SaveToMediaFailedMessageCaption, AppResources.SaveToMediaSingleFailedMessageText);
                }
            }
            else
            {
                ShowToast(AppResources.SaveToMediaFailedMessageCaption, AppResources.SaveToMediaSingleFailedMessageText);
            }
        }

        public void Share(object arg)
        {
            if (LicenseHelper.IsPremium)
                NavigationService.Navigate(ViewLocator.View("Share", new { Id = _memo.Id }));
            else
                UpgradeToPremium();
        }

        public void ShowHideNotes(object arg)
        {
            ShowNotes = !ShowNotes;
            ShowHideNotesText = ShowNotes ? AppResources.HideNotesButtonText : AppResources.ShowNotesButtonText;

        }

        public void ShowMap(object arg)
        {
#if BETA
            ShowMessage(AppResources.NotImplementedMessageText, AppResources.NotImplementedMessageCaption,
                System.Windows.MessageBoxButton.OK);
#else
            // TODO : Add code here
#endif
        }

        public void TagTap(object arg)
        {
            var tag = arg as Tag;

            if (tag != null)
            {
                NavigationService.Navigate(ViewLocator.View("List", new { Filter = "Tag", Id = tag.Id }));
            }
        }

        #endregion

    }
}
