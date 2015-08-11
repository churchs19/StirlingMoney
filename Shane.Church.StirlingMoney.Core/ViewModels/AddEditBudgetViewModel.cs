using GalaSoft.MvvmLight;
using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.Utility.Core.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AddEditBudgetViewModel : ObservableObject
	{
		private IRepository<Budget, Guid> _budgetRepository;
		private IRepository<Category, Guid> _categoryRepository;
		private INavigationService _navService;

		public AddEditBudgetViewModel(IRepository<Budget, Guid> budgetRepo, IRepository<Category, Guid> categoryRepo, INavigationService navService)
		{
			if (budgetRepo == null) throw new ArgumentNullException("budgetRepo");
			_budgetRepository = budgetRepo;
			if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
			_categoryRepository = categoryRepo;
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;

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

			SaveCommand = new AsyncRelayCommand(o => SaveBudget());
		}

		public AddEditBudgetViewModel()
			: this(ContainerService.Container.Locate<IRepository<Budget, Guid>>(), ContainerService.Container.Locate<IRepository<Category, Guid>>(), ContainerService.Container.Locate<INavigationService>())
		{

		}

		private bool _isDeleted;

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
					RaisePropertyChanged(() => IsCustomBudget);
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

		public bool IsCustomBudget
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

		public async Task LoadData(Guid? budgetId)
		{
			TypeList.Clear();
			var categories = await _categoryRepository.GetAllEntriesAsync();
			foreach (Category c in categories.OrderBy(it => it.CategoryName))
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
				var b = await _budgetRepository.GetEntryAsync(budgetId.Value);
				if (b != null)
				{
					BudgetId = b.BudgetId;
					Name = b.BudgetName;
					Amount = b.BudgetAmount;
					StartDate = DateTime.SpecifyKind(b.StartDate.UtcDateTime.Date, DateTimeKind.Utc);
					EndDate = b.EndDate.HasValue ? (DateTime?)DateTime.SpecifyKind(b.EndDate.Value.UtcDateTime.Date, DateTimeKind.Utc) : null;
					_isDeleted = b.IsDeleted;
					if (b.CategoryId.HasValue)
					{
						var cat = await _categoryRepository.GetEntryAsync(b.CategoryId.Value);
						Type = cat.CategoryName;
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

		public async Task SaveBudget()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				Budget b = ContainerService.Container.Locate<Budget>();
				b.BudgetId = BudgetId.HasValue ? BudgetId.Value : Guid.Empty;
				b.BudgetName = Name;
				b.BudgetAmount = Amount;
				b.StartDate = new DateTimeOffset(DateTime.SpecifyKind(StartDate.Date, DateTimeKind.Utc));
				if (Type != Resources.BudgetAllExpensesLabel)
				{
					var catQuery = await _categoryRepository.GetFilteredEntriesAsync(it => it.CategoryName == Type);
					b.CategoryId = catQuery.Select(it => it.CategoryId).FirstOrDefault();
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
					b.EndDate = new DateTimeOffset(DateTime.SpecifyKind(EndDate.Value.Date, DateTimeKind.Utc));
				}
				b.IsDeleted = _isDeleted;

				b = await _budgetRepository.AddOrUpdateEntryAsync(b);
				BudgetId = b.BudgetId;
				_isDeleted = b.IsDeleted;

				if (_navService.CanGoBack)
					_navService.GoBack();
			}
			else
			{
				if (ValidationFailed != null)
					ValidationFailed(this, new ValidationFailedEventArgs(errors));
			}
		}

		public async Task Commit()
		{
			await _budgetRepository.Commit();
		}
	}
}
