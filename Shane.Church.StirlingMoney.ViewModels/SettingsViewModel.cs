using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Shane.Church.Utility;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		private AppSettings settings;

		public SettingsViewModel()
		{
			List<ListDataItem> aso = new List<ListDataItem>() { new ListDataItem() { Text = Resources.ViewModelResources.SettingsAccountSortAlpha, Value = 0 }, new ListDataItem() { Text = Resources.ViewModelResources.SettingsAccountSortMostFrequentlyUsed, Value = 1 } };
			_accountSortOptions = new ReadOnlyCollection<ListDataItem>(aso);

			settings = new AppSettings();
		}

		private ListDataItem _accountSort;
		public ListDataItem AccountSort
		{
			get { return _accountSort; }
			set
			{
				if (_accountSort != value)
				{
					_accountSort = value;
					NotifyPropertyChanged("AccountSort");
				}
			}
		}

		private bool _usePassword;
		public bool UsePassword
		{
			get { return _usePassword; }
			set
			{
				if (_usePassword != value)
				{
					_usePassword = value;
					NotifyPropertyChanged("UsePassword");
					NotifyPropertyChanged("PasswordVisibility");
				}
			}
		}

		public Visibility PasswordVisibility
		{
			get
			{
				if (UsePassword)
					return Visibility.Visible;
				else
					return Visibility.Collapsed;
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				if (_password != value)
				{
					_password = value;
					NotifyPropertyChanged("Password");
				}
			}
		}

		private string _confirmPassword;
		public string ConfirmPassword
		{
			get { return _confirmPassword; }
			set
			{
				if (_confirmPassword != value)
				{
					_confirmPassword = value;
					NotifyPropertyChanged("ConfirmPassword");
				}
			}
		}

		private ReadOnlyCollection<ListDataItem> _accountSortOptions;
		public ReadOnlyCollection<ListDataItem> AccountSortOptions
		{
			get { return _accountSortOptions; }
		}

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		public void LoadData()
		{
			AccountSort = (from a in AccountSortOptions
						   where (int)a.Value == settings.AccountSort
						   select a).FirstOrDefault();

			UsePassword = settings.UsePassword;
			Password = settings.Password;
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (UsePassword)
			{
				if (string.IsNullOrWhiteSpace(Password))
				{
					validationErrors.Add(Resources.ViewModelResources.SettingsPasswordRequiredError);
				}
				if (Password != settings.Password)
				{
					if (string.IsNullOrWhiteSpace(ConfirmPassword))
					{
						validationErrors.Add(Resources.ViewModelResources.SettingsConfirmPasswordRequiredError);
					}
					else if (Password != ConfirmPassword)
					{
						validationErrors.Add(Resources.ViewModelResources.SettingsPasswordMismatchError);
					}
				}
			}

			return validationErrors;
		}

		public void SaveSettings()
		{
			settings.AccountSort = (int)AccountSort.Value;
			settings.UsePassword = UsePassword;
			if (UsePassword)
				settings.Password = Password;
			else
				settings.Password = null;
		}
	}
}
