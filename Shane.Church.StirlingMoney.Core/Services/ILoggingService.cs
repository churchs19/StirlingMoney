using System;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public interface ILoggingService
	{
		void LogMessage(string message);
		void LogException(Exception ex, string message = null);
	}
}
