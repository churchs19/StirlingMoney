using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shane.Church.Utility
{
	public class AppSettings
	{
		// Our isolated storage settings
		IsolatedStorageSettings settings;

		// The isolated storage key names of our settings
		const string AccountSortSettingKeyName = "AccountSortSetting";
		const string UsePasswordSettingKeyName = "UsePassword";
		const string PasswordSettingKeyName = "Password";

		// The default value of our settings
		const int AccountSortSettingDefault = 0;
		const bool UsePasswordSettingDefault = false;
		const byte[] PasswordSettingDefault = null;

		/// <summary>
		/// Constructor that gets the application settings.
		/// </summary>
		public AppSettings()
		{
			// Get the settings for this application.
			settings = IsolatedStorageSettings.ApplicationSettings;
		}

		/// <summary>
		/// Update a setting value for our application. If the setting does not
		/// exist, then add the setting.
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool AddOrUpdateValue(string Key, Object value)
		{
			bool valueChanged = false;

			// If the key exists
			if (settings.Contains(Key))
			{
				// If the value has changed
				if (settings[Key] != value)
				{
					// Store the new value
					settings[Key] = value;
					valueChanged = true;
				}
			}
			// Otherwise create the key.
			else
			{
				settings.Add(Key, value);
				valueChanged = true;
			}
			return valueChanged;
		}

		/// <summary>
		/// Get the current value of the setting, or if it is not found, set the 
		/// setting to the default setting.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T GetValueOrDefault<T>(string Key, T defaultValue)
		{
			T value;

			// If the key exists, retrieve the value.
			if (settings.Contains(Key))
			{
				value = (T)settings[Key];
			}
			// Otherwise, use the default value.
			else
			{
				value = defaultValue;
			}
			return value;
		}

		/// <summary>
		/// Save the settings.
		/// </summary>
		public void Save()
		{
			settings.Save();
		}

		public int AccountSort
		{
			get
			{
				return GetValueOrDefault<int>(AccountSortSettingKeyName, AccountSortSettingDefault);
			}
			set
			{
				if (AddOrUpdateValue(AccountSortSettingKeyName, value))
				{
					Save();
				}
			}
		}

		public bool UsePassword
		{
			get
			{
				return GetValueOrDefault<bool>(UsePasswordSettingKeyName, UsePasswordSettingDefault);
			}
			set
			{
				if (AddOrUpdateValue(UsePasswordSettingKeyName, value))
				{
					Save();
				}
			}
		}

		public string Password
		{
			get
			{
				byte[] bytes = GetValueOrDefault<byte[]>(PasswordSettingKeyName, PasswordSettingDefault);
				if (bytes != null && bytes.Length > 0)
				{
					byte[] decrypted = 	ProtectedData.Unprotect(bytes, null);
					return Encoding.Unicode.GetString(decrypted, 0, decrypted.Length);
				}
				else
					return null;
			}
			set
			{
				bool success = false;
				if (!string.IsNullOrWhiteSpace(value))
					success = AddOrUpdateValue(PasswordSettingKeyName, ProtectedData.Protect(Encoding.Unicode.GetBytes(value), null));
				else
					success = AddOrUpdateValue(PasswordSettingKeyName, null);

				if (success)
				{
					Save();
				}
			}
		}	
	}
}
