using Creobe.VoiceMemos.Core.Commands;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Resources;
using System;
using System.Reflection;
using System.Windows.Input;

namespace Creobe.VoiceMemos.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        #region Commands

        public ICommand RateCommand { get; private set; }
        public ICommand ChangeLogCommand { get; private set; }
        public ICommand SupportCommand { get; private set; }
        public ICommand OtherAppsCommand { get; private set; }
#if BETA
        public ICommand ReportBugCommand { get; private set; }
#endif

        #endregion

        #region Properties

        public string VersionText
        {
            get 
            {
                string versionText = string.Format(AppResources.VersionText, GetVersion());
#if BETA
                versionText += " BETA";
#endif
                return versionText;
            }
        }

        private string _infoText;

        public string InfoText
        {
            get { return _infoText; }
            set 
            { 
                _infoText = value;
                NotifyPropertyChanged("InfoText");
            }
        }

        private bool _showRateButton;

        public bool ShowRateButton
        {
            get { return _showRateButton; }
            set 
            {
                _showRateButton = value;
                NotifyPropertyChanged("ShowRateButton");
            }
        }

        private bool _showSupportButton;

        public bool ShowSupportButton
        {
            get { return _showSupportButton; }
            set 
            { 
                _showSupportButton = value;
                NotifyPropertyChanged("ShowSupportButton");
            }
        }

        private bool _showChangeLogButton;

        public bool ShowChangeLogButton
        {
            get { return _showChangeLogButton; }
            set 
            { 
                _showChangeLogButton = value;
                NotifyPropertyChanged("ShowChangeLogButton");
            }
        }        

        private bool _showOtherAppsButton;

        public bool ShowOtherAppsButton
        {
            get { return _showOtherAppsButton; }
            set 
            { 
                _showOtherAppsButton = value;
                NotifyPropertyChanged("ShowOtherAppsButton");
            }
        }

        private bool _showReportBugButton;

        public bool ShowReportBugButton
        {
            get { return _showReportBugButton; }
            set
            {
                _showReportBugButton = value;
                NotifyPropertyChanged("ShowReportBugButton");
            }
        }


        public string CopyrightText
        {
            get { return string.Format(AppResources.CopyrightText, DateTime.Now.Year); }
        }
        
        
        #endregion

        #region Constructors

        public AboutViewModel()
        {
            InitializeCommands();

#if BETA
            _showReportBugButton = true;
            _showChangeLogButton = false;
            _showRateButton = false;
            _showSupportButton = false;
            _infoText = AppResources.AboutBetaText;
#else
            _showReportBugButton = false;
            _showChangeLogButton = true;
            _showRateButton = true;
            _showSupportButton = true;
            _infoText = AppResources.AboutReleaseText;
#endif

            _showOtherAppsButton = true;

        }

        #endregion

        #region Overrides

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            RateCommand = new DelegateCommand(Rate);
            ChangeLogCommand = new DelegateCommand(ChangeLog);
            SupportCommand = new DelegateCommand(Support);
            OtherAppsCommand = new DelegateCommand(OtherApps);
#if BETA
            ReportBugCommand = new DelegateCommand(ReportBug);
#endif

        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version != null)
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            
            return "";
        }

        #endregion

        #region Command Handlers

        public void Rate(object arg)
        {
            TaskHelper.RateApp();
        }

        public void ChangeLog(object arg)
        {
        }

        public void Support(object arg)
        {
            TaskHelper.ContactSupport();
        }

        public void OtherApps(object arg)
        {
            TaskHelper.FindOtherApps();
        }

#if BETA
        public void ReportBug(object arg)
        {
            TaskHelper.ReportBug();
        }
#endif

        #endregion

    }
}
