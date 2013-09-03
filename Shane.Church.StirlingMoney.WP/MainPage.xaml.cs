﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.WP.Resources;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainViewModel _model;
		// Constructor
		public MainPage()
		{
			InitializeComponent();

			//Shows the rate reminder message, according to the settings of the RateReminder.
			(App.Current as App).rateReminder.Notify();
		}

		protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			_model = KernelService.Kernel.Get<MainViewModel>();
			this.DataContext = _model;

			base.OnNavigatedTo(e);

			await LoadData();
		}

		#region App Bar
		private enum ApplicationBarType
		{
			Accounts,
			Budget,
			Goals
		}

		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();

			ApplicationBarMenuItem appBarMenuItemSettings = new ApplicationBarMenuItem(AppResources.AppBarSettings);
			appBarMenuItemSettings.Click += new EventHandler(appBarMenuItemSettings_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemSettings);

			ApplicationBarMenuItem appBarMenuItemReview = new ApplicationBarMenuItem(AppResources.AppBarMenuItemReview);
			appBarMenuItemReview.Click += new EventHandler(appBarMenuItemReview_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemReview);

			ApplicationBarMenuItem appBarMenuItemAbout = new ApplicationBarMenuItem(AppResources.AppBarAbout);
			appBarMenuItemAbout.Click += new EventHandler(appBarMenuItemAbout_Click);
			ApplicationBar.MenuItems.Add(appBarMenuItemAbout);
		}

		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void BuildApplicationBar(ApplicationBarType type = ApplicationBarType.Accounts)
		{
			if (ApplicationBar == null)
			{
				InitializeApplicationBar();
			}
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;
			ApplicationBar.Buttons.Clear();
			switch (type)
			{
				case ApplicationBarType.Goals:
					ApplicationBarIconButton appBarIconButtonAddGoal = new ApplicationBarIconButton(new Uri("/Images/Add.png", UriKind.Relative));
					appBarIconButtonAddGoal.Text = AppResources.AppBarAdd;
					appBarIconButtonAddGoal.Click += new EventHandler(appBarIconButtonAddGoal_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddGoal);

					break;
				case ApplicationBarType.Budget:
					ApplicationBarIconButton appBarIconButtonAddBudget = new ApplicationBarIconButton(new Uri("/Images/Add.png", UriKind.Relative));
					appBarIconButtonAddBudget.Text = AppResources.AppBarAdd;
					appBarIconButtonAddBudget.Click += new EventHandler(appBarIconButtonAddBudget_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddBudget);
					break;
				case ApplicationBarType.Accounts:
				default:
					ApplicationBarIconButton appBarIconButtonAddAccount = new ApplicationBarIconButton(new Uri("/Images/AddAccount.png", UriKind.Relative));
					appBarIconButtonAddAccount.Text = AppResources.AppBarAddAccount;
					appBarIconButtonAddAccount.Click += new EventHandler(appBarIconButtonAddAccount_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonAddAccount);

					ApplicationBarIconButton appBarIconButtonCategories = new ApplicationBarIconButton(new Uri("/Images/Categories.png", UriKind.Relative));
					appBarIconButtonCategories.Text = AppResources.AppBarCategories;
					appBarIconButtonCategories.Click += new EventHandler(appBarIconButtonCategories_Click);
					ApplicationBar.Buttons.Add(appBarIconButtonCategories);
					break;
			}

			ApplicationBarIconButton appBarIconButtonSync = new ApplicationBarIconButton(new Uri("/Images/Synchronize.png", UriKind.Relative));
			appBarIconButtonSync.Text = AppResources.AppBarSync;
			appBarIconButtonSync.Click +=new EventHandler(appBarIconButtonSync_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonSync);

			ApplicationBarIconButton appBarIconButtonReports = new ApplicationBarIconButton(new Uri("/Images/Reports.png", UriKind.Relative));
			appBarIconButtonReports.Text = AppResources.AppBarReports;
			appBarIconButtonReports.Click += new EventHandler(appBarIconButtonReports_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonReports);
		}

		void appBarIconButtonSync_Click(object sender, EventArgs e)
		{
			_model.SyncCommand.Execute(null);
		}

		void appBarIconButtonReports_Click(object sender, EventArgs e)
		{
			_model.ReportsCommand.Execute(null);
		}

		void appBarMenuItemReview_Click(object sender, EventArgs e)
		{
			_model.RateCommand.Execute(null);
		}

		void appBarMenuItemSettings_Click(object sender, EventArgs e)
		{
			_model.SettingsCommand.Execute(null);
		}

		private void appBarMenuItemAbout_Click(object sender, EventArgs e)
		{
			_model.AboutCommand.Execute(null);
		}

		void appBarIconButtonAddGoal_Click(object sender, EventArgs e)
		{
			
		}

		void appBarIconButtonAddBudget_Click(object sender, EventArgs e)
		{
//			NavigationService.Navigate(new Uri(@"/AddEditBudget.xaml", UriKind.Relative));
		}

		private void appBarIconButtonAddAccount_Click(object sender, EventArgs e)
		{
			_model.AddAccountCommand.Execute(null);
		}

		private void appBarIconButtonCategories_Click(object sender, EventArgs e)
		{
			_model.CategoriesCommand.Execute(null);
		}
		#endregion

		private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PivotItem pi = e.AddedItems[0] as PivotItem;
			UpdateApplicationBar(pi);

			await LoadData();
		}

		private void UpdateApplicationBar(PivotItem pi)
		{
			if (pi != null)
			{
				string header = pi.Header.ToString();
				if (header == AppResources.AccountsTitle)
				{
					BuildApplicationBar(ApplicationBarType.Accounts);
				}
				else if (header == AppResources.BudgetsTitle)
				{
					BuildApplicationBar(ApplicationBarType.Budget);
				}
				else if (header == AppResources.GoalsTitle)
				{
					BuildApplicationBar(ApplicationBarType.Goals);
				}
			}
		}

		private async Task LoadData()
		{
			PivotItem pi = PivotMain.SelectedItem as PivotItem;
			if (pi != null)
			{
				string header = pi.Header.ToString();
				if (header == AppResources.AccountsTitle)
				{
					await _model.LoadAccounts();
				}
				else if (header == AppResources.BudgetsTitle)
				{
					await _model.LoadBudgets();
				}
				else if (header == AppResources.GoalsTitle)
				{
					await _model.LoadGoals();
				}
			}
		}
	}
}
