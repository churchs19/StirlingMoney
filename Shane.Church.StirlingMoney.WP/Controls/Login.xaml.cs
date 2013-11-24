using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.WP.Services;
using System;
using System.Windows.Controls;
using Telerik.Windows.Controls.PhoneTextBox;

namespace Shane.Church.StirlingMoney.WP.Controls
{
	public partial class Login : UserControl
	{
		private PhoneSettingsService _settings;

		public Login(ISettingsService settings)
		{
			InitializeComponent();

			_settings = new PhoneSettingsService();
		}

		public delegate void ActionExecutedHandler();
		public event ActionExecutedHandler ActionExecuted;

		public bool PasswordVerified { get; set; }

		private void radPasswordBox_ActionButtonTap(object sender, EventArgs e)
		{
			radPasswordBox.ChangeValidationState(ValidationState.Validating, Strings.Resources.PasswordValidatingText);
			PasswordVerified = false;
			var pwd = _settings.LoadSetting<string>("Password");
			if (radPasswordBox.Password == pwd)
			{
				radPasswordBox.ChangeValidationState(ValidationState.Valid, "");
				PasswordVerified = true;
				if (ActionExecuted != null)
					ActionExecuted();
			}
			else
			{
				radPasswordBox.ChangeValidationState(ValidationState.Invalid, Strings.Resources.InvalidPassword);
			}
		}

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			this.radPasswordBox.Focus();
		}
	}
}
