using System;
using System.Net;
using System.Windows;
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

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class AddEditBudgetViewModel : INotifyPropertyChanged
	{
		public AddEditBudgetViewModel()
		{
			TypeList = new ObservableCollection<string>();
			PeriodList = new List<string>();
			PeriodList.Add(Resources.ViewModelResources.BudgetWeekly);
			PeriodList.Add(Resources.ViewModelResources.BudgetMonthly);
			PeriodList.Add(Resources.ViewModelResources.BudgetYearly);
			PeriodList.Add(Resources.ViewModelResources.BudgetCustom);
		}

		private Guid? _budgetId;
		public Guid? BudgetId
		{
			get { return _budgetId; }
			set
			{
				if (_budgetId != value)
				{
					_budgetId = value;
					NotifyPropertyChanged("BudgetId");
					NotifyPropertyChanged("TitleText");
				}
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		private double _amount;
		public double Amount
		{
			get { return _amount; }
			set
			{
				if (_amount != value)
				{
					_amount = value;
					NotifyPropertyChanged("Amount");
				}
			}
		}

		private ObservableCollection<string> _typeList;
		public ObservableCollection<string> TypeList
		{
			get { return _typeList; }
			set
			{
				if (_typeList != value)
				{
					_typeList = value;
					if (_typeList != null)
					{
						_typeList.CollectionChanged += delegate
						{
							NotifyPropertyChanged("TypeList");
						};
					}
					NotifyPropertyChanged("TypeList");
				}
			}
		}

		private string _type;
		public string Type
		{
			get { return _type; }
			set
			{
				if (_type != value)
				{
					_type = value;
					NotifyPropertyChanged("Type");
				}
			}
		}

		private List<string> _periodList;
		public List<string> PeriodList
		{
			get { return _periodList; }
			set
			{
				if (_periodList != value)
				{
					_periodList = value;
					NotifyPropertyChanged("PeriodList");
				}
			}
		}

		private string _period;
		public string Period
		{
			get { return _period; }
			set
			{
				if (_period != value)
				{
					_period = value;
					NotifyPropertyChanged("Period");
					NotifyPropertyChanged("EndDateVisibility");
				}
			}
		}

		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				if (_startDate != value)
				{
					_startDate = value;
					NotifyPropertyChanged("StartDate");
				}
			}
		}

		private DateTime? _endDate;
		public DateTime? EndDate
		{
			get { return _endDate; }
			set
			{
				if (_endDate != value)
				{
					_endDate = value;
					NotifyPropertyChanged("EndDate");
				}
			}
		}

		public Visibility EndDateVisibility
		{
			get
			{
				if (Period == Resources.ViewModelResources.BudgetCustom)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			}
		}

		public string TitleText
		{
			get
			{
				if (BudgetId.HasValue)
				{
					return Resources.ViewModelResources.EditBudgetTitle;
				}
				else
				{
					return Resources.ViewModelResources.AddBudgetTitle;
				}
			}
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

		public void LoadData(Guid? budgetId)
		{
			TypeList.Clear();
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				foreach (Category c in _context.Categories.OrderBy(m => m.CategoryName))
#else
			foreach (Category c in ContextInstance.Context.CategoryCollection.OrderBy(m => m.CategoryName))
#endif
				{
					TypeList.Add(c.CategoryName);
				}
				if (TypeList.Count > 0)
				{
					TypeList.Insert(0, Resources.ViewModelResources.BudgetAllExpensesLabel);
				}

				if (budgetId.HasValue && budgetId.Value != Guid.Empty)
				{
#if !PERSONAL
					Budget b = (from bd in _context.Budgets
#else
					Budget b = (from bd in ContextInstance.Context.BudgetCollection
#endif
								where bd.BudgetId == budgetId.Value
								select bd).FirstOrDefault();
					BudgetId = b.BudgetId;
					Name = b.BudgetName;
					Amount = b.BudgetAmount;
					StartDate = b.StartDate;
					EndDate = b.EndDate;

					if (b.CategoryId.HasValue)
					{
#if !PERSONAL
						Type = (from c in _context.Categories
#else
						Type = (from c in ContextInstance.Context.CategoryCollection
#endif
								where c.CategoryId == b.CategoryId.Value
								select c.CategoryName).FirstOrDefault();
					}
					else
					{
						Type = Resources.ViewModelResources.BudgetAllExpensesLabel;
					}

					if (b.BudgetPeriod == 0)
						Period = Resources.ViewModelResources.BudgetWeekly;
					else if (b.BudgetPeriod == 1)
						Period = Resources.ViewModelResources.BudgetMonthly;
					else if (b.BudgetPeriod == 2)
						Period = Resources.ViewModelResources.BudgetYearly;
					else
						Period = Resources.ViewModelResources.BudgetCustom;
				}
				else
				{
					BudgetId = null;
					Name = null;
					Amount = 0;
					StartDate = DateTime.Today;
					EndDate = null;
					Type = Resources.ViewModelResources.BudgetAllExpensesLabel;
					Period = Resources.ViewModelResources.BudgetMonthly;
				}
#if !PERSONAL
			}
#endif
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationErrors.Add(Resources.ViewModelResources.BudgetNameRequiredError);
			}

			if (Amount <= 0)
			{
				validationErrors.Add(Resources.ViewModelResources.BudgetAmountRequiredError);
			}

			if (Period == Resources.ViewModelResources.BudgetCustom)
			{
				if (!EndDate.HasValue)
				{
					validationErrors.Add(Resources.ViewModelResources.BudgetEndDateRequiredError);

				}
				else if (EndDate <= StartDate)
				{
					validationErrors.Add(Resources.ViewModelResources.BudgetEndDateRangeError);
				}
			}

			return validationErrors;
		}

		public void SaveBudget()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				Budget b = new Budget();
				if (BudgetId.HasValue && BudgetId.Value != Guid.Empty)
				{
#if !PERSONAL
					var budgetQuery = (from bd in _context.Budgets
#else
					var budgetQuery = (from bd in ContextInstance.Context.BudgetCollection
#endif
									   where bd.BudgetId == BudgetId.Value
									   select bd);
					if (budgetQuery.Any())
					{
						b = budgetQuery.First();
					}
				}

				b.BudgetName = Name;
				b.BudgetAmount = Amount;
				b.StartDate = StartDate;
				if (Type != Resources.ViewModelResources.BudgetAllExpensesLabel)
				{
#if !PERSONAL
					b.CategoryId = (from c in _context.Categories
#else
					b.CategoryId = (from c in ContextInstance.Context.CategoryCollection
#endif
									where c.CategoryName == Type
									select c.CategoryId).FirstOrDefault();
				}
				else
				{
					b.CategoryId = null;
				}
				if (Period == Resources.ViewModelResources.BudgetWeekly)
				{
					b.BudgetPeriod = 0;
					b.EndDate = null;
				}
				else if (Period == Resources.ViewModelResources.BudgetMonthly)
				{
					b.BudgetPeriod = 1;
					b.EndDate = null;
				}
				else if (Period == Resources.ViewModelResources.BudgetYearly)
				{
					b.BudgetPeriod = 2;
					b.EndDate = null;
				}
				else if (Period == Resources.ViewModelResources.BudgetCustom)
				{
					b.BudgetPeriod = 3;
					b.EndDate = EndDate;
				}

				if (!(BudgetId.HasValue && BudgetId.Value != Guid.Empty))
				{
					b.BudgetId = Guid.NewGuid();
#if !PERSONAL
					_context.Budgets.InsertOnSubmit(b);
#else
					ContextInstance.Context.AddBudget(b);
#endif
				}
#if !PERSONAL
				_context.SubmitChanges();
			}
#else
			ContextInstance.Context.SaveChanges();
#endif
		}
	}
}
