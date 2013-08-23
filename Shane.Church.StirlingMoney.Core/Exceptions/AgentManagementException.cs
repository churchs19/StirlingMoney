using System;

namespace Shane.Church.StirlingMoney.Core.Exceptions
{
    public class AgentManagementException : Exception
    {
        public AgentManagementException()
            : base()
        {

        }

        public AgentManagementException(string message)
            : base(message)
        {

        }

        public AgentManagementException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
