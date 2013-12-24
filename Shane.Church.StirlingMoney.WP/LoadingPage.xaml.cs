using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.SterlingDb;
using Shane.Church.StirlingMoney.Core.Services;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Shane.Church.StirlingMoney.Core.ViewModels;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class LoadingPage : PhoneApplicationPage
	{
		private ILoggingService _log;
		private ISettingsService _settings;
		private INavigationService _navService;
		private SyncService _syncService;

		public LoadingPage()
		{
			InitializeComponent();
		}

		private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			SterlingActivation.ActivateDatabase();

			_log = KernelService.Kernel.Get<ILoggingService>();
			_settings = KernelService.Kernel.Get<ISettingsService>();
			_navService = KernelService.Kernel.Get<INavigationService>();
			_syncService = KernelService.Kernel.Get<SyncService>();

			await Initialize();

			if (_settings.LoadSetting<bool>("EnableSync") && _settings.LoadSetting<bool>("SyncOnStartup"))
			{
				await _syncService.Sync(true);
			}

			_navService.Navigate<MainViewModel>(true);
		}

		public async Task Initialize()
		{
			try
			{
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					var dbVersion = 3;
					var dbFile = "";
					if (store.FileExists(StirlingMoney.Data.v2.StirlingMoneyDataContext.DBFileName))
					{
						dbVersion = 2;
						dbFile = StirlingMoney.Data.v2.StirlingMoneyDataContext.DBFileName;
					}
					else if (store.FileExists(StirlingMoney.Data.v1.StirlingMoneyDataContext.DBFileName))
					{
						dbVersion = 1;
						dbFile = StirlingMoney.Data.v1.StirlingMoneyDataContext.DBFileName;
					}

					if (dbVersion != 3)
					{
						IUpgradeDBService upgrade = KernelService.Kernel.Get<IUpgradeDBService>();

						await upgrade.UpgradeDatabase(dbVersion, dbFile);
					}
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex);
			}
		}

	}
}