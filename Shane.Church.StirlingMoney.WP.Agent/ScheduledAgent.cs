using Microsoft.Phone.Scheduler;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb;
using Shane.Church.StirlingMoney.Core.WP7;
using Shane.Church.Utility.Core.WP;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.WP.Agent
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;

		/// <remarks>
		/// ScheduledAgent constructor, initializes the UnhandledException handler
		/// </remarks>
		public ScheduledAgent()
		{
			if (!_classInitialized)
			{
				_classInitialized = true;

				NinjectBootstrapper.Bootstrap();

				// Subscribe to the managed exception handler
				Deployment.Current.Dispatcher.BeginInvoke(delegate
				{
					Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
#if DEBUG
					Application.Current.Exit += Current_Exit;
#endif
				});
			}
		}

#if DEBUG
		void Current_Exit(object sender, System.EventArgs e)
		{
			DebugUtility.DebugOutputMemoryUsage("Agent Exit");
		}
#endif

		/// Code to execute on Unhandled Exceptions
		private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			DebugUtility.SaveDiagnosticException(e.ExceptionObject);
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		/// <summary>
		/// Agent that runs a scheduled task
		/// </summary>
		/// <param name="task">
		/// The invoked task
		/// </param>
		/// <remarks>
		/// This method is called when a periodic or resource intensive task is invoked
		/// </remarks>
		protected override void OnInvoke(ScheduledTask task)
		{
#if DEBUG
			AgentExitReason reason = task.LastExitReason;
			DebugUtility.SaveDiagnostics(new Diagnostics() { DeviceUniqueId = DebugUtility.GetDeviceUniqueID(), AgentExitReason = reason.ToString() });
			Debug.WriteLine("Agent Last Exited for Reason: " + reason.ToString());
			DebugUtility.DebugStartStopwatch();
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Initial Memory Snapshot");
#endif
			var engine = KernelService.Kernel.Get<SterlingEngine>();
			SterlingDefaultLogger logger = new SterlingDefaultLogger(engine.SterlingDatabase, SterlingLogLevel.Verbose);

			engine.Activate();

			engine.SterlingDatabase.RegisterDatabase<StirlingMoneyDatabaseInstance>("Money", KernelService.Kernel.Get<ISterlingDriver>());

			engine.SterlingDatabase.GetDatabase("Money").RefreshAsync().Wait(1000);

			var settingsService = KernelService.Kernel.Get<ISettingsService>();

			if (settingsService.LoadSetting<bool>("EnableSync"))
			{
				var syncService = KernelService.Kernel.Get<SyncService>();
				syncService.Sync(true).Wait(15000);
				syncService = null;
				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
				System.GC.Collect();
			}

			var tileService = KernelService.Kernel.Get<ITileService<Account, Guid>>();
			var accountRepo = KernelService.Kernel.Get<IRepository<Account, Guid>>();

			var accountKeys = accountRepo.GetAllKeys().ToList();

			accountRepo = null;
			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();

			foreach (var k in accountKeys)
			{
				tileService.UpdateTile(k);
			}

			NotifyComplete();
#if DEBUG
			DebugUtility.DebugOutputElapsedTime("Scheduled Task Final Time Snapshot:");
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Final Memory Snapshot");
#endif

#if DEBUG_AGENT
			//			ScheduledActionService.LaunchForTest("StirlingBirthdayTileUpdateTask", TimeSpan.FromSeconds(60));
#endif
		}
	}
}