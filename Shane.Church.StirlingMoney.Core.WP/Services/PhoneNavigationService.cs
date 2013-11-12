using Newtonsoft.Json;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class PhoneNavigationService : INavigationService
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
																				{ typeof(TransactionListViewModel), "Transactions.xaml" }
		};

		public bool CanGoBack
		{
			get
			{
				return RootFrame.CanGoBack;
			}
		}

		private Frame RootFrame
		{
			get { return Application.Current.RootVisual as Frame; }
		}

		/// <summary>
		/// Decodes the navigation parameter.
		/// </summary>
		/// <typeparam name="TJson">The type of the json.</typeparam>
		/// <param name="context">The context.</param>
		/// <returns>The json result.</returns>
		public static TJson DecodeNavigationParameter<TJson>(NavigationContext context)
		{
			if (context.QueryString.ContainsKey("param"))
			{
				var param = context.QueryString["param"];
				return string.IsNullOrWhiteSpace(param) ? default(TJson) : JsonConvert.DeserializeObject<TJson>(param);
			}

			throw new KeyNotFoundException();
		}

		/// <summary>
		/// The go back.
		/// </summary>
		public void GoBack()
		{
			RootFrame.GoBack();
		}

		/// <summary>
		/// Navigates the specified parameter.
		/// </summary>
		/// <typeparam name="TDestinationViewModel">The type of the destination view model.</typeparam>
		/// <param name="parameter">The parameter.</param>
		public void Navigate<TDestinationViewModel>(object parameter = null)
		{
			var navUri = NavigationUri<TDestinationViewModel>(parameter);
			if(navUri != null)
			{
				this.RootFrame.Navigate(navUri);
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
	}
}
