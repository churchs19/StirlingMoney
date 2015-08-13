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
    public class CategoryRepository : IDataRepository<Core.Data.Category, Guid>
    {
        public Category AddOrUpdateEntry(Category entry)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                entry.EditDateTime = DateTimeOffset.Now;
                db.InsertOrReplace(Data.Category.FromCore(entry));
            }
            return entry;
        }

        public async Task<Category> AddOrUpdateEntryAsync(Category entry)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            entry.EditDateTime = DateTimeOffset.Now;
            await db.InsertOrReplaceAsync(Data.Category.FromCore(entry));
            return entry;
        }

        public void BatchUpdateEntries(ICollection<Category> entries)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                foreach(var entry in entries)
                {
                    entry.EditDateTime = DateTime.Now;
                }
                var sqliteEntries = AutoMapper.Mapper.Map<List<Category>, List<Data.Category>>(entries.ToList());
                db.UpdateAll(sqliteEntries);
            }
        }

        public async Task BatchUpdateEntriesAsync(ICollection<Category> entries)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            foreach (var entry in entries)
            {
                entry.EditDateTime = DateTime.Now;
            }
            var sqliteEntries = AutoMapper.Mapper.Map<List<Category>, List<Data.Category>>(entries.ToList());
            await db.UpdateAllAsync(sqliteEntries);
        }

        public void DeleteEntry(Guid entryId, bool hardDelete = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                if(hardDelete)
                {
                    db.Delete<Data.Category>(entryId);
                }
                else
                {
                    var entry = db.Get<Data.Category>(entryId);
                    if(entry!= null)
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
            var entry = await db.GetAsync<Data.Category>(entryId);
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

        public IQueryable<Category> GetAllEntries(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var resultsQuery = db.Table<Data.Category>().OrderBy(it => it.CategoryName);
                if (!includeDeleted) resultsQuery = resultsQuery.Where(it => !it.IsDeleted);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                return AutoMapper.Mapper.Map<List<Data.Category>, List<Category>>(results).AsQueryable();
            }
        }

        public async Task<IQueryable<Category>> GetAllEntriesAsync(bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var resultsQuery = db.Table<Data.Category>().OrderBy(it => it.CategoryName);
            if (!includeDeleted) resultsQuery = resultsQuery.Where(it => it.IsDeleted);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            return AutoMapper.Mapper.Map<List<Data.Category>, List<Category>>(results).AsQueryable();
        }

        public int GetEntriesCount(bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                return includeDeleted ? db.Table<Data.Category>().Count() : db.Table<Data.Category>().Where(it => !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetEntriesCountAsync(bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            return includeDeleted ? await db.Table<Data.Category>().CountAsync() : await db.Table<Data.Category>().Where(it => !it.IsDeleted).CountAsync();
        }

        public Category GetEntry(Guid key)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var entry = db.Get<Data.Category>(key);
                return entry != null ? entry.ToCore() : null;
            }
        }

        public async Task<Category> GetEntryAsync(Guid key)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var entry = await db.GetAsync<Data.Category>(key);
            return entry != null ? entry.ToCore() : null;
        }

        public IQueryable<Category> GetFilteredEntries(Expression<Func<Category, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                var resultsQuery = db.Table<Data.Category>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).OrderBy(it => it.CategoryName);
                if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
                var results = resultsQuery.ToList();
                var coreResults = AutoMapper.Mapper.Map<List<Data.Category>, List<Category>>(results);
                return coreResults.AsQueryable();
            }
        }

        public async  Task<IQueryable<Category>> GetFilteredEntriesAsync(Expression<Func<Category, bool>> filter, bool includeDeleted = false, int currentRow = 0, int? pageSize = null)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            var resultsQuery = db.Table<Data.Category>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).OrderBy(it => it.CategoryName);
            if (pageSize.HasValue && pageSize.Value > 0) resultsQuery = resultsQuery.Skip(currentRow).Take(pageSize.Value);
            var results = await resultsQuery.ToListAsync();
            var coreResults = AutoMapper.Mapper.Map<List<Data.Category>, List<Category>>(results);
            return coreResults.AsQueryable();
        }

        public int GetFilteredEntriesCount(Expression<Func<Core.Data.Category, bool>> filter, bool includeDeleted = false)
        {
            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                var filterDelegate = filter.Compile();
                return db.Table<Data.Category>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).Count();
            }
        }

        public async Task<int> GetFilteredEntriesCountAsync(Expression<Func<Core.Data.Category, bool>> filter, bool includeDeleted = false)
        {
            var db = StirlingMoneyDatabaseInstance.GetDbAsync();
            var filterDelegate = filter.Compile();
            return await db.Table<Data.Category>().Where(it => includeDeleted ? filterDelegate(it.ToCore()) : filterDelegate(it.ToCore()) && !it.IsDeleted).CountAsync();
        }
    }
}
