using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Repositories
{
    public class AccountRepository : IDataRepository<Core.Data.Account, Guid>
    {
        public Core.Data.Account AddOrUpdateEntry(Core.Data.Account entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (entry.AccountId.Equals(Guid.Empty)) entry.AccountId = Guid.NewGuid();
                entry.EditDateTime = DateTimeOffset.Now;
                var sqliteEntry = Data.Account.FromCore(entry);
                db.InsertOrReplace(sqliteEntry);
                return entry;
            }
        }

        public async Task<Core.Data.Account> AddOrUpdateEntryAsync(Core.Data.Account entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            if (entry.AccountId.Equals(Guid.Empty)) entry.AccountId = Guid.NewGuid();
            entry.EditDateTime = DateTimeOffset.Now;
            var sqliteEntry = Data.Account.FromCore(entry);
            await db.InsertOrReplaceAsync(sqliteEntry);
            return entry;
        }

        public void BatchUpdateEntries(ICollection<Core.Data.Account> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                foreach (var entry in entries)
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                    db.InsertOrReplace(Data.Account.FromCore(entry));
                }
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Core.Data.Account> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
                await db.InsertOrReplaceAsync(Data.Account.FromCore(entry));
            }
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                //TransactionRepository transRepo = new TransactionRepository();
                //var transactions = transRepo.GetFilteredEntries(string.Format("[AccountId] = '{0}'", entryId));
                //foreach (var t in transactions)
                //{
                //    transRepo.DeleteEntry(t.TransactionId, hardDelete);
                //}
                var transactionsQuery = hardDelete ? string.Format("delete from [Transaction] where [AccountId] = '{0}'", entryId) : string.Format("update [Transaction] set [IsDeleted] = 1 where [AccountId] = '{0}'", entryId);
                db.Execute(transactionsQuery);
                if (hardDelete)
                {
                    db.Delete<Sqlite.Data.Account>(entryId);
                }
                else
                {
                    var account = db.Get<Data.Account>(entryId);
                    if(account != null)
                    {
                        account.EditDateTime = DateTimeOffset.Now;
                        account.IsDeleted = true;
                        db.Update(account);
                    }
                }
            }
        }

        public async Task DeleteEntryAsync(Guid entryId, bool hardDelete = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.Account>(entryId);
            if(entry != null)
            {
                TransactionRepository transRepo = new TransactionRepository();
                var transactions = await transRepo.GetFilteredEntriesAsync(string.Format("[AccountId] = '{0}'", entryId));
                foreach (var t in transactions)
                {
                    await transRepo.DeleteEntryAsync(t.TransactionId, hardDelete);
                }
                if (hardDelete)
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

        public IQueryable<Core.Data.Account> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var resultsQuery = db.Table<Data.Account>();
                if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                return results.Select(it=>it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Core.Data.Account>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Account>();
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.Account>().Count() : db.Table<Data.Account>().Where(it => !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            return includeDeleted ? await db.Table<Data.Account>().CountAsync() : await db.Table<Data.Account>().Where(it => !it.IsDeleted).CountAsync();
        }

        public Core.Data.Account GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Get<Sqlite.Data.Account>(key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public async Task<Core.Data.Account> GetEntryAsync(Guid key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Sqlite.Data.Account>(key);
            return entry != null ? entry.ToCore() : null;
        }

        public IQueryable<Core.Data.Account> GetFilteredEntries(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select * from [Account] where {0}", filter);
                if(!includeDeleted)
                {
                    query += " and [IsDeleted] = 0";
                }
                var resultsQuery = db.Query<Data.Account>(query);
                List<Data.Account> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Core.Data.Account>> GetFilteredEntriesAsync(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var query = string.Format("select * from [Account] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            var resultsQuery = await db.QueryAsync<Data.Account>(query);
            List<Data.Account> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetFilteredEntriesCount(string filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select count(*) from [Account] where {0}", filter);
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
            var query = string.Format("select count(*) from [Account] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            return await db.ExecuteScalarAsync<int>(query);
        }
    }
}
