using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalWebNavigationService : IWebNavigationService
    {
        public async void NavigateTo(Uri page)
        {
            await Windows.System.Launcher.LaunchUriAsync(page);
        }
    }
}
