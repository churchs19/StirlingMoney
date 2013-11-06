using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class AppSyncUserRepository : Core.Data.IRepository<Core.Data.AppSyncUser>
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public AppSyncUserRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.AppSyncUser> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.AppSyncUsers.ToList().Select(it => it.ToCoreAppAuthorizedUser()).AsQueryable();
				else
					return _context.AppSyncUsers.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == true)).ToList().Select(it => it.ToCoreAppAuthorizedUser()).AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.AppSyncUser>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.AppSyncUser>>(() =>
			{
				return GetAllEntries(includeDeleted);
			});
		}

		public IQueryable<Core.Data.AppSyncUser> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.AppSyncUser, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.AppSyncUsers.ToList().Select(it => it.ToCoreAppAuthorizedUser());
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public Task<IQueryable<Core.Data.AppSyncUser>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Core.Data.AppSyncUser, bool>> filter, bool includeDeleted = false)
		{
			return Task.Factory.StartNew<IQueryable<Core.Data.AppSyncUser>>(() =>
			{
				return GetFilteredEntries(filter, includeDeleted);
			});
		}

		public void DeleteEntry(Core.Data.AppSyncUser entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.AppSyncUsers.Where(it => it.UserEmail == entry.UserEmail).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.AppSyncUsers.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTime.UtcNow;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Task DeleteEntryAsync(Core.Data.AppSyncUser entry, bool hardDelete = false)
		{
			return Task.Factory.StartNew(() =>
			{
				DeleteEntry(entry, hardDelete);
			});
		}

		public Core.Data.AppSyncUser AddOrUpdateEntry(Core.Data.AppSyncUser entry)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (!string.IsNullOrEmpty(entry.UserEmail))
				{
					var item = _context.AppSyncUsers.Where(it => it.UserEmail == entry.UserEmail).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.AppSyncUser();
						item.UserEmail = entry.UserEmail;
						_context.AppSyncUsers.InsertOnSubmit(item);
					}
					item.EditDateTime = DateTime.UtcNow;
					item.Id = entry.Id;
					item.IsDeleted = entry.IsDeleted;
					item.IsSyncOwner = entry.IsSyncOwner;

					_context.SubmitChanges();

					entry.Id = item.Id;
					entry.EditDateTime = DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc);
				}
			}
			return entry;
		}

		public Task<Core.Data.AppSyncUser> AddOrUpdateEntryAsync(Core.Data.AppSyncUser entry)
		{
			return Task.Factory.StartNew<Core.Data.AppSyncUser>(() =>
			{
				return AddOrUpdateEntry(entry);
			});
		}


		public void BatchAddOrUpdateEntries(System.Collections.Generic.ICollection<Core.Data.AppSyncUser> entries)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				foreach (var entry in entries)
				{
					if (!string.IsNullOrEmpty(entry.UserEmail))
					{
						var item = _context.AppSyncUsers.Where(it => it.UserEmail == entry.UserEmail).FirstOrDefault();
						if (item == null)
						{
							item = new StirlingMoney.Data.v3.AppSyncUser();
							item.UserEmail = entry.UserEmail;
							_context.AppSyncUsers.InsertOnSubmit(item);
						}
						item.EditDateTime = DateTime.UtcNow;
						item.Id = entry.Id;
						item.IsDeleted = entry.IsDeleted;
						item.IsSyncOwner = entry.IsSyncOwner;
					}
				}
				_context.SubmitChanges();
			}
		}

		public Task BatchAddOrUpdateEntriesAsync(System.Collections.Generic.ICollection<Core.Data.AppSyncUser> entries)
		{
			return Task.Factory.StartNew(() =>
			{
				BatchAddOrUpdateEntries(entries);
			});
		}
	}

	public static class AuthorizedUserExtensions
	{
		public static Core.Data.AppSyncUser ToCoreAppAuthorizedUser(this Shane.Church.StirlingMoney.Data.v3.AppSyncUser item)
		{
			return new Core.Data.AppSyncUser()
			{
				Id = item.Id,
				UserEmail = item.UserEmail,
				IsSyncOwner = item.IsSyncOwner,
				EditDateTime = new DateTimeOffset(DateTime.SpecifyKind(item.EditDateTime, DateTimeKind.Utc), new TimeSpan(0)),
				IsDeleted = item.IsDeleted
			};
		}
	}
}
