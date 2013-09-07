using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Properties;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AddEditBudgetViewModel : ObservableObject
	{
		public AddEditBudgetViewModel()
		{
			_typeList = new ObservableCollection<string>();
			_typeList.CollectionChanged += (s, e) =>
				{
					RaisePropertyChanged(() => TypeList);
				};
			_periodList = new List<string>();
			PeriodList.Add(Resources.BudgetWeekly);
			PeriodList.Add(Resources.BudgetMonthly);
			PeriodList.Add(Resources.BudgetYearly);
			PeriodList.Add(Resources.BudgetCustom);

			SaveCommand = new RelayCommand(SaveBudget);
		}

		private long? _id = null;
		private bool? _isDeleted = null;

		private Guid? _budgetId;
		public Guid? BudgetId
		{
			get { return _budgetId; }
			set
			{
				if (Set(() => BudgetId, ref _budgetId, value))
					RaisePropertyChanged(() => TitleText);
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				Set(() => Name, ref _name, value);
			}
		}

		private double _amount;
		public double Amount
		{
			get { return _amount; }
			set
			{
				Set(() => Amount, ref _amount, value);
			}
		}

		private ObservableCollection<string> _typeList;
		public ObservableCollection<string> TypeList
		{
			get { return _typeList; }
		}

		private string _type;
		public string Type
		{
			get { return _type; }
			set
			{
				Set(() => Type, ref _type, value);
			}
		}

		private List<string> _periodList;
		public List<string> PeriodList
		{
			get { return _periodList; }
		}

		private string _period;
		public string Period
		{
			get { return _period; }
			set
			{
				if (Set(() => Period, ref _period, value))
					RaisePropertyChanged(() => EndDateVisible);
			}
		}

		private DateTime _startDate;
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				Set(() => StartDate, ref _startDate, value);
			}
		}

		private DateTime? _endDate;
		public DateTime? EndDate
		{
			get { return _endDate; }
			set
			{
				Set(() => EndDate, ref _endDate, value);
			}
		}

		public bool EndDateVisible
		{
			get
			{
				return Period == Resources.BudgetCustom;
			}
		}

		public string TitleText
		{
			get
			{
				if (BudgetId.HasValue)
				{
					return Resources.EditBudgetTitle;
				}
				else
				{
					return Resources.AddBudgetTitle;
				}
			}
		}

		public void LoadData(Guid? budgetId)
		{
			TypeList.Clear();
			var categoryRepository = KernelService.Kernel.Get<IRepository<Category>>();
			foreach (Category c in categoryRepository.GetAllEntries().OrderBy(it => it.CategoryName))
			{
				TypeList.Add(c.CategoryName);
			}
			if (TypeList.Count > 0)
			{
				TypeList.Insert(0, Resources.BudgetAllExpensesLabel);
			}

			BudgetId = null;
			Name = null;
			Amount = 0;
			StartDate = DateTime.Today;
			EndDate = null;
			Type = Resources.BudgetAllExpensesLabel;
			Period = Resources.BudgetMonthly;
			Type = Resources.BudgetAllExpensesLabel;
			if (budgetId.HasValue && budgetId.Value != Guid.Empty)
			{
				var b = KernelService.Kernel.Get<IRepository<Budget>>().GetFilteredEntries(it => it.BudgetId == budgetId.Value).FirstOrDefault();
				if (b != null)
				{
					BudgetId = b.BudgetId;
					Name = b.BudgetName;
					Amount = b.BudgetAmount;
					StartDate = b.StartDate;
					EndDate = b.EndDate;
					_id = b.Id;
					_isDeleted = b.IsDeleted;
					if (b.CategoryId.HasValue)
					{
						Type = categoryRepository.GetFilteredEntries(it => it.CategoryId == b.CategoryId.Value).Select(it => it.CategoryName).FirstOrDefault();
					}

					if (b.BudgetPeriod == PeriodType.Weekly)
						Period = Resources.BudgetWeekly;
					else if (b.BudgetPeriod == PeriodType.Monthly)
						Period = Resources.BudgetMonthly;
					else if (b.BudgetPeriod == PeriodType.Yearly)
						Period = Resources.BudgetYearly;
					else
						Period = Resources.BudgetCustom;
				}
			}
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationErrors.Add(Resources.BudgetNameRequiredError);
			}

			if (Amount <= 0)
			{
				validationErrors.Add(Resources.BudgetAmountRequiredError);
			}

			if (Period == Resources.BudgetCustom)
			{
				if (!EndDate.HasValue)
				{
					validationErrors.Add(Resources.BudgetEndDateRequiredError);

				}
				else if (EndDate <= StartDate)
				{
					validationErrors.Add(Resources.BudgetEndDateRangeError);
				}
			}

			return validationErrors;
		}

		public ICommand SaveCommand { get; private set; }

		public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
		public event ValidationFailedHandler ValidationFailed;

		public void SaveBudget()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				var budgetRepository = KernelService.Kernel.Get<IRepository<Budget>>();
				var navService = KernelService.Kernel.Get<INavigationService>();

				Budget b = null;
				b.BudgetId = BudgetId.HasValue ? BudgetId.Value : Guid.Empty;
				b.BudgetName = Name;
				b.BudgetAmount = Amount;
				b.StartDate = DateTime.SpecifyKind(StartDate, DateTimeKind.Utc);
				if (Type != Resources.BudgetAllExpensesLabel)
				{
					b.CategoryId = KernelService.Kernel.Get<IRepository<Category>>().GetFilteredEntries(it => it.CategoryName == Type).Select(it => it.CategoryId).FirstOrDefault();
				}
				else
				{
					b.CategoryId = null;
				}
				if (Period == Resources.BudgetWeekly)
				{
					b.BudgetPeriod = PeriodType.Weekly;
					b.EndDate = null;
				}
				else if (Period == Resources.BudgetMonthly)
				{
					b.BudgetPeriod = PeriodType.Monthly;
					b.EndDate = null;
				}
				else if (Period == Resources.BudgetYearly)
				{
					b.BudgetPeriod = PeriodType.Yearly;
					b.EndDate = null;
				}
				else if (Period == Resources.BudgetCustom)
				{
					b.BudgetPeriod = PeriodType.Custom;
					b.EndDate = DateTime.SpecifyKind(EndDate.Value, DateTimeKind.Utc);
				}
				b.Id = _id;
				b.IsDeleted = _isDeleted;

				b = budgetRepository.AddOrUpdateEntry(b);
				BudgetId = b.BudgetId;
				_id = b.Id;
				_isDeleted = b.IsDeleted;

				if (navService.CanGoBack)
					navService.GoBack();
			}
			else
			{
				if (ValidationFailed != null)
					ValidationFailed(this, new ValidationFailedEventArgs(errors));
			}
		}
	}
}
