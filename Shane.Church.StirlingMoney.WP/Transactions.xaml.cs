using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
#if DEBUG
using Shane.Church.Utility.Core.WP;
#endif
using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Data;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class Transactions : AdvertisingPage
	{
		TransactionListViewModel _model;
		INavigationService _navService;
		ILoggingService _log;
		bool _isNew = true;
		bool _isPinned = false;

		public Transactions()
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
			_isNew = navMode == System.Windows.Navigation.NavigationMode.New;

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				InitializeApplicationBar();

				InitializeJumpList();
			});

			FlurryWP8SDK.Api.LogPageView();
			_log = KernelService.Kernel.Get<ILoggingService>();
			_model = KernelService.Kernel.Get<TransactionListViewModel>();
			_navService = KernelService.Kernel.Get<INavigationService>();
			_model.BusyChanged += _model_BusyChanged;

			//await _model.ActivateAsync();

			//if (_isNew)
			//{
			TransactionListParams param = null;
			try
			{
				param = PhoneNavigationService.DecodeNavigationParameter<TransactionListParams>(navContext);
			}
			catch (KeyNotFoundException)
			{
				param = new TransactionListParams();
			}

			await _model.LoadData(param.Id);

			_isNew = false;
			_isPinned = param.PinnedTile;

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				this.DataContext = _model;
				this.jumpListTransactions.ItemsSource = _model.Transactions;
				this.ContentPanel.Visibility = System.Windows.Visibility.Visible;
			});
			//}
			//else
			//{
			//	Deployment.Current.Dispatcher.BeginInvoke(() =>
			//	{
			//		this.DataContext = _model;
			//		this.jumpListTransactions.ItemsSource = null;
			//		this.jumpListTransactions.ItemsSource = _model.Transactions;
			//	});

			//}

#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Transactions_Initialize");
#endif
		}

		protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			if (e.NavigationMode != System.Windows.Navigation.NavigationMode.Back)
			{
				//				_model.Deactivate();
			}
			base.OnNavigatingFrom(e);
		}

		protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
		{
			if (_isPinned)
			{
				_navService.Navigate<MainViewModel>(true);
				e.Cancel = true;
			}
			base.OnBackKeyPress(e);
		}

		void _model_BusyChanged(Core.Data.BusyEventArgs args)
		{
			if (args.IsBusy)
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					LoadingBusy.Content = args.Message;
					LoadingBusy.AnimationStyle = (Telerik.Windows.Controls.AnimationStyle)args.AnimationType;
					LoadingBusy.IsRunning = true;
				});
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
					LoadingBusy.IsRunning = false;
				});
			}
		}

		private void InitializeJumpList()
		{
			GenericGroupDescriptor<TransactionListItemViewModel, DateTime> grouping = new GenericGroupDescriptor<TransactionListItemViewModel, DateTime>(it => it.TransactionDate.Date);
			grouping.GroupFormatString = "{0:D}";
			grouping.SortMode = ListSortMode.Descending;

			jumpListTransactions.GroupDescriptorsSource = new List<DataDescriptor>() { grouping };

			GenericSortDescriptor<TransactionListItemViewModel, TransactionListItemViewModel> sort = new GenericSortDescriptor<TransactionListItemViewModel, TransactionListItemViewModel>(it => it);
			sort.SortMode = ListSortMode.Descending;

			jumpListTransactions.SortDescriptorsSource = new List<DataDescriptor>() { sort };
		}


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

		private async void jumpListTransactions_DataRequested(object sender, EventArgs e)
		{
			await _model.LoadNextTransactions();
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				if (_model.Transactions.Count >= _model.TotalRows)
					jumpListTransactions.DataVirtualizationMode = Telerik.Windows.Controls.DataVirtualizationMode.None;
			});
#if DEBUG
			DebugUtility.DebugOutputMemoryUsage("Transactions_jumpListTransactions_DataRequested");
#endif
		}
	}
}