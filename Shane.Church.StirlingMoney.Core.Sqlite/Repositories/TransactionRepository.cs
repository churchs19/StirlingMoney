﻿using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Data;
using System.Linq.Expressions;
using SQLite.Net;
using SQLiteNetExtensions;
using SQLiteNetExtensionsAsync;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Repositories
{
    public class TransactionRepository : IDataRepository<Core.Data.Transaction, Guid>, ITransactionSum, ITransactionSearch
    {
        public Transaction AddOrUpdateEntry(Transaction entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (entry.TransactionId.Equals(Guid.Empty)) entry.TransactionId = Guid.NewGuid();
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Transaction.FromCore(entry));
            }
            return entry;
        }

        public async Task<Transaction> AddOrUpdateEntryAsync(Transaction entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            if (entry.TransactionId.Equals(Guid.Empty)) entry.TransactionId = Guid.NewGuid();
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
                    db.InsertOrReplace(Data.Transaction.FromCore(entry));
                }
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Transaction> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
                await db.InsertOrReplaceAsync(Data.Transaction.FromCore(entry));
            }
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
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Transaction>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Transaction>().OrderByDescending(it => it.TransactionDate).ThenByDescending(it => it.EditDateTime);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return results.Select(it => it.ToCore()).AsQueryable();
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

        public IQueryable<Transaction> GetFilteredEntries(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select * from [Transaction] where {0}", filter);
                if (!includeDeleted)
                {
                    query += " and [IsDeleted] = 0";
                }
                query += " order by [TransactionDate] desc, [EditDateTime] desc";
                var resultsQuery = db.Query<Data.Transaction>(query);
                List<Data.Transaction> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Transaction>> GetFilteredEntriesAsync(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var query = string.Format("select * from [Transaction] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            query += " order by [TransactionDate] desc, [EditDateTime] desc";
            var resultsQuery = await db.QueryAsync<Data.Transaction>(query);
            List<Data.Transaction> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetFilteredEntriesCount(string filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select count(*) from [Transaction] where {0}", filter);
                if (!includeDeleted)
                {
                    query += " and [IsDeleted] = 0";
                }
                return db.ExecuteScalar<int>(query);
            }
        }

        public async Task<int> GetFilteredEntriesCountAsync(string filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var query = string.Format("select count(*) from [Transaction] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            return await db.ExecuteScalarAsync<int>(query);
        }

        public double GetPostedSumByAccount(Guid accountId)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return db.Table<Data.Transaction>().Where(it => it.AccountId == accountId && it.Posted && !it.IsDeleted).Select(it => it.Amount).Sum();
            }
        }

        public List<Transaction> GetSearchResults(Guid AccountId, string searchText)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                throw new NotImplementedException();
            }
        }

        public Task<List<Transaction>> GetSearchResultsAsync(Guid AccountId, string searchText)
        {
            throw new NotImplementedException();
        }

        public double GetSumBetweenDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return db.Table<Data.Transaction>().Where(it => it.TransactionDate >= startDate &&
                                                            it.TransactionDate < endDate &&
                                                            !it.IsDeleted).Select(it => it.Amount).Sum();
            }
        }

        public double GetSumBetweenDatesByCategory(Guid categoryId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return db.Table<Data.Transaction>().Where(it => it.CategoryId == categoryId &&
                                                            it.TransactionDate >= startDate &&
                                                            it.TransactionDate < endDate &&
                                                            !it.IsDeleted).Select(it => it.Amount).Sum();
            }
        }

        public double GetSumByAccount(Guid accountId)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return db.Table<Data.Transaction>().Where(it => it.AccountId == accountId && !it.IsDeleted).Select(it => it.Amount).Sum();
            }
        }
    }
}
