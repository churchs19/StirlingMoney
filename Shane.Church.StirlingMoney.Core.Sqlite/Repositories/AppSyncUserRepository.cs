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
    public class AppSyncUserRepository : IDataRepository<Core.Data.AppSyncUser, string>
    {
        public AppSyncUser AddOrUpdateEntry(AppSyncUser entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (entry.AppSyncId.Equals(string.Empty)) entry.AppSyncId = Guid.NewGuid();
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.AppSyncUser.FromCore(entry));
            }
            return entry;
        }

        public async Task<AppSyncUser> AddOrUpdateEntryAsync(AppSyncUser entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            if (entry.AppSyncId.Equals(string.Empty)) entry.AppSyncId = Guid.NewGuid();
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
                    db.InsertOrReplace(Data.AppSyncUser.FromCore(entry));
                }
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<AppSyncUser> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
                await db.InsertOrReplaceAsync(Data.AppSyncUser.FromCore(entry));
            }
        }

        public void DeleteEntry(string entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Find<Data.AppSyncUser>(it => it.UserId == entryId);
                if (entry != null)
                {
                    if (hardDelete)
                    {
                        db.Delete(entry);
                    }
                    else
                    {
                        entry.EditDateTime = DateTimeOffset.Now;
                        entry.IsDeleted = true;
                        db.Update(entry);
                    }
                }
            }
        }

        public async Task DeleteEntryAsync(string entryId, bool hardDelete = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.FindAsync<Data.AppSyncUser>(it=>it.UserId == entryId);
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

        public IQueryable<AppSyncUser> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var resultsQuery = db.Table<Data.AppSyncUser>().OrderBy(it => it.UserEmail);
                if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<AppSyncUser>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.AppSyncUser>().OrderBy(it => it.UserEmail);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return results.Select(it => it.ToCore()).AsQueryable();
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

        public AppSyncUser GetEntry(string key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Find<Data.AppSyncUser>(it => it.UserId == key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public async Task<AppSyncUser> GetEntryAsync(string key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.FindAsync<Data.AppSyncUser>(it => it.UserId == key);
            return entry != null ? entry.ToCore() : null;
        }

        public IQueryable<AppSyncUser> GetFilteredEntries(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select * from [AppSyncUser] where {0}", filter);
                if (!includeDeleted)
                {
                    query += " and [IsDeleted] = 0";
                }
                var resultsQuery = db.Query<Data.AppSyncUser>(query);
                List<Data.AppSyncUser> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
                return results.Select(it => it.ToCore()).AsQueryable();
            }
        }

        public async Task<IQueryable<AppSyncUser>> GetFilteredEntriesAsync(string filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var query = string.Format("select * from [AppSyncUser] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            var resultsQuery = await db.QueryAsync<Data.AppSyncUser>(query);
            List<Data.AppSyncUser> results = pageSize.HasValue && pageSize.Value > 0 ? resultsQuery.Skip(currentRow).Take(pageSize.Value).ToList() : resultsQuery;
            return results.Select(it => it.ToCore()).AsQueryable();
        }

        public int GetFilteredEntriesCount(string filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var query = string.Format("select count(*) from [AppSyncUser] where {0}", filter);
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
            var query = string.Format("select count(*) from [AppSyncUser] where {0}", filter);
            if (!includeDeleted)
            {
                query += " and [IsDeleted] = 0";
            }
            return await db.ExecuteScalarAsync<int>(query);
        }
    }
}