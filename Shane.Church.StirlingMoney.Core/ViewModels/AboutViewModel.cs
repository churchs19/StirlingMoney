using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
    public abstract class AboutViewModel : ObservableObject
    {
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

        public AboutViewModel()
        {
            GoToSChurchNetCommand = new RelayCommand(NavigateToWebsite);

            Initialize();
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
            KernelService.Kernel.Get<IWebNavigationService>().NavigateTo(new Uri("http://www.s-church.net"));
        }
    }
}
