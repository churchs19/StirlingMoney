using System;
using System.Linq;
using System.Data.Linq;
using Microsoft.Phone.Data.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.Generic;

namespace Shane.Church.StirlingMoney.Data.Update
{
	public static class UpdateController
	{
		public static void CreateContext(Data.v2.StirlingMoneyDataContext context)
		{
			// Create the local database.
			context.CreateDatabase();

			//Prepopulate the  default category list
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryAutomobile });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryBills });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryBusiness });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryCharity });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryClothing });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryDeposit });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryEntertainment });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryFood });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryHealth });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryHome });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryUtilities });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryMiscellaneous });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryVacation });

			context.SubmitChanges();

			//DatabaseSchemaUpdater updater = context.CreateDatabaseSchemaUpdater();
			//updater.DatabaseSchemaVersion = 1;
			//updater.Execute();
		}


		public static void UpdateContext(Data.v2.StirlingMoneyDataContext context)
		{
			//DatabaseSchemaUpdater updater = context.CreateDatabaseSchemaUpdater();

			//if (updater.DatabaseSchemaVersion == 0)
			//{
			//    updater.AddTable<Budget>();
			//    updater.AddTable<Goal>();
			//    updater.DatabaseSchemaVersion = 1;
			//    updater.Execute();
			//}
		}

		public static void UpgradeV1(v1.StirlingMoneyDataContext oldContext, Data.v2.StirlingMoneyDataContext context)
		{
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
				Data.v2.Category item = new Data.v2.Category() { CategoryId = c.CategoryId, CategoryName = c.CategoryName};
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

		public static void DeleteV1()
		{
			using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				try
				{
					store.DeleteFile(v1.StirlingMoneyDataContext.DBFileName);
				}
				catch { }
			}
		}
	}
}
