using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;

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
					return _context.Categories.Select(it => it.ToCoreCategory());
				else
					return _context.Categories.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).Select(it => it.ToCoreCategory());
			}
		}

		public IQueryable<Core.Data.Category> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Category, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Categories.Select(it => it.ToCoreCategory()).ToList();
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
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
						pEntry.EditDateTime = DateTimeOffset.Now;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
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
					item.EditDateTime = DateTimeOffset.Now;
					item.Id = entry.Id;
					item.IsDeleted = entry.IsDeleted;

					_context.SubmitChanges();

					entry.CategoryId = item.CategoryId;
					entry.Id = item.Id;
					entry.EditDateTime = item.EditDateTime;
				}
			}
			return entry;
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
				EditDateTime = item.EditDateTime,
				IsDeleted = item.IsDeleted
			};
		}
	}
}
