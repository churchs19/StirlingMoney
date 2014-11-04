using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Data.Update
{
	public static class UpdateController
	{
		public static async Task UpgradeV2(string connectionString)
		{
			using (var context = new StirlingMoney.Data.v2.StirlingMoneyDataContext(connectionString))
			{
				var engine = ContainerService.Container.Locate<SterlingEngine>();
				var db = engine.SterlingDatabase.GetDatabase("Money");
				//await db.PurgeAsync();

				foreach (Data.v2.Category c in context.Categories)
				{
					await db.SaveAsync<Category>(new Category() { CategoryId = c.CategoryId, CategoryName = c.CategoryName, EditDateTime = DateTimeOffset.Now, IsDeleted = false });
				}

				foreach (Data.v2.Account a in context.Accounts)
				{
					await db.SaveAsync<Account>(new Account() { AccountId = a.AccountId, AccountName = a.AccountName, InitialBalance = a.InitialBalance, EditDateTime = DateTimeOffset.Now, IsDeleted = false });
				}

				foreach (Data.v2.Budget b in context.Budgets)
				{
					await db.SaveAsync<Budget>(new Budget()
					{
						BudgetId = b.BudgetId,
						BudgetAmount = b.BudgetAmount,
						BudgetName = b.BudgetName,
						BudgetPeriod = (PeriodType)b.BudgetPeriod,
						CategoryId = b.CategoryId,
						StartDate = b.StartDate,
						EndDate = b.EndDate,
						EditDateTime = DateTimeOffset.Now,
						IsDeleted = false
					});
				}

				foreach (Data.v2.Goal g in context.Goals)
				{
					await db.SaveAsync<Goal>(new Goal()
					{
						GoalId = g.GoalId,
						GoalName = g.GoalName,
						AccountId = g._accountId,
						Amount = g.Amount,
						StartDate = new DateTimeOffset(DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc), new TimeSpan(0)),
						TargetDate = new DateTimeOffset(DateTime.SpecifyKind(g.TargetDate, DateTimeKind.Utc), new TimeSpan(0)),
						InitialBalance = g.Account.AccountBalance,
						EditDateTime = DateTimeOffset.Now,
						IsDeleted = false
					});
				}

				foreach (Data.v2.Transaction t in context.Transactions)
				{
					await db.SaveAsync<Transaction>(new Transaction()
					{
						TransactionId = t.TransactionId,
						TransactionDate = new DateTimeOffset(DateTime.SpecifyKind(t.TransactionDate, DateTimeKind.Utc), new TimeSpan(0)),
						AccountId = t._accountId,
						Amount = t.Amount,
						CategoryId = t.CategoryId.HasValue ? t.CategoryId.Value : Guid.Empty,
						CheckNumber = t.CheckNumber.HasValue ? t.CheckNumber.Value : 0,
						Location = t.Location,
						Note = t.Note,
						Posted = t.Posted,
						EditDateTime = DateTimeOffset.Now,
						IsDeleted = false
					});
				}

				await db.FlushAsync();
			}
		}

		//public static void CreateV2Context(Data.v2.StirlingMoneyDataContext context)
		//{
		//	// Create the local database.
		//	context.CreateDatabase();

		//	//Prepopulate the  default category list
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryAutomobile });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryBills });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryBusiness });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryCharity });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryClothing });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryDeposit });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryEntertainment });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryFood });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryHealth });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryHome });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryUtilities });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryMiscellaneous });
		//	context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryVacation });

		//	context.SubmitChanges();

		//	//DatabaseSchemaUpdater updater = context.CreateDatabaseSchemaUpdater();
		//	//updater.DatabaseSchemaVersion = 1;
		//	//updater.Execute();
		//}

		public static void UpgradeV1(string oldConnectionString, string connectionString)
		{
			using (var oldContext = new Data.v1.StirlingMoneyDataContext(oldConnectionString))
			{
				using (var context = new Data.v2.StirlingMoneyDataContext(connectionString))
				{
					context.CreateDatabase();

					context.Categories.DeleteAllOnSubmit(context.Categories);
					context.SubmitChanges();
					var categoriesQuery = (from c in oldContext.Categories
										   select new { OldCategoryId = c.CategoryId, CategoryName = c.CategoryName });
					List<Update.v2.Category> categories = new List<v2.Category>();
					foreach (var c in categoriesQuery)
					{
						categories.Add(new v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = c.CategoryName, OldCategoryId = c.OldCategoryId });
					}
					foreach (var c in categories)
					{
						c.CategoryId = Guid.NewGuid();
						Data.v2.Category item = new Data.v2.Category() { CategoryId = c.CategoryId, CategoryName = c.CategoryName };
						context.Categories.InsertOnSubmit(item);
					}
					context.SubmitChanges();
					var accountsQuery = (from a in oldContext.Accounts
										 select new { OldAccountId = a.AccountId, AccountName = a.AccountName, InitialBalance = a.InitialBalance });
					List<Update.v2.Account> accounts = new List<v2.Account>();
					foreach (var a in accountsQuery)
					{
						accounts.Add(new Update.v2.Account() { AccountId = Guid.NewGuid(), AccountName = a.AccountName, InitialBalance = a.InitialBalance, OldAccountId = a.OldAccountId });
					}
					foreach (var a in accounts)
					{
						Data.v2.Account item = new Data.v2.Account() { AccountId = a.AccountId, AccountName = a.AccountName, InitialBalance = a.InitialBalance };
						context.Accounts.InsertOnSubmit(item);
					}
					context.SubmitChanges();
					var transactions = (from tx in oldContext.Transactions
										select new { TransactionDate = tx.TransactionDate, Amount = tx.Amount, CheckNumber = tx.CheckNumber, Location = tx.Location, Note = tx.Note, Posted = tx.Posted, OldCategoryId = tx.CategoryId, OldAccountId = tx.Account.AccountId });
					foreach (var tx in transactions)
					{
						Guid? categoryId = null;
						if (tx.OldCategoryId.HasValue)
						{
							categoryId = (from c in categories
										  where c.OldCategoryId == tx.OldCategoryId.Value
										  select c.CategoryId).FirstOrDefault();
						}
						Data.v2.Transaction item = new Data.v2.Transaction()
						{
							TransactionId = Guid.NewGuid(),
							Amount = tx.Amount,
							CategoryId = categoryId,
							CheckNumber = tx.CheckNumber,
							Location = tx.Location,
							Note = tx.Note,
							Posted = tx.Posted,
							TransactionDate = tx.TransactionDate
						};
						Guid accountId = (from a in accounts
										  where a.OldAccountId == tx.OldAccountId
										  select a.AccountId).FirstOrDefault();
						item.Account = (from a in context.Accounts
										where a.AccountId == accountId
										select a).FirstOrDefault();
						context.Transactions.InsertOnSubmit(item);
					}
					context.SubmitChanges();
				}
			}
		}
	}
}
