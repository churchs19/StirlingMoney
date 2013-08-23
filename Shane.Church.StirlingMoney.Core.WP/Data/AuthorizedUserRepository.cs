using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class AuthorizedUserRepository : Core.Data.IRepository<Core.Data.AuthorizedUser>
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public AuthorizedUserRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.AuthorizedUser> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.AuthorizedUsers.Select(it => it.ToCoreAuthorizedUser());
				else
					return _context.AuthorizedUsers.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == true)).Select(it => it.ToCoreAuthorizedUser());
			}
		}

		public IQueryable<Core.Data.AuthorizedUser> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.AuthorizedUser, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.AuthorizedUsers.Select(it => it.ToCoreAuthorizedUser()).ToList();
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public void DeleteEntry(Core.Data.AuthorizedUser entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.AuthorizedUsers.Where(it => it.AuthorizedUserId == entry.AuthorizedUserId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.AuthorizedUsers.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTimeOffset.Now;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Core.Data.AuthorizedUser AddOrUpdateEntry(Core.Data.AuthorizedUser entry)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (!string.IsNullOrEmpty(entry.MicrosoftAccountEmail))
				{
					var item = _context.AuthorizedUsers.Where(it => it.AuthorizedUserId == entry.AuthorizedUserId).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.AuthorizedUser();
						item.AuthorizedUserId = entry.AuthorizedUserId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.AuthorizedUserId;
						_context.AuthorizedUsers.InsertOnSubmit(item);
					}
					item.MicrosoftAccountEmail = entry.MicrosoftAccountEmail;
					item.EditDateTime = DateTimeOffset.Now;
					item.Id = entry.Id;
					item.StirlingMoneyAccountId = entry.StirlingMoneyAccountId;
					item.IsDeleted = entry.IsDeleted;

					_context.SubmitChanges();

					entry.AuthorizedUserId = item.AuthorizedUserId;
					entry.Id = item.Id;
					entry.EditDateTime = item.EditDateTime;
				}
			}
			return entry;
		}
	}

	public static class AuthorizedUserExtensions
	{
		public static Core.Data.AuthorizedUser ToCoreAuthorizedUser(this Shane.Church.StirlingMoney.Data.v3.AuthorizedUser item)
		{
			return new Core.Data.AuthorizedUser()
			{
				Id = item.Id,
				AuthorizedUserId = item.AuthorizedUserId,
				MicrosoftAccountEmail = item.MicrosoftAccountEmail,
				StirlingMoneyAccountId = item.StirlingMoneyAccountId,
				EditDateTime = item.EditDateTime,
				IsDeleted = item.IsDeleted
			};
		}
	}
}
