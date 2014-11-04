using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Universal.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.ViewModels
{
    public class UniversalAboutViewModel : AboutViewModel
    {
        public UniversalAboutViewModel(IWebNavigationService navService)
			: base(navService)
		{

		}

        public override void Initialize()
        {
            RateThisAppCommand = new RateThisAppCommand();
            SendAnEmailCommand = new SendAnEmailCommand();
            OtherAppsCommand = new OtherAppsCommand();

            var versionAttrib = new AssemblyName(typeof(App).GetTypeInfo().Assembly.FullName);
            Version = versionAttrib.Version.ToString();
        }
    }
}
