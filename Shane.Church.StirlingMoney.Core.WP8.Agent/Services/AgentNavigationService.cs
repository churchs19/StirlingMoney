using Newtonsoft.Json;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using Shane.Church.StirlingMoney.Core.ViewModels.Reports;
using System;
using System.Collections.Generic;

namespace Shane.Church.StirlingMoney.Core.WP8.Agent.Services
{
	public class AgentNavigationService : INavigationService
	{
		private static readonly Dictionary<Type, string> ViewModelRouting = new Dictionary<Type, string>() {
																				{ typeof(AboutViewModel), "About.xaml" },
																				{ typeof(MainViewModel), "MainPage.xaml" },
																				{ typeof(SettingsViewModel), "Settings.xaml" },
																				{ typeof(AddEditAccountViewModel), "AddEditAccount.xaml" },
																				{ typeof(AddEditBudgetViewModel), "AddEditBudget.xaml" },
																				{ typeof(AddEditGoalViewModel), "AddEditGoal.xaml" },
																				{ typeof(AddEditTransactionViewModel), "AddEditTransaction.xaml" },
																				{ typeof(CategoryListViewModel), "Categories.xaml" },
																				{ typeof(CategoryViewModel), "AddEditCategory.xaml" },
																				{ typeof(TransactionListViewModel), "Transactions.xaml" },
																				{ typeof(BackupViewModel), "SkyDrive.xaml" },
																				{ typeof(ReportsViewModel), "Reports.xaml" }
		};

		public bool CanGoBack
		{
			get { throw new NotImplementedException(); }
		}

		public void GoBack()
		{
			throw new NotImplementedException();
		}

		public void Navigate<TDestinationViewModel>(object parameter = null)
		{
			throw new NotImplementedException();
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
	}
}
