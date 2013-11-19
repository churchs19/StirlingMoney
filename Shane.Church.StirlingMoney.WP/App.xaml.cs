using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Ninject;
using Shane.Church.StirlingMoney.Core.Exceptions;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb;
using Shane.Church.StirlingMoney.Core.WP;
using Shane.Church.StirlingMoney.Core.WP7;
using Shane.Church.Utility.Core.WP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Wintellect.Sterling.Core;

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
			diagnostics.MessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.Diagnostics_MessageBox_Title;
			diagnostics.MessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.Diagnostics_MessageBox_Content;
			diagnostics.ExceptionOccurred += diagnostics_ExceptionOccurred;

			//Initializes this instance.
			diagnostics.Init();

			//Creates a new instance of the RadRateApplicationReminder component.
			rateReminder = new RadRateApplicationReminder();
			rateReminder.MessageBoxInfo.Title = Shane.Church.StirlingMoney.Strings.Resources.RateReminder_MessageBox_Title;
			rateReminder.MessageBoxInfo.Content = Shane.Church.StirlingMoney.Strings.Resources.RateReminder_MessageBox_Content;
			rateReminder.MessageBoxInfo.SkipFurtherRemindersMessage = Shane.Church.StirlingMoney.Strings.Resources.RateReminder_MessageBox_SkipFurtherRemindersMessage;

			//Sets how often the rate reminder is displayed.
			rateReminder.RecurrencePerUsageCount = 5;
			rateReminder.AllowUsersToSkipFurtherReminders = true;

			rateReminder.ReminderClosed += rateReminder_ReminderClosed;
		}

		void rateReminder_ReminderClosed(object sender, ReminderClosedEventArgs e)
		{
			if (e.MessageBoxEventArgs.Result == DialogResult.Cancel)
			{
				RadMessageBox.Show(buttonsContent: new List<object>() { Shane.Church.StirlingMoney.Strings.Resources.GiveFeedbackButton, Shane.Church.StirlingMoney.Strings.Resources.NoThanksButton },
					title: Shane.Church.StirlingMoney.Strings.Resources.FeedbackTitle,
					message: Shane.Church.StirlingMoney.Strings.Resources.FeedbackContent,
					closedHandler: (eArgs) =>
					{
						if (eArgs.ButtonIndex == 0)
						{
							EmailComposeTask emailTask = new EmailComposeTask();
							emailTask.To = "shane@s-church.net";
							emailTask.Subject = emailTask.Subject = Shane.Church.StirlingMoney.Strings.Resources.TechnicalSupportEmailSubject;
							emailTask.Show();
						}
					});
			}
		}

		void diagnostics_ExceptionOccurred(object sender, ExceptionOccurredEventArgs e)
		{
			if (e.Exception.StackTrace.Contains("Inneractive.Ad"))
			{
				e.Handled = true;
				e.Cancel = true;
			}
			if (e.Exception.Message.Equals("User has not granted the application consent to access data in Windows Live."))
			{
				e.Handled = true;
				e.Cancel = true;
			}
			if (e.Exception.Message.Equals("0x8000ffff"))
			{
				e.Handled = true;
				e.Cancel = true;
			}
		}

		// Initialize the app's font and flow direction as defined in its localized resource strings. 
		// 
		// To ensure that your apps font is aligned with its supported languages and that the 
		// FlowDirection for each of those languages follows its traditional direction, ResourceLanguage 
		// and ResourceFlowDirection should be initialized in each .resx file to match these values with that 
		// file's culture. For example: 
		// 
		// Resources.es-ES.resx 
		//    ResourceLanguage's value should be "es-ES" 
		//    ResourceFlowDirection's value should be "LeftToRight" 
		// 
		// Resources.ar-SA.resx 
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
				RootFrame.Language = XmlLanguage.GetLanguage(Shane.Church.StirlingMoney.Strings.Resources.ResourceLanguage);

				// Set the FlowDirection of all elements under the root frame based 
				// on the ResourceFlowDirection resource string for each 
				// supported language. 
				// 
				// If a compiler error occurs, ResourceFlowDirection is missing from 
				// the resource file. 
				FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), Shane.Church.StirlingMoney.Strings.Resources.ResourceFlowDirection, false);
				RootFrame.FlowDirection = flow;

				//Initialiaze Telerik Localization Manager
				Telerik.Windows.Controls.InputLocalizationManager.Instance.ResourceManager = Shane.Church.StirlingMoney.Strings.Resources.ResourceManager;
			}
			catch (Exception ex)
			{
				// If an exception is caught here it is most likely due to either 
				// ResourceLangauge not being correctly set to a supported language 
				// code or ResourceFlowDirection is set to a value other than LeftToRight 
				// or RightToLeft. 
				FlurryWP8SDK.Api.LogError("InitializeLanguage", ex);
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}

				throw;
			}
		}

		private void InitializeBackgroundAgent()
		{
			try
			{
				IAgentManagementService service = KernelService.Kernel.Get<IAgentManagementService>();
				service.StartAgent();
			}
			catch (AgentManagementException)
			{
				//Eat any Agent Management exceptions here.
			}
		}

		// Code to execute when the application is launching (eg, from Start)
		// This code will not execute when the application is reactivated
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
			//Before using any of the ApplicationBuildingBlocks, this class should be initialized with the version of the application.
			//Before using any of the ApplicationBuildingBlocks, this class should be initialized with the version of the application.
			var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			ApplicationUsageHelper.Init(versionAttrib.Version.ToString());
			FlurryWP8SDK.Api.StartSession(FlurryConfig.ApiKey);
			FlurryWP8SDK.Api.SetVersion(versionAttrib.Version.ToString());

			SterlingActivation.ActivateDatabase();

			InitializeBackgroundAgent();
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Application_Launching");
#endif
		}

		// Code to execute when the application is activated (brought to foreground)
		// This code will not execute when the application is first launched
		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
			//Before using any of the ApplicationBuildingBlocks, this class should be initialized with the version of the application.
			var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			FlurryWP8SDK.Api.StartSession(FlurryConfig.ApiKey);
			FlurryWP8SDK.Api.SetVersion(versionAttrib.Version.ToString());

			if (!e.IsApplicationInstancePreserved)
			{
				//This will ensure that the ApplicationUsageHelper is initialized again if the application has been in Tombstoned state.
				ApplicationUsageHelper.OnApplicationActivated();
			}

			SterlingActivation.ActivateDatabase();

#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Application_Activated");
#endif
		}

		// Code to execute when the application is deactivated (sent to background)
		// This code will not execute when the application is closing
		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
			// Ensure that required application state is persisted here.
			KernelService.Kernel.Get<SterlingEngine>().Dispose();
			FlurryWP8SDK.Api.EndSession();
		}

		// Code to execute when the application is closing (eg, user hit Back)
		// This code will not execute when the application is deactivated
		private void Application_Closing(object sender, ClosingEventArgs e)
		{
			KernelService.Kernel.Get<SterlingEngine>().Dispose();
			FlurryWP8SDK.Api.EndSession();
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
			//FlurryWP8SDK.Api.LogError("Application_UnhandledException", e.ExceptionObject);
			if (e.ExceptionObject.Message.Equals("0x8000ffff")) e.Handled = true;
			if (e.ExceptionObject.StackTrace.Contains("Inneractive.Ad")) e.Handled = true;
			if (e.ExceptionObject.Message.Equals("User has not granted the application consent to access data in Windows Live.")) e.Handled = true;
			if (System.Diagnostics.Debugger.IsAttached && !e.Handled)
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
