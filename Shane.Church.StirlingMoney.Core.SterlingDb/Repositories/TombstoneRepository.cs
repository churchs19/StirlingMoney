using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb.Repositories
{
	public class TombstoneRepository : IRepository<Tombstone, string>
	{
		private SterlingEngine _engine;
		private ISterlingDatabaseInstance _db;

		public TombstoneRepository(SterlingEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			_db = _engine.SterlingDatabase.GetDatabase("Money");
		}

		public IQueryable<string> GetAllKeys(bool includeDeleted = false)
		{
			return _db.Query<Tombstone, string>().Select(it => it.Key).AsQueryable();
		}

		public Dictionary<string, TIndex> GetAllIndexKeys<TIndex>(string indexName, bool includeDeleted = false)
		{
			throw new NotSupportedException();
		}

		public Task<IQueryable<Tombstone>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Run(() => _db.Query<Tombstone, string>().Select(it => it.LazyValue.Value).AsQueryable());
		}

		public int GetEntriesCount(bool includeDeleted = false)
		{
			return _db.Query<Tombstone, string>().Count;
		}

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			return Task.Run(() => GetEntriesCount(includeDeleted));
		}

		public Task<IQueryable<Tombstone>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Tombstone, bool>> filter, bool includeDeleted = false)
		{
			throw new NotSupportedException();
		}

		public Task<IQueryable<Tombstone>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			throw new NotSupportedException();
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			throw new NotSupportedException();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			throw new NotSupportedException();
		}

		public async Task<Tombstone> GetEntryAsync(string key)
		{
			return await _db.LoadAsync<Tombstone>(key);
		}

		public async Task DeleteEntryAsync(string entryId, bool hardDelete = false)
		{
			await _db.DeleteAsync(typeof(Tombstone), entryId);
		}

		public async Task<Tombstone> AddOrUpdateEntryAsync(Tombstone entry)
		{
			await _db.SaveAsync<Tombstone>(entry);
			return entry;
		}

		public Task BatchUpdateEntriesAsync(ICollection<Tombstone> entries)
		{
			throw new NotSupportedException();
		}

		public async Task Commit()
		{
			await _db.FlushAsync();
		}

		public Task<IQueryable<Tombstone>> GetUpdatedEntries(DateTimeOffset date)
		{
			throw new NotSupportedException();
		}
	}
}
