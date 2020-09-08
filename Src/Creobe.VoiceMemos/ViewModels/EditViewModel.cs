using Creobe.VoiceMemos.Controls;
using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Data.Models;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Models;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Creobe.VoiceMemos.ViewModels
{
    public class EditViewModel : ViewModelBase
    {
        #region Private Members

        private BindableApplicationBarIconButton _saveButton;
        private BindableApplicationBarIconButton _cancelButton;

        private BindableApplicationBarMenuItem _deleteMenuItem;

        private PhotoChooserTask _photoChooser;
        private Memo _memo;

        #endregion

        #region Commands

        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }
        public ICommand TagTapCommand { get; private set; }

        #endregion

        #region Properties

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

        private string _tags;

        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                NotifyPropertyChanged("Tags");
            }
        }

        public List<Tag> TagList
        {
            get
            {
                return Uow.TagRepository.All
                    .OrderByDescending(t => Uow.MemoRepository.All.Count(m => m.Tags.Contains(t)))
                    .ToList();

            }
        }

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

        private BitmapImage _chosenImage;

        public BitmapImage ChosenImage
        {
            get { return _chosenImage; }
            set
            {
                _chosenImage = value;
                NotifyPropertyChanged("ChosenImage");
            }
        }


        #endregion

        #region Constructors

        public EditViewModel()
        {
            InitializeCommands();
            InitializeAppBar();

            _photoChooser = new PhotoChooserTask();
            _photoChooser.PixelWidth = 720;
            _photoChooser.PixelHeight = 405;
            _photoChooser.ShowCamera = true;

            _photoChooser.Completed += _photoChooser_Completed;
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
                    Tags = string.Join(", ", _memo.Tags.Select(t => t.Name.ToLower()).ToArray());
                    ImageFile = _memo.ImageFile;
                }
            }

            if (NavigationContext.QueryString.ContainsKey("SourceView"))
            {
                switch (NavigationContext.QueryString["SourceView"])
                {
                    case "Record":
                        NavigationService.RemoveBackEntry();
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            DeleteCommand = new DelegateCommand(Delete);
            AddImageCommand = new DelegateCommand(AddImage);
            TagTapCommand = new DelegateCommand(TagTap);
        }

        private void InitializeAppBar()
        {
            _saveButton = new BindableApplicationBarIconButton();
            _cancelButton = new BindableApplicationBarIconButton();
            _deleteMenuItem = new BindableApplicationBarMenuItem();

            _saveButton.IconUri = new Uri("/Assets/AppBar/save.png", UriKind.RelativeOrAbsolute);
            _cancelButton.IconUri = new Uri("/Assets/AppBar/cancel.png", UriKind.RelativeOrAbsolute);

            _saveButton.Text = AppResources.SaveButtonText;
            _cancelButton.Text = AppResources.CancelButtonText;
            _deleteMenuItem.Text = AppResources.DeleteMenuItemText;

            _saveButton.Command = SaveCommand;
            _cancelButton.Command = CancelCommand;
            _deleteMenuItem.Command = DeleteCommand;

            AddAppBarItem(_saveButton);
            AddAppBarItem(_cancelButton);
            AddAppBarItem(_deleteMenuItem);
        }

        private void _photoChooser_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK && e.ChosenPhoto != null)
            {
                if (ChosenImage == null)
                    ChosenImage = new BitmapImage();

                ChosenImage.SetSource(e.ChosenPhoto);
            }
        }

        #endregion

        #region Command Handlers

        public void Save(object arg)
        {
            _memo.Title = Title;
            _memo.Description = Description;

            var tags1 = Tags.Split(new char[] { ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLower()).Distinct().ToArray();

            var tags2 = _memo.Tags.Select(t => t.Name.ToLower()).ToArray();

            foreach (var tag in tags2.Except(tags1))
            {
                var memoTag = _memo.Tags.Where(t => t.Name.ToLower().Equals(tag)).FirstOrDefault();
                //Uow.MemoTagRepository.Delete(memoTag);
                _memo.Tags.Remove(memoTag);
            }

            foreach (var tag in tags1.Except(tags2))
            {
                var tg = Uow.TagRepository.All
                    .Where(t => t.Name.ToLower().Equals(tag.ToLower()))
                    .FirstOrDefault();

                if (tg == null)
                {
                    tg = new Tag { Name = tag.ToLower() };
                    Uow.TagRepository.Add(tg);
                }

                //var memoTag = new MemoTag { Tag = tg };
                //Uow.MemoTagRepository.Add(memoTag);
                _memo.Tags.Add(tg);
            }

            Tags = string.Join(", ", _memo.Tags.Select(t => t.Name.ToLower()).ToArray());

            if (ChosenImage != null)
            {
                var imageFileName = string.IsNullOrWhiteSpace(_memo.ImageFile) ? Guid.NewGuid().ToString() + ".jpg" : _memo.ImageFile;
                StorageHelper.SaveImageFile(imageFileName, ChosenImage);

                _memo.ImageFile = imageFileName;
            }

            //Uow.Save();
            Uow.MemoRepository.Update(_memo);

            TileHelper.UpdateMemo(_memo);

            //if (_memo.AudioFormat.Equals("MP3"))
            //{
            //    StorageHelper.WriteId3Tag(_memo.AudioFile, ChosenImage, _memo.Title, "Voice Memos", "Voice Memos", _memo.Description);
            //}

            if (NavigationService.CanGoBack)
                NavigationService.GoBack();


        }

        public void Cancel(object arg)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        public async void Delete(object arg)
        {

            if (!App.SettingsHelper.ConfirmDelete || ShowMessage(AppResources.DeleteSingleMessageText,
                AppResources.DeleteMessageCaption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //Uow.MemoTagRepository.DeleteByMemo(_memo);
                Uow.MemoRepository.Delete(_memo);
                //Uow.Save();

                TileHelper.UnPinMemo(_memo);


                if (BackgroundAudioPlayer.Instance.Track != null &&
                    BackgroundAudioPlayer.Instance.Track.Source.OriginalString.Contains(_memo.AudioFile))
                {
                    BackgroundAudioPlayer.Instance.Close();
                }

                await StorageHelper.DeleteFileAsync(_memo.AudioFile);

                if (!string.IsNullOrWhiteSpace(_memo.ImageFile))
                    await (StorageHelper.DeleteFileAsync(_memo.ImageFile));

                if (NavigationContext.QueryString.ContainsKey("SourceView") &&
                    NavigationContext.QueryString["SourceView"] == "Memo")
                {
                    NavigationService.RemoveBackEntry();
                }

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }

        public void AddImage(object arg)
        {
            _photoChooser.Show();
        }

        public void TagTap(object arg)
        {
            var tag = arg as Tag;

            if (tag != null)
            {
                if (!Tags.ToLower().Contains(tag.Name.ToLower()))
                {
                    var tags = Tags.Trim(new char[] { ',', ';', ':', ' ' });
                    tags += ", " + tag.Name;
                    tags = tags.Trim(new char[] { ',', ';', ':', ' ' });
                    Tags = tags;
                }
                else
                {
                    var tagList = Tags.ToLower()
                        .Split(new char[] { ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    tagList.RemoveAll(t => t.Trim().ToLower().Equals(tag.Name.Trim().ToLower()));

                    var tags = string.Join(", ", tagList.Select(t => t.Trim(new char[] { ',', ';', ':', ' ' })));
                    tags = tags.Trim(new char[] { ',', ';', ':', ' ' });
                    Tags = tags;
                }
            }

        }

        #endregion

    }
}
