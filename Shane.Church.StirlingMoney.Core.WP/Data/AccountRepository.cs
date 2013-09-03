using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class AccountRepository : Core.Data.IRepository<Core.Data.Account>
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public AccountRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.Account> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.Accounts.ToList().Select(it => it.ToCoreAccount()).AsQueryable();
				else
					return _context.Accounts.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).ToList().Select(it => it.ToCoreAccount()).AsQueryable();
			}
		}

		public IQueryable<Core.Data.Account> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Account, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Accounts.ToList().Select(it => it.ToCoreAccount());
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public void DeleteEntry(Core.Data.Account entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.Accounts.Where(it => it.AccountId == entry.AccountId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Accounts.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTimeOffset.Now;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Core.Data.Account AddOrUpdateEntry(Core.Data.Account entry)
		{
			if (!string.IsNullOrWhiteSpace(entry.AccountName))
			{
				lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
				{
					var item = _context.Accounts.Where(it => it.AccountId.Equals(entry.AccountId)).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.Account();
						item.AccountId = (entry.AccountId.Equals(Guid.Empty)) ? Guid.NewGuid() : entry.AccountId;
						_context.Accounts.InsertOnSubmit(item);
					}
					item.AccountName = entry.AccountName;
					item.EditDateTime = DateTimeOffset.Now;
					item.Id = entry.Id;
					item.InitialBalance = entry.InitialBalance;
					item.IsDeleted = entry.IsDeleted;

					_context.SubmitChanges();

					entry.Id = item.Id;
					entry.AccountId = item.AccountId;
					entry.EditDateTime = item.EditDateTime;
				}
			}
			return entry;
		}


		public Task<IQueryable<Core.Data.Account>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Account>>(() =>
			{
				return GetAllEntries(includeDeleted);
			});
		}

		public Task<IQueryable<Core.Data.Account>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Core.Data.Account, bool>> filter, bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Account>>(() =>
			{
				return GetFilteredEntries(filter, includeDeleted);
			});
		}

		public Task DeleteEntryAsync(Core.Data.Account entry, bool hardDelete = false)
		{
			return Task.Factory.StartNew(() =>
			{
				DeleteEntry(entry, hardDelete);
			});
		}

		public Task<Core.Data.Account> AddOrUpdateEntryAsync(Core.Data.Account entry)
		{
			return Task.Factory.StartNew<Core.Data.Account>(() =>
			{
				return AddOrUpdateEntry(entry);
			});
		}
	}

	public static class AccountExtensions
	{
		public static Core.Data.Account ToCoreAccount(this Shane.Church.StirlingMoney.Data.v3.Account account)
		{
			return new Core.Data.Account()
			{
				AccountId = account.AccountId,
				AccountName = account.AccountName,
				InitialBalance = account.InitialBalance,
				EditDateTime = account.EditDateTime,
				IsDeleted = account.IsDeleted
			};
		}
	}
}
