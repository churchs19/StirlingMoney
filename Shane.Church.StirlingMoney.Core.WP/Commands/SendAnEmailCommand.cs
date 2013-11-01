using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using Shane.Church.StirlingMoney.Strings;

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

		public void Execute(object parameter)
		{
			EmailComposeTask emailTask = new EmailComposeTask();
			emailTask.To = "shane@s-church.net";
			emailTask.Subject = Resources.TechnicalSupportEmailSubject;
			emailTask.Show();
		}
	}
}
