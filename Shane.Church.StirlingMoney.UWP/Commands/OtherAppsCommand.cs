using System;
using System.Windows.Input;
using Windows.System;

namespace Shane.Church.StirlingMoney.UWP.Commands
{
    public class OtherAppsCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public async void Execute(object parameter)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://publisher/?name=Shane Church"));
        }
    }
}
