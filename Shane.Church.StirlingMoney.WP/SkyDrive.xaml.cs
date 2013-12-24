using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class SkyDrive : AdvertisingPage
	{
		BackupViewModel _model;
		ILoggingService _log;

		public SkyDrive()
		{
			InitializeComponent();

			InitializeAdControl(this.AdPanel, this.AdControl);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			TaskEx.Run(() => Initialize());
		}

		protected void Initialize()
		{
			FlurryWP8SDK.Api.LogPageView();
			_model = KernelService.Kernel.Get<BackupViewModel>();
			_log = KernelService.Kernel.Get<ILoggingService>();

			_model.BusyChanged += _model_BusyChanged;

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				BusyIndicator.DataContext = new { ProgressText = "", ProgressPercentage = 0 };
				this.DataContext = _model;
			});
		}

		void _model_BusyChanged(Core.Data.ProgressBusyEventArgs args)
		{
			if (args.IsBusy)
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					BusyIndicator.DataContext = new { ProgressText = args.Message, ProgressPercentage = args.ProgressPercentage };
					BusyIndicator.IsRunning = true;
				});
			}
			else if (args.IsError)
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					BusyIndicator.IsRunning = false;
					_log.LogException(args.Error);
					MessageBox.Show(args.Error.Message, Strings.Resources.GeneralErrorCaption, MessageBoxButton.OK);
				});
			}
			else
			{
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					BusyIndicator.IsRunning = false;
				});
			}
		}
	}
}