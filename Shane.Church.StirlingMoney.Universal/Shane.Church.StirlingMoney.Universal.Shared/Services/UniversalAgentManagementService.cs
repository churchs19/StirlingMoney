using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalAgentManagementService : IAgentManagementService
    {
        public void StartAgent()
        {
            //throw new NotImplementedException();
        }

        public void RemoveAgent()
        {
           //throw new NotImplementedException();
        }

        public bool IsAgentEnabled
        {
            get { return false; }
        }

        public bool AreAgentsSupported
        {
            get { return false; }
        }
    }
}
