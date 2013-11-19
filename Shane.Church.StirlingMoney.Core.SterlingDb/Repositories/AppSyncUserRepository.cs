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
	public class AppSyncUserRepository : IRepository<AppSyncUser, string>
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;
		private ISettingsService _settings;

		public AppSyncUserRepository(SterlingEngine engine, ISettingsService settings)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			if (settings == null) throw new ArgumentNullException("settings");
			_settings = settings;
			_db = _engine.SterlingDatabase.GetDatabase("Money");
		}

		public IQueryable<string> GetAllKeys(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<AppSyncUser, string>().Select(it => it.Key).AsQueryable();
			else
			{
				return _db.Query<AppSyncUser, bool, string>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).AsQueryable();
			}
		}

		public Dictionary<string, TIndex> GetAllIndexKeys<TIndex>(string indexName, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<AppSyncUser, TIndex, string>(indexName).ToDictionary(key => key.Key, val => val.Index);
			else
			{
				var activeKeys = _db.Query<AppSyncUser, bool, string>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
				return _db.Query<AppSyncUser, TIndex, string>(indexName).Where(it => activeKeys.Contains(it.Key)).ToDictionary(key => key.Key, val => val.Index);
			}
		}

		IQueryable<AppSyncUser> GetAllEntries(bool includeDeleted = false)
		{
			var entries = _db.Query<AppSyncUser, bool, string>("IsDeleted")
							.Where(it => !includeDeleted ? it.Index == false : true)
							.Select(it => it.Value.Result)
							.AsQueryable();
			return entries;
		}

		public Task<IQueryable<AppSyncUser>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<AppSyncUser>>(() => GetAllEntries(includeDeleted));
		}

		IQueryable<AppSyncUser> GetFilteredEntries(System.Linq.Expressions.Expression<Func<AppSyncUser, bool>> filter, bool includeDeleted = false)
		{
			var filterDelegate = filter.Compile();
			var results = _db.Query<AppSyncUser, bool, string>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
			return results.AsQueryable();
		}

		public Task<IQueryable<AppSyncUser>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<AppSyncUser, bool>> filter, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<AppSyncUser>>(() => GetFilteredEntries(filter, includeDeleted));
		}

		public async Task DeleteEntryAsync(string entryId, bool hardDelete = false)
		{
			if (hardDelete)
				await _db.DeleteAsync(typeof(AppSyncUser), entryId);
			else
			{
				var entry = await this.GetEntryAsync(entryId);
				entry.EditDateTime = DateTimeOffset.Now;
				entry.IsDeleted = true;
				await _db.SaveAsync<AppSyncUser>(entry);
			}
		}


		public async Task<AppSyncUser> AddOrUpdateEntryAsync(AppSyncUser entry)
		{
			entry.EditDateTime = DateTimeOffset.Now;
			await _db.SaveAsync(entry);
			return entry;
		}

		public async Task BatchUpdateEntriesAsync(ICollection<AppSyncUser> entries)
		{
			foreach (var entry in entries)
			{
				if (entry.IsDeleted)
				{
					await DeleteEntryAsync(entry.UserEmail, true);
				}
				else
				{
					await AddOrUpdateEntryAsync(entry);
				}
			}
		}

		public async Task<AppSyncUser> GetEntryAsync(string key)
		{
			return await _db.LoadAsync<AppSyncUser>(key);
		}

		public async Task Commit()
		{
			//await _db.FlushAsync();
			//_engine.Activate();
			await _engine.SterlingDatabase.GetDatabase("Money").RefreshAsync();
		}

		public Task<IQueryable<AppSyncUser>> GetUpdatedEntries(DateTimeOffset date)
		{
			return TaskEx.Run<IQueryable<AppSyncUser>>(() => _db.Query<AppSyncUser, DateTimeOffset, string>("EditDateTime")
				.Where(it => it.Index >= date)
				.Select(it => it.Value.Result).AsQueryable());
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<AppSyncUser, string>().Select(it => it.Key).Count();
			else
				return _db.Query<AppSyncUser, bool, string>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).Count();
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<AppSyncUser>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<AppSyncUser>>(() =>
			{
				if (includeDeleted)
					return _db.Query<AppSyncUser, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Value.Result).AsQueryable();
				else
					return _db.Query<AppSyncUser, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Value.Result).AsQueryable();
			});
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<AppSyncUser, TIndex, string>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Key).Count();
			else
				return _db.Query<AppSyncUser, TIndex, bool, string>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Key).Count();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
		}
	}
}
