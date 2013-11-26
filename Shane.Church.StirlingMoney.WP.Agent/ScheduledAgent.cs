﻿using Microsoft.Phone.Scheduler;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Exceptions;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb;
#if WP8
using Shane.Church.StirlingMoney.Core.WP8;
#else
using Shane.Church.StirlingMoney.Core.WP7;
#endif
using Shane.Church.Utility.Core.WP;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Shane.Church.StirlingMoney.Core.WP.Services;

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
			try
			{
#if DEBUG
				AgentExitReason reason = task.LastExitReason;
				DebugUtility.SaveDiagnostics(new Diagnostics() { DeviceUniqueId = DebugUtility.GetDeviceUniqueID(), AgentExitReason = reason.ToString() });
				Debug.WriteLine("Agent Last Exited for Reason: " + reason.ToString());
				DebugUtility.DebugStartStopwatch();
				DebugUtility.DebugOutputMemoryUsage("Scheduled Task Initial Memory Snapshot");
#endif
				var settingsService = new PhoneSettingsService();

				SterlingActivation.ActivateDatabase();

				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
				System.GC.Collect();
#if DEBUG
				DebugUtility.DebugOutputMemoryUsage("After Database Activation");
#endif
				var sync = settingsService.LoadSetting<bool>("EnableSync");

				settingsService = null;

				System.GC.Collect();
				System.GC.WaitForPendingFinalizers();
				System.GC.Collect();

				if (sync)
				{
					var syncService = KernelService.Kernel.Get<SyncService>();
					try
					{
						syncService.Sync(true).Wait(15000);
					}
					catch (NotAuthorizedException naex)
					{
						//Don't sync if the user doesn't have a subscription but no need to throw an exception in the agent
#if DEBUG
						DebugUtility.SaveDiagnosticException(naex);
#endif
					}
					finally
					{
						syncService = null;
						System.GC.Collect();
						System.GC.WaitForPendingFinalizers();
						System.GC.Collect();
					}
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
			}
			catch (Exception ex)
			{
#if DEBUG
				DebugUtility.SaveDiagnosticException(ex);
#endif
			}

#if DEBUG
			DebugUtility.SaveDiagnosticMessage("Agent Completed Successfully");
			DebugUtility.DebugOutputElapsedTime("Scheduled Task Final Time Snapshot:");
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Final Memory Snapshot");
#endif
#if DEBUG_AGENT
			//			ScheduledActionService.LaunchForTest("StirlingBirthdayTileUpdateTask", TimeSpan.FromSeconds(60));
#endif
			NotifyComplete();
		}
	}
}