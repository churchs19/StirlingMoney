using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Phone.Controls;
using Shane.Church.Utility;
using Shane.Church.StirlingMoney.Tiles;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif

namespace Shane.Church.StirlingMoney
{
	public partial class Transactions : PhoneApplicationPage
	{
		Guid? currentAccountId = null;
		ProgressIndicator _progress = null;

		public Transactions()
		{
			InitializeComponent();

			InitializeAdControl();

			BuildApplicationBar();

			_progress = new ProgressIndicator();
			_progress.IsVisible = false;
			_progress.IsIndeterminate = true;
			SystemTray.SetProgressIndicator(this, _progress);

			Visibility darkBackgroundVisibility = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"];
			if (darkBackgroundVisibility != Visibility.Visible)
			{
				textBoxSearch.ActionIcon = new BitmapImage(new Uri("/Images/Cancel.light.png", UriKind.Relative));
			}
		}

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

		#region Navigation
		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			try
			{
				string currentAccount, isPinLink;
				try
				{
					currentAccount = NavigationContext.QueryString["AccountId"];
				}
				catch { currentAccount = null; }
				try
				{
					isPinLink = NavigationContext.QueryString["PinnedTile"];
				}
				catch { isPinLink = null; }
				if (currentAccount != null)
				{
					PhoneApplicationService.Current.State["CurrentAccount"] = Guid.Parse(currentAccount);
				}
				currentAccountId = PhoneApplicationService.Current.State["CurrentAccount"] as Guid?;
				if (isPinLink != null)
				{
					_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarSyncText;
					_progress.IsVisible = true;
#if !PERSONAL
					RefreshData();
#else
					ContextInstance.Context.CacheController.RefreshCompleted += new EventHandler<Microsoft.Synchronization.ClientServices.RefreshCompletedEventArgs>(CacheController_RefreshCompleted);
					ContextInstance.Context.CacheController.RefreshAsync();
#endif
					return;
				}
				else if (currentAccountId.HasValue)
				{
					RefreshData();
				}
			}
			catch/* (Exception ex)*/
			{

			}
		}

#if PERSONAL
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
				if (currentAccountId.HasValue)
				{
					RefreshData();
				}
				else
				{
					_progress.IsVisible = false;
				}
			});
		}
#endif

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			if (currentAccountId.HasValue && TileUtility.TileExists(currentAccountId.Value))
			{
				AccountViewModel model = new AccountViewModel();
				model.LoadData(this.currentAccountId.Value);
				TileUtility.AddOrUpdateAccountTile(model.AccountId, model.AccountName, model.AccountBalance);
			}
			base.OnNavigatedFrom(e);
		}

		private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (App.ViewModel.Transactions.IsSearchVisible)
			{
				ClearSearch();
				e.Cancel = true;
			}
			else if (NavigationService.CanGoBack)
			{
				App.ViewModel.LoadData();
			}
		}
		#endregion

		#region App Bar
		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void BuildApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;

			ApplicationBarIconButton ApplicationBarWithdraw = new ApplicationBarIconButton(new Uri("/Images/Withdraw.png", UriKind.Relative));
			ApplicationBarWithdraw.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarWithdraw;
			ApplicationBarWithdraw.Click +=new EventHandler(ApplicationBarWithdraw_Click);
			ApplicationBar.Buttons.Add(ApplicationBarWithdraw);

			ApplicationBarIconButton ApplicationBarWriteCheck = new ApplicationBarIconButton(new Uri("/Images/WriteCheck.png", UriKind.Relative));
			ApplicationBarWriteCheck.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarWriteCheck;
			ApplicationBarWriteCheck.Click += new EventHandler(ApplicationBarWriteCheck_Click);
			ApplicationBar.Buttons.Add(ApplicationBarWriteCheck);

			ApplicationBarIconButton ApplicationBarDeposit = new ApplicationBarIconButton(new Uri("/Images/Deposit.png", UriKind.Relative));
			ApplicationBarDeposit.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarDeposit;
			ApplicationBarDeposit.Click += new EventHandler(ApplicationBarDeposit_Click);
			ApplicationBar.Buttons.Add(ApplicationBarDeposit);

			ApplicationBarIconButton ApplicationBarTransfer = new ApplicationBarIconButton(new Uri("/Images/Transfer.png", UriKind.Relative));
			ApplicationBarTransfer.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarTransfer;
			ApplicationBarTransfer.Click += new EventHandler(ApplicationBarTransfer_Click);
			ApplicationBar.Buttons.Add(ApplicationBarTransfer);
		}

		private void ApplicationBarWithdraw_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentTransaction"] = null;
			PhoneApplicationService.Current.State["CurrentTransactionType"] = TransactionType.Withdrawal;
			NavigationService.Navigate(new Uri(@"/AddEditTransaction.xaml", UriKind.Relative));
		}

		private void ApplicationBarWriteCheck_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentTransaction"] = null;
			PhoneApplicationService.Current.State["CurrentTransactionType"] = TransactionType.Check;
			NavigationService.Navigate(new Uri(@"/AddEditTransaction.xaml", UriKind.Relative));
		}

		private void ApplicationBarDeposit_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentTransaction"] = null;
			PhoneApplicationService.Current.State["CurrentTransactionType"] = TransactionType.Deposit;
			NavigationService.Navigate(new Uri(@"/AddEditTransaction.xaml", UriKind.Relative));
		}

		private void ApplicationBarTransfer_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentTransaction"] = null;
			PhoneApplicationService.Current.State["CurrentTransactionType"] = TransactionType.Transfer;
			NavigationService.Navigate(new Uri(@"/AddEditTransaction.xaml", UriKind.Relative));
		}
		#endregion

		#region Context Menu
		private void menuItemEdit_Click(object sender, RoutedEventArgs e)
		{
			Guid? transactionId = ((sender as MenuItem).Tag) as Guid?;
			PhoneApplicationService.Current.State["CurrentTransaction"] = transactionId;
			PhoneApplicationService.Current.State["CurrentTransactionType"] = TransactionType.Unknown;
			NavigationService.Navigate(new Uri(@"/AddEditTransaction.xaml", UriKind.Relative));
		}

		private void menuItemDelete_Click(object sender, RoutedEventArgs e)
		{
			Guid? transactionId = ((sender as MenuItem).Tag) as Guid?;
			if (transactionId.HasValue)
			{
				if (MessageBox.Show(Shane.Church.StirlingMoney.Resources.AppResources.DeleteTransactionWarning, Shane.Church.StirlingMoney.Resources.AppResources.DeleteTransactionTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
				{
#if !PERSONAL
					using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
					{
						var transaction = (from t in _context.Transactions
										   where t.TransactionId == transactionId.Value
										   select t).FirstOrDefault();
						_context.Transactions.DeleteOnSubmit(transaction);
						_context.SubmitChanges();
					}
					
#else
					var transaction = (from t in ContextInstance.Context.TransactionCollection
									   where t.TransactionId == transactionId.Value
									   select t).FirstOrDefault();
					ContextInstance.Context.DeleteTransaction(transaction);
					ContextInstance.Context.SaveChanges();
#endif
					RefreshData();
				}
			}
		}
		#endregion

		#region Helper Functions
		private void RefreshData()
		{
			try
			{
				_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarText;
				_progress.IsVisible = true;

				App.ViewModel.Transactions.LoadData(currentAccountId.Value);
				App.ViewModel.Transactions.LoadDataCompleted += new ViewModels.TransactionListViewModel.LoadDataComplete(Transactions_LoadDataCompleted);
			}
			catch/* (Exception ex) */
			{

			}
		}

		void  Transactions_LoadDataCompleted(object sender, EventArgs e)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				this.DataContext = App.ViewModel.Transactions;
				_progress.IsVisible = false;
			});
		}
		#endregion

		private void textBoxSearch_ActionIconTapped(object sender, EventArgs e)
		{
			ClearSearch();
		}

		private void ImageSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			App.ViewModel.Transactions.IsSearchVisible = !App.ViewModel.Transactions.IsSearchVisible;
		}

		private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.Focus();
				App.ViewModel.Transactions.SearchText = textBoxSearch.Text;
			}
		}

		private void ClearSearch()
		{
			this.Focus();
			App.ViewModel.Transactions.IsSearchVisible = false;
			App.ViewModel.Transactions.SearchText = "";
		}
	}
}