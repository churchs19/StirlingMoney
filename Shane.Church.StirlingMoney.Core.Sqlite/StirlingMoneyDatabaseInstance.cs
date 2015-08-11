using AutoMapper;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Sqlite
{
    public class StirlingMoneyDatabaseInstance
    {
        private const string DB_PATH = "StirlingMoney.sqlite";

        public static void Initialize()
        {
            Mapper.CreateMap<Sqlite.Data.Account, Core.Data.Account>();
            Mapper.CreateMap<Sqlite.Data.AppSyncUser, Core.Data.AppSyncUser>();
            Mapper.CreateMap<Sqlite.Data.Budget, Core.Data.Budget>();
            Mapper.CreateMap<Sqlite.Data.Category, Core.Data.Category>();
            Mapper.CreateMap<Sqlite.Data.Goal, Core.Data.Goal>();
            Mapper.CreateMap<Sqlite.Data.Transaction, Core.Data.Transaction>();

            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                db.CreateTable<Sqlite.Data.Category>();
                db.CreateTable<Sqlite.Data.AppSyncUser>();                           
                db.CreateTable<Sqlite.Data.Account>();
                db.CreateTable<Sqlite.Data.Transaction>();
                db.CreateTable<Sqlite.Data.Budget>();
                db.CreateTable<Sqlite.Data.Goal>();
            }
        }

        internal static SQLiteConnection GetDb()
        {
            return new SQLiteConnection(DB_PATH);
        }

        internal static SQLiteAsyncConnection GetDbAsync()
        {
            return new SQLiteAsyncConnection(DB_PATH);
        }
    }
}
