using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Data
{
    public interface IRepository<T>
        where T : class
    {
        /// <summary>
        /// Get all entries.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAllEntries(bool includeDeleted = false);

		Task<IQueryable<T>> GetAllEntriesAsync(bool includeDeleted = false);

        /// <summary>
        /// Get filtered entries.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IQueryable<T> GetFilteredEntries(Expression<Func<T, bool>> filter, bool includeDeleted = false);

		Task<IQueryable<T>> GetFilteredEntriesAsync(Expression<Func<T, bool>> filter, bool includeDeleted = false);

		/// <summary>
        /// DeleteEntry
        /// </summary>
        /// <param name="entry"></param>
        void DeleteEntry(T entry, bool hardDelete = false);

		Task DeleteEntryAsync(T entry, bool hardDelete = false);

		/// <summary>
        /// Add or update an entry.
        /// </summary>
        /// <param name="Entry"></param>
        /// <returns></returns>
        T AddOrUpdateEntry(T entry);

		Task<T> AddOrUpdateEntryAsync(T entry);
	}
}
