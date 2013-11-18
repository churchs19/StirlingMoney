using Microsoft.Phone.Controls;
using System.Windows.Controls;

namespace Shane.Church.StirlingMoney.WP
{
	public class AdvertisingPage : PhoneApplicationPage
	{
		public AdvertisingPage()
			: base()
		{

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
	}
}
