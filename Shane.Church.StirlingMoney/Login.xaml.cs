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

namespace Shane.Church.StirlingMoney
{
	public partial class Login : PhoneApplicationPage
	{
		public Login()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			System.Windows.Navigation.JournalEntry entry = NavigationService.BackStack.FirstOrDefault();
			if (entry != null && entry.Source.ToString().Contains("Login.xaml"))
			{
				NavigationService.RemoveBackEntry();
			}
			base.OnNavigatedFrom(e);
		}

		private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && App.Settings.Password == PasswordBox.Password)
			{
				App.LoggedIn = true;
				this.Focus();
				NavigationService.Navigate((Uri)PhoneApplicationService.Current.State["RedirectUri"]);
			}
		}
	}
}