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
#if !PERSONAL
using Shane.Church.StirlingMoney.Data;
using Shane.Church.StirlingMoney.Data.Update;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
//using NLog;
using Shane.Church.Utility;

namespace Shane.Church.StirlingMoney
{
	public partial class App : Application
	{
		private static MainViewModel viewModel = null;
//		private static Logger _logger;
		private static bool _loggedIn = false;
		private static AppSettings _settings = null;

		/// <summary>
		/// A static ViewModel used by the views to bind against.
		/// </summary>
		/// <returns>The MainViewModel object.</returns>
		public static MainViewModel ViewModel
		{
			get
			{
				// Delay creation of the view model until necessary
				if (viewModel == null)
					viewModel = new MainViewModel();

				return viewModel;
			}
		}

		public static bool LoggedIn
		{
			get { return _loggedIn; }
			set
			{
				if (_loggedIn != value)
					_loggedIn = value;
			}
		}

		public static AppSettings Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = new AppSettings();
				}
				return _settings;
			}
		}

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

			// Show graphics profiling information while debugging.
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// Display the current frame rate counters.
				//Application.Current.Host.Settings.EnableFrameRateCounter = true;

				// Show the areas of the app that are being redrawn in each frame.
				//Application.Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode, 
				// which shows areas of a page that are handed off GPU with a colored overlay.
				//Application.Current.Host.Settings.EnableCacheVisualization = true;

				// Disable the application idle detection by setting the UserIdleDetectionMode property of the
				// application's PhoneApplicationService object to Disabled.
				// Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
				// and consume battery power when the user is not using the phone.
				//PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
			}

			RootFrame.Navigating += new NavigatingCancelEventHandler(RootFrame_Navigating);

#if !PERSONAL
			// Create the database if it does not exist.
			bool v1dbExists = false;
			using (Data.v2.StirlingMoneyDataContext db = new Data.v2.StirlingMoneyDataContext(Data.v2.StirlingMoneyDataContext.DBConnectionString))
			{
				if (db.DatabaseExists() == false)
				{
					UpdateController.CreateContext(db);
				}
				else
				{
					UpdateController.UpdateContext(db);
				}
				using (Data.v1.StirlingMoneyDataContext v1db = new Data.v1.StirlingMoneyDataContext(Data.v1.StirlingMoneyDataContext.DBConnectionString))
				{
					if (v1db.DatabaseExists() == true)
					{
						v1dbExists = true;
						UpdateController.UpgradeV1(v1db, db);
					}
				}
			}
			if (v1dbExists)
			{
				UpdateController.DeleteV1();
			}
#endif

			// Create the ViewModel object.
			viewModel = new MainViewModel();

//			_logger = LogManager.GetCurrentClassLogger();
		}

		void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			if (Settings.UsePassword && !LoggedIn && !e.Uri.ToString().Contains("/Login.xaml"))
			{
				e.Cancel = true;
				RootFrame.Dispatcher.BeginInvoke(() =>
				{
					PhoneApplicationService.Current.State["RedirectUri"] = e.Uri;
					RootFrame.Navigate(new Uri("/Login.xaml", UriKind.Relative));
				});
			}
			else if (!LoggedIn)
			{
				LoggedIn = true;
			}
		}

		// Code to execute when the application is launching (eg, from Start)
		// This code will not execute when the application is reactivated
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
#if PERSONAL
			try
			{
				if (AgentManagement.AutoRestartAgent)
					AgentManagement.StartPeriodicAgent();
			}
			catch (AgentManagementException aex)
			{
				//_logger.DebugException("Error launching scheduled agent", aex);
			}
#endif
		}

		// Code to execute when the application is activated (brought to foreground)
		// This code will not execute when the application is first launched
		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
#if PERSONAL
			try
			{
				if (AgentManagement.AutoRestartAgent)
					AgentManagement.StartPeriodicAgent();
			}
			catch (AgentManagementException aex)
			{
//				_logger.DebugException("Error launching scheduled agent", aex);
			}
#endif
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
//			_logger.ErrorException("Navigation Failed", e.Exception);
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// A navigation has failed; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		// Code to execute on Unhandled Exceptions
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
//			_logger.ErrorException("Unhandled Exception", e.ExceptionObject);
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
			RootFrame = new PhoneApplicationFrame();
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