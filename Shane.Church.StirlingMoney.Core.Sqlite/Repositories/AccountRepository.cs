﻿using Shane.Church.StirlingMoney.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Repositories
{
    public class AccountRepository : IDataRepository<Core.Data.Account, Guid>
    {
        public Core.Data.Account AddOrUpdateEntry(Core.Data.Account entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                entry.EditDateTime = DateTimeOffset.Now;
                var sqliteEntry = Data.Account.FromCore(entry);
                db.InsertOrReplace(sqliteEntry);
                return entry;
            }
        }

        public async Task<Core.Data.Account> AddOrUpdateEntryAsync(Core.Data.Account entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
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
                }
                var sqliteEntries = Mapper.Map<List<Core.Data.Account>, List<Data.Account>>(entries.ToList());
                db.UpdateAll(sqliteEntries);
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Core.Data.Account> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTimeOffset.Now;
            }
            var sqliteEntries = Mapper.Map<List<Core.Data.Account>, List<Data.Account>>(entries.ToList());
            await db.UpdateAllAsync(sqliteEntries);
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if (hardDelete)
                {
                    //TODO: delete associated records
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

        public IQueryable<Core.Data.Account> GetAllEntries(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var accounts = includeDeleted ? db.Table<Data.Account>().ToList() : db.Table<Data.Account>().Where(it => !it.IsDeleted).ToList();
                return Mapper.Map<List<Sqlite.Data.Account>, List<Core.Data.Account>>(accounts).AsQueryable();
            }
        }

        public async Task<IQueryable<Core.Data.Account>> GetAllEntriesAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var accounts = includeDeleted ? await db.Table<Data.Account>().ToListAsync() : await db.Table<Data.Account>().Where(it => !it.IsDeleted).ToListAsync();
            return Mapper.Map<List<Sqlite.Data.Account>, List<Core.Data.Account>>(accounts).AsQueryable();
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

        public IQueryable<Core.Data.Account> GetFilteredEntries(Expression<Func<Core.Data.Account, bool>> filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                var results = db.Table<Data.Account>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).ToList();
                var coreResults = Mapper.Map<List<Data.Account>, List<Core.Data.Account>>(results);
                return coreResults.AsQueryable();
            }
        }

        public async Task<IQueryable<Core.Data.Account>> GetFilteredEntriesAsync(Expression<Func<Core.Data.Account, bool>> filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            var results = await db.Table<Data.Account>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).ToListAsync();
            var coreResults = Mapper.Map<List<Data.Account>, List<Core.Data.Account>>(results);
            return coreResults.AsQueryable();
        }
    }
}