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
using Shane.Church.StirlingMoney.ViewModels;

namespace Shane.Church.StirlingMoney
{
	public partial class Settings : PhoneApplicationPage
	{
		SettingsViewModel _model;

		public Settings()
		{
			InitializeComponent();

			InitializeAdControl();

			_model = new SettingsViewModel();
			_model.LoadData();
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

		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			var validationResults = _model.Validate();
			if (validationResults.Count > 0)
			{
				string errorMessages = String.Join(
					Environment.NewLine + Environment.NewLine,
					validationResults.ToArray());
				if (!String.IsNullOrEmpty(errorMessages))
				{
					MessageBox.Show(errorMessages,
						Shane.Church.StirlingMoney.Resources.AppResources.InvalidValuesTitle, MessageBoxButton.OK);
				}
			}
			else
			{
				_model.SaveSettings();
				NavigationService.GoBack();
			}
		}
	}
}