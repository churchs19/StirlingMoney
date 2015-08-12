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
    public class AppSyncUserRepository : IDataRepository<Core.Data.AppSyncUser, Guid>
    {
        public AppSyncUser AddOrUpdateEntry(AppSyncUser entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.AppSyncUser.FromCore(entry));
            }
            return entry;
        }

        public async Task<AppSyncUser> AddOrUpdateEntryAsync(AppSyncUser entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            entry.EditDateTime = DateTimeOffset.Now;
            await db.InsertOrReplaceAsync(Data.AppSyncUser.FromCore(entry));
            return entry;
        }

        public void BatchUpdateEntries(ICollection<AppSyncUser> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                foreach (var entry in entries)
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                }
                var sqliteEntries = AutoMapper.Mapper.Map<List<AppSyncUser>, List<Data.AppSyncUser>>(entries.ToList());
                db.UpdateAll(entries);
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<AppSyncUser> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
            }
            var sqliteEntries = AutoMapper.Mapper.Map<List<AppSyncUser>, List<Data.AppSyncUser>>(entries.ToList());
            await db.UpdateAllAsync(sqliteEntries);
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (hardDelete)
                {
                    db.Delete<Data.AppSyncUser>(entryId);
                }
                else
                {
                    var entry = db.Get<Data.AppSyncUser>(entryId);
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
            var entry = await db.GetAsync<Data.AppSyncUser>(entryId);
            if (entry != null)
            {
                if (hardDelete)
                {
                    await db.DeleteAsync(entry);
                }
                else
                {
                    entry.EditDateTime = DateTimeOffset.Now;
                    entry.IsDeleted = false;
                    await db.UpdateAsync(entry);
                }
            }
        }

        public IQueryable<AppSyncUser> GetAllEntries(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var sqliteEntries = includeDeleted ? db.Table<Data.AppSyncUser>().ToList() : db.Table<Data.AppSyncUser>().Where(it => !it.IsDeleted).ToList();
                return AutoMapper.Mapper.Map<List<Data.AppSyncUser>, List<AppSyncUser>>(sqliteEntries).AsQueryable();
            }
        }

        public async Task<IQueryable<AppSyncUser>> GetAllEntriesAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var sqliteEntries = includeDeleted ? await db.Table<Data.AppSyncUser>().ToListAsync() : await db.Table<Data.AppSyncUser>().Where(it => !it.IsDeleted).ToListAsync();
            return AutoMapper.Mapper.Map<List<Data.AppSyncUser>, List<AppSyncUser>>(sqliteEntries).AsQueryable();
        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.AppSyncUser>().Count() : db.Table<Data.AppSyncUser>().Where(it => !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            return includeDeleted ? await db.Table<Data.AppSyncUser>().CountAsync() : await db.Table<Data.AppSyncUser>().Where(it => !it.IsDeleted).CountAsync();
        }

        public AppSyncUser GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Get<Data.AppSyncUser>(key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public async Task<AppSyncUser> GetEntryAsync(Guid key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.AppSyncUser>(key);
            return entry != null ? entry.ToCore() : null;
        }

        public IQueryable<AppSyncUser> GetFilteredEntries(Expression<Func<AppSyncUser, bool>> filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                var results = db.Table<Data.AppSyncUser>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).ToList();
                var coreResults = AutoMapper.Mapper.Map<List<Data.AppSyncUser>, List<AppSyncUser>>(results);
                return coreResults.AsQueryable();
            }
        }

        public async Task<IQueryable<AppSyncUser>> GetFilteredEntriesAsync(Expression<Func<AppSyncUser, bool>> filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            var results = await db.Table<Data.AppSyncUser>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).ToListAsync();
            var coreResults = AutoMapper.Mapper.Map<List<Data.AppSyncUser>, List<AppSyncUser>>(results);
            return coreResults.AsQueryable();
        }
    }
}