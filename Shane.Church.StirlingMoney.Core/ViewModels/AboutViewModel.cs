using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public abstract class AboutViewModel : ObservableObject
	{
		private IWebNavigationService _navService;

		public AboutViewModel(IWebNavigationService navService)
		{
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

			GoToSChurchNetCommand = new RelayCommand(NavigateToWebsite);

			Initialize();
		}

		public ICommand RateThisAppCommand
		{
			get;
			protected set;
		}

		public ICommand OtherAppsCommand
		{
			get;
			protected set;
		}

		public ICommand SendAnEmailCommand
		{
			get;
			protected set;
		}

		public ICommand GoToSChurchNetCommand
		{
			get;
			protected set;
		}

		private string _version;
		public string Version
		{
			get { return "Version " + _version; }
			set
			{
				Set(() => Version, ref _version, value);
			}
		}

		public abstract void Initialize();

		public void NavigateToWebsite()
		{
			_navService.NavigateTo(new Uri("http://www.s-church.net"));
		}
	}
}
