using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;
using Windows.UI.Xaml.Controls;

namespace Shane.Church.StirlingMoney.Core.UWP.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _mainFrame;

        public NavigationService()
        {
            ContainerService.Container.Locate<Frame>();
        }

        public bool CanGoBack
        {
            get
            {
                return _mainFrame.CanGoBack;
            }
        }

        public void GoBack()
        {
            _mainFrame.GoBack();
        }

        public void Navigate<TDestinationViewModel>(object parameter = null)
        {
            throw new NotImplementedException();
        }

        public Uri NavigationUri<TDestinationViewModel>(object parameter = null)
        {
            throw new NotImplementedException();
        }

        public void RemoveBackEntry()
        {
            throw new NotImplementedException();
        }
    }
}
