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
    public class GoalRepository : IDataRepository<Core.Data.Goal, Guid>
    {
        public Goal AddOrUpdateEntry(Goal entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (entry.GoalId.Equals(Guid.Empty)) entry.GoalId = Guid.NewGuid();
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Goal.FromCore(entry));
            }
            return entry;
        }

        public async Task<Goal> AddOrUpdateEntryAsync(Goal entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            if (entry.GoalId.Equals(Guid.Empty)) entry.GoalId = Guid.NewGuid();
            entry.EditDateTime = DateTimeOffset.Now;
            await db.InsertOrReplaceAsync(Data.Goal.FromCore(entry));
            return entry;
        }

        public void BatchUpdateEntries(ICollection<Goal> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                foreach(var entry in entries)
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                    db.InsertOrReplace(Data.Goal.FromCore(entry));
                }
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Goal> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
                await db.InsertOrReplaceAsync(Data.Goal.FromCore(entry));
            }
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (hardDelete)
                {
                    db.Delete<Data.Goal>(entryId);
                }
                else
                {
                    var entry = db.Get<Data.Goal>(entryId);
                    if (entry != null)
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
            var entry = await db.GetAsync<Data.Goal>(entryId);
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

        public IQueryable<Goal> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var resultsQuery = db.Table<Data.Goal>().OrderBy(it => it.GoalName);
                if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Goal>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Goal>().OrderBy(it => it.GoalName);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.Goal>().Count() : db.Table<Data.Goal>().Where(it => !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            return includeDeleted ? await db.Table<Data.Goal>().CountAsync() : await db.Table<Data.Goal>().Where(it => !it.IsDeleted).CountAsync();
        }

        public Goal GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Get<Data.Goal>(key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public async Task<Goal> GetEntryAsync(Guid key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.Goal>(key);
            return entry != null ? entry.ToCore() : null;
        }

        public IQueryable<Goal> GetFilteredEntries(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select * from [Goal] where {0}", filter);
                if (!includeDeleted)
                {
                    query += " and [IsDeleted] = 0";
                }
                var resultsQuery = db.Query<Data.Goal>(query);
                List<Data.Goal> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<Goal>> GetFilteredEntriesAsync(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var query = string.Format("select * from [Goal] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            var resultsQuery = await db.QueryAsync<Data.Goal>(query);
            List<Data.Goal> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetFilteredEntriesCount(string filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select count(*) from [Goal] where {0}", filter);
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
            var query = string.Format("select count(*) from [Goal] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            return await db.ExecuteScalarAsync<int>(query);
        }
    }
}
