using Newtonsoft.Json;
using Grace;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Threading.Tasks;
using Wintellect.Sterling.Core.Serialization;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Budget
	{
		private IDataRepository<Transaction, Guid> _transactionRepository;
		private IDataRepository<Category, Guid> _categoryRepository;
		private ITransactionSum _transactionSum;

		public Budget(IDataRepository<Transaction, Guid> transactionRepo, IDataRepository<Category, Guid> categoryRepo, ITransactionSum transactionSum)
		{
			if (transactionRepo == null) throw new ArgumentNullException("transactionRepo");
			_transactionRepository = transactionRepo;
			if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
			_categoryRepository = categoryRepo;
			if (transactionSum == null) throw new ArgumentNullException("transactionSum");
			_transactionSum = transactionSum;

			BudgetName = "";
		}

		public Budget()
			: this(ContainerService.Container.Locate<IDataRepository<Transaction, Guid>>(),
				ContainerService.Container.Locate<IDataRepository<Category, Guid>>(),
				ContainerService.Container.Locate<ITransactionSum>())
		{

		}

		public Guid BudgetId { get; set; }
		public string BudgetName { get; set; }
		public double BudgetAmount { get; set; }
		public Guid? CategoryId { get; set; }
		public PeriodType BudgetPeriod { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset? EndDate { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

		[JsonIgnore]
		[SterlingIgnore]
		public DateTimeOffset CurrentPeriodStart
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

		[JsonIgnore]
		[SterlingIgnore]
		public DateTimeOffset CurrentPeriodEnd
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

		public async Task<Category> GetCategory()
		{
			try
			{
				if (!CategoryId.HasValue)
					return null;
				else
				{
					return await _categoryRepository.GetEntryAsync(CategoryId.Value);
				}
			}
			catch
			{
				return null;
			}
		}

		[JsonIgnore]
		[SterlingIgnore]
		public double AmountSpent
		{
			get
			{
				double amount = 0;
				if (!CategoryId.HasValue)
				{
					amount = _transactionSum.GetSumBetweenDates(CurrentPeriodStart, CurrentPeriodEnd);
				}
				else
				{
					amount = _transactionSum.GetSumBetweenDatesByCategory(CategoryId.Value, CurrentPeriodStart, CurrentPeriodEnd);
				}
				return -amount;
			}
		}
	}
}
