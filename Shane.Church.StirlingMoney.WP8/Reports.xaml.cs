using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Reports;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.WP
{
	public class LegendItem
	{
		public string Title { get; set; }
		public Brush Brush { get; set; }
	}

	public partial class Reports : AdvertisingPage
	{
		private ReportsViewModel _model;
		private ILoggingService _log;
		private ISettingsService _settings;
		private ILicensingService _license;
		private INavigationService _navService;

		private ObservableCollection<LegendItem> _legendItems;

		public Reports()
		{
			InitializeComponent();

			InitializeAdControl(AdPanel, AdControl);

			_legendItems = new ObservableCollection<LegendItem>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			TaskEx.Run(() => Initialize());
		}

		protected void Initialize()
		{
			FlurryWP8SDK.Api.LogPageView();
			_log = KernelService.Kernel.Get<ILoggingService>();
			_settings = KernelService.Kernel.Get<ISettingsService>();
			_license = KernelService.Kernel.Get<ILicensingService>();
			_navService = KernelService.Kernel.Get<INavigationService>();
			_model = KernelService.Kernel.Get<ReportsViewModel>();
			_model.SpendingByCategoryReportReloaded += _model_SpendingByCategoryReportReloaded;

			_model.LoadData();

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				this.DataContext = _model;
			});
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);
		}

		void _model_SpendingByCategoryReportReloaded()
		{
			_legendItems.Clear();
			for (int i = 0; i < _model.SpendingByCategoryReportCollection.Count; i++)
			{
				var paletteItem = pieChartCategories.Palette.GetEntry(pieChartCategories.Series[0], i);
				if (paletteItem.HasValue)
				{
					var brush = paletteItem.Value.Fill;
					var item = new LegendItem() { Title = _model.SpendingByCategoryReportCollection[i].Title, Brush = brush };
					_legendItems.Add(item);
				}
				else
				{
					var item = new LegendItem() { Title = _model.SpendingByCategoryReportCollection[i].Title, Brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0)) };
					_legendItems.Add(item);
				}
			}
			pieChartLegend.ItemsSource = _legendItems;
		}
	}
}