using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Linq;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using Shane.Church.StirlingMoney.ViewModels;
using Shane.Church.Utility;
using Shane.Church.StirlingMoney.Tiles;


namespace Shane.Church.StirlingMoney
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			Categories = new ObservableCollection<CategoryViewModel>();
			Accounts = new AccountListViewModel();
			Budgets = new ObservableCollection<BudgetSummaryViewModel>();
			Goals = new ObservableCollection<GoalSummaryViewModel>();
		}

		private AccountListViewModel _accounts;
		public AccountListViewModel Accounts
		{
			get { return _accounts; }
			set
			{
				if (_accounts != value)
				{
					_accounts = value;
					NotifyPropertyChanged("Accounts");
				}
			}
		}

		private TransactionListViewModel _transactions;
		public TransactionListViewModel Transactions
		{
			get
			{
				if (_transactions == null)
					_transactions = new TransactionListViewModel();
				return _transactions;
			}
			set
			{
				if (_transactions != value)
				{
					_transactions = value;
					NotifyPropertyChanged("Transactions");
				}
			}
		}

		private ObservableCollection<CategoryViewModel> _categories;
		public ObservableCollection<CategoryViewModel> Categories
		{
			get { return _categories; }
			set
			{
				if (_categories != value)
				{
					_categories = value;
					if (_categories != null)
					{
						_categories.CollectionChanged += delegate
						{
							NotifyPropertyChanged("Categories");
						};
					}
					NotifyPropertyChanged("Categories");
				}
			}
		}

		private ObservableCollection<BudgetSummaryViewModel> _budgets;
		public ObservableCollection<BudgetSummaryViewModel> Budgets
		{
			get { return _budgets; }
			set
			{
				if (_budgets != value)
				{
					_budgets = value;
					if (_budgets != null)
					{
						_budgets.CollectionChanged += delegate
						{
							NotifyPropertyChanged("Budgets");
						};
					}
					NotifyPropertyChanged("Budgets");
				}
			}
		}

		private ObservableCollection<GoalSummaryViewModel> _goals;
		public ObservableCollection<GoalSummaryViewModel> Goals
		{
			get { return _goals; }
			set
			{
				if (_goals != value)
				{
					_goals = value;
					if (_goals != null)
					{
						_goals.CollectionChanged += delegate
						{
							NotifyPropertyChanged("Goals");
						};
					}
					NotifyPropertyChanged("Goals");
				}
			}
		}

		/// <summary>
		/// Creates and adds a few ItemViewModel objects into the Items collection.
		/// </summary>
		public void LoadData()
		{
			Transactions = null;
			this.Accounts.LoadData();
			foreach (AccountViewModel a in this.Accounts.Accounts)
			{
				if (TileUtility.TileExists(a.AccountId))
				{
					TileUtility.AddOrUpdateAccountTile(a.AccountId, a.AccountName, a.AccountBalance);
				}
			}
		}

		public void LoadCategories()
		{
			Transactions = null;
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				this.Categories.Clear();
#if !PERSONAL
				foreach (Category c in _context.Categories.OrderBy(m => m.CategoryName))
#else
			foreach (Category c in ContextInstance.Context.CategoryCollection.OrderBy(m => m.CategoryName))
#endif
				{
					Categories.Add(new CategoryViewModel() { CategoryId = c.CategoryId, CategoryName = c.CategoryName });
				}
#if !PERSONAL
			}
#endif
		}

		public void LoadBudgets()
		{
			Transactions = null;
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				this.Budgets.Clear();
#if !PERSONAL
				foreach (Budget b in _context.Budgets.OrderBy(m => m.BudgetName))
#else
				foreach (Budget b in ContextInstance.Context.BudgetCollection.OrderBy(m => m.BudgetName))
#endif
				{
					BudgetSummaryViewModel model = new BudgetSummaryViewModel();
					model.LoadData(b.BudgetId);
					Budgets.Add(model);
				}
#if !PERSONAL
			}
#endif
		}

		public void LoadGoals()
		{
			Transactions = null;
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				this.Goals.Clear();
#if !PERSONAL
				foreach (Goal g in _context.Goals.OrderBy(m => m.GoalName))
#else
			foreach (Goal g in ContextInstance.Context.GoalCollection.OrderBy(m => m.GoalName))
#endif
				{
					GoalSummaryViewModel model = new GoalSummaryViewModel();
					model.LoadData(g.GoalId);
					Goals.Add(model);
				}
#if !PERSONAL
			}
#endif
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}