using Shane.Church.StirlingMoney.Core.Services;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;
using System;

namespace Shane.Church.StirlingMoney.Core.Sqlite
{
    public class StirlingMoneyDatabaseInstance
    {
        private const string DB_FILE = "StirlingMoney.sqlite";
        private static string DbPath;
        private static ISQLitePlatform _platform;

        public static void Initialize(ISQLitePlatform platform, string path)
        {
            if (platform == null) throw new ArgumentNullException("platform");
            _platform = platform;
            if (path == null) throw new ArgumentNullException("path");
            DbPath = System.IO.Path.Combine(path, DB_FILE);

            //Mapper.Initialize(cfg =>
            //{
            //    cfg.ConstructServicesUsing(type => ContainerService.Container.Locate(type));
            //    cfg.CreateMap<Data.Account, Core.Data.Account>().ConstructUsingServiceLocator();
            //    cfg.CreateMap<Core.Data.Account, Data.Account>();
            //    cfg.CreateMap<Data.AppSyncUser, Core.Data.AppSyncUser>().ConstructUsingServiceLocator();
            //    cfg.CreateMap<Core.Data.AppSyncUser, Data.AppSyncUser>();
            //    cfg.CreateMap<Data.Budget, Core.Data.Budget>().ConstructUsingServiceLocator();
            //    cfg.CreateMap<Core.Data.Budget, Data.Budget>();
            //    cfg.CreateMap<Data.Category, Core.Data.Category>().ConstructUsingServiceLocator();
            //    cfg.CreateMap<Core.Data.Category, Data.Category>();
            //    cfg.CreateMap<Data.Goal, Core.Data.Goal>().ConstructUsingServiceLocator();
            //    cfg.CreateMap<Core.Data.Goal, Data.Goal>();
            //    cfg.CreateMap<Data.Transaction, Core.Data.Transaction>().ConstructUsingServiceLocator();
            //    cfg.CreateMap<Core.Data.Transaction, Data.Transaction>();
            //});

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
            return new SQLiteConnection(_platform, DbPath);
        }

        internal static SQLiteAsyncConnection GetDbAsync()
        {
            return new SQLiteAsyncConnection(() => new SQLiteConnectionWithLock(_platform, new SQLiteConnectionString(DbPath, true)));
        }
    }
}
