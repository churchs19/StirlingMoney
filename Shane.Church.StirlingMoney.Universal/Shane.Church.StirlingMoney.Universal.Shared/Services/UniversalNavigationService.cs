using Newtonsoft.Json;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalNavigationService : INavigationService
    {
        private static readonly Dictionary<Type, Type> ViewModelRouting = new Dictionary<Type, Type>() {
//																				{ typeof(AboutViewModel), typeof(AboutViewModel) },
																				{ typeof(MainViewModel), typeof(HubPage) },
//																				{ typeof(SettingsViewModel), "Settings.xaml" },
                                                                                //{ typeof(AddEditAccountViewModel), "AddEditAccount.xaml" },
                                                                                //{ typeof(AddEditBudgetViewModel), "AddEditBudget.xaml" },
                                                                                //{ typeof(AddEditGoalViewModel), "AddEditGoal.xaml" },
                                                                                //{ typeof(AddEditTransactionViewModel), "AddEditTransaction.xaml" },
                                                                                //{ typeof(CategoryListViewModel), "Categories.xaml" },
                                                                                //{ typeof(CategoryViewModel), "AddEditCategory.xaml" },
                                                                                //{ typeof(TransactionListViewModel), "Transactions.xaml" },
                                                                                //{ typeof(BackupViewModel), "SkyDrive.xaml" },
                                                                                //{ typeof(ReportsViewModel), "Reports.xaml" }
		};
        private Frame _mainFrame;

        public UniversalNavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }
        
        public bool CanGoBack
        {
            get { return false; }
        }

        public void GoBack()
        {
            
        }

        public void RemoveBackEntry()
        {
            
        }

        public void Navigate<TDestinationViewModel>(object parameter = null)
        {
            if(parameter!=null)
            {
                _mainFrame.Navigate(ViewModelRouting[typeof(TDestinationViewModel)], JsonConvert.SerializeObject(parameter));
            }
            else
            {
                _mainFrame.Navigate(ViewModelRouting[typeof(TDestinationViewModel)]);
            }
        }

        public Uri NavigationUri<TDestinationViewModel>(object parameter = null)
        {
            var navParameter = string.Empty;
            if (parameter != null)
            {
                navParameter = "?param=" + JsonConvert.SerializeObject(parameter);
            }

            if (ViewModelRouting.ContainsKey(typeof(TDestinationViewModel)))
            {
                var page = ViewModelRouting[typeof(TDestinationViewModel)];
                return new Uri("/" + page + navParameter, UriKind.Relative);
            }
            return null;
        }

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
    }
}
