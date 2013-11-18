using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
    public class PhoneSettingsService : ISettingsService
    {
        IRepository<Setting, string> _settings;

        public PhoneSettingsService(IRepository<Setting, string> settings)
        {
			if (settings == null) throw new ArgumentNullException("settings");
            _settings = settings;
        }

        public bool SaveSetting<T>(T value, string key)
        {
			bool valueChanged = false;

			if (_settings.GetAllKeys().Contains(key))
			{
				var entry = _settings.GetEntryAsync(key).Result;
				if (!(entry.Value is T) || !entry.Value.Equals(value))
				{
					entry.Value = value;
					_settings.AddOrUpdateEntryAsync(entry).Wait();
					valueChanged = true;
				}
			}
			else
			{
				Setting entry = new Setting() { Key = key, Value = value };
				_settings.AddOrUpdateEntryAsync(entry).Wait();
				valueChanged = true;
			}
			return valueChanged;
        }

        public T LoadSetting<T>(string key)
        {
            try
            {
                // If the key exists, retrieve the value.
                if (_settings.GetAllKeys().Contains(key))
                {
					return (T)_settings.GetEntryAsync(key).Result.Value;
                }
                // Otherwise, use the default value.
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }

        public bool RemoveSetting(string key)
        {
            var removed = false;
            if (_settings.GetAllKeys().Contains(key))
            {
				var entry = _settings.GetEntryAsync(key).Result;
				_settings.DeleteEntryAsync(entry, true).Wait();
				removed = true;
            }
            return removed;
        }
    }
}
