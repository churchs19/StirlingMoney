using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.Utility.Core.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class SettingsViewModel : ObservableObject, ICommittable
	{
		private ISettingsService _settings;
		private INavigationService _navService;
		private IRepository<AppSyncUser, string> _userRepository;
		private SyncService _syncService;

		public event ActionCompleteEventHandler AddActionCompleted;

		public SettingsViewModel(ISettingsService settings, INavigationService navService, IRepository<AppSyncUser, string> userRepository, SyncService syncService)
		{
			if (settings == null) throw new ArgumentNullException("settings");
			_settings = settings;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (userRepository == null) throw new ArgumentNullException("userRepository");
			_userRepository = userRepository;
			if (syncService == null) throw new ArgumentNullException("syncService");
			_syncService = syncService;
			List<ListDataItem> aso = new List<ListDataItem>() { new ListDataItem() { Text = Resources.SettingsAccountSortAlpha, Value = 0 }, new ListDataItem() { Text = Resources.SettingsAccountSortMostFrequentlyUsed, Value = 1 } };
			_accountSortOptions = new ReadOnlyCollection<ListDataItem>(aso);

			_authorizedUsers = new ObservableCollection<SettingsAppSyncUserViewModel>();
			_authorizedUsers.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => AuthorizedUsers);
			};

			SaveCommand = new RelayCommand(SaveSettings);
			AddEntryCommand = new AsyncRelayCommand(o => AddEntry());
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

		private bool _enableSync;
		public bool EnableSync
		{
			get { return _enableSync; }
			set
			{
				if (Set(() => EnableSync, ref _enableSync, value))
				{
					//TODO: Login or Logout from Azure Service if value changes
					if (EnableSync)
					{
						_syncService.Authenticate().ContinueWith((t) =>
						{
						});
					}
					else
					{
						SyncOnStartup = false;
						_syncService.Disconnect();
					}
				}
			}
		}

		private bool _syncOnStartup;
		public bool SyncOnStartup
		{
			get { return _syncOnStartup; }
			set
			{
				Set(() => SyncOnStartup, ref _syncOnStartup, value);
			}
		}

		private ReadOnlyCollection<ListDataItem> _accountSortOptions;
		public ReadOnlyCollection<ListDataItem> AccountSortOptions
		{
			get { return _accountSortOptions; }
		}

		private ObservableCollection<SettingsAppSyncUserViewModel> _authorizedUsers;
		public ObservableCollection<SettingsAppSyncUserViewModel> AuthorizedUsers
		{
			get { return _authorizedUsers; }
		}

		private string _newUserEmail;
		public string NewUserEmail
		{
			get { return _newUserEmail; }
			set
			{
				Set(() => NewUserEmail, ref _newUserEmail, value);
			}
		}

		public async Task LoadData()
		{
			AccountSort = (from a in AccountSortOptions
						   where (int)a.Value == _settings.LoadSetting<int>("AccountSort")
						   select a).FirstOrDefault();

			UsePassword = _settings.LoadSetting<bool>("UsePassword");
			Password = _settings.LoadSetting<string>("Password");
			EnableSync = _settings.LoadSetting<bool>("EnableSync");
			SyncOnStartup = _settings.LoadSetting<bool>("SyncOnStartup");

			var users = _userRepository.GetAllKeys();
			foreach (var u in users)
			{
				var authUser = KernelService.Kernel.Get<SettingsAppSyncUserViewModel>();
				await authUser.LoadEntry(u);
				AuthorizedUsers.Add(authUser);
			}
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

		public ICommand AddEntryCommand { get; protected set; }

		public bool IsNewEntryValid()
		{
			return !string.IsNullOrEmpty(NewUserEmail);
		}

		public async Task AddEntry()
		{
			if (IsNewEntryValid())
			{
				var newEntry = KernelService.Kernel.Get<AppSyncUser>();
				newEntry.UserEmail = this.NewUserEmail;
				newEntry = await _userRepository.AddOrUpdateEntryAsync(newEntry);
				var evm = KernelService.Kernel.Get<SettingsAppSyncUserViewModel>();
				evm.LoadEntry(newEntry);
				evm.RemoveActionCompleted += (sender, args) =>
				{
					var item = sender as SettingsAppSyncUserViewModel;
					if (item != null)
					{
						AuthorizedUsers.Remove(item);
					}
				};
				AuthorizedUsers.Add(evm);
				NewUserEmail = "";
				if (AddActionCompleted != null)
				{
					AddActionCompleted(this, new ValidationResultEventArgs());
				}
			}
			else
			{
				if (AddActionCompleted != null)
				{
					AddActionCompleted(this, new ValidationResultEventArgs(false));
				}
			}
		}


		public ICommand SaveCommand { get; private set; }

		public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
		public event ValidationFailedHandler ValidationFailed;

		public void SaveSettings()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				_settings.SaveSetting<int>((int)AccountSort.Value, "AccountSort");
				_settings.SaveSetting<bool>(UsePassword, "UsePassword");
				if (UsePassword)
					_settings.SaveSetting<string>(Password, "Password");
				else
					_settings.RemoveSetting("Password");
				_settings.SaveSetting<bool>(EnableSync, "EnableSync");
				_settings.SaveSetting<bool>(SyncOnStartup, "SyncOnStartup");

				if (_navService.CanGoBack)
					_navService.GoBack();
			}
			else
			{
				if (ValidationFailed != null)
					ValidationFailed(this, new ValidationFailedEventArgs(errors));
			}
		}

		public ICommand SyncFeedbackCommand { get; protected set; }

		public async Task Commit()
		{
			await _userRepository.Commit();
		}
	}
}
