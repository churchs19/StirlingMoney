﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.Core.UWP.Services
{
    public class AgentManagementService : IAgentManagementService
    {
        public bool AreAgentsSupported
        {
            get
            {
                return false;
            }
        }

        public bool IsAgentEnabled
        {
            get
            {
//                throw new NotImplementedException();
                return false;
            }
        }

        public void RemoveAgent()
        {
//            throw new NotImplementedException();
        }

        public void StartAgent()
        {
//            throw new NotImplementedException();
        }
    }
}