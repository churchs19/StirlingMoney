using System;
using SQLite;
using System.IO;
using Shane.Church.StirlingMoney.Core.Data;

namespace Shane.Church.StirlingMoney.WP.Data
{
    public class StirlingMoneyDatabaseInstance
    {
        private static string _dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "stirlingmoney.sqlite");
        public static void Init()
        {
            using (var db = new SQLiteConnection(_dbPath))
            {
                db.CreateTable<AppSyncUser>();
                db.CreateTable<Category>();
                db.CreateTable<Account>();
                db.CreateTable<Budget>();
                db.CreateTable<Goal>();
                db.CreateTable<Transaction>();
                db.CreateTable<Setting>();
                db.RunInTransaction(() =>
                {
                    db.Insert(new Account() { AccountId = new Guid(), AccountName = "Test", EditDateTime = DateTimeOffset.Now, InitialBalance = 0, IsDeleted = false });
                });
            }
        }
    }
}
