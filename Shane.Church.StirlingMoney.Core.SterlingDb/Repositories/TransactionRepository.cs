using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb.Repositories
{
	public class TransactionRepository : IRepository<Transaction, Guid>, ITransactionSum
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;
		private ISettingsService _settings;

		public TransactionRepository(SterlingEngine engine, ISettingsService settings)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			if (settings == null) throw new ArgumentNullException("settings");
			_settings = settings;
			_db = _engine.SterlingDatabase.GetDatabase("Money");
		}

		public IQueryable<Guid> GetAllKeys(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Transaction, Guid>().Select(it => it.Key).AsQueryable();
			else
			{
				return _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).AsQueryable();
			}
		}

		public Dictionary<Guid, TIndex> GetAllIndexKeys<TIndex>(string indexName, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Transaction, TIndex, Guid>(indexName).ToDictionary(key => key.Key, val => val.Index);
			else
			{
				var activeKeys = _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
				return _db.Query<Transaction, TIndex, Guid>(indexName).Where(it => activeKeys.Contains(it.Key)).ToDictionary(key => key.Key, val => val.Index);
			}
		}

		IQueryable<Transaction> GetAllEntries(bool includeDeleted = false)
		{
			var entries = _db.Query<Transaction, bool, Guid>("IsDeleted")
							.Where(it => !includeDeleted ? it.Index == false : true)
							.Select(it => it.Value.Result)
							.AsQueryable();
			return entries;
		}

		public Task<IQueryable<Transaction>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Transaction>>(() => GetAllEntries(includeDeleted));
		}

		IQueryable<Transaction> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Transaction, bool>> filter, bool includeDeleted = false)
		{
			var filterDelegate = filter.Compile();
			var results = _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
			return results.AsQueryable();
		}

		public Task<IQueryable<Transaction>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Transaction, bool>> filter, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Transaction>>(() => GetFilteredEntries(filter, includeDeleted));
		}

		public async Task DeleteEntryAsync(Guid entryId, bool hardDelete = false)
		{
			if (hardDelete)
				await _db.DeleteAsync(typeof(Transaction), entryId);
			else
			{
				var entry = await this.GetEntryAsync(entryId);
				entry.EditDateTime = DateTimeOffset.Now;
				entry.IsDeleted = true;
				await _db.SaveAsync<Transaction>(entry);
			}
		}


		public async Task<Transaction> AddOrUpdateEntryAsync(Transaction entry)
		{
			if (entry.TransactionId.Equals(Guid.Empty)) entry.TransactionId = Guid.NewGuid();
			entry.EditDateTime = DateTimeOffset.Now;
			await _db.SaveAsync(entry);
			return entry;
		}

		public async Task BatchUpdateEntriesAsync(ICollection<Transaction> entries)
		{
			foreach (var entry in entries)
			{
				if (entry.IsDeleted)
				{
					await DeleteEntryAsync(entry.TransactionId, true);
				}
				else
				{
					await AddOrUpdateEntryAsync(entry);
				}
			}
		}

		public async Task<Transaction> GetEntryAsync(Guid key)
		{
			return await _db.LoadAsync<Transaction>(key);
		}

		public double GetSumByAccount(Guid accountId)
		{
			var activeIds = _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
			return _db.Query<Transaction, Guid, double, Guid>("TransactionAccountIdAmount").Where(it => it.Index.Item1.Equals(accountId) && activeIds.Contains(it.Key)).Sum(it => it.Index.Item2);
		}

		public double GetPostedSumByAccount(Guid accountId)
		{
			var postedIds = _db.Query<Transaction, bool, Guid>("Posted").Where(it => it.Index).Select(it => it.Key).ToList();
			var activeIds = _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
			var activePostedIds = postedIds.Intersect(activeIds).ToList();
			return _db.Query<Transaction, Guid, double, Guid>("TransactionAccountIdAmount")
					.Where(it => it.Index.Item1.Equals(accountId) && activePostedIds.Contains(it.Key))
					.Sum(it => it.Index.Item2);
		}

		public async Task Commit()
		{
			//await _db.FlushAsync();
			//_engine.Activate();
			await _engine.SterlingDatabase.GetDatabase("Money").RefreshAsync();
		}

		public async Task<IQueryable<Transaction>> GetUpdatedEntries(DateTimeOffset date)
		{
			var list = _db.Query<Transaction, DateTimeOffset, Guid>("EditDateTime")
			.Where(it => it.Index >= date)
			.Select(it => it.Key).ToList();
			var items = new List<Transaction>();
			foreach (var i in list)
			{
				var item = await _db.LoadAsync<Transaction>(i);
				items.Add(item);
			}
			return items.AsQueryable();
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Transaction, Guid>().Select(it => it.Key).Count();
			else
				return _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).Count();
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<Transaction>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Transaction>>(() =>
			{
				if (includeDeleted)
					return _db.Query<Transaction, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Value.Result).AsQueryable();
				else
					return _db.Query<Transaction, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Value.Result).AsQueryable();
			});
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Transaction, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Key).Count();
			else
				return _db.Query<Transaction, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Key).Count();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
		}

		public double GetSumBetweenDates(DateTimeOffset startDate, DateTimeOffset endDate)
		{
			var activeIds = _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
			return _db.Query<Transaction, DateTimeOffset, double, Guid>("TransactionDate")
				.Where(it => it.Index.Item1 >= startDate && it.Index.Item1 < endDate && activeIds.Contains(it.Key))
				.Sum(it => it.Index.Item2);
		}

		public double GetSumBetweenDatesByCategory(Guid categoryId, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			var activeIds = _db.Query<Transaction, bool, Guid>("IsDeleted")
				.Where(it => !it.Index)
				.Select(it => it.Key)
				.ToList();
			var dateIds = _db.Query<Transaction, DateTimeOffset, double, Guid>("TransactionDate")
				.Where(it => it.Index.Item1 >= startDate && it.Index.Item1 < endDate && activeIds.Contains(it.Key))
				.Select(it => it.Key)
				.ToList();
			return _db.Query<Transaction, Guid, double, Guid>("TransactionCategoryAmount")
				.Where(it => it.Index.Item1 == categoryId && dateIds.Contains(it.Key))
				.Sum(it => it.Index.Item2);
		}
	}
}
