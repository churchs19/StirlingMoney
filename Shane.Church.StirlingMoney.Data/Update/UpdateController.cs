﻿using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;

namespace Shane.Church.StirlingMoney.Data.Update
{
	public static class UpdateController
	{
		public static void ConfigureDatabase()
		{
			// Create the database if it does not exist.
			bool v1dbExists = false;
			using (Data.v2.StirlingMoneyDataContext v2db = new Data.v2.StirlingMoneyDataContext(Data.v2.StirlingMoneyDataContext.DBConnectionString))
			{
				if (v2db.DatabaseExists() == false)
				{
					CreateContext(v2db);
				}
				using (Data.v1.StirlingMoneyDataContext v1db = new v1.StirlingMoneyDataContext(Data.v1.StirlingMoneyDataContext.DBConnectionString))
				{
					if (v1db.DatabaseExists())
					{
						v1dbExists = true;
						UpgradeV1(v1db, v2db);
					}
				}
			}
			if (v1dbExists)
			{
				DeleteV1();
			}
		}

		private static void CreateContext(Data.v2.StirlingMoneyDataContext context)
		{
			// Create the local database.
			context.CreateDatabase();

			//Prepopulate the  default category list
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryAutomobile });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryBills });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryBusiness });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryCharity });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryClothing });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryDeposit });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryEntertainment });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryFood });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryHealth });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryHome });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryUtilities });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryMiscellaneous });
			context.Categories.InsertOnSubmit(new Data.v2.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Strings.Resources.CategoryVacation });

			context.SubmitChanges();

			//DatabaseSchemaUpdater updater = context.CreateDatabaseSchemaUpdater();
			//updater.DatabaseSchemaVersion = 1;
			//updater.Execute();
		}

		private static void DeleteV2()
		{
			using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				try
				{
					store.DeleteFile(Data.v2.StirlingMoneyDataContext.DBFileName);
				}
				catch { }
			}
		}

		private static void UpgradeV1(v1.StirlingMoneyDataContext oldContext, Data.v2.StirlingMoneyDataContext context)
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

		private static void DeleteV1()
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
