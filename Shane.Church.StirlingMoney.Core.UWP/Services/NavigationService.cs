using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.Core.UWP.Services
{
    public class NavigationService : INavigationService
    {
        public bool CanGoBack
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void GoBack()
        {
            throw new NotImplementedException();
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
