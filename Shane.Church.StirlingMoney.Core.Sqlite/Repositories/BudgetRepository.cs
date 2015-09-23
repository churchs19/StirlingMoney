using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shane.Church.StirlingMoney.Core.Data;
using System.Linq.Expressions;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Repositories
{
    public class BudgetRepository : IDataRepository<Core.Data.Budget, Guid>
    {
        public Budget AddOrUpdateEntry(Budget entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (entry.BudgetId.Equals(Guid.Empty)) entry.BudgetId = Guid.NewGuid();
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Budget.FromCore(entry));
            }
            return entry;
        }

        public async Task<Budget> AddOrUpdateEntryAsync(Budget entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            if (entry.BudgetId.Equals(Guid.Empty)) entry.BudgetId = Guid.NewGuid();
            entry.EditDateTime = DateTimeOffset.Now;
            await db.InsertOrReplaceAsync(Data.Budget.FromCore(entry));
            return entry;
        }

        public void BatchUpdateEntries(ICollection<Budget> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                foreach(var entry in entries)
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                    db.InsertOrReplace(Data.Budget.FromCore(entry));
                }
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Budget> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
                await db.InsertOrReplaceAsync(Data.Budget.FromCore(entry));
            }
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if(hardDelete)
                {
                    db.Delete<Data.Budget>(entryId);
                }
                else
                {
                    var entry = db.Get<Data.Budget>(entryId);
                    if(entry != null)
                    {
                        entry.IsDeleted = true;
                        entry.EditDateTime = DateTimeOffset.Now;
                        db.Update(entry);
                    }
                }
            }
        }

        public async Task DeleteEntryAsync(Guid entryId, bool hardDelete = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.Budget>(entryId);
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

        public IQueryable<Budget> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var resultsQuery = db.Table<Data.Budget>().OrderBy(it => it.BudgetName);
                if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Budget>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Budget>().OrderBy(it => it.BudgetName);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.Budget>().Count() : db.Table<Data.Budget>().Where(it => !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            return includeDeleted ? await db.Table<Data.Budget>().CountAsync() : await db.Table<Data.Budget>().Where(it => !it.IsDeleted).CountAsync();

        }

        public Budget GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Get<Data.Budget>(key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public async Task<Budget> GetEntryAsync(Guid key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.Budget>(key);
            return entry != null ? entry.ToCore() : null;
        }

        public IQueryable<Budget> GetFilteredEntries(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select * from [Budget] where {0}", filter);
                if (!includeDeleted)
                {
                    query += " and [IsDeleted] = 0";
                }
                var resultsQuery = db.Query<Data.Budget>(query);
                List<Data.Budget> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async  Task<IQueryable<Budget>> GetFilteredEntriesAsync(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var query = string.Format("select * from [Budget] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            var resultsQuery = await db.QueryAsync<Data.Budget>(query);
            List<Data.Budget> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetFilteredEntriesCount(string filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select count(*) from [Budget] where {0}", filter);
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
            var query = string.Format("select count(*) from [Budget] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            return await db.ExecuteScalarAsync<int>(query);
        }
    }
}
