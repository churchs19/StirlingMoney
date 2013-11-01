using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AddEditGoalViewModel : ObservableObject
	{
		private INavigationService _navService;
		private IRepository<Goal> _goalRepository;
		private IRepository<Account> _accountRepository;

		public AddEditGoalViewModel(INavigationService navService, IRepository<Goal> goalRepository, IRepository<Account> accountRepository)
		{
			if (navService == null) throw new ArgumentNullException("navService");
			_navService = navService;
			if (goalRepository == null) throw new ArgumentNullException("goalRepository");
			_goalRepository = goalRepository;
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;

			_accounts = new ObservableCollection<string>();
			_accounts.CollectionChanged += (s, e) =>
			{
				RaisePropertyChanged(() => Accounts);
			};
			SaveCommand = new RelayCommand(SaveGoal);
		}

		private long? _id = null;
		private bool? _isDeleted = null;

		private Guid? _goalId;
		public Guid? GoalId
		{
			get { return _goalId; }
			set
			{
				if (Set(() => GoalId, ref _goalId, value))
				{
					RaisePropertyChanged(() => TitleText);
				}
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

		private string _account;
		public string Account
		{
			get { return _account; }
			set
			{
				Set(() => Account, ref _account, value);
			}
		}

		public bool IsAccountReadonly
		{
			get { return !string.IsNullOrWhiteSpace(_account); }
		}

		private ObservableCollection<string> _accounts;
		public ObservableCollection<string> Accounts
		{
			get { return _accounts; }
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

		private DateTime _startDate;

		private DateTime _targetDate;
		public DateTime TargetDate
		{
			get { return _targetDate; }
			set
			{
				Set(() => TargetDate, ref _targetDate, value);
			}
		}

		public string TitleText
		{
			get
			{
				if (GoalId.HasValue)
				{
					return Resources.EditGoalTitle;
				}
				else
				{
					return Resources.AddGoalTitle;
				}
			}
		}

		public void LoadData(Guid? goalId)
		{
			var accts = _accountRepository.GetAllEntries().Select(it => it.AccountName);
			foreach (var a in accts)
				Accounts.Add(a);

			if (goalId.HasValue && !goalId.Equals(Guid.Empty))
			{
				var goal = _goalRepository.GetFilteredEntries(it => it.GoalId == goalId.Value).FirstOrDefault();
				if (goal != null)
				{
					GoalId = goal.GoalId;
					Name = goal.GoalName;
					_startDate = goal.StartDate;
					TargetDate = goal.TargetDate;
					Amount = goal.Amount;
					_id = goal.Id;
					_isDeleted = goal.IsDeleted;
				}
			}
			else
			{
				_startDate = new DateTime(DateTime.Today.Ticks, DateTimeKind.Utc);
				TargetDate = DateTime.Today.AddMonths(1);
			}
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(Name))
			{
				validationErrors.Add(Resources.GoalNameRequiredError);
			}

			if (string.IsNullOrWhiteSpace(Account))
			{
				validationErrors.Add(Resources.GoalAccountRequiredError);
			}

			if (TargetDate <= DateTime.Today)
			{
				validationErrors.Add(Resources.GoalDateError);
			}
			if (TargetDate <= _startDate)
			{
				validationErrors.Add(String.Format(Resources.GoalDateComparisonError, _startDate));
			}

			var account = _accountRepository.GetFilteredEntries(it => it.AccountName == Account).FirstOrDefault();
			double accountBalance = 0;
			if (account != null)
			{
				accountBalance = account.AccountBalance;
			}
			if (Amount <= accountBalance)
			{
				validationErrors.Add(Resources.GoalAmountError);
			}

			return validationErrors;
		}

		public ICommand SaveCommand { get; private set; }

		public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
		public event ValidationFailedHandler ValidationFailed;

		public void SaveGoal()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				Goal g = new Goal();
				g.GoalId = GoalId.HasValue ? GoalId.Value : Guid.Empty;
				g.GoalName = Name;
				if (GoalId.HasValue && !GoalId.Equals(Guid.Empty))
				{
					g.StartDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
				}
				else
				{
					g.StartDate = _startDate;
				}
				g.TargetDate = TargetDate;
				g.Amount = Amount;
				g.Id = _id;
				g.IsDeleted = _isDeleted;
				var account = _accountRepository.GetFilteredEntries(it => it.AccountName == Account).FirstOrDefault();
				g.AccountId = account.AccountId;
				g.InitialBalance = account.InitialBalance + KernelService.Kernel.Get<ITransactionSum>().GetSumByAccount(account.AccountId);

				g = _goalRepository.AddOrUpdateEntry(g);
				GoalId = g.GoalId;
				_id = g.Id;
				_isDeleted = g.IsDeleted;

				if (_navService.CanGoBack)
					_navService.GoBack();
			}
			else
			{
				if (ValidationFailed != null)
					ValidationFailed(this, new ValidationFailedEventArgs(errors));
			}
		}
	}
}
