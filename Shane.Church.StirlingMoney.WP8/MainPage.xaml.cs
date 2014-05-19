using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
using Shane.Church.Utility.Core.WP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class MainPage : AdvertisingPage
	{
		private MainViewModel _model;
		private ILoggingService _log;
		private ISettingsService _settings;
		private ILicensingService _license;
		private INavigationService _navService;

		private bool _refreshAccounts;
		private bool _refreshBudgets;
		private bool _refreshGoals;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			InitializeAdControl(this.AdPanel, this.AdControl);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var navContext = Newtonsoft.Json.Linq.JObject.FromObject(this.NavigationContext);

			TaskEx.Run(() => Initialize(e.NavigationMode, navContext));
		}

		protected async Task Initialize(System.Windows.Navigation.NavigationMode navMode, Newtonsoft.Json.Linq.JObject navContext)
		{
			FlurryWP8SDK.Api.LogPageView();

#if !PERSONAL
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				//Shows the trial reminder message, according to the settings of the TrialReminder.
				KernelService.Kernel.Get<RadTrialApplicationReminder>().Notify();
			});

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				//Shows the rate reminder message, according to the settings of the RateReminder.
				(App.Current as App).rateReminder.Notify();
			});
#endif

			_log = KernelService.Kernel.Get<ILoggingService>();
			_settings = KernelService.Kernel.Get<ISettingsService>();
			_license = KernelService.Kernel.Get<ILicensingService>();
			_navService = KernelService.Kernel.Get<INavigationService>();
			_model = KernelService.Kernel.Get<MainViewModel>();
			_model.BusyChanged += _model_BusyChanged;
			_model.SyncCompleted += _model_SyncCompleted;

			await _model.Initialize();

			try
			{
				if (PhoneNavigationService.DecodeNavigationParameter<bool>(navContext) && _navService.CanGoBack)
				{
					NavigationService.RemoveBackEntry();
				}
			}
			catch { }

			if (navMode == System.Windows.Navigation.NavigationMode.New && _settings.LoadSetting<bool>("EnableSync") && _settings.LoadSetting<bool>("SyncOnStartup"))
			{
				_model.SyncCommand.Execute(null);
			}
			else
			{
				_refreshAccounts = true;
				_refreshBudgets = true;
				_refreshGoals = true;

				LoadData();
			}

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				this.DataContext = _model;
				this.PivotMain.Visibility = System.Windows.Visibility.Visible;
			});
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
				_model.Commit().Wait(1000);
			base.OnNavigatedFrom(e);
		}

		async void _model_SyncCompleted()
		{
			await Deployment.Current.Dispatcher.InvokeAsync(() =>
			{
				_refreshAccounts = true;
				_refreshBudgets = true;
				_refreshGoals = true;

				LoadData();
			});
		}

		void _model_BusyChanged(object sender, Core.Data.BusyEventArgs args)
		{
			if (args.IsBusy)
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					LoadingBusy.Content = args.Message;
					LoadingBusy.AnimationStyle = (Telerik.Windows.Controls.AnimationStyle)args.AnimationType;
					LoadingBusy.IsRunning = true;
				});
				return;
			}
			else if (args.IsError)
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					LoadingBusy.IsRunning = false;
					_log.LogException(args.Error);
					MessageBox.Show(args.Error.Message, Strings.Resources.GeneralErrorCaption, MessageBoxButton.OK);
				});
			}
			else
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					if (!string.IsNullOrWhiteSpace(args.Message))
					{

					}
					LoadingBusy.IsRunning = false;
				});
			}
		}

		#region App Bar
		private enum ApplicationBarType
		{
			Accounts,
			Budget,
			Goals
		}

		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();

			ApplicationBarMenuItem appBarMenuItemSettings = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Strings.Resources.AppBarSettings);
			appBarMenuItemSettings.Click += new EventHandler(appBarMenuItemSettings_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemSettings);

			ApplicationBarMenuItem appBarMenuItemReview = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Strings.Resources.AppBarMenuItemReview);
			appBarMenuItemReview.Click += new EventHandler(appBarMenuItemReview_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemReview);

			ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Strings.Resources.AppBarAbout);
			appBarMenuItemAbout.Click += new EventHandler(appBarMenuItemAbout_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemAbout);
		}

		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void BuildApplicationBar(ApplicationBarType type = ApplicationBarType.Accounts)
		{
			if (ApplicationBar == null)
			{
				InitializeApplicationBar();
			}
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;
			ApplicationBar.Buttons.Clear();
			switch (type)
			{
				case ApplicationBarType.Goals:
					ApplicationBarIconButton appBarIconButtonAddGoal = new ApplicationBarIconButton(new Uri("/Images/Add.png", UriKind.Relative));
					appBarIconButtonAddGoal.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarAdd;
					appBarIconButtonAddGoal.Click += new EventHandler(appBarIconButtonAddGoal_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddGoal);

					break;
				case ApplicationBarType.Budget:
					ApplicationBarIconButton appBarIconButtonAddBudget = new ApplicationBarIconButton(new Uri("/Images/Add.png", UriKind.Relative));
					appBarIconButtonAddBudget.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarAdd;
					appBarIconButtonAddBudget.Click += new EventHandler(appBarIconButtonAddBudget_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddBudget);
					break;
				case ApplicationBarType.Accounts:
				default:
					ApplicationBarIconButton appBarIconButtonAddAccount = new ApplicationBarIconButton(new Uri("/Images/AddAccount.png", UriKind.Relative));
					appBarIconButtonAddAccount.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarAddAccount;
					appBarIconButtonAddAccount.Click += new EventHandler(appBarIconButtonAddAccount_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddAccount);

					ApplicationBarIconButton appBarIconButtonCategories = new ApplicationBarIconButton(new Uri("/Images/Categories.png", UriKind.Relative));
					appBarIconButtonCategories.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarCategories;
					appBarIconButtonCategories.Click += new EventHandler(appBarIconButtonCategories_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonCategories);
					break;
			}

			if (_settings.LoadSetting<bool>("EnableSync") && _license.IsSyncLicensed())
			{
				ApplicationBarIconButton appBarIconButtonSync = new ApplicationBarIconButton(new Uri("/Images/Synchronize.png", UriKind.Relative));
				appBarIconButtonSync.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarSync;
				appBarIconButtonSync.Click += new EventHandler(appBarIconButtonSync_Click);
				ApplicationBar.Buttons.Add(appBarIconButtonSync);

				if (!ApplicationBar.MenuItems.OfType<ApplicationBarMenuItem>().Where(it => it.Text == Shane.Church.StirlingMoney.Strings.Resources.BackupButton).Any())
				{
					ApplicationBarMenuItem appBarMenuItemBackup = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Strings.Resources.BackupButton);
					appBarMenuItemBackup.Click += new EventHandler(appBarIconButtonSkyDrive_Click);
					ApplicationBar.MenuItems.Insert(2, appBarMenuItemBackup);
				}
			}
			else
			{
				ApplicationBarIconButton appBarIconButtonSkyDrive = new ApplicationBarIconButton(new Uri("/Images/BackupToCloud.png", UriKind.Relative));
				appBarIconButtonSkyDrive.Text = Shane.Church.StirlingMoney.Strings.Resources.BackupButton;
				appBarIconButtonSkyDrive.Click += new EventHandler(appBarIconButtonSkyDrive_Click);
				ApplicationBar.Buttons.Add(appBarIconButtonSkyDrive);

				var backupMenuQuery = ApplicationBar.MenuItems.OfType<ApplicationBarMenuItem>().Where(it => it.Text == Shane.Church.StirlingMoney.Strings.Resources.BackupButton);
				if (backupMenuQuery.Any())
				{
					ApplicationBar.MenuItems.Remove(backupMenuQuery.First());
				}
			}

			ApplicationBarIconButton appBarIconButtonReports = new ApplicationBarIconButton(new Uri("/Images/Reports.png", UriKind.Relative));
			appBarIconButtonReports.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarReports;
			appBarIconButtonReports.Click += new EventHandler(appBarIconButtonReports_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonReports);
		}

		private void appBarIconButtonSkyDrive_Click(object sender, EventArgs e)
		{
			_model.BackupCommand.Execute(null);
		}

		void appBarIconButtonSync_Click(object sender, EventArgs e)
		{
			_model.SyncCommand.Execute(null);
		}

		void appBarIconButtonReports_Click(object sender, EventArgs e)
		{
			_model.ReportsCommand.Execute(null);
		}

		void appBarMenuItemReview_Click(object sender, EventArgs e)
		{
			_model.RateCommand.Execute(null);
		}

		void appBarMenuItemSettings_Click(object sender, EventArgs e)
		{
			_model.SettingsCommand.Execute(null);
		}

		private void appBarMenuItemAbout_Click(object sender, EventArgs e)
		{
			_model.AboutCommand.Execute(null);
		}

		void appBarIconButtonAddGoal_Click(object sender, EventArgs e)
		{
			_model.AddGoalCommand.Execute(null);
		}

		void appBarIconButtonAddBudget_Click(object sender, EventArgs e)
		{
			_model.AddBudgetCommand.Execute(null);
		}

		private void appBarIconButtonAddAccount_Click(object sender, EventArgs e)
		{
			_model.AddAccountCommand.Execute(null);
		}

		private void appBarIconButtonCategories_Click(object sender, EventArgs e)
		{
			_model.CategoriesCommand.Execute(null);
		}
		#endregion

		private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadData();
		}

		private void UpdateApplicationBar(string header)
		{
			if (header == Shane.Church.StirlingMoney.Strings.Resources.AccountsTitle)
			{
				BuildApplicationBar(ApplicationBarType.Accounts);
			}
			else if (header == Shane.Church.StirlingMoney.Strings.Resources.BudgetsTitle)
			{
				BuildApplicationBar(ApplicationBarType.Budget);
			}
			else if (header == Shane.Church.StirlingMoney.Strings.Resources.GoalsTitle)
			{
				BuildApplicationBar(ApplicationBarType.Goals);
			}
		}

		private void LoadData()
		{
			Deployment.Current.Dispatcher.BeginInvoke(async () =>
			{
				PivotItem pi = PivotMain.SelectedItem as PivotItem;
				string header = Shane.Church.StirlingMoney.Strings.Resources.AccountsTitle;
				if (pi != null)
				{
					header = pi.Header.ToString();
				}
				if (_model != null)
				{
					UpdateApplicationBar(header);
					if (header == Shane.Church.StirlingMoney.Strings.Resources.AccountsTitle)
					{
						await _model.LoadAccounts(_refreshAccounts);
						_refreshAccounts = false;
						Deployment.Current.Dispatcher.BeginInvoke(() =>
						{
							if (AccountPanel.Visibility == System.Windows.Visibility.Collapsed)
								AccountPanel.Visibility = System.Windows.Visibility.Visible;
						});
					}
					else if (header == Shane.Church.StirlingMoney.Strings.Resources.BudgetsTitle)
					{
						await _model.LoadBudgets(_refreshBudgets);
						_refreshBudgets = false;
						Deployment.Current.Dispatcher.BeginInvoke(() =>
						{
							if (BudgetPanel.Visibility == System.Windows.Visibility.Collapsed)
								BudgetPanel.Visibility = System.Windows.Visibility.Visible;
						});
					}
					else if (header == Shane.Church.StirlingMoney.Strings.Resources.GoalsTitle)
					{
						await _model.LoadGoals(_refreshGoals);
						_refreshGoals = false;
						Deployment.Current.Dispatcher.BeginInvoke(() =>
						{
							if (GoalsPanel.Visibility == System.Windows.Visibility.Collapsed)
								GoalsPanel.Visibility = System.Windows.Visibility.Visible;
						});
					}
#if DEBUG
					DebugUtility.DebugOutputMemoryUsage("MainPage_LoadData");
#endif
				}
			});
		}
	}
}
