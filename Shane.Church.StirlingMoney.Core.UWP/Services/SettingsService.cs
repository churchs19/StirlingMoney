using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.Core.UWP.Services
{
    public class SettingsService : ISettingsService
    {
        public T LoadSetting<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSetting(string key)
        {
            throw new NotImplementedException();
        }

        public bool SaveSetting<T>(T value, string key)
        {
            throw new NotImplementedException();
        }
    }
}
