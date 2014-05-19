using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.Utility.Core.Command;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.PhoneTextBox;

namespace Shane.Church.StirlingMoney.WP
{
	public partial class Settings : AdvertisingPage
	{
		private SettingsViewModel _model;
		private ILoggingService _logService;

		private PivotItem _savedPivotItem;

		public Settings()
		{
			InitializeComponent();

			InitializeAdControl(this.AdPanel, this.AdControl);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			TaskEx.Run(() => Initialize());
		}

		protected async Task Initialize()
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				InitializeApplicationBar();
			});
			FlurryWP8SDK.Api.LogPageView();
			_logService = KernelService.Kernel.Get<ILoggingService>();
			_model = KernelService.Kernel.Get<SettingsViewModel>();
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
			_model.AddActionCompleted += (s, args) =>
			{
				var isSuccess = true;
				if (args is ValidationResultEventArgs)
					isSuccess = ((ValidationResultEventArgs)args).IsValid;

				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					if (isSuccess)
					{
						this.newAuthorizedUser.Text = "";
					}
					else
						newAuthorizedUser.ChangeValidationState(ValidationState.Invalid, "Required");
				});
			};

			await _model.LoadData();

			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				_savedPivotItem = SettingsPivot.Items[2] as PivotItem;
				if (!_model.EnableSync)
				{
					SettingsPivot.Items.RemoveAt(2);
				}

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
			Shane.Church.Utility.Core.WP.BindingHelper.UpdatePasswordBindings(this.passwordPassword, this.passwordConfirm);

			_model.SaveCommand.Execute(null);
		}

		private void editTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (sender is RadTextBox)
			{
				Dispatcher.BeginInvoke(() =>
				{
					var tb = ((RadTextBox)sender);
					tb.Focus();
					tb.SelectionStart = tb.Text.Length;
				});
			}
		}

		private void entryModel_SaveActionCompleted(object sender, EventArgs e)
		{
			_logService.LogMessage("Authorized User Saved");
			if (sender is SettingsAppSyncUserViewModel)
			{
				Dispatcher.BeginInvoke(() =>
				{
					((SettingsAppSyncUserViewModel)sender).SaveActionCompleted -= entryModel_SaveActionCompleted;
					AuthorizedUsers.SelectedItem = null;
				});
			}
		}

		private void editTextBox_ActionButtonTap(object sender, EventArgs e)
		{
			var tb = sender as RadTextBox;
			if (tb != null)
			{
				Shane.Church.Utility.Core.WP.BindingHelper.UpdateBindings(tb);
				var viewModel = tb.DataContext as SettingsAppSyncUserViewModel;
				if (viewModel != null)
				{
					viewModel.SaveActionCompleted += entryModel_SaveActionCompleted;
				}
			}
		}

		private void newAuthorizedUser_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			newAuthorizedUser.ChangeValidationState(ValidationState.NotValidated, "");
		}

		private void toggleEnableSync_CheckedChanged(object sender, CheckedChangedEventArgs e)
		{
			if (e.NewState == true)
			{
				if (SettingsPivot.Items.Count < 3)
				{
					SettingsPivot.Items.Insert(2, _savedPivotItem);

				}
			}
			else
			{
				if (SettingsPivot.Items.Count == 3)
				{
					SettingsPivot.Items.RemoveAt(2);
				}
			}
		}
	}
}