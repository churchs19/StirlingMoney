using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb.Repositories
{
	public class AccountRepository : IRepository<Account, Guid>
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;

		public AccountRepository(SterlingEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			_db = _engine.SterlingDatabase.GetDatabase("Money");
		}

		IQueryable<Account> GetAllEntries(bool includeDeleted = false)
		{
			var entries = _db.Query<Account, bool, Guid>("IsDeleted")
							.Where(it => !includeDeleted ? it.Index == false : true)
							.Select(it => it.Value.Result)
							.AsQueryable();
			return entries;
		}

		public Task<IQueryable<Account>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Account>>(() => GetAllEntries(includeDeleted));
		}

		IQueryable<Account> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Account, bool>> filter, bool includeDeleted = false)
		{
			var filterDelegate = filter.Compile();
			var results = _db.Query<Account, bool, Guid>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
			return results.AsQueryable();
		}

		public Task<IQueryable<Account>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Account, bool>> filter, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Account>>(() => GetFilteredEntries(filter, includeDeleted));
		}

		public async Task DeleteEntryAsync(Account entry, bool hardDelete = false)
		{
			if (hardDelete)
				await _db.DeleteAsync<Account>(entry);
			else
			{
				entry.EditDateTime = DateTimeOffset.Now;
				entry.IsDeleted = true;
				await _db.SaveAsync<Account>(entry);
			}
		}

		public async Task<Account> AddOrUpdateEntryAsync(Account entry)
		{
			if (entry.AccountId.Equals(Guid.Empty)) entry.AccountId = Guid.NewGuid();
			entry.EditDateTime = DateTimeOffset.Now;
			await _db.SaveAsync(entry);
			return entry;
		}

		public async Task BatchUpdateEntriesAsync(ICollection<Account> entries)
		{
			foreach (var entry in entries)
			{
				if (entry.IsDeleted)
				{
					await DeleteEntryAsync(entry, true);
				}
				else
				{
					await AddOrUpdateEntryAsync(entry);
				}
			}
		}

		public async Task<Account> GetEntryAsync(Guid key)
		{
			return await _db.LoadAsync<Account>(key);
		}

		public void Dispose()
		{
			_db.FlushAsync().Wait(2000);
		}

		public async Task Commit()
		{
			await _db.FlushAsync();
		}

		public Task<IQueryable<Account>> GetUpdatedEntries(DateTimeOffset date)
		{
			return TaskEx.Run<IQueryable<Account>>(() => _db.Query<Account, DateTimeOffset, Guid>("EditDateTime")
				.Where(it => it.Index >= date)
				.Select(it => it.Value.Result).AsQueryable());
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Account, Guid>().Select(it => it.Key).Count();
			else
				return _db.Query<Account, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).Count();
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<Account>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Account>>(() =>
			{
				if (includeDeleted)
					return _db.Query<Account, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Value.Result).AsQueryable();
				else
					return _db.Query<Account, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Value.Result).AsQueryable();
			});
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Account, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Key).Count();
			else
				return _db.Query<Account, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Key).Count();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
		}
	}
}
