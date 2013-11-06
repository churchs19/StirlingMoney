using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class TransactionRepository : Core.Data.IRepository<Core.Data.Transaction>, Core.Data.ITransactionSum
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public TransactionRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.Transaction> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.Transactions.ToList().Select(it => it.ToCoreTransaction()).AsQueryable();
				else
					return _context.Transactions.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).ToList().Select(it => it.ToCoreTransaction()).AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Transaction>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Transaction>>(() =>
			{
				return GetAllEntries(includeDeleted);
			});
		}

		public IQueryable<Core.Data.Transaction> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Transaction, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Transactions.ToList().Select(it => it.ToCoreTransaction());
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Transaction>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Core.Data.Transaction, bool>> filter, bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Transaction>>(() =>
				{
					return GetFilteredEntries(filter, includeDeleted);
				});
		}

		public void DeleteEntry(Core.Data.Transaction entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.Transactions.Where(it => it.TransactionId == entry.TransactionId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Transactions.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTime.UtcNow;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Task DeleteEntryAsync(Core.Data.Transaction entry, bool hardDelete = false)
		{
			return Task.Factory.StartNew(() =>
			{
				DeleteEntry(entry, hardDelete);
			});
		}

		public Core.Data.Transaction AddOrUpdateEntry(Core.Data.Transaction entry)
		{
			if (!entry.AccountId.Equals(Guid.Empty))
			{
				lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
				{
					var item = _context.Transactions.Where(it => it.TransactionId == entry.TransactionId).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.Transaction();
						item.TransactionId = entry.TransactionId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.TransactionId;
						_context.Transactions.InsertOnSubmit(item);
					}
					item.Account = _context.Accounts.Where(it => it.AccountId == entry.AccountId).FirstOrDefault();
					item.Amount = entry.Amount;
					item.CategoryId = entry.CategoryId;
					item.CheckNumber = entry.CheckNumber;
					item.EditDateTime = DateTime.UtcNow;
					item.Id = entry.Id;
					item.IsDeleted = entry.IsDeleted;
					item.Location = entry.Location;
					item.Note = entry.Note;
					item.Posted = entry.Posted;
					item.TransactionDate = DateTime.SpecifyKind(entry.TransactionDate, DateTimeKind.Utc);

					_context.SubmitChanges();

					entry.TransactionId = item.TransactionId;
					entry.Id = item.Id;
					entry.EditDateTime = DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc);
				}
			}
			return entry;
		}

		public Task<Core.Data.Transaction> AddOrUpdateEntryAsync(Core.Data.Transaction entry)
		{
			return Task.Factory.StartNew<Core.Data.Transaction>(() =>
				{
					return AddOrUpdateEntry(entry);
				});
		}

		public double GetSumByAccount(Guid accountId)
		{
			var query = _context.Transactions.Where(it => it.Account.AccountId == accountId).Select(it => it.Amount);
			return query.Any() ? query.Sum() : 0;
		}

		public double GetPostedSumByAccount(Guid accountId)
		{
			var query = _context.Transactions.Where(it => it.Account.AccountId == accountId && it.Posted).Select(it => it.Amount);
			return query.Any() ? query.Sum() : 0;
		}

		public void BatchAddOrUpdateEntries(System.Collections.Generic.ICollection<Core.Data.Transaction> entries)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				foreach (var entry in entries)
				{
					if (!entry.AccountId.Equals(Guid.Empty))
					{
						var item = _context.Transactions.Where(it => it.TransactionId == entry.TransactionId).FirstOrDefault();
						if (item == null)
						{
							item = new StirlingMoney.Data.v3.Transaction();
							item.TransactionId = entry.TransactionId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.TransactionId;
							_context.Transactions.InsertOnSubmit(item);
						}
						item.Account = _context.Accounts.Where(it => it.AccountId == entry.AccountId).FirstOrDefault();
						item.Amount = entry.Amount;
						item.CategoryId = entry.CategoryId;
						item.CheckNumber = entry.CheckNumber;
						item.EditDateTime = DateTime.UtcNow;
						item.Id = entry.Id;
						item.IsDeleted = entry.IsDeleted;
						item.Location = entry.Location;
						item.Note = entry.Note;
						item.Posted = entry.Posted;
						item.TransactionDate = DateTime.SpecifyKind(entry.TransactionDate, DateTimeKind.Utc);
					}
				}
				_context.SubmitChanges();
			}
		}

		public Task BatchAddOrUpdateEntriesAsync(System.Collections.Generic.ICollection<Core.Data.Transaction> entries)
		{
			return Task.Factory.StartNew(() =>
			{
				BatchAddOrUpdateEntries(entries);
			});
		}
	}

	public static class TransactionExtensions
	{
		public static Core.Data.Transaction ToCoreTransaction(this Shane.Church.StirlingMoney.Data.v3.Transaction item)
		{
			return new Core.Data.Transaction()
			{
				AccountId = item.Account.AccountId,
				Amount = item.Amount,
				CategoryId = item.CategoryId,
				CheckNumber = item.CheckNumber,
				EditDateTime = new DateTimeOffset(DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc), new TimeSpan(0)),
				Id = item.Id,
				IsDeleted = item.IsDeleted,
				Location = item.Location,
				Note = item.Note,
				Posted = item.Posted,
				TransactionDate = DateTime.SpecifyKind(item.TransactionDate, DateTimeKind.Utc),
				TransactionId = item.TransactionId
			};
		}
	}
}
