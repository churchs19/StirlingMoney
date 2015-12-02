using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.Core.UWP.Services
{
    public class LicensingService : ILicensingService
    {
        public bool IsAdvancedReportingLicensed()
        {
            return true;
        }

        public bool IsCSVLicensed()
        {
            return true;
        }

        public bool IsSyncLicensed()
        {
            return true;
        }
    }
}
