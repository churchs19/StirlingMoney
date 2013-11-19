using Shane.Church.StirlingMoney.Core.Data;
using System;
using System.Collections.Generic;
using Wintellect.Sterling.Core.Database;

namespace Shane.Church.StirlingMoney.Core.SterlingDb
{
	public class StirlingMoneyDatabaseInstance : BaseDatabaseInstance
	{
		protected override List<ITableDefinition> RegisterTables()
		{
			var tables = new List<ITableDefinition>()
			{
				CreateTableDefinition<AppSyncUser, string>(it => it.UserEmail)
					.WithIndex<AppSyncUser, DateTimeOffset, string>("EditDateTime", it => it.EditDateTime)
					.WithIndex<AppSyncUser, Boolean, string>("IsDeleted", it => it.IsDeleted),
				CreateTableDefinition<Category, Guid>(it => it.CategoryId)
					.WithIndex<Category, string, Guid>("CategoryName", it=>it.CategoryName)
					.WithIndex<Category, DateTimeOffset, Guid>("EditDateTime", it => it.EditDateTime)
					.WithIndex<Category, Boolean, Guid>("IsDeleted", it => it.IsDeleted)
					.WithIndex<Category, string, Boolean, Guid>("CategoryNameIsDeleted", it => new Tuple<string, Boolean>(it.CategoryName, it.IsDeleted)),
				CreateTableDefinition<Account, Guid>(it => it.AccountId)
					.WithIndex<Account, double, Guid>("InitialBalance", it => it.InitialBalance)
					.WithIndex<Account, DateTimeOffset, Guid>("EditDateTime", it => it.EditDateTime)
					.WithIndex<Account, Boolean, Guid>("IsDeleted", it => it.IsDeleted),
				CreateTableDefinition<Budget, Guid>(it => it.BudgetId)
					.WithIndex<Budget, DateTimeOffset, Guid>("EditDateTime", it => it.EditDateTime)
					.WithIndex<Budget, Boolean, Guid>("IsDeleted", it => it.IsDeleted),
				CreateTableDefinition<Goal, Guid>(it => it.GoalId)
					.WithIndex<Goal, DateTimeOffset, Guid>("EditDateTime", it => it.EditDateTime)
					.WithIndex<Goal, Boolean, Guid>("IsDeleted", it => it.IsDeleted),
				CreateTableDefinition<Transaction, Guid>(it => it.TransactionId)
					.WithIndex<Transaction, Guid, Guid>("TransactionAccountId", it => it.AccountId)
					.WithIndex<Transaction, Guid, Double, Guid>("TransactionAccountIdAmount", it=>new Tuple<Guid, Double>(it.AccountId, it.Amount))
					.WithIndex<Transaction, Guid, Boolean, Guid>("TransactionAccountIdIsDeleted", it => new Tuple<Guid, Boolean>(it.AccountId, it.IsDeleted))
					.WithIndex<Transaction, Guid, DateTimeOffset, Guid>("TransactionAccountIdEditDateTime", it => new Tuple<Guid, DateTimeOffset>(it.AccountId, it.EditDateTime))
					.WithIndex<Transaction, Guid, long, Guid>("TransactionAccountIdCheckNumber", it=>new Tuple<Guid, long>(it.AccountId, it.CheckNumber))
					.WithIndex<Transaction, Boolean, Guid>("Posted", it=> it.Posted)
					.WithIndex<Transaction, DateTimeOffset, double, Guid>("TransactionDate", it=>new Tuple<DateTimeOffset, double>(it.TransactionDate, it.Amount))
					.WithIndex<Transaction, Guid, double, Guid>("TransactionCategoryAmount", it=>new Tuple<Guid, double>(it.CategoryId, it.Amount))
					.WithIndex<Transaction, DateTimeOffset, Guid>("EditDateTime", it => it.EditDateTime)
					.WithIndex<Transaction, DateTimeOffset, DateTimeOffset, Guid>("TransactionDateEditDateTime", it=>new Tuple<DateTimeOffset, DateTimeOffset>(it.TransactionDate, it.EditDateTime))
					.WithIndex<Transaction, DateTimeOffset, Boolean, Guid>("EditDateTimePosted", it => new Tuple<DateTimeOffset, Boolean>(it.EditDateTime, it.Posted))
					.WithIndex<Transaction, Boolean, Guid>("IsDeleted", it => it.IsDeleted),
				CreateTableDefinition<Setting, string>(it=>it.Key)
					.WithIndex<Setting, Boolean, string>("IsDeleted", it => it.IsDeleted)
					.WithIndex<Setting, DateTimeOffset, string>("EditDateTime", it => it.EditDateTime)
			};

			return tables;
		}
	}
}
