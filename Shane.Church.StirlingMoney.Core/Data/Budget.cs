using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Budget
	{
		private IRepository<Category> _categories;
		private IRepository<Transaction> _transactions;

		public Budget(IRepository<Category> categoryRepository, IRepository<Transaction> transactionRepository)
		{
			if (categoryRepository == null) throw new ArgumentNullException("categoryRepository");
			_categories = categoryRepository;
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactions = transactionRepository;
		}
		public long? Id { get; set; }
		public Guid BudgetId { get; set; }
		public string BudgetName { get; set; }
		public double BudgetAmount { get; set; }
		public Guid? CategoryId { get; set; }
		public PeriodType BudgetPeriod { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool? IsDeleted { get; set; }

		public DateTime CurrentPeriodStart
		{
			get
			{
				if (BudgetPeriod == PeriodType.Custom)
				{
					return StartDate;
				}
				else
				{
					int day = StartDate.Day;
					DateTime start = DateTime.Today;
					if (start.Day < day)
					{
						start = start.AddMonths(-1);
					}
					if (start.Month == 2 && start.Month > 28)
					{
						if (DateTime.IsLeapYear(start.Year))
						{
							return new DateTime(start.Year, start.Month, 29);
						}
						else
						{
							return new DateTime(start.Year, start.Month, 28);
						}
					}
					else if ((start.Month == 4 || start.Month == 6 || start.Month == 9 || start.Month == 11) && day == 31)
					{
						return new DateTime(start.Year, start.Month, 30);
					}
					else
					{
						return new DateTime(start.Year, start.Month, day);
					}
				}
			}
		}

		public DateTime CurrentPeriodEnd
		{
			get
			{
				switch (BudgetPeriod)
				{
					case PeriodType.Weekly:
						return CurrentPeriodStart.AddDays(7);
					case PeriodType.Monthly:
					default:
						return CurrentPeriodStart.AddMonths(1);
					case PeriodType.Yearly:
						return CurrentPeriodStart.AddYears(1);
				}
			}
		}

		public Category Category
		{
			get
			{
				try
				{
					if (!CategoryId.HasValue) return null;
					return _categories.GetFilteredEntries(it => it.CategoryId == CategoryId.Value).FirstOrDefault();
				}
				catch
				{
					return null;
				}
			}
		}

		public double AmountSpent
		{
			get
			{
				if (!CategoryId.HasValue)
				{
					var amount = _transactions.GetFilteredEntries(it => it.TransactionDate >= CurrentPeriodStart && it.TransactionDate <= CurrentPeriodEnd).Select(it => it.Amount).Sum();
					return -amount;
				}
				else
				{
					var amount = _transactions.GetFilteredEntries(it => it.TransactionDate >= CurrentPeriodStart && it.TransactionDate <= CurrentPeriodEnd && it.CategoryId == CategoryId.Value).Select(it => it.Amount).Sum();
					return -amount;
				}
			}
		}
	}
}
