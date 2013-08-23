using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using Shane.Church.StirlingMoney.WP.Resources;
using System.Windows.Markup;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Shane.Church.StirlingMoney.WP
{
    public partial class App : Application
    {
		// Locale to force CurrentCulture to in InitializeLanguage(). 
		// Use "qps-PLOC" to deploy pseudolocalized strings. 
		// Use "" to let user Phone Language selection determine locale. 
		public static String appForceCulture = "";
		
		/// <summary>
        /// Component used to handle unhandle exceptions, to collect runtime info and to send email to developer.
        /// </summary>
		public RadDiagnostics diagnostics;
        /// <summary>
        /// Component used to raise a notification to the end users to rate the application on the marketplace.
        /// </summary>
        public RadRateApplicationReminder rateReminder;

        
		/// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

			// Language display initialization 
			InitializeLanguage();

			NinjectBootstrapper.Bootstrap();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

				// Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

			//Creates an instance of the Diagnostics component.
            diagnostics = new RadDiagnostics();

            //Defines the default email where the diagnostics info will be send.
            diagnostics.EmailTo = "shane@s-church.net";

            //Initializes this instance.
            diagnostics.Init();
        
		      //Creates a new instance of the RadRateApplicationReminder component.
            rateReminder = new RadRateApplicationReminder();

            //Sets how often the rate reminder is displayed.
            rateReminder.RecurrencePerUsageCount = 5;
			rateReminder.AllowUsersToSkipFurtherReminders = true;

			//if (LiveTileHelper.AreNewTilesSupported)
			//{
			//	var tile = ShellTile.ActiveTiles.First();
			//	var flipTileData = new RadFlipTileData()
			//	{
			//		Title = AppResources.AppTitle,
			//		SmallBackgroundImage = new Uri("/SmallApplicationIcon.png", UriKind.Relative),
			//		BackgroundImage = new Uri("/MediumApplicationIcon.png", UriKind.Relative),
			//		WideBackgroundImage = new Uri("/WideApplicationIcon.png", UriKind.Relative)
			//	};
			//	LiveTileHelper.UpdateTile(tile, flipTileData);
			//}
		}

		// Initialize the app's font and flow direction as defined in its localized resource strings. 
		// 
		// To ensure that your apps font is aligned with its supported languages and that the 
		// FlowDirection for each of those languages follows its traditional direction, ResourceLanguage 
		// and ResourceFlowDirection should be initialized in each .resx file to match these values with that 
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
				// Change locale to appForceCulture if it is not empty 
				if (String.IsNullOrWhiteSpace(appForceCulture) == false)
				{
					// Force app globalization to follow appForceCulture 
					Thread.CurrentThread.CurrentCulture = new CultureInfo(appForceCulture);

					// Force app UI culture to follow appForceCulture 
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(appForceCulture);
				}


				// Set the font to match the display language defined by the 
				// ResourceLanguage resource string for each supported language. 
				// 
				// Fall back to the font of the neutral language if the display 
				// language of the phone is not supported. 
				// 
				// If a compiler error occurs, ResourceLanguage is missing from 
				// the resource file. 
				RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

				// Set the FlowDirection of all elements under the root frame based 
				// on the ResourceFlowDirection resource string for each 
				// supported language. 
				// 
				// If a compiler error occurs, ResourceFlowDirection is missing from 
				// the resource file. 
				FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection, false);
				RootFrame.FlowDirection = flow;

				//Initialiaze Telerik Localization Manager
				Telerik.Windows.Controls.InputLocalizationManager.Instance.ResourceManager = Shane.Church.StirlingMoney.WP.Resources.AppResources.ResourceManager;
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
		
		// Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //Before using any of the ApplicationBuildingBlocks, this class should be initialized with the version of the application.
            ApplicationUsageHelper.Init("3.0");
        
		}

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                //This will ensure that the ApplicationUsageHelper is initialized again if the application has been in Tombstoned state.
                ApplicationUsageHelper.OnApplicationActivated();
            } 
 
	
            //// Ensure that application state is restored appropriately
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
			// Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
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
            RootFrame = new RadPhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
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

        #endregion
    }
}
