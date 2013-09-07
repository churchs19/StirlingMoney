using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
using Shane.Church.StirlingMoney.WP.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class AddEditAccount : PhoneApplicationPage
	{
		AddEditAccountViewModel _model;

		public AddEditAccount()
		{
			InitializeComponent();

			InitializeAdControl();

			InitializeApplicationBar();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			_model = KernelService.Kernel.Get<AddEditAccountViewModel>();
			_model.ValidationFailed += (s, args) =>
				{
					string errorMessages = String.Join(
							Environment.NewLine + Environment.NewLine,
							args.Errors.ToArray());
					if (!String.IsNullOrEmpty(errorMessages))
					{
						MessageBox.Show(errorMessages, AppResources.InvalidValuesTitle, MessageBoxButton.OK);
					}
				};
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

		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();

			ApplicationBarIconButton appBarIconButtonSave = new ApplicationBarIconButton(new Uri("/Images/Save.png", UriKind.Relative));
			appBarIconButtonSave.Text = AppResources.AppBarSave;
			appBarIconButtonSave.Click += appBarIconButtonSave_Click;
			ApplicationBar.Buttons.Add(appBarIconButtonSave);
		}

		void appBarIconButtonSave_Click(object sender, EventArgs e)
		{
			var bind = ((FrameworkElement)this.textBoxInitialBalance).GetBindingExpression(TextBox.TextProperty);

			if (bind != null)
				bind.UpdateSource();

			bind = ((FrameworkElement)this.textBoxCreditLimit).GetBindingExpression(TextBox.TextProperty);

			if (bind != null)
				bind.UpdateSource();

			_model.SaveCommand.Execute(null);
		}

		private void textBoxInitialBalance_GotFocus(object sender, RoutedEventArgs e)
		{
			if (_model.InitialBalance == 0)
			{
				textBoxInitialBalance.Text = "";
			}
		}

		private void textBoxCreditLimit_GotFocus(object sender, RoutedEventArgs e)
		{
			if (_model.CreditLimit == 0)
				textBlockCreditLimit.Text = "";
		}
	}
}