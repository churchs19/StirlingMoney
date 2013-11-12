using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.Utility.Core.WP;
using System;

namespace Shane.Church.StirlingMoney.Core.WP7.Agent.Services
{
	public class AgentLoggingService : ILoggingService
	{
		public void LogMessage(string message)
		{
			DebugUtility.SaveDiagnosticMessage(message);
		}

		public void LogException(Exception ex, string message = null)
		{
			if (!string.IsNullOrWhiteSpace(message))
				LogMessage(message);
			DebugUtility.SaveDiagnosticException(ex);
		}
	}
}
