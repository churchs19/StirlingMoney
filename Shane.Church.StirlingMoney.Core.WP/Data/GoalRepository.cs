﻿using Ninject;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.WP.Data
{
	public class GoalRepository : Core.Data.IRepository<Core.Data.Goal>
	{
		Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext _context;

		public GoalRepository()
		{
			_context = KernelService.Kernel.Get<Shane.Church.StirlingMoney.Data.v3.StirlingMoneyDataContext>();
		}

		public IQueryable<Core.Data.Goal> GetAllEntries(bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				if (includeDeleted)
					return _context.Goals.Select(it => it.ToCoreGoal());
				else
					return _context.Goals.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && it.IsDeleted == false)).Select(it => it.ToCoreGoal());
			}
		}

		public IQueryable<Core.Data.Goal> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Core.Data.Goal, bool>> filter, bool includeDeleted = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Goals.Select(it => it.ToCoreGoal()).ToList();
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public void DeleteEntry(Core.Data.Goal entry, bool hardDelete = false)
		{
			lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
			{
				var pEntry = _context.Goals.Where(it => it.GoalId == entry.GoalId).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Goals.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTimeOffset.Now;
						pEntry.IsDeleted = true;
					}
					_context.SubmitChanges();
				}
			}
		}

		public Core.Data.Goal AddOrUpdateEntry(Core.Data.Goal entry)
		{
			if (!entry.AccountId.Equals(Guid.Empty) && !string.IsNullOrWhiteSpace(entry.GoalName))
			{
				lock (StirlingMoney.Data.v3.StirlingMoneyDataContext.LockObject)
				{
					var item = _context.Goals.Where(it => it.GoalId == entry.GoalId).FirstOrDefault();
					if (item == null)
					{
						item = new StirlingMoney.Data.v3.Goal();
						item.GoalId = entry.GoalId.Equals(Guid.Empty) ? Guid.NewGuid() : entry.GoalId;
						_context.Goals.InsertOnSubmit(item);
					}
					item.Account = _context.Accounts.Where(it => it.AccountId == entry.AccountId).FirstOrDefault();
					item.Amount = entry.Amount;
					item.EditDateTime = DateTimeOffset.Now;
					item.GoalName = entry.GoalName;
					item.Id = entry.Id;
					item.InitialBalance = entry.InitialBalance;
					item.IsDeleted = entry.IsDeleted;
					item.TargetDate = entry.TargetDate;

					_context.SubmitChanges();

					entry.GoalId = item.GoalId;
					entry.Id = item.Id;
					entry.EditDateTime = item.EditDateTime;
				}
			}
			return entry;
		}
	}

	public static class GoalExtensions
	{
		public static Core.Data.Goal ToCoreGoal(this Shane.Church.StirlingMoney.Data.v3.Goal item)
		{
			return new Core.Data.Goal()
			{
				Id = item.Id,
				AccountId = item.Account.AccountId,
				Amount = item.Amount,
				EditDateTime = item.EditDateTime,
				GoalId = item.GoalId,
				GoalName = item.GoalName,
				InitialBalance = item.InitialBalance,
				IsDeleted = item.IsDeleted,
				TargetDate = item.TargetDate
			};
		}
	}
}
