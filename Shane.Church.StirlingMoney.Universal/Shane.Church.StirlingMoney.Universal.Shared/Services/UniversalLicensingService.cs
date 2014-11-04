using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalLicensingService : ILicensingService
    {
        public bool IsSyncLicensed()
        {
            return true;
        }

        public bool IsCSVLicensed()
        {
            return true;
        }

        public bool IsAdvancedReportingLicensed()
        {
            return true;
        }
    }
}
