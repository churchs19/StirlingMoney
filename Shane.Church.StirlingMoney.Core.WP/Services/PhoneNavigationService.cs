using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
    public class PhoneNavigationService : INavigationService
    {
        public void NavigateTo(Uri page)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            if (root != null)
                root.Navigate(page);
        }

        public void NavigateTo(Type pageToNavigateTo)
        {
            this.NavigateTo(new Uri("/" + pageToNavigateTo.Name + ".xaml", UriKind.Relative));
        }

        public void GoBack()
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            if (root != null && root.CanGoBack)
            {
                root.GoBack();
            }
        }
    }
}
