using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class AccountRepository : Core.Repositories.IRepository<Core.Data.Account, Guid>
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
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && !it.IsDeleted).ToList();
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
						pEntry.EditDateTime = DateTime.UtcNow;
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
					item.EditDateTime = DateTime.UtcNow;
					item.Id = entry.Id;
					item.InitialBalance = entry.InitialBalance;
					item.IsDeleted = entry.IsDeleted;
					item.ImageUri = entry.ImageUri;
					item.PostedBalance = entry.PostedBalance;
					item.AccountBalance = entry.AccountBalance;

					_context.SubmitChanges();

					entry.Id = item.Id;
					entry.AccountId = item.AccountId;
					entry.EditDateTime = DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc);
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


		public void BatchAddOrUpdateEntries(System.Collections.Generic.ICollection<Core.Data.Account> entries)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				foreach (var entry in entries)
				{
					if (!string.IsNullOrWhiteSpace(entry.AccountName))
					{
						var item = _context.Accounts.Where(it => it.AccountId.Equals(entry.AccountId)).FirstOrDefault();
						if (item == null)
						{
							item = new StirlingMoney.Data.v3.Account();
							item.AccountId = (entry.AccountId.Equals(Guid.Empty)) ? Guid.NewGuid() : entry.AccountId;
							_context.Accounts.InsertOnSubmit(item);
						}
						item.AccountName = entry.AccountName;
						item.EditDateTime = DateTime.UtcNow;
						item.Id = entry.Id;
						item.InitialBalance = entry.InitialBalance;
						item.IsDeleted = entry.IsDeleted;
						item.ImageUri = entry.ImageUri;
						item.AccountBalance = entry.AccountBalance;
						item.PostedBalance = entry.PostedBalance;
					}
				}
				_context.SubmitChanges();
			}
		}

		public Task BatchAddOrUpdateEntriesAsync(System.Collections.Generic.ICollection<Core.Data.Account> entries)
		{
			return Task.Factory.StartNew(() =>
			{
				BatchAddOrUpdateEntries(entries);
			});
		}
	}

	public static class AccountExtensions
	{
		public static Core.Data.Account ToCoreAccount(this Shane.Church.StirlingMoney.Data.v3.Account item)
		{
			return new Core.Data.Account()
			{
				AccountId = item.AccountId,
				AccountName = item.AccountName,
				InitialBalance = item.InitialBalance,
				EditDateTime = new DateTimeOffset(DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc), new TimeSpan(0)),
				IsDeleted = item.IsDeleted,
				ImageUri = item.ImageUri,
				AccountBalance = item.AccountBalance,
				PostedBalance = item.PostedBalance
			};
		}
	}
}
