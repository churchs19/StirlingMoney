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
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using Shane.Church.StirlingMoney.ViewModels;

namespace Shane.Church.StirlingMoney
{
	public partial class AddEditTransaction : PhoneApplicationPage
	{
		AddEditTransactionViewModel _model;

		public AddEditTransaction()
		{
			InitializeComponent();

			InitializeAdControl();

			Guid? currentAccountId = PhoneApplicationService.Current.State["CurrentAccount"] as Guid?;
			Guid? currentTransactionId = PhoneApplicationService.Current.State["CurrentTransaction"] as Guid?;
			if (!currentTransactionId.HasValue)
				currentTransactionId = Guid.Empty;
			TransactionType? currentTransactionType = PhoneApplicationService.Current.State["CurrentTransactionType"] as TransactionType?;

			if (currentAccountId.HasValue)
			{
				_model = new AddEditTransactionViewModel();
				if (!currentTransactionType.HasValue)
					_model.LoadData(currentAccountId.Value, currentTransactionId.Value);
				else
					_model.LoadData(currentAccountId.Value, currentTransactionId.Value, currentTransactionType.Value);
				DataContext = _model;
			}
			else
			{
				NavigationService.GoBack();
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

		private void buttonSave_Click(object sender, RoutedEventArgs e)
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
				_model.SaveTransaction();
				NavigationService.GoBack();
			}
		}

		private void textBoxAmount_GotFocus(object sender, RoutedEventArgs e)
		{
			if (_model.Amount == 0)
			{
				textBoxAmount.Text = "";
			}
		}
	}
}