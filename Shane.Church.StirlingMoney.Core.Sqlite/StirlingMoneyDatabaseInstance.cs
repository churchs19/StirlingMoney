using AutoMapper;
using SQLite;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
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
        private static ISQLitePlatform _platform;

        public static void Initialize(ISQLitePlatform platform)
        {
            if (platform == null) throw new ArgumentNullException("platform");
            _platform = platform;
            Mapper.CreateMap<Data.Account, Core.Data.Account>();
            Mapper.CreateMap<Data.AppSyncUser, Core.Data.AppSyncUser>();
            Mapper.CreateMap<Data.Budget, Core.Data.Budget>();
            Mapper.CreateMap<Data.Category, Core.Data.Category>();
            Mapper.CreateMap<Data.Goal, Core.Data.Goal>();
            Mapper.CreateMap<Data.Transaction, Core.Data.Transaction>();

            using (var db = StirlingMoneyDatabaseInstance.GetDb())
            {
                db.CreateTable<Data.Category>();
                db.CreateTable<Data.AppSyncUser>();                           
                db.CreateTable<Data.Account>();
                db.CreateTable<Data.Transaction>();
                db.CreateTable<Data.Budget>();
                db.CreateTable<Data.Goal>();
            }
        }

        internal static SQLiteConnection GetDb()
        {
            return new SQLiteConnection(_platform, DB_PATH);
        }

        internal static SQLiteAsyncConnection GetDbAsync()
        {
            return new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(_platform, new SQLiteConnectionString(DB_PATH, true)));
        }
    }
}
