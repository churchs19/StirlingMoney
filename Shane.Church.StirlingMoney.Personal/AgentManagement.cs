using System;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Info;
//using NLog;

namespace Shane.Church.StirlingMoney
{
	public static class AgentManagement
	{
//		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private const string ScheduledTaskName = "StirlingMoneyAgent";

		public static void StartPeriodicAgent()
		{
#if PERSONAL
			// Obtain a reference to the period task, if one exists
			var periodicTask = ScheduledActionService.Find(ScheduledTaskName) as PeriodicTask;

			// If the task already exists and background agents are enabled for the
			// application, you must remove the task and then add it again to update 
			// the schedule
			if (periodicTask != null)
			{
				AgentExitReason reason = periodicTask.LastExitReason;
//				_logger.Debug("Agent Last Exited for Reason: " + reason.ToString());
				RemoveAgent();
			}

			periodicTask = new PeriodicTask(ScheduledTaskName);

			// The description is required for periodic agents. This is the string that the user
			// will see in the background services Settings page on the device.
			periodicTask.Description = "This task is unused in the current version.";
			periodicTask.ExpirationTime = DateTime.Now.AddDays(14);

			// Place the call to Add in a try block in case the user has disabled agents.
			try
			{
				ScheduledActionService.Add(periodicTask);
				// If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
				ScheduledActionService.LaunchForTest(ScheduledTaskName, TimeSpan.FromSeconds(30));
#endif
			}
			catch (InvalidOperationException exception)
			{
				if (exception.Message.Contains("BNS Error: The action is disabled"))
				{
					throw new AgentManagementException("Background agents for this application have been disabled by the user.");
				}
				if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
				{
					if (IsLowMemDevice)
					{
						throw new AgentManagementException("This device does not support background agents, so this feature will be disabled.");
					}
					else
					{
						throw new AgentManagementException("Too many background agents have been enabled. Please go to the settings application to disable other agents before enabling Stirling Money to update.");
					}
				}
			}
#endif
		}

		public static void RemoveAgent()
		{
#if PERSONAL
			try
			{
				ScheduledActionService.Remove(ScheduledTaskName);
			}
			catch { }
#endif
		}

		public static bool IsAgentEnabled
		{
			get
			{
				var periodicTask = ScheduledActionService.Find(ScheduledTaskName) as PeriodicTask;
				return periodicTask != null;
			}
		}

		private static bool IsLowMemDevice
		{
			get
			{
				try
				{
					// check the working set limit    
					var result = (Int64)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
					return result < 94371840L;
				}
				catch (ArgumentOutOfRangeException)
				{
					// OS does not support this call => indicates a 512 MB device   
					return false;
				}
			}
		}

		public static bool AutoRestartAgent
		{
			get
			{
#if PERSONAL
				return true;
#else
				return false;
#endif
			}
		}
	}
}
