using Inneractive.Nokia.Ad;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Data;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class Transactions : PhoneApplicationPage
	{
		TransactionListViewModel _model;
		INavigationService _navService;

		public Transactions()
		{
			InitializeComponent();

			InitializeAdControl();

			InitializeApplicationBar();

			_navService = KernelService.Kernel.Get<INavigationService>();

			GenericGroupDescriptor<TransactionListItemViewModel, DateTime> grouping = new GenericGroupDescriptor<TransactionListItemViewModel, DateTime>(it => it.TransactionDate.Date);
			grouping.GroupFormatString = "{0:D}";
			grouping.SortMode = ListSortMode.Descending;

			jumpListTransactions.GroupDescriptorsSource = new List<DataDescriptor>() { grouping };

			GenericSortDescriptor<TransactionListItemViewModel, TransactionListItemViewModel> sort = new GenericSortDescriptor<TransactionListItemViewModel, TransactionListItemViewModel>(it => it);
			sort.SortMode = ListSortMode.Descending;

			jumpListTransactions.SortDescriptorsSource = new List<DataDescriptor>() { sort };
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			FlurryWP8SDK.Api.LogPageView();
			if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
			{
				_model = KernelService.Kernel.Get<TransactionListViewModel>();

				try
				{
					var id = PhoneNavigationService.DecodeNavigationParameter<Guid>(this.NavigationContext);
					_model.LoadData(id);
				}
				catch (KeyNotFoundException)
				{
					_model.LoadData(Guid.Empty);
				}
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					this.DataContext = _model;
					this.jumpListTransactions.ItemsSource = _model.Transactions;
				});
			}
			else
			{
				_model.RefreshData().ContinueWith((t) =>
				{
					Deployment.Current.Dispatcher.BeginInvoke(() =>
					{
						this.jumpListTransactions.RefreshData();
					});
				});
			}

			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			var account = _model.GetAccount();
			if (account != null)
			{
				account.UpdateBalances().ContinueWith((it) =>
				{
					base.OnNavigatingFrom(e);
				});
			}
			else
			{
				base.OnNavigatingFrom(e);
			}
		}

		#region Ad Control
		private void InitializeAdControl()
		{
#if !PERSONAL
			if ((App.Current as App).trialReminder.IsTrialMode())
			{
				AdControl.AdReceived += new InneractiveAd.IaAdReceived(AdControl_AdReceived);
				AdControl.AdFailed += new InneractiveAd.IaAdFailed(AdControl_AdFailed);
				AdControl.DefaultAdReceived += new InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);
			}
			else
			{
				AdPanel.Children.Remove(AdControl);
				AdControl = null;
			}
#else
			AdPanel.Children.Remove(AdControl);
			AdControl = null;
#endif

		}

		void AdControl_DefaultAdReceived(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdReceived(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdFailed(object sender)
		{
			AdControl.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion

		#region App Bar
		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;

			ApplicationBarIconButton ApplicationBarWithdraw = new ApplicationBarIconButton(new Uri("/Images/Withdraw.png", UriKind.Relative));
			ApplicationBarWithdraw.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarWithdraw;
			ApplicationBarWithdraw.Click += new EventHandler(ApplicationBarWithdraw_Click);
			ApplicationBar.Buttons.Add(ApplicationBarWithdraw);

			ApplicationBarIconButton ApplicationBarWriteCheck = new ApplicationBarIconButton(new Uri("/Images/WriteCheck.png", UriKind.Relative));
			ApplicationBarWriteCheck.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarWriteCheck;
			ApplicationBarWriteCheck.Click += new EventHandler(ApplicationBarWriteCheck_Click);
			ApplicationBar.Buttons.Add(ApplicationBarWriteCheck);

			ApplicationBarIconButton ApplicationBarDeposit = new ApplicationBarIconButton(new Uri("/Images/Deposit.png", UriKind.Relative));
			ApplicationBarDeposit.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarDeposit;
			ApplicationBarDeposit.Click += new EventHandler(ApplicationBarDeposit_Click);
			ApplicationBar.Buttons.Add(ApplicationBarDeposit);

			ApplicationBarIconButton ApplicationBarTransfer = new ApplicationBarIconButton(new Uri("/Images/Transfer.png", UriKind.Relative));
			ApplicationBarTransfer.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarTransfer;
			ApplicationBarTransfer.Click += new EventHandler(ApplicationBarTransfer_Click);
			ApplicationBar.Buttons.Add(ApplicationBarTransfer);
		}

		private void ApplicationBarWithdraw_Click(object sender, EventArgs e)
		{
			_model.WithdrawCommand.Execute(null);
		}

		private void ApplicationBarWriteCheck_Click(object sender, EventArgs e)
		{
			_model.WriteCheckCommand.Execute(null);
		}

		private void ApplicationBarDeposit_Click(object sender, EventArgs e)
		{
			_model.DepositCommand.Execute(null);
		}

		private void ApplicationBarTransfer_Click(object sender, EventArgs e)
		{
			_model.TransferCommand.Execute(null);
		}
		#endregion

		private void jumpListTransactions_DataRequested(object sender, EventArgs e)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				_model.LoadNextTransactions();
				if (_model.TotalRows == _model.Transactions.Count)
					jumpListTransactions.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
			});
		}

	}
}