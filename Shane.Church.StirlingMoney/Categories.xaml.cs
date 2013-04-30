using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using Microsoft.Phone.Shell;

namespace Shane.Church.StirlingMoney
{
	public partial class Categories : PhoneApplicationPage
	{
		public Categories()
		{
			InitializeComponent();

			InitializeAdControl();

			BuildApplicationBar();

			App.ViewModel.LoadCategories();
			DataContext = App.ViewModel;
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
			{
				AdControl.ApplicationId = "test_client";
				AdControl.AdUnitId = "Image480_80";
			}
			else
			{
				AdControl.ApplicationId = "081af108-c899-401e-a44a-b54e303f12dc";
				AdControl.AdUnitId = "92173";
			}
#if PERSONAL
			AdControl.IsEnabled = false;
			AdControl.Height = 0;
#endif
		}

		private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
		{
			AdControl.Height = 0;
		}

		private void AdControl_AdRefreshed(object sender, EventArgs e)
		{
			AdControl.Height = 80;
		}
		#endregion

		/// <summary>
		/// Builds a localized application bar
		/// </summary>
		private void BuildApplicationBar()
		{
			ApplicationBar = new ApplicationBar();
			ApplicationBar.IsVisible = true;
			ApplicationBar.IsMenuEnabled = true;

			ApplicationBarIconButton appBarIconButtonAddCategory = new ApplicationBarIconButton(new Uri("/Images/AddCategory.png", UriKind.Relative));
			appBarIconButtonAddCategory.Text = Shane.Church.StirlingMoney.Resources.AppResources.AppBarAdd;
			appBarIconButtonAddCategory.Click += new EventHandler(appBarIconButtonAddCategory_Click);
			ApplicationBar.Buttons.Add(appBarIconButtonAddCategory);
		}

		private void appBarIconButtonAddCategory_Click(object sender, EventArgs e)
		{
			PhoneApplicationService.Current.State["CurrentCategory"] = null;
			NavigationService.Navigate(new Uri(@"/AddEditCategory.xaml", UriKind.Relative));
		}

		private void menuItemDelete_Click(object sender, RoutedEventArgs e)
		{
			Guid categoryId = (Guid)(sender as MenuItem).Tag;

#if PERSONAL
			var category = (from c in ContextInstance.Context.CategoryCollection
							where c.CategoryId == categoryId
							select c).FirstOrDefault();
			var transactions = (from t in ContextInstance.Context.TransactionCollection
								where t.CategoryId.HasValue && t.CategoryId.Value == categoryId
								select t);
			foreach (Transaction t in transactions)
			{
				t.CategoryId = null;
			}
			ContextInstance.Context.DeleteCategory(category);
			ContextInstance.Context.SaveChanges();
#else
			using (var context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				var category = (from c in context.Categories
								where c.CategoryId == categoryId
								select c).FirstOrDefault();
				var transactions = (from t in context.Transactions
									where t.CategoryId.HasValue && t.CategoryId.Value == categoryId
									select t);
				foreach (Transaction t in transactions)
				{
					t.CategoryId = null;
				}
				context.Categories.DeleteOnSubmit(category);
				context.SubmitChanges();
			}
#endif

			App.ViewModel.LoadCategories();
		}

		private void menuItemEdit_Click(object sender, RoutedEventArgs e)
		{
			Guid? categoryId = (sender as MenuItem).Tag as Guid?;
			PhoneApplicationService.Current.State["CurrentCategory"] = categoryId;
			NavigationService.Navigate(new Uri(@"/AddEditCategory.xaml", UriKind.Relative));
		}
	}
}