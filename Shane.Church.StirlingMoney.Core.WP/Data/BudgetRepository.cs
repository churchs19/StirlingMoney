using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Data.v3;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class BudgetRepository : Core.Data.IRepository<Core.Data.Budget>
	{
		StirlingMoneyDataContext _context;

		public BudgetRepository()
		{
			_context = KernelService.Kernel.Get<StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.Budget> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.Budgets.ToList().Select(it => it.ToCoreBudget()).AsQueryable();
				else
					return _context.Budgets.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).ToList().Select(it => it.ToCoreBudget()).AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Budget>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Budget>>(() =>
				{
					return GetAllEntries(includeDeleted);
				});
		}

		public IQueryable<Core.Data.Budget> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Budget, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Budgets.ToList().Select(it => it.ToCoreBudget());
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Budget>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Core.Data.Budget, bool>> filter, bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Budget>>(() =>
			{
				return GetFilteredEntries(filter, includeDeleted);
			});
		}

		public void DeleteEntry(Core.Data.Budget entry, bool hardDelete = false)
		{
			lock (StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.Budgets.Where(it => it.BudgetId == entry.BudgetId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Budgets.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTime.UtcNow;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Task DeleteEntryAsync(Core.Data.Budget entry, bool hardDelete = false)
		{
			return Task.Factory.StartNew(() =>
			{
				DeleteEntry(entry, hardDelete);
			});
		}

		public Core.Data.Budget AddOrUpdateEntry(Core.Data.Budget entry)
		{
			if (entry.BudgetAmount != 0 && !string.IsNullOrWhiteSpace(entry.BudgetName))
			{
				lock (StirlingMoneyDataContext.LockObject)
				{
					var item = _context.Budgets.Where(it => it.BudgetId == entry.BudgetId).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.Budget();
						item.BudgetId = entry.BudgetId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.BudgetId;
						_context.Budgets.InsertOnSubmit(item);
					}
					item.BudgetAmount = entry.BudgetAmount;
					item.BudgetName = entry.BudgetName;
					item.BudgetPeriod = entry.BudgetPeriod;
					item.CategoryId = entry.CategoryId;
					item.EditDateTime = DateTime.UtcNow;
					item.EndDate = entry.EndDate.HasValue ? new Nullable<DateTime>(DateTime.SpecifyKind(entry.EndDate.Value, DateTimeKind.Utc)) : null;
					item.Id = entry.Id;
					item.IsDeleted = entry.IsDeleted;
					item.StartDate = DateTime.SpecifyKind(entry.StartDate, DateTimeKind.Utc);

					_context.SubmitChanges();

					entry.BudgetId = item.BudgetId;
					entry.Id = item.Id;
					entry.EditDateTime = item.EditDateTime;
				}
			}
			return entry;
		}

		public Task<Core.Data.Budget> AddOrUpdateEntryAsync(Core.Data.Budget entry)
		{
			return Task.Factory.StartNew<Core.Data.Budget>(() =>
			{
				return AddOrUpdateEntry(entry);
			});
		}
	}

	public static class BudgetExtensions
	{
		public static Core.Data.Budget ToCoreBudget(this Shane.Church.StirlingMoney.Data.v3.Budget item)
		{
			var coreBudget = KernelService.Kernel.Get<Core.Data.Budget>();
			coreBudget.Id = item.Id;
			coreBudget.BudgetAmount = item.BudgetAmount;
			coreBudget.BudgetId = item.BudgetId;
			coreBudget.BudgetName = item.BudgetName;
			coreBudget.BudgetPeriod = item.BudgetPeriod;
			coreBudget.CategoryId = item.CategoryId;
			coreBudget.EndDate = item.EndDate.HasValue ? new Nullable<DateTime>(DateTime.SpecifyKind(item.EndDate.Value, DateTimeKind.Utc)) : null;
			coreBudget.EditDateTime = new DateTimeOffset(DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc), new TimeSpan(0));
			coreBudget.StartDate = DateTime.SpecifyKind(item.StartDate, DateTimeKind.Utc);
			coreBudget.IsDeleted = item.IsDeleted;

			return coreBudget;
		}
	}
}
