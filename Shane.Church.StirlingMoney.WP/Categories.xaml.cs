using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.WP.Resources;
using System;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class Categories : PhoneApplicationPage
	{
		private CategoryListViewModel _model;

		public Categories()
		{
			InitializeComponent();

			InitializeAdControl();

			BuildApplicationBar();
		}

		protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			_model = KernelService.Kernel.Get<CategoryListViewModel>();
			this.DataContext = _model;

			base.OnNavigatedTo(e);

			await _model.LoadData();
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

		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void BuildApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;

			ApplicationBarIconButton appBarIconButtonAddCategory = new ApplicationBarIconButton(new Uri("/Images/AddCategory.png", UriKind.Relative));
			appBarIconButtonAddCategory.Text = AppResources.AppBarAdd;
			appBarIconButtonAddCategory.Click += new EventHandler(appBarIconButtonAddCategory_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonAddCategory);
		}

		private void appBarIconButtonAddCategory_Click(object sender, EventArgs e)
		{
			_model.AddCommand.Execute(null);
		}

		private async void menuItemEdit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			await _model.LoadData();
		}

		private async void menuItemDelete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			await _model.LoadData();
		}
	}
}