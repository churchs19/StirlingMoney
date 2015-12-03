using System;
using System.Windows.Input;
using Windows.System;

namespace Shane.Church.StirlingMoney.Core.UWP.Commands
{
    public class RateThisAppCommand : ICommand
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
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=" + Windows.ApplicationModel.Store.CurrentApp.AppId));
        }
    }
}
