using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Services
{
	/// <summary>
	/// This is our settings
	/// servcie. To save and
	/// load settgins.
	/// </summary>
	public interface ISettingsService
	{
		/// <summary>
		/// Save a setting.
		/// </summary>
		/// <typeparam name="T">The type to save.</typeparam>
		/// <param name="value">The value of T to save.</param>
		/// <param name="key">The key under which to save the value.</param>
		/// <returns></returns>
		bool SaveSetting<T>(T value, string key);

		/// <summary>
		/// Load a setting.
		/// </summary>
		/// <typeparam name="T">The type to retrieve.</typeparam>
		/// <param name="key">The key under which to find the value.</param>
		/// <returns></returns>  
		T LoadSetting<T>(string key);

		/// <summary>
		/// Remove a settings entry 
		/// with a specific key.
		/// </summary>
		/// <param name="key">The key to pin the settings entry to delete.</param>
		/// <returns></returns>
		bool RemoveSetting(string key);
	}
}
