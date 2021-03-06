﻿using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.ViewModels.Reports
{
	public class ReportsViewModel : ObservableObject
	{
		private IDataRepository<Category, Guid> _categoryRepo;
		private IDataRepository<Budget, Guid> _budgetRepo;
		private IDataRepository<Transaction, Guid> _transactionRepo;

		public ReportsViewModel(IDataRepository<Category, Guid> categoryRepo, IDataRepository<Budget, Guid> budgetRepo, IDataRepository<Transaction, Guid> transactionRepo)
		{
			if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
			_categoryRepo = categoryRepo;
			if (budgetRepo == null) throw new ArgumentNullException("budgetRepo");
			_budgetRepo = budgetRepo;
			if (transactionRepo == null) throw new ArgumentNullException("transactionRepo");
			_transactionRepo = transactionRepo;

			BudgetList = new ObservableCollection<ListDataItem>();
			BudgetList.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => BudgetList);
			};
			BudgetReportCollection = new ObservableCollection<BudgetReportItem>();
			BudgetReportCollection.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => BudgetReportCollection);
			};
			CategoryList = new ObservableCollection<ListDataItem>();
			CategoryList.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => CategoryList);
				RaisePropertyChanged(() => IsCategoryPaneVisible);
			};
			SpendingByCategoryReportCollection = new ObservableCollection<BasicChartItem>();
			SpendingByCategoryReportCollection.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => SpendingByCategoryReportCollection);
			};
			SpendingByCategoryHistoryReportCollection = new ObservableCollection<BasicChartItem>();
			SpendingByCategoryHistoryReportCollection.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => SpendingByCategoryHistoryReportCollection);
			};
			_chartTypeList = new List<ListDataItem>() { 
				new ListDataItem() { Text = Strings.Resources.CategoryChartTypeByCategory, Value = Strings.Resources.CategoryChartTypeByCategory }, 
				new ListDataItem() { Text = Strings.Resources.CategoryChartTypeHistory, Value = Strings.Resources.CategoryChartTypeHistory } };
			ChartType = ChartTypeList.First();
			NetIncomeReportCollection = new ObservableCollection<NetIncomeReportItem>();
			NetIncomeReportCollection.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => NetIncomeReportCollection);
			};
		}

		private ObservableCollection<ListDataItem> _budgetList;
		public ObservableCollection<ListDataItem> BudgetList
		{
			get { return _budgetList; }
			private set
			{
				Set(() => BudgetList, ref _budgetList, value);
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
				if (Set(() => Budget, ref _budget, value))
				{
					if (_budget != null)
						LoadBudgetReport((Guid)_budget.Value);
				}
			}
		}

		private ObservableCollection<BudgetReportItem> _budgetReportCollection;
		public ObservableCollection<BudgetReportItem> BudgetReportCollection
		{
			get { return _budgetReportCollection; }
			private set
			{
				Set(() => BudgetReportCollection, ref _budgetReportCollection, value);
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
				if (Set(() => ChartType, ref _chartType, value))
				{
					RaisePropertyChanged(() => IsSpendingByCategoryReportVisible);
					RaisePropertyChanged(() => IsSpendingByCategoryHistoryReportVisible);
				}
			}
		}

		public bool IsSpendingByCategoryReportVisible
		{
			get
			{
				if (ChartType != null && ChartType.Text == Resources.CategoryChartTypeByCategory)
				{
					LoadSpendingByCategoryReportData();
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private int _spendingByCategoryMonths = 1;
		public int SpendingByCategoryMonths
		{
			get { return _spendingByCategoryMonths; }
			set
			{
				if (Set(() => SpendingByCategoryMonths, ref _spendingByCategoryMonths, value))
				{
					RaisePropertyChanged(() => SpendingByCategoryStartDisplay);
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
			private set
			{
				Set(() => SpendingByCategoryReportCollection, ref _spendingByCategoryReportCollection, value);
			}
		}

		public bool IsSpendingByCategoryHistoryReportVisible
		{
			get
			{
				if (ChartType != null && ChartType.Text == Resources.CategoryChartTypeHistory)
				{
					LoadSpendingByCategoryHistoryReportData();
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private int _spendingByCategoryHistoryMonths = 6;
		public int SpendingByCategoryHistoryMonths
		{
			get { return _spendingByCategoryHistoryMonths; }
			set
			{
				if (Set(() => SpendingByCategoryHistoryMonths, ref _spendingByCategoryHistoryMonths, value))
				{
					RaisePropertyChanged(() => SpendingByCategoryHistoryStartDisplay);
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
			private set
			{
				Set(() => SpendingByCategoryHistoryReportCollection, ref _spendingByCategoryHistoryReportCollection, value);
			}
		}

		private ListDataItem _category;
		public ListDataItem Category
		{
			get { return _category; }
			set
			{
				if (Set(() => Category, ref _category, value))
				{
					RaisePropertyChanged(() => CategoryDisplay);
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
			private set
			{
				Set(() => CategoryList, ref _categoryList, value);
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
				if (Set(() => NetIncomeMonths, ref _netIncomeMonths, value))
				{
					RaisePropertyChanged(() => NetIncomeStartDisplay);
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
			private set
			{
				Set(() => NetIncomeReportCollection, ref _netIncomeReportCollection, value);
			}
		}

		public void LoadData()
		{
            var budgets = _budgetRepo.GetAllEntries();
			foreach (var b in budgets)
			{
				ListDataItem item = new ListDataItem() { Text = b.BudgetName, Value = b.BudgetId };
				BudgetList.Add(item);
			}
			if (BudgetList.Count > 0)
			{
				Budget = BudgetList.First();
			}

            var categories = _categoryRepo.GetAllEntries();
			foreach (var c in categories)
			{
				ListDataItem item = new ListDataItem() { Text = c.CategoryName, Value = c.CategoryId };
				CategoryList.Add(item);
			}
			if (CategoryList.Count > 0)
			{
				Category = CategoryList.First();
			}
			LoadNetIncomeReportData();
		}

		public void LoadBudgetReport(Guid BudgetId)
		{
			var budget = _budgetRepo.GetEntryAsync(BudgetId).Result;
			if (budget != null)
			{
				BudgetReportCollection.Clear();
				for (int i = 6; i >= 0; i--)
				{
					var StartDate = new DateTimeOffset(DateTimeOffset.Now.Date, DateTimeOffset.Now.Offset);
					var EndDate = new DateTimeOffset(DateTimeOffset.Now.Date, DateTimeOffset.Now.Offset);
					BudgetReportItem item = new BudgetReportItem();
					item.Target = budget.BudgetAmount;
					switch (budget.BudgetPeriod)
					{
						case PeriodType.Weekly:
							StartDate = budget.CurrentPeriodStart.AddDays(7 * (-i));
							EndDate = budget.CurrentPeriodEnd.AddDays(7 * (-i));
							item.Label = StartDate.ToString("d");
							break;
						case PeriodType.Monthly:
						default:
							StartDate = budget.CurrentPeriodStart.AddMonths(-i);
							EndDate = budget.CurrentPeriodEnd.AddMonths(-i);
							item.Label = StartDate.ToString("MMM");
							break;
						case PeriodType.Yearly:
							StartDate = budget.CurrentPeriodStart.AddYears(-i);
							EndDate = budget.CurrentPeriodEnd.AddYears(-i);
							item.Label = StartDate.ToString("YYYY");
							break;
					}
                    //var transactionDateKeys = _transactionRepo.GetFilteredEntries(it=> it.TransactionDate >= StartDate && it.TransactionDate < EndDate)
                    //	.Select(it => it.TransactionId).ToList();
                    //var transactionKeys = _transactionRepo.GetAllIndexKeys<string>("Location")
                    //	.Where(it => !it.Value.Contains(Strings.Resources.TransferToComparisonString) && !it.Value.Contains(Strings.Resources.TransferFromComparisonString))
                    //	.Select(it => it.Key).ToList().Intersect(transactionDateKeys).ToList();
                    if (!budget.CategoryId.HasValue)
					{
                        //var amountSpentQuery = _transactionRepo.GetAllIndexKeys<Tuple<Guid, Double>>("TransactionCategoryAmount")
                        //	.Where(it => transactionKeys.Contains(it.Key) && it.Value.Item2 < 0).Select(it => it.Value.Item2);
                        //var amountSpentQuery = _transactionRepo.GetFilteredEntries(it => (!it.Location.Contains(Strings.Resources.TransferToComparisonString) && !it.Location.Contains(Strings.Resources.TransferFromComparisonString)) &&
                        //                                                                (it.TransactionDate >= StartDate && it.TransactionDate < EndDate) &&
                        //                                                                it.Amount < 0).Select(it => -it.Amount);
                        var amountSpentQuery = _transactionRepo.GetFilteredEntries(string.Format("NOT [Location] LIKE '%{0}%' AND NOT [Location] LIKE '%{1}%' AND [TransactionDate] >= {2} AND [TransactionDate] < {3} AND [Amount] < 0",
                            Strings.Resources.TransferToComparisonString,
                            Strings.Resources.TransferFromComparisonString,
                            StartDate.Ticks,
                            EndDate.Ticks)).Select(it => -it.Amount);
						if (amountSpentQuery.Any())
						{
							item.Actual = amountSpentQuery.Sum();
						}
						else
						{
							item.Actual = 0;
						}
					}
					else
					{
                        //var amountSpentQuery = _transactionRepo.GetAllIndexKeys<Tuple<Guid, Double>>("TransactionCategoryAmount")
                        //	.Where(it => transactionKeys.Contains(it.Key) && it.Value.Item1.Equals(budget.CategoryId.Value) && it.Value.Item2 < 0).Select(it => it.Value.Item2);
                        //var amountSpentQuery = _transactionRepo.GetFilteredEntries(it => (!it.Location.Contains(Strings.Resources.TransferToComparisonString) && !it.Location.Contains(Strings.Resources.TransferFromComparisonString)) &&
                        //                                                                (it.TransactionDate >= StartDate && it.TransactionDate < EndDate) &&
                        //                                                                it.CategoryId == budget.CategoryId.Value &&
                        //                                                                it.Amount < 0).Select(it => -it.Amount);                  
                        var amountSpentQuery = _transactionRepo.GetFilteredEntries(string.Format("NOT [Location] LIKE '%{0}%' AND NOT [Location] LIKE '%{1}%' AND [TransactionDate] >= {2} AND [TransactionDate] < {3} AND [Amount] < 0 AND [CategoryId] = {4}",
                            Strings.Resources.TransferToComparisonString,
                            Strings.Resources.TransferFromComparisonString,
                            StartDate.Ticks,
                            EndDate.Ticks, 
                            budget.CategoryId.Value)).Select(it => -it.Amount);
                        if (amountSpentQuery.Any())
						{
							item.Actual = amountSpentQuery.Sum();
						}
						else
						{
							item.Actual = 0;
						}
					}
					BudgetReportCollection.Add(item);
				}
			}
		}

		public delegate void SpendingByCategoryReportReloadedHandler();
		public event SpendingByCategoryReportReloadedHandler SpendingByCategoryReportReloaded;

		public void LoadSpendingByCategoryReportData()
		{
			var MonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			var MonthEnd = MonthStart.AddMonths(1);
			SpendingByCategoryReportCollection.Clear();
            //var transactionDateKeys = _transactionRepo.GetAllIndexKeys<Tuple<DateTimeOffset, DateTimeOffset>>("TransactionDateEditDateTime")
            //	.Where(it => it.Value.Item1 >= MonthStart.AddMonths(-(SpendingByCategoryMonths - 1)) && it.Value.Item1 < MonthEnd)
            //	.Select(it => it.Key).ToList();
            //var transactionKeys = _transactionRepo.GetAllIndexKeys<string>("Location")
            //	.Where(it => !it.Value.Contains(Strings.Resources.TransferToComparisonString) && !it.Value.Contains(Strings.Resources.TransferFromComparisonString))
            //	.Select(it => it.Key).ToList().Intersect(transactionDateKeys).ToList();
            //var categories = _transactionRepo.GetAllIndexKeys<Tuple<Guid, Double>>("TransactionCategoryAmount")
            //	.Where(it => transactionKeys.Contains(it.Key) && it.Value.Item2 < 0).Select(it => it.Value);
            //var categories = _transactionRepo.GetFilteredEntries(t => t.TransactionDate >= MonthStart.AddMonths(-(SpendingByCategoryMonths - 1)) &&
            //                                                         t.TransactionDate < MonthEnd &&
            //                                                         !t.Location.Contains(Strings.Resources.TransferToComparisonString) &&
            //                                                         !t.Location.Contains(Strings.Resources.TransferFromComparisonString) &&
            //                                                         t.Amount < 0).GroupBy(it => it.CategoryId);
            var categories = _transactionRepo.GetFilteredEntries(string.Format("NOT [Location] LIKE '%{0}%' AND NOT [Location] LIKE '%{1}%' AND [TransactionDate] >= {2} AND [TransactionDate] < {3} AND [Amount] < 0",
                Strings.Resources.TransferToComparisonString,
                Strings.Resources.TransferFromComparisonString,
                MonthStart.AddMonths(-(SpendingByCategoryMonths - 1)).Ticks,
                MonthEnd.Ticks)).GroupBy(it=>it.CategoryId);
            List<BasicChartItem> items = new List<BasicChartItem>();
			foreach (var g in categories)
			{
                var entry = _categoryRepo.GetEntry(g.Key);
                if (entry != null)
                {
                    BasicChartItem item = new BasicChartItem();
                    item.Title = entry.CategoryName;
                    try
                    {
                        item.Value = -g.Sum(m => m.Amount);
                    }
                    catch { item.Value = 0; }
                    items.Add(item);
                }
            }
			var displayItems = items.OrderByDescending(m => m.Value);
			foreach (var i in displayItems.Take(9))
			{
				SpendingByCategoryReportCollection.Add(i);
			}
			if (displayItems.Count() > 9)
			{
				BasicChartItem other = new BasicChartItem();
				other.Title = Resources.OtherLabel;
				try
				{
					if (displayItems.Skip(9).Any())
					{
						other.Value = displayItems.Skip(9).Sum(m => m.Value);
					}
					else
						other.Value = 0;
				}
				catch { other.Value = 0; }
				SpendingByCategoryReportCollection.Add(other);
			}
			if (SpendingByCategoryReportReloaded != null)
				SpendingByCategoryReportReloaded();
		}

		public void LoadSpendingByCategoryHistoryReportData()
		{
			SpendingByCategoryHistoryReportCollection.Clear();
			for (int i = SpendingByCategoryHistoryMonths - 1; i >= 0; i--)
			{
				var EndDate = DateTime.Today.AddMonths(-i);
				var MonthStart = new DateTime(EndDate.Year, EndDate.Month, 1);
				var MonthEnd = MonthStart.AddMonths(1);
                //var transactionKeys = _transactionRepo.GetAllIndexKeys<Tuple<DateTimeOffset, DateTimeOffset>>("TransactionDateEditDateTime")
                //	.Where(it => it.Value.Item1 >= MonthStart && it.Value.Item1 < MonthEnd)
                //	.Select(it => it.Key).ToList();
                //var amountSpentQuery = _transactionRepo.GetAllIndexKeys<Tuple<Guid, Double>>("TransactionCategoryAmount")
                //	.Where(it => transactionKeys.Contains(it.Key) && it.Value.Item1.Equals(Category.Value)).Select(it => it.Value.Item2);
                //var amountSpentQuery = _transactionRepo.GetFilteredEntries(it => it.TransactionDate >= MonthStart &&
                //                                                                it.TransactionDate < MonthEnd &&
                //                                                                it.CategoryId.Equals(Category.Value));
                var amountSpentQuery = _transactionRepo.GetFilteredEntries(string.Format("[TransactionDate] >= {0} AND [TransactionDate] < {1} AND [CategoryId] = '{2}'",
                    MonthStart.Ticks,
                    MonthEnd.Ticks,
                    Category.Value));
                BasicChartItem item = new BasicChartItem();
				item.Title = EndDate.ToString("MMM");
				try
				{
					if (amountSpentQuery.Any())
					{
                        item.Value = -amountSpentQuery.Select(it => it.Amount).Sum();
					}
					else { item.Value = 0; }
				}
				catch { item.Value = 0; }
				SpendingByCategoryHistoryReportCollection.Add(item);
			}
		}

		private void LoadNetIncomeReportData()
		{
			NetIncomeReportCollection.Clear();
			for (int i = NetIncomeMonths - 1; i >= 0; i--)
			{
				var EndDate = DateTime.Today.AddMonths(-i);
				var MonthStart = new DateTime(EndDate.Year, EndDate.Month, 1);
				var MonthEnd = MonthStart.AddMonths(1);
                //var transactionDateKeys = _transactionRepo.GetAllIndexKeys<Tuple<DateTimeOffset, DateTimeOffset>>("TransactionDateEditDateTime")
                //	.Where(it => it.Value.Item1 >= MonthStart && it.Value.Item1 < MonthEnd)
                //	.Select(it => it.Key).ToList();
                //var transactionKeys = _transactionRepo.GetAllIndexKeys<string>("Location")
                //	.Where(it => !it.Value.Contains(Strings.Resources.TransferToComparisonString) && !it.Value.Contains(Strings.Resources.TransferFromComparisonString))
                //	.Select(it => it.Key).ToList().Intersect(transactionDateKeys).ToList();
                //var income = _transactionRepo.GetAllIndexKeys<Tuple<Guid, Double>>("TransactionCategoryAmount")
                //	.Where(it => transactionKeys.Contains(it.Key) && it.Value.Item2 > 0).Select(it => it.Value.Item2);
                //var expenses = _transactionRepo.GetAllIndexKeys<Tuple<Guid, Double>>("TransactionCategoryAmount")
                //	.Where(it => transactionKeys.Contains(it.Key) && it.Value.Item2 < 0).Select(it => it.Value.Item2);
                //var income = _transactionRepo.GetFilteredEntries(it => it.TransactionDate >= MonthStart &&
                //                                                        it.TransactionDate < MonthEnd &&
                //                                                        !it.Location.Contains(Strings.Resources.TransferToComparisonString) &&
                //                                                        !it.Location.Contains(Strings.Resources.TransferFromComparisonString) &&
                //                                                        it.Amount > 0);
                var income = _transactionRepo.GetFilteredEntries(string.Format("NOT [Location] LIKE '%{0}%' AND NOT [Location] LIKE '%{1}%' AND [TransactionDate] >= '{2}' AND [TransactionDate] < {3} AND [Amount] > 0",
                    Strings.Resources.TransferToComparisonString,
                    Strings.Resources.TransferFromComparisonString,
                    MonthStart.Ticks,
                    MonthEnd.Ticks));
                //var expenses = _transactionRepo.GetFilteredEntries(it => it.TransactionDate >= MonthStart &&
                //                                                        it.TransactionDate < MonthEnd &&
                //                                                        !it.Location.Contains(Strings.Resources.TransferToComparisonString) &&
                //                                                        !it.Location.Contains(Strings.Resources.TransferFromComparisonString) &&
                //                                                        it.Amount < 0);
                var expenses = _transactionRepo.GetFilteredEntries(string.Format("NOT [Location] LIKE '%{0}%' AND NOT [Location] LIKE '%{1}%' AND [TransactionDate] >= '{2}' AND [TransactionDate] < {3} AND [Amount] < 0",
                    Strings.Resources.TransferToComparisonString,
                    Strings.Resources.TransferFromComparisonString,
                    MonthStart.Ticks,
                    MonthEnd.Ticks));
                NetIncomeReportItem item = new NetIncomeReportItem();
				item.Label = EndDate.ToString("MMM");
				try
				{
                    if (expenses.Any())
                        item.Expenses = -expenses.Select(it => it.Amount).Sum();
                    else
                        item.Expenses = 0;
				}
				catch { item.Expenses = 0; }
				try
				{
                    if (income.Any())
                        item.Income = income.Select(it => it.Amount).Sum();
                    else
                        item.Income = 0;
				}
				catch { item.Income = 0; }
				NetIncomeReportCollection.Add(item);
			}
		}
	}
}