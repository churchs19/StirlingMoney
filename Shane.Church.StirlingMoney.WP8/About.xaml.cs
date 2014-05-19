using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Ninject;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.WP
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();

            this.DataContext = KernelService.Kernel.Get<AboutViewModel>();
        }
    }
}
