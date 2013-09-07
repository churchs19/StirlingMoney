﻿using GalaSoft.MvvmLight;
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
	public class AddEditGoalViewModel : ObservableObject
	{
		public AddEditGoalViewModel()
		{
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
			if (goalId.HasValue)
			{
				var goalRepository = KernelService.Kernel.Get<IRepository<Goal>>();
				var goal = goalRepository.GetFilteredEntries(it => it.GoalId == goalId.Value).FirstOrDefault();
				if (goal != null)
				{
					GoalId = goal.GoalId;
					Name = goal.GoalName;
					TargetDate = goal.TargetDate;
					Amount = goal.Amount;
					_id = goal.Id;
					_isDeleted = goal.IsDeleted;
				}
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

			var account = KernelService.Kernel.Get<IRepository<Account>>().GetFilteredEntries(it => it.AccountName == Account).FirstOrDefault();
			double accountBalance = 0;
			if (account != null)
			{
				accountBalance = account.InitialBalance + KernelService.Kernel.Get<ITransactionSum>().GetSumByAccount(account.AccountId);
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
				var goalService = KernelService.Kernel.Get<IRepository<Goal>>();
				var navService = KernelService.Kernel.Get<INavigationService>();

				Goal g = new Goal();
				g.GoalId = GoalId.Value;
				g.GoalName = Name;
				g.TargetDate = TargetDate;
				g.Amount = Amount;
				g.Id = _id;
				g.IsDeleted = _isDeleted;
				var account = KernelService.Kernel.Get<IRepository<Account>>().GetFilteredEntries(it => it.AccountName == Account).FirstOrDefault();
				g.AccountId = account.AccountId;
				g.InitialBalance = account.InitialBalance + KernelService.Kernel.Get<ITransactionSum>().GetSumByAccount(account.AccountId);

				g = goalService.AddOrUpdateEntry(g);
				GoalId = g.GoalId;
				_id = g.Id;
				_isDeleted = g.IsDeleted;

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
