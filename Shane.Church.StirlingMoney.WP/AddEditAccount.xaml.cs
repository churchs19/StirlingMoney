using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Services;
using Shane.Church.Utility.Core.WP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class AddEditAccount : AdvertisingPage
	{
		AddEditAccountViewModel _model;

		public AddEditAccount()
		{
			InitializeComponent();

			InitializeAdControl(this.AdPanel, this.AdControl);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var navContext = Newtonsoft.Json.Linq.JObject.FromObject(this.NavigationContext.QueryString);

			TaskEx.Run(() => Initialize(navContext));
		}

		protected async Task Initialize(Newtonsoft.Json.Linq.JObject navContext)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				InitializeApplicationBar();
			});
			FlurryWP8SDK.Api.LogPageView();
			_model = KernelService.Kernel.Get<AddEditAccountViewModel>();
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
			if (e.NavigationMode == NavigationMode.Back)
				_model.Commit().Wait(1000);
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
			BindingHelper.UpdateBindings(this.textBoxAccountName, this.textBoxInitialBalance);

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