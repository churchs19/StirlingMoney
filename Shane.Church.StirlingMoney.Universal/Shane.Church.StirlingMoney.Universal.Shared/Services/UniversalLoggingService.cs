using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalLoggingService : ILoggingService
    {
        public void LogMessage(string message)
        {
            MarkedUp.AnalyticClient.Info(message);
        }

        public void LogException(Exception ex, string message = null)
        {
            if (message == null)
            {
                MarkedUp.AnalyticClient.Error(ex.Message, ex);
            }
            else
            {
                MarkedUp.AnalyticClient.Error(message, ex);
            }
        }
    }
}
