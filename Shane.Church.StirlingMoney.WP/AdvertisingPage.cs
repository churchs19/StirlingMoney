using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.WP
{
	public class AdvertisingPage : PhoneApplicationPage
	{
		public AdvertisingPage()
			: base()
		{
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			var settings = KernelService.Kernel.Get<ISettingsService>();

			if (settings.LoadSetting<bool>("UsePassword") && !((Shane.Church.StirlingMoney.WP.App)App.Current).IsLoggedIn)
			{
				RadWindow loginWindow = new RadWindow();
				loginWindow.IsFullScreen = true;
				loginWindow.IsClosedOnBackButton = false;
				loginWindow.IsClosedOnOutsideTap = false;
				loginWindow.IsOpen = false;
				loginWindow.IsAnimationEnabled = true;
				loginWindow.OpenAnimation = new RadSlideAnimation() { MoveDirection = MoveDirection.BottomIn };
				loginWindow.CloseAnimation = new RadSlideAnimation() { MoveDirection = MoveDirection.BottomOut };
				Controls.Login loginContent = KernelService.Kernel.Get<Controls.Login>();
				loginContent.ActionExecuted += () =>
				{
					Deployment.Current.Dispatcher.BeginInvoke(() =>
					{
						loginWindow.IsOpen = false;
					});
				};
				loginWindow.WindowClosing += (s, ce) =>
				{
					if (!loginContent.PasswordVerified)
					{
						this.OnBackKeyPress(new System.ComponentModel.CancelEventArgs());
					}
					else
					{
						((Shane.Church.StirlingMoney.WP.App)App.Current).IsLoggedIn = true;
					}
				};
				loginWindow.Content = loginContent;
				var root = this.FindName("LayoutRoot") as Panel;
				if (root != null)
				{
					root.Children.Add(loginWindow);
					loginWindow.IsOpen = true;
				}
			}

			base.OnNavigatedTo(e);
		}

		private Inneractive.Nokia.Ad.InneractiveAd _adControl;
		private StackPanel _adPanel;

		#region Ad Control
		protected void InitializeAdControl(StackPanel adPanel, Inneractive.Nokia.Ad.InneractiveAd adControl)
		{
			_adPanel = adPanel;
			_adControl = adControl;
#if !PERSONAL
			if (KernelService.Kernel.Get<Telerik.Windows.Controls.RadTrialApplicationReminder>().IsTrialMode())
			{
				_adControl.AdReceived += new InneractiveAd.IaAdReceived(AdControl_AdReceived);
				_adControl.AdFailed += new InneractiveAd.IaAdFailed(AdControl_AdFailed);
				_adControl.DefaultAdReceived += new InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);
			}
			else
			{
				_adPanel.Children.Remove(AdControl);
				_adControl = null;
			}
#else
			_adPanel.Children.Remove(adControl);
			_adControl = null;
#endif
		}

		protected void AdControl_DefaultAdReceived(object sender)
		{
			_adControl.Visibility = System.Windows.Visibility.Visible;
		}

		protected void AdControl_AdReceived(object sender)
		{
			_adControl.Visibility = System.Windows.Visibility.Visible;
		}

		protected void AdControl_AdFailed(object sender)
		{
			_adControl.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion

		protected void Exit()
		{
			while (NavigationService.BackStack.Any())
				NavigationService.RemoveBackEntry();

			this.IsHitTestVisible = this.IsEnabled = false;

			if (this.ApplicationBar != null)
			{
				foreach (var item in this.ApplicationBar.Buttons.OfType<ApplicationBarIconButton>())
					item.IsEnabled = false;
				foreach (var item in this.ApplicationBar.MenuItems.OfType<ApplicationBarMenuItem>())
					item.IsEnabled = false;
				this.ApplicationBar.IsMenuEnabled = false;
			}
		}
	}
}
