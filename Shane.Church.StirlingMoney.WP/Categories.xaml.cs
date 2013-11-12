﻿using Inneractive.Nokia.Ad;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
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

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			FlurryWP8SDK.Api.LogPageView();
			_model = KernelService.Kernel.Get<CategoryListViewModel>();

			base.OnNavigatedTo(e);

			_model.LoadData();

			this.DataContext = _model;
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
				AdControl = null;
			}
#else
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