using System;
using Microsoft.Phone.Info;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.IO;
//using NLog;
using System.Threading;

namespace Shane.Church.Utility
{
	public static class DebugUtility
	{
		private const long MaximumMilliseconds = 25000L;
		private static readonly Stopwatch _stopwatch = new Stopwatch();
//		private static Logger _logger = LogManager.GetCurrentClassLogger();
		private static Mutex mut = new Mutex(false, "Shane.Church.Utility.DebugUtility");

		public static void DebugOutputMemoryUsage(string label = null)
		{
#if DEBUG
			var limit = DeviceStatus.ApplicationMemoryUsageLimit;
			var current = DeviceStatus.ApplicationCurrentMemoryUsage;
			var remaining = limit - current;
			var peak = DeviceStatus.ApplicationPeakMemoryUsage;
			var safetyMargin = limit - peak;

			//if (label != null)
			//{
			//    _logger.Trace(label);
			//}
			//_logger.Debug(string.Format("Memory limit (bytes): " + limit));
			//_logger.Debug(string.Format("Current memory usage: {0} bytes ({1} bytes remaining)", current, remaining));
			//_logger.Debug(string.Format("Peak memory usage: {0} bytes ({1} bytes safety margin)", peak, safetyMargin));
#endif
		}

		public static void DebugStartStopwatch()
		{
#if DEBUG
			_stopwatch.Start();
#endif
		}
		public static void DebugOutputElapsedTime(string label = null)
		{
#if DEBUG
			var milliSeconds = _stopwatch.ElapsedMilliseconds;
			var remaining = MaximumMilliseconds - milliSeconds;

			//if (label != null)
			//{
			//    _logger.Trace(label);
			//}
			//_logger.Debug(string.Format("Running time: {0} milliseconds", milliSeconds));
			//_logger.Debug(string.Format("Remaining time (max): {0} milliseconds", remaining));
#endif
		}

		public static void LogMessage(string level, string message)
		{
			try
			{
				if (System.Diagnostics.Debugger.IsAttached)
				{
					Debug.WriteLine(string.Format("{0}: {1}", level, message));
				}
				if (mut.WaitOne(5000))
				{
					using (var store = IsolatedStorageFile.GetUserStoreForApplication())
					{
						using (var logStream = store.OpenFile("AppLog.txt", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
						{
							logStream.Seek(0, SeekOrigin.End);
							using (var writer = new StreamWriter(logStream))
							{
								writer.WriteLine(string.Format("{0}: {1}", level, message));
								writer.Flush();
							}
						}
					}
				}
			}
			catch { }
			finally
			{
				mut.ReleaseMutex();
			}
		}
	}
}
