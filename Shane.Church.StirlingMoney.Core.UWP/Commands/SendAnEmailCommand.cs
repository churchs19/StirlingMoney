using System;
using System.Windows.Input;
using Shane.Church.StirlingMoney.Strings;
using Windows.System;

namespace Shane.Church.StirlingMoney.Core.WP.Commands
{
	public class SendAnEmailCommand : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

#pragma warning disable 0067
		public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

		public async void Execute(object parameter)
		{
            await Launcher.LaunchUriAsync(new Uri("mailto:shane@s-church.net?subject=" + Resources.TechnicalSupportEmailSubject));
		}
	}
}
