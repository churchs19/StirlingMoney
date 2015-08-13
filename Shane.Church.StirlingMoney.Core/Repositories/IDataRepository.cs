using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Repositories
{
    public interface IDataRepository<T, TKey>
    {
        IQueryable<T> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null);

        Task<IQueryable<T>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null);

        int GetEntriesCount(bool includeDeleted = false);

        Task<int> GetEntriesCountAsync(bool includeDeleted = false);

        IQueryable<T> GetFilteredEntries(Expression<Func<T, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null);

        Task<IQueryable<T>> GetFilteredEntriesAsync(Expression<Func<T, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null);

        int GetFilteredEntriesCount(Expression<Func<T, bool>> filter, bool includeDeleted = false);

        Task<int> GetFilteredEntriesCountAsync(Expression<Func<T, bool>> filter, bool includeDeleted = false);

        T GetEntry(TKey key);

        Task<T> GetEntryAsync(TKey key);

        void DeleteEntry(TKey entryId, bool hardDelete = false);

        Task DeleteEntryAsync(TKey entryId, bool hardDelete = false);

        T AddOrUpdateEntry(T entry);

        Task<T> AddOrUpdateEntryAsync(T entry);

        void BatchUpdateEntries(ICollection<T> entries);

        Task BatchUpdateEntriesAsync(ICollection<T> entries);
    }
}
