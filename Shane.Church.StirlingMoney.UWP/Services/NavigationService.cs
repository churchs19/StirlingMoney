using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Services;
using Windows.UI.Xaml.Controls;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Newtonsoft.Json;

namespace Shane.Church.StirlingMoney.UWP.Services
{
    public class NavigationService : INavigationService
    {
        private static readonly Dictionary<Type, Type> ViewModelRouting = new Dictionary<Type, Type>() {
                                                                                { typeof(AboutViewModel), typeof(Views.AboutPage) },
                                                                                { typeof(MainViewModel), typeof(Views.MainPage) },
                                                                                { typeof(SettingsViewModel), typeof(Views.SettingsPage) }//,
                                                                                //{ typeof(AddEditAccountViewModel), typeof(Views.AddEditAccountPage) },
                                                                                //{ typeof(AddEditBudgetViewModel), typeof(Views.AddEditBudgetPage) },
                                                                                //{ typeof(AddEditGoalViewModel), typeof(Views.AddEditGoalPage) },
                                                                                //{ typeof(AddEditTransactionViewModel), typeof(Views.AddEditTransactionPage) },
                                                                                //{ typeof(CategoryListViewModel), typeof(Views.CategoriesPage) },
                                                                                //{ typeof(CategoryViewModel), typeof(Views.AddEditCategoryPage) },
                                                                                //{ typeof(TransactionListViewModel), typeof(Views.TransactionsPage) },
                                                                                //{ typeof(BackupViewModel), typeof(Views.SkyDrivePage) },
                                                                                //{ typeof(ReportsViewModel), typeof(Views.ReportsPage) }
        };

        private AppShell _shell;

        /// <summary>
        /// Decodes the navigation parameter.
        /// </summary>
        /// <typeparam name="TJson">The type of the json.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>The json result.</returns>
        public static TJson DecodeNavigationParameter<TJson>(Newtonsoft.Json.Linq.JObject context)
        {
            try
            {
                var param = context["QueryString"]["param"];
                return param == null ? default(TJson) : JsonConvert.DeserializeObject<TJson>(param.ToString());
            }
            catch   /* (Exception ex) */
            {
                throw new KeyNotFoundException();
            }
        }

        public NavigationService()    
        {
            _shell = AppShell.Current;
        }

        public bool CanGoBack
        {
            get
            {
                return _shell.AppFrame.CanGoBack;
            }
        }

        public void GoBack()
        {
            _shell.AppFrame.GoBack();
        }

        public void Navigate<TDestinationViewModel>(object parameter = null)
        {
            if (ViewModelRouting.ContainsKey(typeof(TDestinationViewModel)))
            {
                _shell.AppFrame.Navigate(ViewModelRouting[typeof(TDestinationViewModel)], parameter);
            }
        }

        public Uri NavigationUri<TDestinationViewModel>(object parameter = null)
        {
            throw new NotImplementedException();
        }

        public void RemoveBackEntry()
        {
        }
    }
}
