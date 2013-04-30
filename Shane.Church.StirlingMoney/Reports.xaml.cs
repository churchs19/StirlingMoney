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
using Shane.Church.StirlingMoney.ViewModels.Reports;

namespace Shane.Church.StirlingMoney
{
	public partial class Reports : PhoneApplicationPage
	{
		ProgressIndicator _progress = null;
		ReportsViewModel _model = null;
		
		public Reports()
		{
			InitializeComponent();

			InitializeAdControl();

			_progress = new ProgressIndicator();
			_progress.IsVisible = true;
			_progress.IsIndeterminate = true;
			_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarText;
			SystemTray.SetProgressIndicator(this, _progress);

			_model = new ReportsViewModel();
			_model.LoadData();
			if (!_model.IsBudgetPaneVisible)
				Pivot.Items.Remove(PivotItemBudget);
			if (!_model.IsCategoryPaneVisible)
				Pivot.Items.Remove(PivotItemCategories);
			DataContext = _model;
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

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			_progress.IsVisible = false;
		}
	}
}