using System;
using Shane.Church.StirlingMoney.Core.Services;
using Windows.System;

namespace Shane.Church.StirlingMoney.Core.UWP.Services
{
    public class WebNavigationService : IWebNavigationService
    {
        public async void NavigateTo(Uri page)
        {
            await Launcher.LaunchUriAsync(page);
        }
    }
}
