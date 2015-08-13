using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Data;
using System.Linq.Expressions;
using AutoMapper;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Repositories
{
    public class TransactionRepository : IDataRepository<Core.Data.Transaction, Guid>
    {
        public Transaction AddOrUpdateEntry(Transaction entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Transaction.FromCore(entry));
            }
            return entry;
        }

        public async Task<Transaction> AddOrUpdateEntryAsync(Transaction entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            entry.EditDateTime = DateTimeOffset.Now;
            await db.InsertOrReplaceAsync(Data.Transaction.FromCore(entry));
            return entry;
        }

        public void BatchUpdateEntries(ICollection<Transaction> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                foreach(var entry in entries)
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                }
                var sqliteEntries = Mapper.Map<List<Transaction>, List<Data.Transaction>>(entries.ToList());
                db.UpdateAll(sqliteEntries);
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Transaction> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
            }
            var sqliteEntries = Mapper.Map<List<Transaction>, List<Data.Transaction>>(entries.ToList());
            await db.UpdateAllAsync(sqliteEntries);
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if(hardDelete)
                {
                    db.Delete<Data.Transaction>(entryId);
                }
                else
                {
                    var entry = db.Get<Data.Transaction>(entryId);
                    if(entry != null)
                    {
                        entry.EditDateTime = DateTimeOffset.Now;
                        entry.IsDeleted = true;
                        db.Update(entry);
                    }
                }
            }
        }

        public async Task DeleteEntryAsync(Guid entryId, bool hardDelete = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.Transaction>(entryId);
            if(entry != null)
            {
                if(hardDelete)
                {
                    await db.DeleteAsync(entry);
                }
                else
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                    entry.IsDeleted = true;
                    await db.UpdateAsync(entry);
                }
            }
        }

        public IQueryable<Transaction> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var resultsQuery = db.Table<Data.Transaction>().OrderByDescending(it => it.TransactionDate).ThenByDescending(it => it.EditDateTime);
                if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                return Mapper.Map<List<Data.Transaction>, List<Transaction>>(results).AsQueryable();
            }
        }

        public async Task<IQueryable<Transaction>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Transaction>().OrderByDescending(it => it.TransactionDate).ThenByDescending(it => it.EditDateTime);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return Mapper.Map<List<Data.Transaction>, List<Transaction>>(results).AsQueryable();

        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.Transaction>().Count() : db.Table<Data.Transaction>().Where(it => !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            return includeDeleted ? await db.Table<Data.Transaction>().CountAsync() : await db.Table<Data.Transaction>().Where(it => !it.IsDeleted).CountAsync();
        }

        public Transaction GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var trans = db.Get<Sqlite.Data.Transaction>(key);
                return trans != null ? trans.ToCore() : null;
            }
        }

        public async Task<Transaction> GetEntryAsync(Guid key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var trans = await db.GetAsync<Sqlite.Data.Transaction>(key);
            return trans != null ? trans.ToCore() : null;
        }

        public IQueryable<Transaction> GetFilteredEntries(Expression<Func<Transaction, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                var resultsQuery = db.Table<Data.Transaction>()
                    .Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted)
                    .OrderByDescending(it => it.TransactionDate)
                    .ThenByDescending(it => it.EditDateTime);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                var coreResults = AutoMapper.Mapper.Map<List<Data.Transaction>, List<Transaction>>(results);
                return coreResults.AsQueryable();
            }
        }

        public async Task<IQueryable<Transaction>> GetFilteredEntriesAsync(Expression<Func<Transaction, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            var resultsQuery = db.Table<Data.Transaction>()
                .Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted)
                .OrderByDescending(it => it.TransactionDate)
                .ThenByDescending(it => it.EditDateTime);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            var coreResults = AutoMapper.Mapper.Map<List<Data.Transaction>, List<Transaction>>(results);
            return coreResults.AsQueryable();
        }

        public int GetFilteredEntriesCount(Expression<Func<Core.Data.Transaction, bool>> filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                return db.Table<Data.Transaction>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetFilteredEntriesCountAsync(Expression<Func<Core.Data.Transaction, bool>> filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            return await db.Table<Data.Transaction>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).CountAsync();
        }
    }
}
