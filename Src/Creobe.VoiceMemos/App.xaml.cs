using Creobe.VoiceMemos.Data;
using Creobe.VoiceMemos.Helpers;
using Creobe.VoiceMemos.Locators;
using Creobe.VoiceMemos.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Windows.Phone.Speech.VoiceCommands;
#if (DEBUG && BETA) || DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using System.IO;
using Windows.Storage;
#else
using Windows.ApplicationModel.Store;
#endif

namespace Creobe.VoiceMemos
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to view URIs.
        /// </summary>
        public static ViewLocator ViewLocator { get; private set; }

        /// <summary>
        /// Provides easy access to application settings.
        /// </summary>
        public static SettingsHelper SettingsHelper { get; private set; }

        /// <summary>
        /// Provides easy access to application's authentication state
        /// </summary>
        public static bool IsAuthenticated { get; set; }

        /// <summary>
        /// Provides easy access to application's running state
        /// </summary>
        public static bool RunningInBackground { get; set; }

        /// <summary>
        /// Provides easy access to entity repositories.
        /// </summary>
        public static VoiceMemosUow Uow { get; private set; }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Provides access to a GameTimer that is set up to pump the FrameworkDispatcher.
        /// </summary>
        public GameTimer FrameworkDispatcherTimer { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // XNA initialization
            //InitializeXnaApplication();

            // Language display initialization
            InitializeLanguage();

            // View locator initialization
            ViewLocator = new ViewLocator();

            // Database initialization
            InitializeDatabase();

            // Mock In-App purchase initialization
            InitializeMockIAP();

            // Settings initialization
            InitializeSettings();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private async void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // Voice command initialization
            await InitializeVoiceCommands();
#if !DEBUG && BETA
            FlurryWP8SDK.Api.StartSession("6KF2FBQPF2TYC75WQQ95");
#elif !DEBUG
            FlurryWP8SDK.Api.StartSession("DHRH6HDK2CV8F2RD3WJW");
#endif
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            RunningInBackground = false;

#if !DEBUG && BETA
            FlurryWP8SDK.Api.StartSession("6KF2FBQPF2TYC75WQQ95");
#elif !DEBUG
            FlurryWP8SDK.Api.StartSession("DHRH6HDK2CV8F2RD3WJW");
#endif
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is running in background (sent to background)
        // This code will only when the application is sent to background
        private void Application_RunningInBackground(object sender, RunningInBackgroundEventArgs e)
        {
            RunningInBackground = true;
        }


        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
#if !DEBUG
            FlurryWP8SDK.Api.LogError("Application_UnhandledException", e.ExceptionObject);
#endif


        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame(); //new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Handle navigation requests to check if login is required
            RootFrame.Navigating += CheckForLoginNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        private void CheckForLoginNavigation(object sender, NavigatingCancelEventArgs e)
        {
            if (!LicenseHelper.IsPremium)
            {
                IsAuthenticated = true;
                return;
            }

            if (SettingsHelper.RequirePassword && !IsAuthenticated &&
                e.NavigationMode == NavigationMode.New && !e.Uri.OriginalString.Contains("LoginView"))
            {
                e.Cancel = true;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var url = Uri.EscapeDataString(e.Uri.OriginalString);
                    RootFrame.Navigate(ViewLocator.View("Login", new { Url = url }));
                });
            }
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            if (e.NavigationMode == NavigationMode.New && !e.Uri.OriginalString.Contains("MainView"))
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        #region XNA application initialization

        // Performs initialization of the XNA types required for the application.
        private void InitializeXnaApplication()
        {
            // Create a GameTimer to pump the XNA FrameworkDispatcher
            FrameworkDispatcherTimer = new GameTimer();
            FrameworkDispatcherTimer.FrameAction += FrameworkDispatcherFrameAction;
            FrameworkDispatcherTimer.Start();
        }

        // An event handler that pumps the FrameworkDispatcher each frame.
        // FrameworkDispatcher is required for a lot of the XNA events and
        // for certain functionality such as SoundEffect playback.
        private void FrameworkDispatcherFrameAction(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
        }

        #endregion

        #region Database initialization

        private async void InitializeDatabase()
        {
            //string databasePath = StorageHelper.GetFilePath("db.sqlite");

            //VoiceMemos.Data.Legacy.VoiceMemosContext.Initialize("Data Source=isostore:/VoiceMemos.sdf");
            //Uow = new VoiceMemosUow("Data Source=isostore:/VoiceMemos.sdf");

            //if (!StorageHelper.FileExists("db.sqlite"))
            //{
            //    using (var db = new SQLiteConnection(databasePath))
            //    {
                    
            //    }
            //} 


            Uow = new VoiceMemosUow();

            await Uow.LoadCollectionsAsync();
        }

        #endregion

        #region Settings initialization

        private async void InitializeSettings()
        {
            SettingsHelper = new SettingsHelper();
            await SettingsHelper.LoadSettingsAsync();
        }

        #endregion

        #region Voice command initialization

        private async Task InitializeVoiceCommands()
        {
            try
            {
                await VoiceCommandService.InstallCommandSetsFromFileAsync(
                  new Uri("ms-appx:///VoiceCommandDefinition.xml"));
            }
            catch
            {
                // do nothing
            }
        }

        #endregion

        #region Mock In-App purchase Initialization

        private void InitializeMockIAP()
        {
#if (DEBUG && BETA) || DEBUG
            MockIAP.Init();
            MockIAP.RunInMockMode(true);
            MockIAP.SetListingInformation(1, "en-us", string.Empty, "0.99", "Voice Memos");

            // Add some more items manually.

            ProductListing p = new ProductListing
            {
                Name = "Voice Memos Premium",
                ImageUri = new Uri("/Assets/IAPImages/Premium.png", UriKind.Relative),
                ProductId = "VoiceMemosPremium",
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                FormattedPrice = "$0.99",
                Tag = string.Empty
            };

            MockIAP.AddProductListing("VoiceMemosPremium", p);

#endif
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

    }
}