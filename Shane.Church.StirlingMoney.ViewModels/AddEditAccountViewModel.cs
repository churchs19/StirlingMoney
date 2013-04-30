using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
#if !PERSONAL
using System.Data.Linq;
using Shane.Church.StirlingMoney.Data.v2;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using System.Collections.Generic;
//using System.Data.Linq;

namespace Shane.Church.StirlingMoney.ViewModels
{
	public class AddEditAccountViewModel : INotifyPropertyChanged
	{
		public AddEditAccountViewModel()
		{
		}

		private Guid _accountId;
		public Guid AccountId
		{
			get { return _accountId; }
			set
			{
				if (_accountId != value)
				{
					_accountId = value;
					NotifyPropertyChanged("AccountId");
					NotifyPropertyChanged("IsInitialBalanceReadOnly");
				}
			}
		}

		private string _accountName;
		public string AccountName
		{
			get { return _accountName; }
			set
			{
				if (_accountName != value)
				{
					_accountName = value;
					NotifyPropertyChanged("AccountName");
				}
			}
		}

		private double _initialBalance;
		public double InitialBalance
		{
			get { return _initialBalance; }
			set
			{
				if (_initialBalance != value)
				{
					_initialBalance = value;
					NotifyPropertyChanged("InitialBalance");
				}
			}
		}

		public string PageTitle
		{
			get
			{
				if (AccountId != Guid.Empty)
				{
					return Resources.ViewModelResources.EditAccountTitle;
				}
				else
				{
					return Resources.ViewModelResources.AddAccountTitle;
				}
			}
		}

		public bool IsInitialBalanceReadOnly
		{
			get
			{
				if (AccountId != null && AccountId != Guid.Empty)
				{
					return true;
				}
				else
				{
					return false;
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

		public void LoadData(Guid accountId)
		{
			if (accountId != Guid.Empty)
			{
#if !PERSONAL
				using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
				{
					var account = (from a in _context.Accounts
								   where a.AccountId == accountId
								   select a);					
#else
				var account = (from a in ContextInstance.Context.AccountCollection
							   where a.AccountId == accountId
							   select a);
#endif
					if (account.Any())
					{
						Account a = account.First();
						AccountId = a.AccountId;
						AccountName = a.AccountName;
						InitialBalance = a.InitialBalance;
					}
#if !PERSONAL
				}
#endif
			}
		}

		public IList<string> Validate()
		{
			List<string> validationErrors = new List<string>();

			if (string.IsNullOrWhiteSpace(AccountName))
			{
				validationErrors.Add(Resources.ViewModelResources.AccountNameRequiredError);
			}

			return validationErrors;
		}

		public void SaveAccount()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				var account = (from a in _context.Accounts
							   where a.AccountId == AccountId
							   select a);
#else
			var account = (from a in ContextInstance.Context.AccountCollection
						   where a.AccountId == AccountId
						   select a);
#endif
				if (account.Any())
				{
					Account a = account.First();
					a.AccountName = AccountName;
				}
				else
				{
					Account a = new Account();
					a.AccountId = Guid.NewGuid();
					a.AccountName = AccountName;
					a.InitialBalance = InitialBalance;
#if !PERSONAL
					_context.Accounts.InsertOnSubmit(a);
#else
					ContextInstance.Context.AddAccount(a);
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
