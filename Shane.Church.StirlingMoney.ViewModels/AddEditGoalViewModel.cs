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
	public class AddEditGoalViewModel : INotifyPropertyChanged
	{
		public AddEditGoalViewModel()
		{
			_accounts = new ObservableCollection<string>();
		}

		private Guid? _goalId;
		public Guid? GoalId
		{
			get { return _goalId; }
			set
			{
				if (_goalId != value)
				{
					_goalId = value;
					NotifyPropertyChanged("GoalId");
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

		private string _account;
		public string Account
		{
			get { return _account; }
			set
			{
				if (_account != value)
				{
					_account = value;
					NotifyPropertyChanged("Account");
				}
			}
		}

		private ObservableCollection<string> _accounts;
		public ObservableCollection<string> Accounts
		{
			get { return _accounts; }
			set
			{
				if (_accounts != value)
				{
					_accounts = value;
					if (_accounts != null)
					{
						_accounts.CollectionChanged += delegate
						{
							NotifyPropertyChanged("Accounts");
						};
					}
					NotifyPropertyChanged("Accounts");
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

		private DateTime _targetDate;
		public DateTime TargetDate
		{
			get { return _targetDate; }
			set
			{
				if (_targetDate != value)
				{
					_targetDate = value;
					NotifyPropertyChanged("TargetDate");
				}
			}
		}

		public string TitleText
		{
			get
			{
				if (GoalId.HasValue)
				{
					return Resources.ViewModelResources.EditGoalTitle;
				}
				else
				{
					return Resources.ViewModelResources.AddGoalTitle;
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

		public void LoadData(Guid? goalId)
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				if (goalId.HasValue && goalId.Value != Guid.Empty)
				{
#if!PERSONAL
					Goal g = (from gi in _context.Goals
#else
					Goal g = (from gi in ContextInstance.Context.GoalCollection
#endif
							  where gi.GoalId == goalId.Value
							  select gi).FirstOrDefault();
					GoalId = g.GoalId;
					Name = g.GoalName;
					Amount = g.Amount;
#if !PERSONAL
					Account = g.Account.AccountName;
#else
					Account = (from a in ContextInstance.Context.AccountCollection
							   where a.AccountId == g._accountId
							   select a.AccountName).FirstOrDefault();
#endif
					TargetDate = g.TargetDate;
				}
				else
				{
					GoalId = null;
					Name = null;
					Amount = 0;
					TargetDate = DateTime.Today.AddMonths(1);
				}

#if !PERSONAL
				foreach (Account a in _context.Accounts.OrderBy(m => m.AccountName))
#else
				foreach (Account a in ContextInstance.Context.AccountCollection.OrderBy(m => m.AccountName))
#endif
				{
					Accounts.Add(a.AccountName);
				}
				if (Accounts.Count > 0)
				{
					Accounts.Insert(0, "");
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
				validationErrors.Add(Resources.ViewModelResources.GoalNameRequiredError);
			}

			if (string.IsNullOrWhiteSpace(Account))
			{
				validationErrors.Add(Resources.ViewModelResources.GoalAccountRequiredError);
			}

			if (TargetDate <= DateTime.Today)
			{
				validationErrors.Add(Resources.ViewModelResources.GoalDateError);
			}

			double accountBalance = 0;
#if !PERSONAL
			using(StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
				accountBalance = (from a in _context.Accounts
							 where a.AccountName == Account
							 select a.AccountBalance).FirstOrDefault();
			}
#else
			accountBalance = (from a in ContextInstance.Context.AccountCollection
							where a.AccountName == Account
							select a.AccountBalance).FirstOrDefault();
#endif

			if (Amount <= accountBalance)
			{
				validationErrors.Add(Resources.ViewModelResources.GoalAmountError);
			}

			return validationErrors;
		}

		public void SaveGoal()
		{
#if !PERSONAL
			using (StirlingMoneyDataContext _context = new StirlingMoneyDataContext(StirlingMoneyDataContext.DBConnectionString))
			{
#endif
				Goal g = new Goal();
				if (GoalId.HasValue && GoalId.Value != Guid.Empty)
				{
#if !PERSONAL
					var goalQuery = (from gi in _context.Goals
#else
					var goalQuery = (from gi in ContextInstance.Context.GoalCollection
#endif
									 where gi.GoalId == GoalId.Value
									 select gi);
					if (goalQuery.Any())
					{
						g = goalQuery.First();
					}
				}
				else
				{
					g.GoalId = Guid.NewGuid();
				}

				g.GoalName = Name;
				g.Amount = Amount;
				g.TargetDate = TargetDate;
#if !PERSONAL
				g.Account = (from a in _context.Accounts
							 where a.AccountName == Account
							 select a).FirstOrDefault();
#else
				g._accountId = (from a in ContextInstance.Context.AccountCollection
								where a.AccountName == Account
								select a.AccountId).FirstOrDefault();
#endif

				if (!(GoalId.HasValue && GoalId.Value != Guid.Empty))
				{
#if !PERSONAL
					g.InitialBalance = g.Account.AccountBalance;
					_context.Goals.InsertOnSubmit(g);
#else
					g.InitialBalance = (from a in ContextInstance.Context.AccountCollection
										where a.AccountId == g._accountId
										select a.AccountBalance).FirstOrDefault();
					ContextInstance.Context.AddGoal(g);
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
