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
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Goal.FromCore(entry));
            }
            return entry;
        }

        public async Task<Goal> AddOrUpdateEntryAsync(Goal entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
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
                }
                var sqliteEntries = AutoMapper.Mapper.Map<List<Goal>, List<Data.Goal>>(entries.ToList());
                db.UpdateAll(sqliteEntries);
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Goal> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
            }
            var sqliteEntries = AutoMapper.Mapper.Map<List<Goal>, List<Data.Goal>>(entries.ToList());
            await db.UpdateAllAsync(sqliteEntries);
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
                return AutoMapper.Mapper.Map<List<Data.Goal>, List<Goal>>(results).AsQueryable();
            }
        }

        public async Task<IQueryable<Goal>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Goal>().OrderBy(it => it.GoalName);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return AutoMapper.Mapper.Map<List<Data.Goal>, List<Goal>>(results).AsQueryable();
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

        public IQueryable<Goal> GetFilteredEntries(Expression<Func<Goal, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                var resultsQuery = db.Table<Data.Goal>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).OrderBy(it => it.GoalName);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                var coreResults = AutoMapper.Mapper.Map<List<Data.Goal>, List<Goal>>(results);
                return coreResults.AsQueryable();
            }
        }

        public async Task<IQueryable<Goal>> GetFilteredEntriesAsync(Expression<Func<Goal, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            var resultsQuery = db.Table<Data.Goal>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).OrderBy(it => it.GoalName);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            var coreResults = AutoMapper.Mapper.Map<List<Data.Goal>, List<Goal>>(results);
            return coreResults.AsQueryable();
        }

        public int GetFilteredEntriesCount(Expression<Func<Core.Data.Goal, bool>> filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                return db.Table<Data.Goal>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetFilteredEntriesCountAsync(Expression<Func<Core.Data.Goal, bool>> filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            return await db.Table<Data.Goal>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).CountAsync();
        }
    }
}
