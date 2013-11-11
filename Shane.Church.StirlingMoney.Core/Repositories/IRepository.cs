using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Repositories
{
	public interface IRepository<T, TKey> : IDisposable
	{
		/// <summary>
		/// Get all entries.
		/// </summary>
		/// <returns></returns>
		//IQueryable<T> GetAllEntries(bool includeDeleted = false);

		Task<IQueryable<T>> GetAllEntriesAsync(bool includeDeleted = false);

		int GetEntriesCount(bool includeDeleted = false);

		Task<int> GetEntriesCountAsync(bool includeDeleted = false);

		/// <summary>
		/// Get filtered entries.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		//IQueryable<T> GetFilteredEntries(Expression<Func<T, bool>> filter, bool includeDeleted = false);

		Task<IQueryable<T>> GetFilteredEntriesAsync(Expression<Func<T, bool>> filter, bool includeDeleted = false);

		Task<IQueryable<T>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false);

		int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false);

		Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false);

		Task<T> GetEntryAsync(TKey key);

		/// <summary>
		/// DeleteEntry
		/// </summary>
		/// <param name="entry"></param>
		//void DeleteEntry(T entry, bool hardDelete = false);

		Task DeleteEntryAsync(T entry, bool hardDelete = false);

		/// <summary>
		/// Add or update an entry.
		/// </summary>
		/// <param name="Entry"></param>
		/// <returns></returns>
		//T AddOrUpdateEntry(T entry);

		Task<T> AddOrUpdateEntryAsync(T entry);

		//void BatchUpdateEntries(ICollection<T> entries);

		Task BatchUpdateEntriesAsync(ICollection<T> entries);

		Task Commit();

		Task<IQueryable<T>> GetUpdatedEntries(DateTimeOffset date);
	}
}
