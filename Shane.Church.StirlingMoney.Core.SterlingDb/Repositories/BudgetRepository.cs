using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb.Repositories
{
	public class BudgetRepository : IRepository<Budget, Guid>
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;

		public BudgetRepository(SterlingEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			_db = _engine.SterlingDatabase.GetDatabase("Money");
		}

		IQueryable<Budget> GetAllEntries(bool includeDeleted = false)
		{
			var entries = _db.Query<Budget, bool, Guid>("IsDeleted")
							.Where(it => !includeDeleted ? it.Index == false : true)
							.Select(it => it.Value.Result)
							.AsQueryable();
			return entries;
		}

		public Task<IQueryable<Budget>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Budget>>(() => GetAllEntries(includeDeleted));
		}

		IQueryable<Budget> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Budget, bool>> filter, bool includeDeleted = false)
		{
			var filterDelegate = filter.Compile();
			var results = _db.Query<Budget, bool, Guid>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
			return results.AsQueryable();
		}

		public Task<IQueryable<Budget>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Budget, bool>> filter, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Budget>>(() => GetFilteredEntries(filter, includeDeleted));
		}

		public async Task DeleteEntryAsync(Budget entry, bool hardDelete = false)
		{
			if (hardDelete)
				await _db.DeleteAsync<Budget>(entry);
			else
			{
				entry.EditDateTime = DateTimeOffset.Now;
				entry.IsDeleted = true;
				await _db.SaveAsync<Budget>(entry);
			}
		}

		public async Task<Budget> AddOrUpdateEntryAsync(Budget entry)
		{
			if (entry.BudgetId.Equals(Guid.Empty)) entry.BudgetId = Guid.NewGuid();
			entry.EditDateTime = DateTimeOffset.Now;
			await _db.SaveAsync(entry);
			return entry;
		}

		public async Task BatchUpdateEntriesAsync(ICollection<Budget> entries)
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

		public async Task<Budget> GetEntryAsync(Guid key)
		{
			return await _db.LoadAsync<Budget>(key);
		}

		public void Dispose()
		{
			_db.FlushAsync().Wait(2000);
		}

		public async Task Commit()
		{
			await _db.FlushAsync();
		}

		public Task<IQueryable<Budget>> GetUpdatedEntries(DateTimeOffset date)
		{
			return TaskEx.Run<IQueryable<Budget>>(() => _db.Query<Budget, DateTimeOffset, Guid>("EditDateTime")
				.Where(it => it.Index >= date)
				.Select(it => it.Value.Result).AsQueryable());
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Budget, Guid>().Select(it => it.Key).Count();
			else
				return _db.Query<Budget, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).Count();
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<Budget>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Budget>>(() =>
			{
				if (includeDeleted)
					return _db.Query<Budget, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Value.Result).AsQueryable();
				else
					return _db.Query<Budget, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Value.Result).AsQueryable();
			});
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Budget, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Key).Count();
			else
				return _db.Query<Budget, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Key).Count();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
		}
	}
}
