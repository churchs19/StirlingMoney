using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Phone.Controls;
using System.Threading;
using System.Windows.Media.Imaging;
using Shane.Church.Utility;
using Microsoft.Phone.Tasks;
#if !PERSONAL
using Shane.Church.StirlingMoney.Data;
using Shane.Church.StirlingMoney.Data.v2;
using Shane.Church.StirlingMoney.Tiles;
#else
using Shane.Church.StirlingMoney.Data.Sync;
using Shane.Church.StirlingMoney.Tiles;
#endif


namespace Shane.Church.StirlingMoney
{
	public partial class MainPage : PhoneApplicationPage
	{
		ProgressIndicator _progress = null;
		private bool _isLoaded = false;

		public MainPage()
		{
			InitializeComponent();

			InitializeAdControl();

			_progress = new ProgressIndicator();
			_progress.IsVisible = true;
			_progress.IsIndeterminate = true;
			_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarText;
			SystemTray.SetProgressIndicator(this, _progress);

			DataContext = App.ViewModel;
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
#if !PERSONAL
			_isLoaded = true;
			BindData();
#else
			ContextInstance.Context.LoadCompleted += new EventHandler<Microsoft.Synchronization.ClientServices.IsolatedStorage.LoadCompletedEventArgs>(Context_LoadCompleted);
			ContextInstance.Context.LoadAsync();
#endif
		}

#if PERSONAL
		void Context_LoadCompleted(object sender, Microsoft.Synchronization.ClientServices.IsolatedStorage.LoadCompletedEventArgs e)
		{
			try
			{
				if (e.Exception != null)
				{
					Dispatcher.BeginInvoke(() =>
					{
						//this.loadErrorTextBlk.Text = e.Exception.Message;
						//this.loadErrorTextBlk.Visibility = System.Windows.Visibility.Visible;
						//MessageBox.Show(e.Exception.Message, "Data Syncronization Error", MessageBoxButton.OK);
						if (e.Exception.Message.Contains("NotFound"))
						{
							ToastMessage.Show(Shane.Church.StirlingMoney.Resources.AppResources.ToastSyncError, Shane.Church.StirlingMoney.Resources.AppResources.ToastServerUnavailable);
						}
						else
						{
							ToastMessage.Show(Shane.Church.StirlingMoney.Resources.AppResources.ToastSyncError, e.Exception.Message);
						}
						_isLoaded = true;
						BindData();
					});
				}
				else
				{
					Dispatcher.BeginInvoke(() =>
					{
						//this.popupTitle.Text = "Synchronizing..Please wait.";
						ContextInstance.Context.CacheController.RefreshCompleted += new EventHandler<Microsoft.Synchronization.ClientServices.RefreshCompletedEventArgs>(CacheController_RefreshCompleted);
						ContextInstance.Context.CacheController.RefreshAsync();
					});
					//this._cacheLoaded = true;
				}
			}
			catch(Exception ex)
			{
				Dispatcher.BeginInvoke(() =>
					{
						if (ex.Message.Contains("NotFound"))
						{
							ToastMessage.Show(Shane.Church.StirlingMoney.Resources.AppResources.ToastSyncError, Shane.Church.StirlingMoney.Resources.AppResources.ToastServerUnavailable);
						}
						else
						{
							ToastMessage.Show(Shane.Church.StirlingMoney.Resources.AppResources.ToastSyncError, ex.Message);
						}
						_isLoaded = true;
						BindData();
					});
			}
			finally
			{
				ContextInstance.Context.LoadCompleted -= Context_LoadCompleted;
			}
		}

		void CacheController_RefreshCompleted(object sender, Microsoft.Synchronization.ClientServices.RefreshCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Dispatcher.BeginInvoke(() =>
				{
					//this.loadErrorTextBlk.Text = e.Error.Message;
					//this.loadErrorTextBlk.Visibility = System.Windows.Visibility.Visible;
					if (e.Error.Message.Contains("NotFound"))
					{
						ToastMessage.Show(Shane.Church.StirlingMoney.Resources.AppResources.ToastSyncError, Shane.Church.StirlingMoney.Resources.AppResources.ToastServerUnavailable);
					}
					else
					{
						ToastMessage.Show(Shane.Church.StirlingMoney.Resources.AppResources.ToastSyncError, e.Error.Message);
					}
				});
			}
			else
			{
				ContextInstance.Context.CacheController.RefreshCompleted -= CacheController_RefreshCompleted;
//				ContextInstance.AddStats(e.Statistics, e.Error);
//				Dispatcher.BeginInvoke(() => this.popupPanel.IsOpen = false);
			}
			Dispatcher.BeginInvoke(() =>
			{
				_isLoaded = true;
				BindData();
			});
		}
#endif

		private void Tile_Click(object sender, RoutedEventArgs e)
		{
			if (((Tile)sender).Tag != null)
			{
				Guid? currentAccountId = ((Tile)sender).Tag as Guid?;
				PhoneApplicationService.Current.State["CurrentAccount"] = currentAccountId;
				NavigationService.Navigate(new Uri(@"/Transactions.xaml?AccountId=" + currentAccountId.Value.ToString(), UriKind.Relative));
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

			ApplicationBarMenuItem appBarMenuItemSettings = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Resources.AppResources.AppBarSettings);
			appBarMenuItemSettings.Click += new EventHandler(appBarMenuItemSettings_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemSettings);

			ApplicationBarMenuItem appBarMenuItemReview = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Resources.AppResources.AppBarMenuItemReview);
			appBarMenuItemReview.Click += new EventHandler(appBarMenuItemReview_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemReview);

			ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem(Shane.Church.StirlingMoney.Resources.AppResources.AppBarAbout);
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
					appBarIconButtonAddGoal.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarAdd;
					appBarIconButtonAddGoal.Click += new EventHandler(appBarIconButtonAddGoal_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddGoal);

					break;
				case ApplicationBarType.Budget:
					ApplicationBarIconButton appBarIconButtonAddBudget = new ApplicationBarIconButton(new Uri("/Images/Add.png", UriKind.Relative));
					appBarIconButtonAddBudget.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarAdd;
					appBarIconButtonAddBudget.Click += new EventHandler(appBarIconButtonAddBudget_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddBudget);
					break;
				case ApplicationBarType.Accounts:
				default:
					ApplicationBarIconButton appBarIconButtonAddAccount = new ApplicationBarIconButton(new Uri("/Images/AddAccount.png", UriKind.Relative));
					appBarIconButtonAddAccount.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarAddAccount;
					appBarIconButtonAddAccount.Click += new EventHandler(appBarIconButtonAddAccount_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddAccount);

					ApplicationBarIconButton appBarIconButtonCategories = new ApplicationBarIconButton(new Uri("/Images/Categories.png", UriKind.Relative));
					appBarIconButtonCategories.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarCategories;
					appBarIconButtonCategories.Click += new EventHandler(appBarIconButtonCategories_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonCategories);

#if !PERSONAL
					ApplicationBarIconButton appBarIconButtonBackup = new ApplicationBarIconButton(new Uri("/Images/BackupToCloud.png", UriKind.Relative));
					appBarIconButtonBackup.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarBackup;
					appBarIconButtonBackup.Click += new EventHandler(appBarIconButtonBackup_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonBackup);
#endif
					break;
			}

#if PERSONAL
			ApplicationBarIconButton appBarIconButtonSync = new ApplicationBarIconButton(new Uri("/Images/Synchronize.png", UriKind.Relative));
			appBarIconButtonSync.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarSync;
			appBarIconButtonSync.Click +=new EventHandler(appBarIconButtonSync_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonSync);
#endif

			ApplicationBarIconButton appBarIconButtonReports = new ApplicationBarIconButton(new Uri("/Images/Reports.png", UriKind.Relative));
			appBarIconButtonReports.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarReports;
			appBarIconButtonReports.Click += new EventHandler(appBarIconButtonReports_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonReports);
		}

#if PERSONAL
		void appBarIconButtonSync_Click(object sender, EventArgs e)
		{
			if (!ContextInstance.Context.CacheController.IsBusy)
			{
				_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarSyncText;
				_progress.IsVisible = true;
				ContextInstance.Context.CacheController.RefreshCompleted += new EventHandler<Microsoft.Synchronization.ClientServices.RefreshCompletedEventArgs>(CacheController_RefreshCompleted);
				ContextInstance.Context.CacheController.RefreshAsync();
			}
		}
#endif

		void appBarIconButtonReports_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(@"/Reports.xaml", UriKind.Relative));
		}

		void appBarMenuItemReview_Click(object sender, EventArgs e)
		{
			MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

			marketplaceReviewTask.Show();
		}

		void appBarMenuItemSettings_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(@"/Settings.xaml", UriKind.Relative));
		}

		private void appBarMenuItemAbout_Click(object sender, EventArgs e)
		{
			AboutPrompt prompt = new AboutPrompt();
			prompt.Show("Shane Church", null, "shane@s-church.net", "http://www.s-church.net");
		}

		void appBarIconButtonAddGoal_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentGoal"] = null;
			NavigationService.Navigate(new Uri(@"/AddEditGoal.xaml", UriKind.Relative));
		}

		void appBarIconButtonAddBudget_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentBudget"] = null;
			NavigationService.Navigate(new Uri(@"/AddEditBudget.xaml", UriKind.Relative));
		}

		private void appBarIconButtonAddAccount_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentAccount"] = null;
			NavigationService.Navigate(new Uri(@"/AddEditAccount.xaml", UriKind.Relative));
		}

		private void appBarIconButtonCategories_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(@"/Categories.xaml", UriKind.Relative));
		}

#if !PERSONAL
		private void appBarIconButtonBackup_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(@"/SkyDrive.xaml", UriKind.Relative));
		}
#endif
		#endregion

		#region Account Context Menu
		private void menuItemPinToStart_Click(object sender, RoutedEventArgs e)
		{
			Guid? accountId = ((sender as MenuItem).Tag) as Guid?;
			if (accountId.HasValue && accountId.Value != Guid.Empty)
			{
				// See whether the Tile is pinned, and if so, make sure the check box for it is checked.
				// (User may have deleted it manually.)

				if (!TileUtility.TileExists(accountId.Value))
				{
#if PERSONAL
					Account accountToPin = (from a in ContextInstance.Context.AccountCollection
											where a.AccountId == accountId.Value
											select a).FirstOrDefault();
#else
					using (StirlingMoneyDataContext context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						Account accountToPin = (from a in context.Accounts
												where a.AccountId == accountId.Value
												select a).FirstOrDefault();
#endif
						TileUtility.AddOrUpdateAccountTile(accountToPin.AccountId, accountToPin.AccountName, accountToPin.AccountBalance);
#if !PERSONAL
					}
#endif
				}
				else
				{
					TileUtility.DeleteTile(accountId.Value);
				}
				var accountViewModel = (from a in App.ViewModel.Accounts.Accounts
										where a.AccountId == accountId.Value
										select a).FirstOrDefault();
				if (accountViewModel != null)
				{
					accountViewModel.PinMenuText = "";
				}
			}
		}

		private void menuItemEdit_Click(object sender, RoutedEventArgs e)
		{
			Guid? accountId = ((sender as MenuItem).Tag) as Guid?;
			PhoneApplicationService.Current.State["CurrentAccount"] = accountId;
			NavigationService.Navigate(new Uri(@"/AddEditAccount.xaml", UriKind.Relative));
		}

		private void menuItemDelete_Click(object sender, RoutedEventArgs e)
		{
			Guid? accountId = ((sender as MenuItem).Tag) as Guid?;
			if (accountId.HasValue && accountId.Value != Guid.Empty)
			{
				if (MessageBox.Show(Shane.Church.StirlingMoney.Resources.AppResources.DeleteAccountWarning, Shane.Church.StirlingMoney.Resources.AppResources.DeleteAccountTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
					if (TileUtility.TileExists(accountId.Value))
						TileUtility.DeleteTile(accountId.Value);
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						var account = (from a in _context.Accounts
									   where a.AccountId == accountId.Value
									   select a).FirstOrDefault();
						var transactions = (from t in _context.Transactions
											where t.Account.AccountId == accountId.Value
											select t);
						var goals = (from g in _context.Goals
									 where g.Account.AccountId == accountId.Value
									 select g);
						_context.Goals.DeleteAllOnSubmit(goals);
						_context.Transactions.DeleteAllOnSubmit(transactions);
						_context.Accounts.DeleteOnSubmit(account);
						_context.SubmitChanges();
					}
#else
					var account = (from a in ContextInstance.Context.AccountCollection
								   where a.AccountId == accountId.Value
								   select a).FirstOrDefault();
					var transactions = (from t in ContextInstance.Context.TransactionCollection
										where t._accountId == accountId.Value
										select t);
					var goals = (from g in ContextInstance.Context.GoalCollection
								 where g._accountId == accountId.Value
								 select g);
					foreach (Goal g in goals)
					{
						ContextInstance.Context.DeleteGoal(g);
					}
					foreach (Transaction t in transactions)
					{
						ContextInstance.Context.DeleteTransaction(t);
					}
					ContextInstance.Context.DeleteAccount(account);
					ContextInstance.Context.SaveChanges();
#endif
					BindData();
				}
			}
		}
		#endregion

		#region Budget Context Menu
		private void menuItemBudgetPinToStart_Click(object sender, RoutedEventArgs e)
		{

		}

		private void menuItemBudgetEdit_Click(object sender, RoutedEventArgs e)
		{
			Guid? budgetId = ((sender as MenuItem).Tag) as Guid?;
			PhoneApplicationService.Current.State["CurrentBudget"] = budgetId;
			NavigationService.Navigate(new Uri(@"/AddEditBudget.xaml", UriKind.Relative));
		}

		private void menuItemBudgetDelete_Click(object sender, RoutedEventArgs e)
		{
			Guid? budgetId = ((sender as MenuItem).Tag) as Guid?;
			if (budgetId.HasValue && budgetId.Value != Guid.Empty)
			{
				if (MessageBox.Show(Shane.Church.StirlingMoney.Resources.AppResources.DeleteBudgetWarning, Shane.Church.StirlingMoney.Resources.AppResources.DeleteBudgetTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
					if (TileUtility.TileExists(budgetId.Value))
						TileUtility.DeleteTile(budgetId.Value);
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						var budget = (from b in _context.Budgets
									  where b.BudgetId == budgetId.Value
									  select b).FirstOrDefault();
						_context.Budgets.DeleteOnSubmit(budget);
						_context.SubmitChanges();
					}
#else
					var budget = (from b in ContextInstance.Context.BudgetCollection
								  where b.BudgetId == budgetId.Value
								  select b).FirstOrDefault();
					ContextInstance.Context.DeleteBudget(budget);
					ContextInstance.Context.SaveChanges();
#endif
					BindData();
				}
			}
		}
		#endregion

		#region Goal Context Menu
		private void menuItemGoalPinToStart_Click(object sender, RoutedEventArgs e)
		{

		}

		private void menuItemGoalEdit_Click(object sender, RoutedEventArgs e)
		{
			Guid? goalId = ((sender as MenuItem).Tag) as Guid?;
			PhoneApplicationService.Current.State["CurrentGoal"] = goalId;
			NavigationService.Navigate(new Uri(@"/AddEditGoal.xaml", UriKind.Relative));
		}

		private void menuItemGoalDelete_Click(object sender, RoutedEventArgs e)
		{
			Guid? goalId = ((sender as MenuItem).Tag) as Guid?;
			if (goalId.HasValue && goalId.Value != Guid.Empty)
			{
				if (MessageBox.Show(Shane.Church.StirlingMoney.Resources.AppResources.DeleteGoalWarning, Shane.Church.StirlingMoney.Resources.AppResources.DeleteGoalTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
					if (TileUtility.TileExists(goalId.Value))
						TileUtility.DeleteTile(goalId.Value);
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						var goal = (from g in _context.Goals
									where g.GoalId == goalId.Value
									select g).FirstOrDefault();
						_context.Goals.DeleteOnSubmit(goal);
						_context.SubmitChanges();
					}
#else
					var goal = (from g in ContextInstance.Context.GoalCollection
								where g.GoalId == goalId.Value
								select g).FirstOrDefault();
					ContextInstance.Context.DeleteGoal(goal);
					ContextInstance.Context.SaveChanges();
#endif
					BindData();
				}
			}
		}
		#endregion

		#region Ad Control
		private void InitializeAdControl()
		{
			if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
			{
				AdControl.ApplicationId = "test_client";
				AdControl.AdUnitId = "Image480_80";
			}
			else
			{
				AdControl.ApplicationId = "081af108-c899-401e-a44a-b54e303f12dc";
				AdControl.AdUnitId = "92173";
			}
#if PERSONAL
			AdControl.IsEnabled = false;
			AdControl.Height = 0;
#endif
		}

		private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
		{
			AdControl.Height = 0;
		}

		private void AdControl_AdRefreshed(object sender, EventArgs e)
		{
			AdControl.Height = 80;
		}
		#endregion

		private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PivotItem pi = e.AddedItems[0] as PivotItem;
			if (pi != null)
			{
				string header = pi.Header.ToString();
				if (header == Shane.Church.StirlingMoney.Resources.AppResources.AccountsTitle)
				{
					BuildApplicationBar(ApplicationBarType.Accounts);
				}
				else if (header == Shane.Church.StirlingMoney.Resources.AppResources.BudgetsTitle)
				{
					BuildApplicationBar(ApplicationBarType.Budget);
				}
				else if (header == Shane.Church.StirlingMoney.Resources.AppResources.GoalsTitle)
				{
					BuildApplicationBar(ApplicationBarType.Goals);
				}
			}
			if (_isLoaded)
			{
				BindData();
			}
		}

		private void BindData()
		{
			_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarText;
			_progress.IsVisible = true;
			PivotItem pi = PivotMain.SelectedItem as PivotItem;
			if (pi != null)
			{
				string header = pi.Header.ToString();
				if (header == Shane.Church.StirlingMoney.Resources.AppResources.AccountsTitle)
				{
					App.ViewModel.LoadData();
				}
				else if (header == Shane.Church.StirlingMoney.Resources.AppResources.BudgetsTitle)
				{
					App.ViewModel.LoadBudgets();
				}
				else if (header == Shane.Church.StirlingMoney.Resources.AppResources.GoalsTitle)
				{
					App.ViewModel.LoadGoals();
				}
			}
			_progress.IsVisible = false;
		}
	}
}