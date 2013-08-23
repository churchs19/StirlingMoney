using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.WP.Commands;
using System.Reflection;

namespace Shane.Church.StirlingMoney.Core.WP.ViewModels
{
    public class PhoneAboutViewModel : AboutViewModel
    {
        public override void Initialize()
        {
            RateThisAppCommand = new RateThisAppCommand();
            SendAnEmailCommand = new SendAnEmailCommand();
            OtherAppsCommand = new OtherAppsCommand();

            var versionAttrib = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            Version = versionAttrib.Version.ToString();
        }
    }
}
