﻿using Shane.Church.StirlingMoney.Core.Repositories;
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
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Budget.FromCore(entry));
            }
            return entry;
        }

        public async Task<Budget> AddOrUpdateEntryAsync(Budget entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
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
                }
                var sqliteEntries = AutoMapper.Mapper.Map<List<Budget>, List<Data.Budget>>(entries.ToList());
                db.UpdateAll(sqliteEntries);
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Budget> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
            }
            var sqliteEntries = AutoMapper.Mapper.Map<List<Budget>, List<Data.Budget>>(entries.ToList());
            await db.UpdateAllAsync(sqliteEntries);
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

        public IQueryable<Budget> GetAllEntries(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var sqliteEntries = includeDeleted ? db.Table<Data.Budget>().ToList() : db.Table<Data.Budget>().Where(it => !it.IsDeleted).ToList();
                return AutoMapper.Mapper.Map<List<Data.Budget>, List<Budget>>(sqliteEntries).AsQueryable();
            }
        }

        public async Task<IQueryable<Budget>> GetAllEntriesAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var sqliteEntries = includeDeleted ? await db.Table<Data.Budget>().ToListAsync() : await db.Table<Data.Budget>().Where(it => !it.IsDeleted).ToListAsync();
            return AutoMapper.Mapper.Map<List<Data.Budget>, List<Budget>>(sqliteEntries).AsQueryable();
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

        public IQueryable<Budget> GetFilteredEntries(Expression<Func<Budget, bool>> filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                var results = db.Table<Data.Budget>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).ToList();
                var coreResults = AutoMapper.Mapper.Map<List<Data.Budget>, List<Budget>>(results);
                return coreResults.AsQueryable();
            }
        }

        public async  Task<IQueryable<Budget>> GetFilteredEntriesAsync(Expression<Func<Budget, bool>> filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            var results = await db.Table<Data.Budget>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).ToListAsync();
            var coreResults = AutoMapper.Mapper.Map<List<Data.Budget>, List<Budget>>(results);
            return coreResults.AsQueryable();
        }
    }
}