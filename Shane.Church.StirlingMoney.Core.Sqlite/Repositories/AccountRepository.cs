using Shane.Church.StirlingMoney.Core.Repositories;
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
                var sqliteEntry = Data.Account.FromCore(entry);
                db.InsertOrReplace(sqliteEntry);
                return entry;
            }
        }

        public Task<Core.Data.Account> AddOrUpdateEntryAsync(Core.Data.Account entry)
        {
            throw new NotImplementedException();
        }

        public void BatchUpdateEntries(ICollection<Core.Data.Account> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var sqliteEntries = Mapper.Map<List<Core.Data.Account>, List<Data.Account>>(entries.ToList());
                db.UpdateAll(sqliteEntries);
            }
        }

        public Task BatchUpdateEntriesAsync(ICollection<Core.Data.Account> entries)
        {
            throw new NotImplementedException();
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
                        account.IsDeleted = true;
                        db.Update(account);
                    }
                }
            }
        }

        public Task DeleteEntryAsync(Guid entryId, bool hardDelete = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Core.Data.Account> GetAllEntries(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var accounts = includeDeleted ? db.Table<Data.Account>().ToList() : db.Table<Data.Account>().Where(it => !it.IsDeleted).ToList();
                return Mapper.Map<List<Sqlite.Data.Account>, List<Core.Data.Account>>(accounts).AsQueryable();
            }
        }

        public Task<IQueryable<Core.Data.Account>> GetAllEntriesAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.Account>().Count() : db.Table<Data.Account>().Where(it => !it.IsDeleted).Count();
            }
        }

        public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Core.Data.Account GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Get<Sqlite.Data.Account>(key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public Task<Core.Data.Account> GetEntryAsync(Guid key)
        {
            throw new NotImplementedException();
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

        public Task<IQueryable<Core.Data.Account>> GetFilteredEntriesAsync(Expression<Func<Core.Data.Account, bool>> filter, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}
