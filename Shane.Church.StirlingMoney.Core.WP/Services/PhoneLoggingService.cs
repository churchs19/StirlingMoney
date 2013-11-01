using Shane.Church.StirlingMoney.Core.Services;
using System;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class PhoneLoggingService : ILoggingService
	{
		public void LogMessage(string message)
		{
			FlurryWP8SDK.Api.LogEvent(message);
		}

		public void LogException(Exception ex, string message = null)
		{
			var msg = message;
			if (string.IsNullOrWhiteSpace(msg)) msg = "Stirling Money Exception";
			FlurryWP8SDK.Api.LogError(msg, ex);
		}
	}
}
