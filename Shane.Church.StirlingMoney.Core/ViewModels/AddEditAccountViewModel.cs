﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ninject;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Properties;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AddEditAccountViewModel : ObservableObject
	{
		public AddEditAccountViewModel()
		{
			SaveCommand = new RelayCommand(SaveAccount);
		}

		private long? _id = null;
		private bool? _isDeleted = null;

		private Guid _accountId;
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				if (Set(() => AccountId, ref _accountId, value))
				{
					RaisePropertyChanged(() => IsInitialBalanceReadOnly);
					RaisePropertyChanged(() => PageTitle);
				}
			}
		}

		private string _accountName;
		public string AccountName
		{
			get { return _accountName; }
			set
			{
				Set(() => AccountName, ref _accountName, value);
			}
		}

		private double _initialBalance;
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
				Set(() => InitialBalance, ref _initialBalance, value);
			}
		}

		private bool _isCreditCard;
		public bool IsCreditCard
		{
			get { return _isCreditCard; }
			set
			{
				if (Set(() => IsCreditCard, ref _isCreditCard, value))
				{
					RaisePropertyChanged(() => IsCreditCardLabel);
				}
			}
		}

		private double _creditLimit;
		public double CreditLimit
		{
			get { return _creditLimit; }
			set
			{
				Set(() => CreditLimit, ref _creditLimit, value);
			}
		}

		public string PageTitle
		{
			get
			{
				if (AccountId != Guid.Empty)
				{
					return Resources.EditAccountTitle;
				}
				else
				{
					return Resources.AddAccountTitle;
				}
			}
		}

		public bool IsInitialBalanceReadOnly
		{
			get
			{
				return AccountId != null && AccountId != Guid.Empty;
			}
		}

		public string IsCreditCardLabel
		{
			get { return IsCreditCard ? Resources.Yes : Resources.No; }
		}

		public void LoadData(Guid accountId)
		{
			if (accountId != Guid.Empty)
			{
				Account a = KernelService.Kernel.Get<IRepository<Account>>().GetFilteredEntries(it => it.AccountId == accountId).FirstOrDefault();
				if (a != null)
				{
					AccountId = a.AccountId;
					AccountName = a.AccountName;
					InitialBalance = a.InitialBalance;
					_id = a.Id;
					_isDeleted = a.IsDeleted;
				}
			}
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(AccountName))
			{
				validationErrors.Add(Resources.AccountNameRequiredError);
			}

			return validationErrors;
		}

		public ICommand SaveCommand { get; private set; }

		public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
		public event ValidationFailedHandler ValidationFailed;

		public void SaveAccount()
		{
			var errors = Validate();
			if (errors.Count == 0)
			{
				var accountRepository = KernelService.Kernel.Get<IRepository<Account>>();
				var navService = KernelService.Kernel.Get<INavigationService>();

				Account a = new Account();
				a.Id = _id;
				a.IsDeleted = _isDeleted;
				a.AccountId = AccountId;
				a.InitialBalance = InitialBalance;
				a.AccountName = AccountName;
				a = accountRepository.AddOrUpdateEntry(a);
				AccountId = a.AccountId;
				_id = a.Id;
				_isDeleted = a.IsDeleted;

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
