using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;
#if PERSONAL
using Shane.Church.StirlingMoney.Data.Sync;
#endif
//using NLog;
using Shane.Church.Utility;
using Microsoft.Phone.Shell;
using Shane.Church.StirlingMoney.ViewModels;
using Shane.Church.StirlingMoney.Tiles;

namespace Shane.Church.StirlingMoney.Agent
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;
//		private static Logger _logger = LogManager.GetCurrentClassLogger();

		/// <remarks>
		/// ScheduledAgent constructor, initializes the UnhandledException handler
		/// </remarks>
		public ScheduledAgent()
		{
			if (!_classInitialized)
			{
				_classInitialized = true;
				// Subscribe to the managed exception handler
				Deployment.Current.Dispatcher.BeginInvoke(delegate
				{
					Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
				});
			}
		}

		/// Code to execute on Unhandled Exceptions
		private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			//_logger.ErrorException("Scheduled Agent Unhandled Exception", e.ExceptionObject);
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
//			_logger.Debug("Launching Scheduled Task");
			AgentExitReason reason = task.LastExitReason;
//			_logger.Debug("Agent Last Exited for Reason: " + reason.ToString());
#if DEBUG
			DebugUtility.DebugStartStopwatch();
			DebugUtility.DebugOutputMemoryUsage("Scheduled Task Initial Memory Snapshot");
#endif

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					ContextInstance.Context.CacheController.RefreshCompleted += new EventHandler<Microsoft.Synchronization.ClientServices.RefreshCompletedEventArgs>(CacheController_RefreshCompleted);
					ContextInstance.Context.CacheController.RefreshAsync();
				}
				catch (InvalidOperationException)
				{
					//The main application is open, so skip sync
					NotifyComplete();
				}
			});
		}

		void CacheController_RefreshCompleted(object sender, Microsoft.Synchronization.ClientServices.RefreshCompletedEventArgs e)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				if (e.Error != null)
				{
//					_logger.DebugException("Error synchronizing database", e.Error);
				}
				else
				{
					ContextInstance.Context.CacheController.RefreshCompleted -= CacheController_RefreshCompleted;
					AccountListViewModel model = new AccountListViewModel();
					model.LoadData();
					foreach (AccountViewModel a in model.Accounts)
					{
                        if(TileUtility.TileExists(a.AccountId))
						    TileUtility.AddOrUpdateAccountTile(a.AccountId, a.AccountName, a.AccountBalance);
					}
//					_logger.Debug("Database sync completed");
				}
#if DEBUG
				DebugUtility.DebugOutputElapsedTime("Scheduled Task Final Time Snapshot:");
				DebugUtility.DebugOutputMemoryUsage("Scheduled Task Final Memory Snapshot");
#endif
//				_logger.Debug("Completed Scheduled Task");
				NotifyComplete();
			});
		}
	}
}