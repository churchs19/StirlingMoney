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
using System.ComponentModel;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class TrialMessageViewModel : INotifyPropertyChanged
	{
		public string _message;
		public string Message
		{
			get { return string.Format(Resources.ViewModelResources.TrialMessagePurchaseText, _message); }
			set
			{
				if (_message != value)
				{
					_message = value;
					NotifyPropertyChanged("Message");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
