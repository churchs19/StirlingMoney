﻿using Microsoft.Phone.Shell;
using Grace;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class AddEditGoal : AdvertisingPage
	{
		private AddEditGoalViewModel _model;

		public AddEditGoal()
		{
			InitializeComponent();

			InitializeAdControl(this.AdPanel, this.AdControl);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var navContext = Newtonsoft.Json.Linq.JObject.FromObject(this.NavigationContext);

			Task.Run(() => Initialize(navContext));
		}

		protected async Task Initialize(Newtonsoft.Json.Linq.JObject navContext)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				InitializeApplicationBar();
			});
			FlurryWP8SDK.Api.LogPageView();
			_model = ContainerService.Container.Locate<AddEditGoalViewModel>();
			_model.ValidationFailed += (s, args) =>
			{
				string errorMessages = String.Join(
						Environment.NewLine + Environment.NewLine,
						args.Errors.ToArray());
				if (!String.IsNullOrEmpty(errorMessages))
				{
					Deployment.Current.Dispatcher.BeginInvoke(() =>
					{
						MessageBox.Show(errorMessages, Shane.Church.StirlingMoney.Strings.Resources.InvalidValuesTitle, MessageBoxButton.OK);
					});
				}
			};

			Guid id;
			try
			{
				id = PhoneNavigationService.DecodeNavigationParameter<Guid>(navContext);
			}
			catch (KeyNotFoundException)
			{
				id = Guid.Empty;
			}

			await _model.LoadData(id);

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				this.DataContext = _model;
			});
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
		}

		private void InitializeApplicationBar()
		{
			ApplicationBar = new ApplicationBar();

			ApplicationBarIconButton appBarIconButtonSave = new ApplicationBarIconButton(new Uri("/Images/Save.png", UriKind.Relative));
			appBarIconButtonSave.Text = Shane.Church.StirlingMoney.Strings.Resources.AppBarSave;
			appBarIconButtonSave.Click += appBarIconButtonSave_Click;
			ApplicationBar.Buttons.Add(appBarIconButtonSave);
		}

		void appBarIconButtonSave_Click(object sender, EventArgs e)
		{
			Shane.Church.Utility.Core.WP.BindingHelper.UpdateBindings(this.textBoxGoalAmount, this.textBoxGoalName);

			_model.SaveCommand.Execute(null);
		}

		private void textBoxGoalAmount_GotFocus(object sender, RoutedEventArgs e)
		{
			if (_model.Amount == 0)
			{
				textBoxGoalAmount.Text = "";
			}
		}

	}
}