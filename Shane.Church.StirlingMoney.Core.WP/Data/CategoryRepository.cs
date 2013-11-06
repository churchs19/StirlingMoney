using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class CategoryRepository : Core.Data.IRepository<Core.Data.Category>
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public CategoryRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.Category> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.Categories.ToList().Select(it => it.ToCoreCategory()).AsQueryable();
				else
					return _context.Categories.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).ToList().Select(it => it.ToCoreCategory()).AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Category>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Category>>(() =>
			{
				return GetAllEntries(includeDeleted);
			});
		}

		public IQueryable<Core.Data.Category> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Category, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Categories.ToList().Select(it => it.ToCoreCategory());
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.Category>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Core.Data.Category, bool>> filter, bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.Category>>(() =>
			{
				return GetFilteredEntries(filter, includeDeleted);
			});
		}

		public void DeleteEntry(Core.Data.Category entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.Categories.Where(it => it.CategoryId == entry.CategoryId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Categories.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTime.UtcNow;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Task DeleteEntryAsync(Core.Data.Category entry, bool hardDelete = false)
		{
			return Task.Factory.StartNew(() =>
			{
				DeleteEntry(entry, hardDelete);
			});
		}

		public Core.Data.Category AddOrUpdateEntry(Core.Data.Category entry)
		{
			if (!string.IsNullOrWhiteSpace(entry.CategoryName))
			{
				lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
				{
					var item = _context.Categories.Where(it => it.CategoryId == entry.CategoryId).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.Category();
						item.CategoryId = entry.CategoryId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.CategoryId;
						_context.Categories.InsertOnSubmit(item);
					}
					item.CategoryName = entry.CategoryName;
					item.EditDateTime = DateTime.UtcNow;
					item.Id = entry.Id;
					item.IsDeleted = entry.IsDeleted;

					_context.SubmitChanges();

					entry.CategoryId = item.CategoryId;
					entry.Id = item.Id;
					entry.EditDateTime = DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc);
				}
			}
			return entry;
		}

		public Task<Core.Data.Category> AddOrUpdateEntryAsync(Core.Data.Category entry)
		{
			return Task.Factory.StartNew<Core.Data.Category>(() =>
				{
					return AddOrUpdateEntry(entry);
				});
		}

		public void BatchAddOrUpdateEntries(System.Collections.Generic.ICollection<Core.Data.Category> entries)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				foreach (var entry in entries)
				{
					if (!string.IsNullOrWhiteSpace(entry.CategoryName))
					{
						var item = _context.Categories.Where(it => it.CategoryId == entry.CategoryId).FirstOrDefault();
						if (item == null)
						{
							item = new StirlingMoney.Data.v3.Category();
							item.CategoryId = entry.CategoryId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.CategoryId;
							_context.Categories.InsertOnSubmit(item);
						}
						item.CategoryName = entry.CategoryName;
						item.EditDateTime = DateTime.UtcNow;
						item.Id = entry.Id;
						item.IsDeleted = entry.IsDeleted;
					}
				}
				_context.SubmitChanges();
			}
		}

		public Task BatchAddOrUpdateEntriesAsync(System.Collections.Generic.ICollection<Core.Data.Category> entries)
		{
			return Task.Factory.StartNew(() =>
			{
				BatchAddOrUpdateEntries(entries);
			});
		}
	}

	public static class CategoryExtensions
	{
		public static Core.Data.Category ToCoreCategory(this Shane.Church.StirlingMoney.Data.v3.Category item)
		{
			return new Core.Data.Category()
			{
				Id = item.Id,
				CategoryId = item.CategoryId,
				CategoryName = item.CategoryName,
				EditDateTime = new DateTimeOffset(DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc), new TimeSpan(0)),
				IsDeleted = item.IsDeleted
			};
		}
	}
}
