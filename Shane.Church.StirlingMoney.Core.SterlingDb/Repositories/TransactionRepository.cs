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
    public class TransactionRepository : IRepository<Transaction, Guid>, ITransactionSum, ITransactionSearch
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
            return Task.Run<IQueryable<Transaction>>(() => GetAllEntries(includeDeleted));
        }

        IQueryable<Transaction> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Transaction, bool>> filter, bool includeDeleted = false)
        {
            var filterDelegate = filter.Compile();
            var results = _db.Query<Transaction, bool, Guid>("IsDeleted").Where(it => includeDeleted ? filterDelegate(it.Value.Result) : filterDelegate(it.Value.Result) && !it.Index).Select(it => it.Value.Result);
            return results.AsQueryable();
        }

        public Task<IQueryable<Transaction>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Transaction, bool>> filter, bool includeDeleted = false)
        {
            return Task.Run<IQueryable<Transaction>>(() => GetFilteredEntries(filter, includeDeleted));
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
            return Task.Run<int>(() => GetEntriesCount(includeDeleted));
        }

        public Task<IQueryable<Transaction>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
        {
            return Task.Run<IQueryable<Transaction>>(() =>
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
            return Task.Run<int>(() => GetIndexFilteredEntriesCount<TIndex>(indexName, indexValue, includeDeleted));
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

        public List<Transaction> GetSearchResults(Guid AccountId, string searchText)
        {
            var searchString = String.IsNullOrWhiteSpace(searchText) ? "" : searchText.Trim().ToLowerInvariant();

            var activeTransactions = _db.Query<Transaction, Guid, Boolean, Guid>("TransactionAccountIdIsDeleted").Where(it => !it.Index.Item2 && it.Index.Item1.Equals(AccountId));
            var categories = _db.Query<Category, string, Boolean, Guid>("CategoryNameIsDeleted").Where(it => !it.Index.Item2);
            var activeTransactionIds = activeTransactions.Select(it => it.Key).ToList();

            //Locations Indexed Search
            var locations = _db.Query<Transaction, string, Guid>("Location").Where(it => activeTransactionIds.Contains(it.Key) && it.Index.ToLowerInvariant().Contains(searchString)).ToList();
            activeTransactionIds = activeTransactionIds.Except(locations.Select(it => it.Key)).ToList();

            //Categories Indexed Search
            var categoryItems = (from t in _db.Query<Transaction, Guid, double, Guid>("TransactionCategoryAmount")
                              join c in categories on t.Index.Item1 equals c.Key
                              where c.Index.Item1.ToLowerInvariant().Contains(searchString) &&
                                    activeTransactionIds.Contains(t.Key)
                              select t).ToList();
            activeTransactionIds = activeTransactionIds.Except(categoryItems.Select(it => it.Key)).ToList();

            //Amounts Indexed Search
            var amounts = _db.Query<Transaction, Guid, Double, Guid>("TransactionAccountIdAmount").Where(it => activeTransactionIds.Contains(it.Key) && it.Index.Item2.ToString("C").ToLowerInvariant().Contains(searchString)).ToList();
            activeTransactionIds = activeTransactionIds.Except(amounts.Select(it => it.Key)).ToList();

            //Check Number Indexed Search
            var checkNumbers = _db.Query<Transaction, Guid, long, Guid>("TransactionAccountIdCheckNumber").Where(it => activeTransactionIds.Contains(it.Key) && it.Index.Item2.ToString().Contains(searchString)).ToList();
            activeTransactionIds = activeTransactionIds.Except(checkNumbers.Select(it => it.Key)).ToList();

            //Dates Indexed Search
            var transactionDates = (from t in _db.Query<Transaction, DateTimeOffset, DateTimeOffset, Guid>("TransactionDateEditDateTime")
                                    where activeTransactionIds.Contains(t.Key) &&
                                        (t.Index.Item1.ToString("d").ToLowerInvariant().Contains(searchString) ||
                                        t.Index.Item1.ToString("D").ToLowerInvariant().Contains(searchString) ||
                                        t.Index.Item1.ToString("M").ToLowerInvariant().Contains(searchString) ||
                                        t.Index.Item1.ToString("Y").ToLowerInvariant().Contains(searchString) ||
                                        t.Index.Item1.ToString().ToLowerInvariant().Contains(searchString))
                                    select t).ToList();
            activeTransactionIds = activeTransactionIds.Except(transactionDates.Select(it => it.Key)).ToList();

            //Unindexed Search Results (slower)
            var notes = (from t in activeTransactions.Where(it => activeTransactionIds.Contains(it.Key))
                            where String.IsNullOrWhiteSpace(t.Value.Result.Note) ? false : t.Value.Result.Note.ToLowerInvariant().Contains(searchString)
                            select t.Value.Result).ToList();
            activeTransactionIds = activeTransactionIds.Except(notes.Select(it => it.TransactionId)).ToList();

            var results = locations.Select(it => it.Value.Result).Union(
                categoryItems.Select(it => it.Value.Result)).Union(
                amounts.Select(it => it.Value.Result)).Union(
                checkNumbers.Select(it => it.Value.Result)).Union(
                transactionDates.Select(it => it.Value.Result)).Union(
                notes).ToList();

            var distinctResults = results.Distinct(new TransactionComparer()).ToList();

            return distinctResults.OrderByDescending(it => it.TransactionDate).ThenBy(it => it.EditDateTime).ToList();
        }

        public Task<List<Transaction>> GetSearchResultsAsync(Guid AccountId, string searchText)
        {
            return Task.Run<List<Transaction>>(() => GetSearchResults(AccountId, searchText));
        }
    }

    internal class TransactionComparer : IEqualityComparer<Transaction>
    {
        public bool Equals(Transaction x, Transaction y)
        {
            return x.TransactionId.Equals(y.TransactionId);
        }

        public int GetHashCode(Transaction obj)
        {
            return obj.GetHashCode();
        }
    }
}
