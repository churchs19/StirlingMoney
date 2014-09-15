using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class Categories : AdvertisingPage
	{
		private CategoryListViewModel _model;

		public Categories()
		{
			InitializeComponent();

			InitializeAdControl(this.AdPanel, this.AdControl);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			Task.Run(() => Initialize());
		}

		protected void Initialize()
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				BuildApplicationBar();
			});
			FlurryWP8SDK.Api.LogPageView();
			_model = KernelService.Kernel.Get<CategoryListViewModel>();

			_model.LoadData();

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				this.DataContext = _model;
			});
		}

		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void BuildApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;

			ApplicationBarIconButton appBarIconButtonAddCategory = new ApplicationBarIconButton(new Uri("/Images/AddCategory.png", UriKind.Relative));
			appBarIconButtonAddCategory.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarAdd;
			appBarIconButtonAddCategory.Click += new EventHandler(appBarIconButtonAddCategory_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonAddCategory);
		}

		private void appBarIconButtonAddCategory_Click(object sender, EventArgs e)
		{
			_model.AddCommand.Execute(null);
		}

		private void menuItemDelete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			_model.LoadData();
		}
	}
}