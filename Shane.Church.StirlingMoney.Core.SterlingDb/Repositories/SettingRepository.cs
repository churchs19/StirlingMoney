using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb.Repositories
{
	public class SettingRepository : IRepository<Setting, string>
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;

		public SettingRepository(SterlingEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			_db = _engine.SterlingDatabase.GetDatabase("Money");
		}

		public IQueryable<string> GetAllKeys(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Setting, string>().Select(it => it.Key).AsQueryable();
			else
			{
				return _db.Query<Setting, bool, string>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).AsQueryable();
			}
		}

		public Dictionary<string, TIndex> GetAllIndexKeys<TIndex>(string indexName, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Setting, TIndex, string>(indexName).ToDictionary(key => key.Key, val => val.Index);
			else
			{
				var activeKeys = _db.Query<Setting, bool, string>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
				return _db.Query<Setting, TIndex, string>(indexName).Where(it => activeKeys.Contains(it.Key)).ToDictionary(key => key.Key, val => val.Index);
			}
		}

		IQueryable<Setting> GetAllEntries(bool includeDeleted = false)
		{
			var entries = _db.Query<Setting, bool, string>("IsDeleted")
							.Where(it => !includeDeleted ? it.Index == false : true)
							.Select(it => it.Value.Result)
							.AsQueryable();
			return entries;
		}

		public Task<IQueryable<Setting>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Setting>>(() => GetAllEntries(includeDeleted));
		}

		IQueryable<Setting> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Setting, bool>> filter, bool includeDeleted = false)
		{
			var filterDelegate = filter.Compile();
			var results = _db.Query<Setting, bool, string>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
			return results.AsQueryable();
		}

		public Task<IQueryable<Setting>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Setting, bool>> filter, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Setting>>(() => GetFilteredEntries(filter, includeDeleted));
		}

		public async Task DeleteEntryAsync(string entryId, bool hardDelete = false)
		{
			await _db.DeleteAsync(typeof(Setting), entryId);
		}

		public async Task<Setting> AddOrUpdateEntryAsync(Setting entry)
		{
			entry.EditDateTime = DateTimeOffset.Now;
			await _db.SaveAsync(entry);
			return entry;
		}

		public async Task BatchUpdateEntriesAsync(ICollection<Setting> entries)
		{
			foreach (var entry in entries)
			{
				if (entry.IsDeleted)
				{
					await DeleteEntryAsync(entry.Key, true);
				}
				else
				{
					await AddOrUpdateEntryAsync(entry);
				}
			}
		}

		public async Task<Setting> GetEntryAsync(string key)
		{
			return await _db.LoadAsync<Setting>(key);
		}

		public async Task Commit()
		{
			//await _db.FlushAsync();
			//_engine.Activate();
			await _engine.SterlingDatabase.GetDatabase("Money").RefreshAsync();
		}

		public Task<IQueryable<Setting>> GetUpdatedEntries(DateTimeOffset date)
		{
			return TaskEx.Run<IQueryable<Setting>>(() => _db.Query<Setting, DateTimeOffset, string>("EditDateTime")
				.Where(it => it.Index >= date)
				.Select(it => it.Value.Result).AsQueryable());
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Setting, string>().Select(it => it.Key).Count();
			else
				return _db.Query<Setting, bool, string>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).Count();
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<Setting>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Setting>>(() =>
			{
				if (includeDeleted)
					return _db.Query<Setting, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Value.Result).AsQueryable();
				else
					return _db.Query<Setting, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Value.Result).AsQueryable();
			});
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Setting, TIndex, string>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Key).Count();
			else
				return _db.Query<Setting, TIndex, bool, string>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Key).Count();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
		}
	}
}
