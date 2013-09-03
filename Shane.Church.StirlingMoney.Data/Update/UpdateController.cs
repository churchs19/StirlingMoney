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
			bool v2dbExists = false;
			using (Data.v3.StirlingMoneyDataContext db = new Data.v3.StirlingMoneyDataContext(Data.v3.StirlingMoneyDataContext.DBConnectionString))
			{
				if (db.DatabaseExists() == false)
				{
					CreateContext(db);
				}
				using (Data.v2.StirlingMoneyDataContext v2db = new Data.v2.StirlingMoneyDataContext(Data.v2.StirlingMoneyDataContext.DBConnectionString))
				{
					using (Data.v1.StirlingMoneyDataContext v1db = new v1.StirlingMoneyDataContext(Data.v1.StirlingMoneyDataContext.DBConnectionString))
					{
						if (!v2db.DatabaseExists() && v1db.DatabaseExists())
						{
							v1dbExists = true;
							v2dbExists = true;
							v2db.CreateDatabase();
							UpgradeV1(v1db, v2db);
							UpgradeV2(v2db, db);
						}
						else if (v2db.DatabaseExists())
						{
							v2dbExists = true;
							UpgradeV2(v2db, db);
						}
					}
				}
			}
			if (v1dbExists)
			{
				DeleteV1();
			}
			if (v2dbExists)
			{
				DeleteV2();
			}
		}

		private static void CreateContext(Data.v3.StirlingMoneyDataContext context)
		{
			// Create the local database.
			context.CreateDatabase();

			//Prepopulate the  default category list
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryAutomobile, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryBills, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryBusiness, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryCharity, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryClothing, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryDeposit, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryEntertainment, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryFood, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryHealth, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryHome, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryUtilities, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryMiscellaneous, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });
			context.Categories.InsertOnSubmit(new Data.v3.Category() { CategoryId = Guid.NewGuid(), CategoryName = Shane.Church.StirlingMoney.Data.Resources.AppResources.CategoryVacation, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) });

			context.SubmitChanges();

			//DatabaseSchemaUpdater updater = context.CreateDatabaseSchemaUpdater();
			//updater.DatabaseSchemaVersion = 1;
			//updater.Execute();
		}


		private static void UpgradeV2(Data.v2.StirlingMoneyDataContext oldContext, Data.v3.StirlingMoneyDataContext context)
		{
			context.Categories.DeleteAllOnSubmit(context.Categories);
			context.SubmitChanges();
			var categoriesQuery = (from c in oldContext.Categories
								   select c);
			List<Data.v3.Category> categories = new List<v3.Category>();
			foreach (var c in categoriesQuery)
			{
				Data.v3.Category item = new Data.v3.Category() { CategoryId = c.CategoryId, CategoryName = c.CategoryName, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) };
				context.Categories.InsertOnSubmit(item);
			}
			context.SubmitChanges();
			var accountsQuery = (from a in oldContext.Accounts
								 select a);
			foreach (var a in accountsQuery)
			{
				Data.v3.Account item = new Data.v3.Account() { AccountId = a.AccountId, AccountName = a.AccountName, InitialBalance = a.InitialBalance, Id = null, IsDeleted = false, EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) };
				context.Accounts.InsertOnSubmit(item);
			}
			context.SubmitChanges();
			var transactions = (from tx in oldContext.Transactions
								select tx);
			foreach (var tx in transactions)
			{
				Guid? categoryId = null;
				Data.v3.Transaction item = new Data.v3.Transaction()
				{
					TransactionId = tx.TransactionId,
					Amount = tx.Amount,
					CategoryId = categoryId,
					CheckNumber = tx.CheckNumber,
					Location = tx.Location,
					Note = tx.Note,
					Posted = tx.Posted,
					TransactionDate = tx.TransactionDate,
					Id = null,
					IsDeleted = false,
					EditDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
				};
				item.Account = (from a in context.Accounts
								where a.AccountId == tx.Account.AccountId
								select a).FirstOrDefault();
				context.Transactions.InsertOnSubmit(item);
			}
			context.SubmitChanges();
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
