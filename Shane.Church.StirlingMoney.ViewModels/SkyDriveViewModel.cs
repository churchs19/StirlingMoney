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
	public class SkyDriveViewModel : INotifyPropertyChanged
	{
		private bool _isSignedIn;
		public bool IsSignedIn
		{
			get { return _isSignedIn; }
			set
			{
				if (_isSignedIn != value)
				{
					_isSignedIn = value;
					NotifyPropertyChanged("IsSignedIn");
					NotifyPropertyChanged("ButtonVisibility");
				}
			}
		}

		public Visibility ButtonVisibility
		{
			get
			{
				if (IsSignedIn)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		private string _infoBoxText;
		public string InfoBoxText
		{
			get { return _infoBoxText; }
			set
			{
				if (_infoBoxText != value)
				{
					_infoBoxText = value;
					NotifyPropertyChanged("InfoBoxText");
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
