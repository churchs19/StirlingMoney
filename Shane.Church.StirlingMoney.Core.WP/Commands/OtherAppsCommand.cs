using Microsoft.Phone.Tasks;
using System;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.WP.Commands
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

        public void Execute(object parameter)
        {
            MarketplaceSearchTask task = new MarketplaceSearchTask();
            task.ContentType = MarketplaceContentType.Applications;
            task.SearchTerms = "Shane Church";
            task.Show();
        }
    }
}
