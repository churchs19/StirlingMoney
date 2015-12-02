using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.Services;

namespace Shane.Church.StirlingMoney.Core.UWP.ViewModels
{
    public class AboutViewModel : Core.ViewModels.AboutViewModel
    {
        public AboutViewModel(IWebNavigationService navService)
			: base(navService)
        {

        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
