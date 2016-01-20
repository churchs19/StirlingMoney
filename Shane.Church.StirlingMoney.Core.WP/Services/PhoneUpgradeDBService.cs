using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class PhoneUpgradeDBService : IUpgradeDBService
	{
        private ISettingsService _settings;
        private SyncService _sync;

		public PhoneUpgradeDBService(ISettingsService settings, SyncService sync)
		{
            if (settings == null) throw new ArgumentNullException("settings");
            _settings = settings;
            if (sync == null) throw new ArgumentNullException("sync");
            _sync = sync;
		}

		public async Task UpgradeDatabase(int dbVersion, string fileName)
		{
			var connectionStringPrefix = "Data Source=isostore:/";
            if(dbVersion == 3)
            {
                if (_settings.LoadSetting<bool>("EnableSync"))
                {
                    await _sync.Sync();
                }
                else
                {
                    await Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeSterlingToSqlite();
                }
                DeleteFolder(fileName);
            }
			else if (dbVersion == 2)
			{
				await Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeV2(connectionStringPrefix + fileName);
                DeleteFile(fileName);
            }
            else if (dbVersion == 1)
			{
				var v2FileName = "v2.sdf";
				Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeV1(connectionStringPrefix + fileName, connectionStringPrefix + v2FileName);
				await Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeV2(connectionStringPrefix + v2FileName);
				DeleteFile("v2.sdf");
                DeleteFile(fileName);
            }
        }

		private void DeleteFile(string fileName)
		{
			using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
			{
				try
				{
					store.DeleteFile(fileName);
				}
				catch { }
			}
		}

        private void DeleteFolder(string folder)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    var files = store.GetFileNames(folder + "/*");
                    foreach(var file in files)
                    {
                        DeleteFile(folder + "/" + file);
                    }
                    var folders = store.GetDirectoryNames(folder + "/*");
                    foreach(var subfolder in folders)
                    {
                        DeleteFolder(folder + "/" + subfolder);
                    }
                    store.DeleteDirectory(folder);
                }
                catch { }
            }
        }
	}
}
