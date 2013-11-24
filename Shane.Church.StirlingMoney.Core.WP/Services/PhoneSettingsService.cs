using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class PhoneSettingsService : ISettingsService
	{
		IsolatedStorageSettings _settings;

		public PhoneSettingsService()
		{
			_settings = IsolatedStorageSettings.ApplicationSettings;
		}

		public string EncryptPassword(string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value.ToString());
			var encryptedBytes = ProtectedData.Protect(bytes, null);
			return Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
		}

		public string DecryptPassword(string value)
		{
			var bytes = Convert.FromBase64String(value);
			var decryptedBytes = ProtectedData.Unprotect(bytes, null);
			return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
		}

		public bool SaveSetting<T>(T value, string key)
		{
			bool valueChanged = false;

			var data = value;
			if (key.ToLower() == "password" && typeof(T) == typeof(string))
			{
				data = (T)Convert.ChangeType(EncryptPassword(value.ToString()), typeof(T), Thread.CurrentThread.CurrentUICulture);
			};

			// If the key exists
			if (_settings.Contains(key))
			{
				// If the value has changed
				if (_settings[key] is T)
				{
					T currentVal = (T)_settings[key];
					if (!currentVal.Equals(data))
					{
						// Store the new value
						_settings[key] = data;
						valueChanged = true;
					}
				}
				else
				{
					_settings[key] = data;
					valueChanged = true;
				}
			}
			// Otherwise create the key.
			else
			{
				_settings.Add(key, data);
				valueChanged = true;
			}
			if (valueChanged)
				_settings.Save();
			return valueChanged;
		}

		public T LoadSetting<T>(string key)
		{
			try
			{
				// If the key exists, retrieve the value.
				if (_settings.Contains(key))
				{
					if (key.ToLower() == "password" && typeof(T) == typeof(string))
					{
						var value = (T)_settings[key];
						return (T)Convert.ChangeType(DecryptPassword(value.ToString()), typeof(T), Thread.CurrentThread.CurrentUICulture);
					}
					else
					{
						return (T)_settings[key];
					}
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
			if (_settings.Contains(key))
			{
				_settings.Remove(key);
				_settings.Save();
			}
			return removed;
		}
	}
}
