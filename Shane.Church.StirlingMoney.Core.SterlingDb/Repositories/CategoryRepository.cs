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
	public class CategoryRepository : IRepository<Category, Guid>
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;
		private ISettingsService _settings;

		public CategoryRepository(SterlingEngine engine, ISettingsService settings)
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
				return _db.Query<Category, Guid>().Select(it => it.Key).AsQueryable();
			else
			{
				return _db.Query<Category, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).AsQueryable();
			}
		}

		public Dictionary<Guid, TIndex> GetAllIndexKeys<TIndex>(string indexName, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Category, TIndex, Guid>(indexName).ToDictionary(key => key.Key, val => val.Index);
			else
			{
				var activeKeys = _db.Query<Category, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).ToList();
				return _db.Query<Category, TIndex, Guid>(indexName).Where(it => activeKeys.Contains(it.Key)).ToDictionary(key => key.Key, val => val.Index);
			}
		}

		public IQueryable<Category> GetAllEntries(bool includeDeleted = false)
		{
			var entries = _db.Query<Category, bool, Guid>("IsDeleted")
							.Where(it => !includeDeleted ? it.Index == false : true)
							.Select(it => it.Value.Result)
							.AsQueryable();
			return entries;
		}

		public Task<IQueryable<Category>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Category>>(() => GetAllEntries(includeDeleted));
		}

		IQueryable<Category> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Category, bool>> filter, bool includeDeleted = false)
		{
			var filterDelegate = filter.Compile();
			var results = _db.Query<Category, bool, Guid>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
			return results.AsQueryable();
		}

		public Task<IQueryable<Category>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Category, bool>> filter, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Category>>(() => GetFilteredEntries(filter, includeDeleted));
		}

		public async Task DeleteEntryAsync(Guid entryId, bool hardDelete = false)
		{
			if (hardDelete)
				await _db.DeleteAsync(typeof(Category), entryId);
			else
			{
				var entry = await this.GetEntryAsync(entryId);
				entry.EditDateTime = DateTimeOffset.Now;
				entry.IsDeleted = true;
				await _db.SaveAsync<Category>(entry);
			}
		}

		public async Task<Category> AddOrUpdateEntryAsync(Category entry)
		{
			if (entry.CategoryId.Equals(Guid.Empty)) entry.CategoryId = Guid.NewGuid();
			entry.EditDateTime = DateTimeOffset.Now;
			await _db.SaveAsync(entry);
			return entry;
		}

		public async Task BatchUpdateEntriesAsync(ICollection<Category> entries)
		{
			foreach (var entry in entries)
			{
				if (entry.IsDeleted)
				{
					await DeleteEntryAsync(entry.CategoryId, true);
				}
				else
				{
					await AddOrUpdateEntryAsync(entry);
				}
			}
		}

		public async Task<Category> GetEntryAsync(Guid key)
		{
			return await _db.LoadAsync<Category>(key);
		}

		public async Task Commit()
		{
			//await _db.FlushAsync();
			//_engine.Activate();
			await _engine.SterlingDatabase.GetDatabase("Money").RefreshAsync();
		}

		public Task<IQueryable<Category>> GetUpdatedEntries(DateTimeOffset date)
		{
			return TaskEx.Run<IQueryable<Category>>(() => _db.Query<Category, DateTimeOffset, Guid>("EditDateTime")
				.Where(it => it.Index >= date)
				.Select(it => it.Value.Result).AsQueryable());
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Category, Guid>().Select(it => it.Key).Count();
			else
				return _db.Query<Category, bool, Guid>("IsDeleted").Where(it => !it.Index).Select(it => it.Key).Count();
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<Category>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<IQueryable<Category>>(() =>
			{
				if (includeDeleted)
					return _db.Query<Category, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Value.Result).AsQueryable();
				else
					return _db.Query<Category, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Value.Result).AsQueryable();
			});
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			if (includeDeleted)
				return _db.Query<Category, TIndex, Guid>(indexName).Where(it => it.Index.Equals(indexValue)).Select(it => it.Key).Count();
			else
				return _db.Query<Category, TIndex, bool, Guid>(indexName + "IsDeleted").Where(it => it.Index.Item1.Equals(indexValue) && !it.Index.Item2).Select(it => it.Key).Count();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			return TaskEx.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
		}
	}
}
