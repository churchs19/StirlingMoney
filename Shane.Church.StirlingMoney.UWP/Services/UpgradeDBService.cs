using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.UWP.Services
{
    public class UpgradeDBService : IUpgradeDBService
    {
        public Task UpgradeDatabase(int dbVersion, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
