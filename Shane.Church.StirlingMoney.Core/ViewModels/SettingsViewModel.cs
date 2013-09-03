using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Properties;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class SettingsViewModel : ObservableObject
	{
		private ISettingsService _settings;

		public SettingsViewModel(ISettingsService settings)
		{
			if (settings == null) throw new ArgumentNullException("settings");
			_settings = settings;
			List<ListDataItem> aso = new List<ListDataItem>() { new ListDataItem() { Text = Resources.SettingsAccountSortAlpha, Value = 0 }, new ListDataItem() { Text = Resources.SettingsAccountSortMostFrequentlyUsed, Value = 1 } };
			_accountSortOptions = new ReadOnlyCollection<ListDataItem>(aso);
		}

		private ListDataItem _accountSort;
		public ListDataItem AccountSort
		{
			get { return _accountSort; }
			set
			{
				Set(() => AccountSort, ref _accountSort, value);
			}
		}

		private bool _usePassword;
		public bool UsePassword
		{
			get { return _usePassword; }
			set
			{
				Set(() => UsePassword, ref _usePassword, value);
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				Set(() => Password, ref _password, value);
			}
		}

		private string _confirmPassword;
		public string ConfirmPassword
		{
			get { return _confirmPassword; }
			set
			{
				Set(() => ConfirmPassword, ref _confirmPassword, value);
			}
		}

		private ReadOnlyCollection<ListDataItem> _accountSortOptions;
		public ReadOnlyCollection<ListDataItem> AccountSortOptions
		{
			get { return _accountSortOptions; }
		}

		public void LoadData()
		{
			AccountSort = (from a in AccountSortOptions
						   where (int)a.Value == _settings.LoadSetting<int>("AccountSort")
						   select a).FirstOrDefault();

			UsePassword = _settings.LoadSetting<bool>("UsePassword");
			Password = _settings.LoadSetting<string>("Password");
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (UsePassword)
			{
				if (string.IsNullOrWhiteSpace(Password))
				{
					validationErrors.Add(Resources.SettingsPasswordRequiredError);
				}
				if (Password != _settings.LoadSetting<string>("Password"))
				{
					if (string.IsNullOrWhiteSpace(ConfirmPassword))
					{
						validationErrors.Add(Resources.SettingsConfirmPasswordRequiredError);
					}
					else if (Password != ConfirmPassword)
					{
						validationErrors.Add(Resources.SettingsPasswordMismatchError);
					}
				}
			}

			return validationErrors;
		}

		public void SaveSettings()
		{
			_settings.SaveSetting<int>((int)AccountSort.Value, "AccountSort");
			_settings.SaveSetting<bool>(UsePassword, "UsePassword");
			if (UsePassword)
				_settings.SaveSetting<string>(Password, "Password");
			else
				_settings.RemoveSetting("Password");
		}
	}
}
