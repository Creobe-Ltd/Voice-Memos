using Creobe.VoiceMemos.ViewModels;

namespace Creobe.VoiceMemos.Locators
{
    public class ViewModelLocator
    {
        public MainViewModel Main
        {
            get
            {
                return new MainViewModel();
            }
        }

        public RecordViewModel Record
        {
            get
            {
                return new RecordViewModel();
            }
        }

        public AboutViewModel About
        {
            get
            {
                return new AboutViewModel();
            }
        }

        public MemoViewModel Memo
        {
            get
            {
                return new MemoViewModel();
            }
        }

        public EditViewModel Edit
        {
            get
            {
                return new EditViewModel();
            }
        }

        public ListViewModel List
        {
            get
            {
                return new ListViewModel();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                return new SettingsViewModel();
            }
        }

        public RecordingSettingsViewModel RecordingSettings
        {
            get
            {
                return new RecordingSettingsViewModel();
            }
        }

        public AccountSettingsViewModel AccountSettings
        {
            get
            {
                return new AccountSettingsViewModel();
            }
        }

        public ListMapViewModel ListMap
        {
            get
            {
                return new ListMapViewModel();
            }
        }

        public ShareViewModel Share
        {
            get
            {
                return new ShareViewModel();
            }
        }

        public SecuritySettingsViewModel SecuritySettings
        {
            get
            {
                return new SecuritySettingsViewModel();
            }
        }

        public PasswordViewModel Password
        {
            get
            {
                return new PasswordViewModel();
            }
        }

        public LoginViewModel Login
        {
            get
            {
                return new LoginViewModel();
            }
        }

        public StorageSettingsViewModel StorageSettings
        {
            get
            {
                return new StorageSettingsViewModel();
            }
        }

        public FavoritesViewModel Favorites
        {
            get
            {
                return new FavoritesViewModel();
            }
        }

        public GeneralSettingsViewModel GeneralSettings
        {
            get
            {
                return new GeneralSettingsViewModel();
            }
        }

        public SearchViewModel Search
        {
            get
            {
                return new SearchViewModel();
            }
        }
    }
}
