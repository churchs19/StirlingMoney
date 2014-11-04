using Shane.Church.StirlingMoney.Strings;
using System;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Universal.Commands
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
            var rateUri = new Uri("ms-windows-store:Review?PFN=22591ShaneChurch.StirlingMoney_d0m6v4h0mhrme");
            await Windows.System.Launcher.LaunchUriAsync(rateUri);
        }
    }
}
