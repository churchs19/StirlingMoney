using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class GoalRepository : Core.Data.IRepository<Core.Data.Goal>
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public GoalRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.Goal> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.Goals.ToList().Select(it => it.ToCoreGoal()).AsQueryable();
				else
					return _context.Goals.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).ToList().Select(it => it.ToCoreGoal()).AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Goal>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Goal>>(() =>
			{
				return GetAllEntries(includeDeleted);
			});
		}

		public IQueryable<Core.Data.Goal> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Goal, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Goals.ToList().Select(it => it.ToCoreGoal());
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Goal>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Core.Data.Goal, bool>> filter, bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Goal>>(() =>
			{
				return GetFilteredEntries(filter, includeDeleted);
			});
		}

		public void DeleteEntry(Core.Data.Goal entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.Goals.Where(it => it.GoalId == entry.GoalId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Goals.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTime.UtcNow;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Task DeleteEntryAsync(Core.Data.Goal entry, bool hardDelete = false)
		{
			return Task.Factory.StartNew(() =>
			{
				DeleteEntry(entry, hardDelete);
			});
		}

		public Core.Data.Goal AddOrUpdateEntry(Core.Data.Goal entry)
		{
			if (!entry.AccountId.Equals(Guid.Empty) && !string.IsNullOrWhiteSpace(entry.GoalName))
			{
				lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
				{
					var item = _context.Goals.Where(it => it.GoalId == entry.GoalId).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.Goal();
						item.GoalId = entry.GoalId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.GoalId;
						_context.Goals.InsertOnSubmit(item);
					}
					item.Account = _context.Accounts.Where(it => it.AccountId == entry.AccountId).FirstOrDefault();
					item.Amount = entry.Amount;
					item.EditDateTime = DateTime.UtcNow;
					item.GoalName = entry.GoalName;
					item.Id = entry.Id;
					item.InitialBalance = entry.InitialBalance;
					item.IsDeleted = entry.IsDeleted;
					item.TargetDate = DateTime.SpecifyKind(entry.TargetDate, DateTimeKind.Utc);
					item.StartDate = DateTime.SpecifyKind(entry.StartDate, DateTimeKind.Utc);

					_context.SubmitChanges();

					entry.GoalId = item.GoalId;
					entry.Id = item.Id;
					entry.EditDateTime = DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc);
				}
			}
			return entry;
		}

		public Task<Core.Data.Goal> AddOrUpdateEntryAsync(Core.Data.Goal entry)
		{
			return Task.Factory.StartNew<Core.Data.Goal>(() =>
			{
				return AddOrUpdateEntry(entry);
			});
		}
	}

	public static class GoalExtensions
	{
		public static Core.Data.Goal ToCoreGoal(this Shane.Church.StirlingMoney.Data.v3.Goal item)
		{
			return new Core.Data.Goal()
			{
				Id = item.Id,
				AccountId = item.Account.AccountId,
				Amount = item.Amount,
				EditDateTime = new DateTimeOffset(DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc), new TimeSpan(0)),
				GoalId = item.GoalId,
				GoalName = item.GoalName,
				InitialBalance = item.InitialBalance,
				IsDeleted = item.IsDeleted,
				TargetDate = DateTime.SpecifyKind(item.TargetDate, DateTimeKind.Utc),
				StartDate = DateTime.SpecifyKind(item.StartDate, DateTimeKind.Utc)
			};
		}
	}
}
