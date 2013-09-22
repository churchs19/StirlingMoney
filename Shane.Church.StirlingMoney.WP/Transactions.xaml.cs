using Inneractive.Nokia.Ad;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
using Shane.Church.StirlingMoney.WP.Resources;
using System;
using System.Collections.Generic;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class Transactions : PhoneApplicationPage
	{
		TransactionListViewModel _model;

		public Transactions()
		{
			InitializeComponent();

			InitializeAdControl();

			InitializeApplicationBar();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
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

			this.DataContext = _model;

			base.OnNavigatedTo(e);
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
				AdControl = null;
			}
#else
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
			ApplicationBarWithdraw.Text = AppResources.AppBarWithdraw;
			ApplicationBarWithdraw.Click += new EventHandler(ApplicationBarWithdraw_Click);
			ApplicationBar.Buttons.Add(ApplicationBarWithdraw);

			ApplicationBarIconButton ApplicationBarWriteCheck = new ApplicationBarIconButton(new Uri("/Images/WriteCheck.png", UriKind.Relative));
			ApplicationBarWriteCheck.Text = AppResources.AppBarWriteCheck;
			ApplicationBarWriteCheck.Click += new EventHandler(ApplicationBarWriteCheck_Click);
			ApplicationBar.Buttons.Add(ApplicationBarWriteCheck);

			ApplicationBarIconButton ApplicationBarDeposit = new ApplicationBarIconButton(new Uri("/Images/Deposit.png", UriKind.Relative));
			ApplicationBarDeposit.Text = AppResources.AppBarDeposit;
			ApplicationBarDeposit.Click += new EventHandler(ApplicationBarDeposit_Click);
			ApplicationBar.Buttons.Add(ApplicationBarDeposit);

			ApplicationBarIconButton ApplicationBarTransfer = new ApplicationBarIconButton(new Uri("/Images/Transfer.png", UriKind.Relative));
			ApplicationBarTransfer.Text = AppResources.AppBarTransfer;
			ApplicationBarTransfer.Click += new EventHandler(ApplicationBarTransfer_Click);
			ApplicationBar.Buttons.Add(ApplicationBarTransfer);
		}

		private void ApplicationBarWithdraw_Click(object sender, EventArgs e)
		{
		}

		private void ApplicationBarWriteCheck_Click(object sender, EventArgs e)
		{
		}

		private void ApplicationBarDeposit_Click(object sender, EventArgs e)
		{
		}

		private void ApplicationBarTransfer_Click(object sender, EventArgs e)
		{
		}
		#endregion

		private void jumpListTransactions_DataRequested(object sender, EventArgs e)
		{
			_model.LoadNextTransactions();
			if (_model.TotalRows == _model.Transactions.Count)
				jumpListTransactions.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;

		}

	}
}