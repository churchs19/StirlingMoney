using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using System.Windows;
using System.Windows.Media;
using Shane.Church.Utility.Colors;

namespace Shane.Church.StirlingMoney.ViewModels.Reports
{
	public class ReportsViewModel : INotifyPropertyChanged
	{
		public ReportsViewModel()
		{
			BudgetList = new ObservableCollection<ListDataItem>();
			BudgetReportCollection = new ObservableCollection<BudgetReportItem>();
			CategoryList = new ObservableCollection<ListDataItem>();
			SpendingByCategoryReportCollection = new ObservableCollection<BasicChartItem>();
			SpendingByCategoryHistoryReportCollection = new ObservableCollection<BasicChartItem>();
			_chartTypeList = new List<ListDataItem>() { 
				new ListDataItem() { Text = Resources.ViewModelResources.CategoryChartTypeByCategory, Value = Resources.ViewModelResources.CategoryChartTypeByCategory }, 
				new ListDataItem() { Text = Resources.ViewModelResources.CategoryChartTypeHistory, Value = Resources.ViewModelResources.CategoryChartTypeHistory } };
			ChartType = ChartTypeList.First();
			NetIncomeReportCollection = new ObservableCollection<NetIncomeReportItem>();
		}

		private ObservableCollection<ListDataItem> _budgetList;
		public ObservableCollection<ListDataItem> BudgetList
		{
			get { return _budgetList; }
			set
			{
				if (_budgetList != value)
				{
					_budgetList = value;
					if (_budgetList != null)
					{
						_budgetList.CollectionChanged += delegate
						{
							NotifyPropertyChanged("BudgetList");
							NotifyPropertyChanged("IsBudgetPaneVisible");
						};
					}
					NotifyPropertyChanged("BudgetList");
					NotifyPropertyChanged("IsBudgetPaneVisible");
				}
			}
		}

		public bool IsBudgetPaneVisible
		{
			get
			{
				return BudgetList.Count > 0;
			}
		}

		private ListDataItem _budget;
		public ListDataItem Budget
		{
			get { return _budget; }
			set
			{
				if (_budget != value)
				{
					_budget = value;
					NotifyPropertyChanged("Budget");
					LoadBudgetReport((Guid)_budget.Value);
				}
			}
		}

		private ObservableCollection<BudgetReportItem> _budgetReportCollection;
		public ObservableCollection<BudgetReportItem> BudgetReportCollection
		{
			get { return _budgetReportCollection; }
			set
			{
				if (_budgetReportCollection != value)
				{
					_budgetReportCollection = value;
					if (_budgetReportCollection != null)
					{
						_budgetReportCollection.CollectionChanged += delegate
						{
							NotifyPropertyChanged("BudgetReportCollection");
						};
					}
					NotifyPropertyChanged("BudgetReportCollection");
				}
			}
		}

		private List<ListDataItem> _chartTypeList;
		public List<ListDataItem> ChartTypeList
		{
			get
			{
				return _chartTypeList;
			}
		}

		private ListDataItem _chartType;
		public ListDataItem ChartType
		{
			get { return _chartType; }
			set
			{
				if (_chartType != value)
				{
					_chartType = value;
					NotifyPropertyChanged("ChartType");
					NotifyPropertyChanged("SpendingByCategoryReportVisibility");
					NotifyPropertyChanged("SpendingByCategoryHistoryReportVisibility");
				}
			}
		}

		public Visibility SpendingByCategoryReportVisibility
		{
			get
			{
				if (ChartType != null && ChartType.Text == Resources.ViewModelResources.CategoryChartTypeByCategory)
				{
					LoadSpendingByCategoryReportData();
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		private int _spendingByCategoryMonths = 1;
		public int SpendingByCategoryMonths
		{
			get { return _spendingByCategoryMonths; }
			set
			{
				if (_spendingByCategoryMonths != value)
				{
					_spendingByCategoryMonths = value;
					NotifyPropertyChanged("SpendingByCategoryMonths");
					NotifyPropertyChanged("SpendingByCategoryStartDisplay");
					LoadSpendingByCategoryReportData();
				}
			}
		}

		public string SpendingByCategoryStartDisplay
		{
			get
			{
				return DateTime.Today.AddMonths(-(SpendingByCategoryMonths - 1)).ToString("MMM yyyy");
			}
		}

		public string SpendingByCategoryEndDisplay
		{
			get
			{
				return DateTime.Today.ToString("MMM yyyy");
			}
		}

		private ObservableCollection<BasicChartItem> _spendingByCategoryReportCollection;
		public ObservableCollection<BasicChartItem> SpendingByCategoryReportCollection
		{
			get { return _spendingByCategoryReportCollection; }
			set
			{
				if (_spendingByCategoryReportCollection != value)
				{
					_spendingByCategoryReportCollection = value;
					if (_spendingByCategoryReportCollection != null)
					{
						_spendingByCategoryReportCollection.CollectionChanged += delegate
						{
							NotifyPropertyChanged("SpendingByCategoryReportCollection");
						};
					}
					NotifyPropertyChanged("SpendingByCategoryReportCollection");
				}
			}
		}

		public Visibility SpendingByCategoryHistoryReportVisibility
		{
			get
			{
				if (ChartType != null && ChartType.Text == Resources.ViewModelResources.CategoryChartTypeHistory)
				{
					LoadSpendingByCategoryHistoryReportData();
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		private int _spendingByCategoryHistoryMonths = 6;
		public int SpendingByCategoryHistoryMonths
		{
			get { return _spendingByCategoryHistoryMonths; }
			set
			{
				if (_spendingByCategoryHistoryMonths != value)
				{
					_spendingByCategoryHistoryMonths = value;
					NotifyPropertyChanged("SpendingByCategoryHistoryMonths");
					NotifyPropertyChanged("SpendingByCategoryHistoryStartDisplay");
					LoadSpendingByCategoryHistoryReportData();
				}
			}
		}

		public string SpendingByCategoryHistoryStartDisplay
		{
			get
			{
				return DateTime.Today.AddMonths(-(SpendingByCategoryHistoryMonths - 1)).ToString("MMM yyyy");
			}
		}

		public string SpendingByCategoryHistoryEndDisplay
		{
			get
			{
				return DateTime.Today.ToString("MMM yyyy");
			}
		}

		private ObservableCollection<BasicChartItem> _spendingByCategoryHistoryReportCollection;
		public ObservableCollection<BasicChartItem> SpendingByCategoryHistoryReportCollection
		{
			get { return _spendingByCategoryHistoryReportCollection; }
			set
			{
				if (_spendingByCategoryHistoryReportCollection != value)
				{
					_spendingByCategoryHistoryReportCollection = value;
					if (_spendingByCategoryHistoryReportCollection != null)
					{
						_spendingByCategoryHistoryReportCollection.CollectionChanged += delegate
						{
							NotifyPropertyChanged("SpendingByCategoryHistoryReportCollection");
						};
					}
					NotifyPropertyChanged("SpendingByCategoryHistoryReportCollection");
				}
			}
		}

		private ListDataItem _category;
		public ListDataItem Category
		{
			get { return _category; }
			set
			{
				if (_category != value)
				{
					_category = value;
					NotifyPropertyChanged("Category");
					NotifyPropertyChanged("CategoryDisplay");
					LoadSpendingByCategoryHistoryReportData();
				}
			}
		}

		public string CategoryDisplay
		{
			get
			{
				if (Category == null)
					return "";
				else
					return Category.Text;
			}
		}

		private ObservableCollection<ListDataItem> _categoryList;
		public ObservableCollection<ListDataItem> CategoryList
		{
			get { return _categoryList; }
			set
			{
				if (_categoryList != value)
				{
					_categoryList = value;
					if (_categoryList != null)
					{
						_categoryList.CollectionChanged += delegate
						{
							NotifyPropertyChanged("CategoryList");
							NotifyPropertyChanged("IsCategoryPaneVisible");
						};
					}
					NotifyPropertyChanged("CategoryList");
					NotifyPropertyChanged("IsCategoryPaneVisible");
				}
			}
		}

		public bool IsCategoryPaneVisible
		{
			get
			{
				return CategoryList.Count > 0;
			}
		}

		private int _netIncomeMonths = 6;
		public int NetIncomeMonths
		{
			get { return _netIncomeMonths; }
			set
			{
				if (_netIncomeMonths != value)
				{
					_netIncomeMonths = value;
					NotifyPropertyChanged("NetIncomeMonths");
					NotifyPropertyChanged("NetIncomeStartDisplay");
					LoadNetIncomeReportData();
				}
			}
		}

		public string NetIncomeStartDisplay
		{
			get
			{
				return DateTime.Today.AddMonths(-(NetIncomeMonths - 1)).ToString("MMM yyyy");
			}
		}

		public string NetIncomeEndDisplay
		{
			get
			{
				return DateTime.Today.ToString("MMM yyyy");
			}
		}

		private ObservableCollection<NetIncomeReportItem> _netIncomeReportCollection;
		public ObservableCollection<NetIncomeReportItem> NetIncomeReportCollection
		{
			get { return _netIncomeReportCollection; }
			set
			{
				if (_netIncomeReportCollection != value)
				{
					_netIncomeReportCollection = value;
					if (_netIncomeReportCollection != null)
					{
						_netIncomeReportCollection.CollectionChanged += delegate
						{
							NotifyPropertyChanged("NetIncomeReportCollection");
						};
					}
					NotifyPropertyChanged("NetIncomeReportCollection");
				}
			}
		}

		public void LoadData()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				foreach (Budget b in _context.Budgets.OrderBy(m => m.BudgetName))
#else
				foreach (Budget b in ContextInstance.Context.BudgetCollection.OrderBy(m=>m.BudgetName))
#endif
				{
					ListDataItem item = new ListDataItem() { Text = b.BudgetName, Value = b.BudgetId };
					BudgetList.Add(item);
				}
				if (BudgetList.Count > 0)
				{
					Budget = BudgetList.First();
				}

#if !PERSONAL
				foreach (Category c in _context.Categories.OrderBy(m => m.CategoryName))
#else
				foreach (Category c in ContextInstance.Context.CategoryCollection.OrderBy(m => m.CategoryName))
#endif
				{
					ListDataItem item = new ListDataItem() { Text = c.CategoryName, Value = c.CategoryId };
					CategoryList.Add(item);
				}
				if (CategoryList.Count > 0)
				{
					Category = CategoryList.First();
				}
#if !PERSONAL
			}
#endif
			LoadNetIncomeReportData();
		}

		public void LoadBudgetReport(Guid BudgetId)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				var budget = (from b in _context.Budgets
							  where b.BudgetId == BudgetId
							  select b).FirstOrDefault();
#else
			var budget = (from b in ContextInstance.Context.BudgetCollection
						  where b.BudgetId == BudgetId
						  select b).FirstOrDefault();
#endif
				if (budget != null)
				{
					BudgetReportCollection.Clear();
					for (int i = 0; i < 7; i++)
					{
						var StartDate = DateTime.Today;
						var EndDate = DateTime.Today;
						BudgetReportItem item = new BudgetReportItem();
						item.Target = budget.BudgetAmount;
						switch (budget.BudgetPeriod)
						{
							case 0:
								StartDate = budget.CurrentPeriodStart.AddDays(7 * (-i));
								EndDate = budget.CurrentPeriodEnd.AddDays(7 * (-i));
								item.Label = StartDate.ToShortDateString();
								break;
							case 1:
							default:
								StartDate = budget.CurrentPeriodStart.AddMonths(-i);
								EndDate = budget.CurrentPeriodEnd.AddMonths(-i);
								item.Label = StartDate.ToString("MMM");
								break;
							case 2:
								StartDate = budget.CurrentPeriodStart.AddMonths(-i);
								EndDate = budget.CurrentPeriodEnd.AddMonths(-i);
								item.Label = StartDate.ToString("YYYY");
								break;
						}
						if (!budget.CategoryId.HasValue)
						{
#if !PERSONAL
							var amountSpentQuery = (from tx in _context.Transactions
													where tx.TransactionDate >= StartDate &&
													tx.TransactionDate < EndDate &&
													tx.Amount < 0 &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
													select tx.Amount);
#else
							var amountSpentQuery = (from tx in ContextInstance.Context.TransactionCollection
													where tx.TransactionDate >= StartDate &&
													tx.TransactionDate < EndDate &&
													tx.Amount < 0 &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
													select tx.Amount);
#endif
							if (amountSpentQuery.Any())
							{
								item.Actual = -amountSpentQuery.Sum();
							}
							else
							{
								item.Actual = 0;
							}
						}
						else
						{
#if !PERSONAL
							var amountSpentQuery = (from tx in _context.Transactions
													where tx.TransactionDate >= StartDate &&
													tx.TransactionDate < EndDate &&
													tx.Amount < 0 &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString) &&
													tx.CategoryId == budget.CategoryId
#else
							var amountSpentQuery = (from tx in ContextInstance.Context.TransactionCollection
													where tx.TransactionDate >= StartDate &&
													tx.TransactionDate < EndDate &&
													tx.Amount < 0 &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
													!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString) &&
													tx.CategoryId == budget.CategoryId
#endif
													select tx.Amount);
							if (amountSpentQuery.Any())
							{
								item.Actual = -amountSpentQuery.Sum();
							}
							else
							{
								item.Actual = 0;
							}
						}
						BudgetReportCollection.Add(item);
					}
				}
#if !PERSONAL
			}
#endif
		}

		public void LoadSpendingByCategoryReportData()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				var MonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
				var MonthEnd = MonthStart.AddMonths(1);
				SpendingByCategoryReportCollection.Clear();
#if !PERSONAL
				var categories = (from tx in _context.Transactions
								  join c in _context.Categories on tx.CategoryId equals c.CategoryId into outer
#else
				var categories = (from tx in ContextInstance.Context.TransactionCollection
								  join c in ContextInstance.Context.CategoryCollection on tx.CategoryId equals c.CategoryId into outer
#endif
								  from o in outer.DefaultIfEmpty()
								  where tx.Amount < 0 &&
									tx.TransactionDate < MonthEnd &&
									tx.TransactionDate >= MonthStart.AddMonths(-(SpendingByCategoryMonths - 1)) &&
									!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
									!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
								  select new { Name = ((o == null) ? Resources.ViewModelResources.NoCategoryLabel : o.CategoryName), Value = tx.Amount });
				List<BasicChartItem> items = new List<BasicChartItem>();
				foreach (var g in categories.GroupBy(m => m.Name))
				{
					BasicChartItem item = new BasicChartItem();
					item.Title = g.Key;
					try
					{
						if (g.Any())
							item.Value = -g.Sum(m => m.Value);
						else
							item.Value = 0;
					}
					catch { item.Value = 0; }
					items.Add(item);
				}
				var displayItems = items.OrderByDescending(m => m.Value);
				foreach (var i in displayItems.Take(9))
				{
					SpendingByCategoryReportCollection.Add(i);
				}
				if (displayItems.Count() > 9)
				{
					BasicChartItem other = new BasicChartItem();
					other.Title = Resources.ViewModelResources.OtherLabel;
					try
					{
						if (displayItems.Skip(9).Any())
							other.Value = -displayItems.Skip(9).Sum(m => m.Value);
						else
							other.Value = 0;
					}
					catch { other.Value = 0; }
					SpendingByCategoryReportCollection.Add(other);
				}
#if !PERSONAL
			}
#endif
		}

		public void LoadSpendingByCategoryHistoryReportData()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				SpendingByCategoryHistoryReportCollection.Clear();
				for (int i = 0; i < SpendingByCategoryHistoryMonths; i++)
				{
					var EndDate = DateTime.Today.AddMonths(-i);
					var MonthStart = new DateTime(EndDate.Year, EndDate.Month, 1);
					var MonthEnd = MonthStart.AddMonths(1);
#if !PERSONAL
					var total = (from tx in _context.Transactions
								 join c in _context.Categories on tx.CategoryId equals c.CategoryId
#else
					var total = (from tx in ContextInstance.Context.TransactionCollection
								 join c in ContextInstance.Context.CategoryCollection on tx.CategoryId equals c.CategoryId
#endif
								 where c.CategoryId == (Guid)Category.Value &&
								   tx.Amount < 0 &&
								   tx.TransactionDate >= MonthStart &&
								   tx.TransactionDate < MonthEnd &&
								   !tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
								   !tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
								 select -tx.Amount);
					BasicChartItem item = new BasicChartItem();
					item.Title = EndDate.ToString("MMM");
					try
					{
						if (total.Any())
						{
							item.Value = total.Sum();
						}
						else { item.Value = 0; }
					}
					catch { item.Value = 0; }
					SpendingByCategoryHistoryReportCollection.Add(item);
				}
#if !PERSONAL
			}
#endif

		}

		private void LoadNetIncomeReportData()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				NetIncomeReportCollection.Clear();
				for (int i = 0; i < NetIncomeMonths; i++)
				{
					var EndDate = DateTime.Today.AddMonths(-i);
					var MonthStart = new DateTime(EndDate.Year, EndDate.Month, 1);
					var MonthEnd = MonthStart.AddMonths(1);
#if !PERSONAL
					var expenses = (from tx in _context.Transactions
#else
					var expenses = (from tx in ContextInstance.Context.TransactionCollection
#endif
									where tx.Amount < 0 &&
										tx.TransactionDate >= MonthStart &&
										tx.TransactionDate < MonthEnd &&
										!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
										!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
									select -tx.Amount);
#if !PERSONAL
					var income = (from tx in _context.Transactions
#else
					var income = (from tx in ContextInstance.Context.TransactionCollection
#endif
								  where tx.Amount > 0 &&
									tx.TransactionDate >= MonthStart &&
									tx.TransactionDate <= MonthEnd &&
									!tx.Location.Contains(Resources.ViewModelResources.TransferToComparisonString) &&
									!tx.Location.Contains(Resources.ViewModelResources.TransferFromComparisonString)
								  select tx.Amount);
					NetIncomeReportItem item = new NetIncomeReportItem();
					item.Label = EndDate.ToString("MMM");
					try
					{
						if (expenses.Any())
							item.Expenses = expenses.Sum();
						else
							item.Expenses = 0;
					}
					catch { item.Expenses = 0; }
					try
					{
						if (income.Any())
							item.Income = income.Sum();
						else
							item.Income = 0;
					}
					catch { item.Income = 0; }
					NetIncomeReportCollection.Add(item);
				}
#if !PERSONAL
			}
#endif
		}

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
