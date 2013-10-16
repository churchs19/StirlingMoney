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
#if !PERSONAL
			if ((App.Current as App).trialReminder.IsTrialMode())
			{
				AdControl.AdReceived += new InneractiveAd.IaAdReceived(AdControl_AdReceived);
				AdControl.AdFailed += new InneractiveAd.IaAdFailed(AdControl_AdFailed);
				AdControl.DefaultAdReceived += new InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);
			}
			else
			{
				AdPanel.Children.Remove(AdControl);
				AdControl = null;
			}
#else
			AdPanel.Children.Remove(AdControl);
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


			_model.SaveCommand.Execute(null);
		}

		private void textBoxInitialBalance_GotFocus(object sender, RoutedEventArgs e)
		{
			if (_model.InitialBalance == 0)
			{
				textBoxInitialBalance.Text = "";
			}
		}
	}
}