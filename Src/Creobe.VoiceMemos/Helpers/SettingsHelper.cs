using Creobe.VoiceMemos.Core.Media;
using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Creobe.VoiceMemos.Helpers
{
    public class SettingsHelper
    {
        private IsolatedStorageSettings _settings;

        public bool IsSettingsLoaded { get; private set; }

        private bool _addLocation;

        public bool AddLocation
        {
            get { return _addLocation; }
            set { _addLocation = value; }
        }

        private string _channels;

        public string Channels
        {
            get { return _channels; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _channels = value;
            }
        }

        private bool _confirmCancel;

        public bool ConfirmCancel
        {
            get { return _confirmCancel; }
            set { _confirmCancel = value; }
        }

        private bool _confirmDelete;

        public bool ConfirmDelete
        {
            get { return _confirmDelete; }
            set { _confirmDelete = value; }
        }

        private string _encodingFormat;

        public string EncodingFormat
        {
            get { return _encodingFormat; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _encodingFormat = value;
            }
        }

        private int _encodingQuality;

        public int EncodingQuality
        {
            get { return _encodingQuality; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _encodingQuality = value;
            }
        }

        private bool _editDetails;

        public bool EditDetails
        {
            get { return _editDetails; }
            set { _editDetails = value; }
        }

        private string _passwordHash;

        public string PasswordHash
        {
            get { return _passwordHash; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _passwordHash = value;
            }
        }

        private int _passwordLength;

        public int PasswordLength
        {
            get { return _passwordLength; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _passwordLength = value;
            }
        }

        private string _quality;

        public string Quality
        {
            get { return _quality; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _quality = value;
            }
        }

        private bool _recordInBackground;

        public bool RecordInBackground
        {
            get { return _recordInBackground; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _recordInBackground = value;
            }
        }

        private bool _recordUnderLock;

        public bool RecordUnderLock
        {
            get { return _recordUnderLock; }
            set { _recordUnderLock = value; }
        }

        private bool _requirePassword;

        public bool RequirePassword
        {
            get { return _requirePassword; }
            set
            {
                if (LicenseHelper.IsPremium)
                    _requirePassword = value;
            }
        }

        private bool _showUnplayedCount;

        public bool ShowUnplayedCount
        {
            get { return _showUnplayedCount; }
            set { _showUnplayedCount = value; }
        }

        private bool _skipSilence;

        public bool SkipSilence
        {
            get { return _skipSilence; }
            set { _skipSilence = value; }
        }

        private bool _useSystemPlayer;

        public bool UseSystemPlayer
        {
            get { return _useSystemPlayer; }
            set { _useSystemPlayer = value; }
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private string _preferredAccount;

        public string PreferredAccount
        {
            get { return _preferredAccount; }
            set { _preferredAccount = value; }
        }

        private int _launchCount;

        public int LaunchCount
        {
            get { return _launchCount; }
            set { _launchCount = value; }
        }

        private DateTime _lastLaunchDate;

        public DateTime LastLaunchDate
        {
            get { return _lastLaunchDate; }
            set { _lastLaunchDate = value; }
        }

        private bool _isAppRated;

        public bool IsAppRated
        {
            get { return _isAppRated; }
            set { _isAppRated = value; }
        }

        private bool _isDbMigrated;

        public bool IsDbMigrated
        {
            get { return _isDbMigrated; }
            set { _isDbMigrated = value; }
        }


        public SettingsHelper()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;

            _quality = RecordingQuality.Low.ToString();
            _channels = RecordingChannels.Mono.ToString();
            _encodingFormat = Creobe.VoiceMemos.Core.Media.EncodingFormat.Wave.ToString();
            _encodingQuality = 128;
            _skipSilence = false;
            _recordUnderLock = true;
            _recordInBackground = false;
            _addLocation = false;
            _requirePassword = false;
            _passwordHash = null;
            _passwordLength = 0;
            _editDetails = false;
            _confirmDelete = true;
            _showUnplayedCount = true;
            _useSystemPlayer = false;
            _confirmCancel = false;
            _version = "0.0.0.0";
            _preferredAccount = "SkyDrive";
            _launchCount = 0;
            _isAppRated = false;
            _isDbMigrated = false;
        }

        public async Task LoadSettingsAsync()
        {
            await Task.Run(() =>
            {
                if (LicenseHelper.IsPremium)
                {
                    GetValue<string>("Quality", ref _quality);
                    GetValue<string>("Channels", ref _channels);
                    GetValue<string>("EncodingFormat", ref _encodingFormat);
                    GetValue<int>("EncodingQuality", ref _encodingQuality);
                    GetValue<bool>("RequirePassword", ref _requirePassword);
                    GetValue<string>("PasswordHash", ref _passwordHash);
                    GetValue<int>("PasswordLength", ref _passwordLength);
                    GetValue<bool>("RecordInBackground", ref _recordInBackground);
                }

                GetValue<bool>("SkipSilence", ref _skipSilence);
                GetValue<bool>("RecordUnderLock", ref _recordUnderLock);
                GetValue<bool>("AddLocation", ref _addLocation);
                GetValue<bool>("EditDetails", ref _editDetails);
                GetValue<bool>("ConfirmDelete", ref _confirmDelete);
                GetValue<bool>("UseSystemPlayer", ref _useSystemPlayer);
                GetValue<bool>("ShowUnplayedCount", ref _showUnplayedCount);
                GetValue<bool>("ConfirmCancel", ref _confirmCancel);
                GetValue<string>("Version", ref _version);
                GetValue<string>("PreferredAccount", ref _preferredAccount);
                GetValue<int>("LaunchCount", ref _launchCount);
                GetValue<DateTime>("LastLaunchDate", ref _lastLaunchDate);
                GetValue<bool>("IsAppRated", ref _isAppRated);
                GetValue<bool>("IsDbMigrated", ref _isDbMigrated);

                try
                {
                    PhoneApplicationService.Current.ApplicationIdleDetectionMode =
                        _recordUnderLock ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;
                }
                catch
                {
                    // Do nothing
                }
            });
        }

        public void SaveSettings()
        {
            if (LicenseHelper.IsPremium)
            {
                SetValue("Quality", _quality);
                SetValue("Channels", _channels);
                SetValue("EncodingFormat", _encodingFormat);
                SetValue("EncodingQuality", _encodingQuality);
                SetValue("RequirePassword", _requirePassword);
                SetValue("PasswordHash", _passwordHash);
                SetValue("PasswordLength", _passwordLength);
                SetValue("RecordInBackground", _recordInBackground);
            }

            SetValue("SkipSilence", _skipSilence);
            SetValue("RecordUnderLock", _recordUnderLock);
            SetValue("AddLocation", _addLocation);
            SetValue("EditDetails", _editDetails);
            SetValue("ConfirmDelete", _confirmDelete);
            SetValue("UseSystemPlayer", _useSystemPlayer);
            SetValue("ShowUnplayedCount", _showUnplayedCount);
            SetValue("ConfirmCancel", _confirmCancel);
            SetValue("Version", _version);
            SetValue("PreferredAccount", _preferredAccount);
            SetValue("LaunchCount", _launchCount);
            SetValue("LastLaunchDate", _lastLaunchDate);
            SetValue("IsAppRated", _isAppRated);
            SetValue("IsDbMigrated", _isDbMigrated);

            try
            {
                PhoneApplicationService.Current.ApplicationIdleDetectionMode =
                    _recordUnderLock ? IdleDetectionMode.Disabled : IdleDetectionMode.Enabled;
            }
            catch
            {
                // Do nothing
            }

            _settings.Save();
        }

        private void SetValue(string key, object value)
        {
            if (_settings.Contains(key))
            {
                _settings[key] = value;
            }
            else
            {
                _settings.Add(key, value);
            }
        }

        private void GetValue<T>(string key, ref T property)
        {
            if (_settings.Contains(key))
            {
                property = (T)_settings[key];
            }
        }
    }
}
