using System;
using Microsoft.Synchronization.ClientServices;
using System.Collections.Generic;

namespace Shane.Church.StirlingMoney.Data.Sync
{
	public static class ContextInstance
	{
		static IsoStore_OfflineContext context;
//		static List<CacheRefreshStatistics> statsLog = new List<CacheRefreshStatistics>(10);

		public static IsoStore_OfflineContext Context
		{
			get
			{
				if (context == null)
				{
					InitContext();
				}
				return context;
			}
		}

		private static void InitContext()
		{
#if DEBUG
			context = new IsoStore_OfflineContext("DataCache", new Uri("http://localhost:1919/Server_SyncService.svc/"));
#else
			context = new IsoStore_OfflineContext("DataCache", new Uri("http://23.21.189.192/Server_SyncService.svc"));
#endif
			context.CacheController.ControllerBehavior.SerializationFormat = SerializationFormat.ODataJSON;
		}

		public static void ClearContext()
		{
			context = null;
		}

		//public static void AddStats(CacheRefreshStatistics stats, Exception e)
		//{
		//    if (SettingsViewModel.Instance.SyncLogEnabled)
		//    {
		//        if (statsLog.Count == 10)
		//        {
		//            statsLog.RemoveAt(index);
		//        }
		//        statsLog.Insert(index, stats);
		//        index = index++ / 10;
		//    }
		//}

		//public static IEnumerable<CacheRefreshStatistics> Stats
		//{
		//    get
		//    {
		//        return statsLog.OrderByDescending((e) => e.StartTime).Take(10);
		//    }
		//}
	}
}
