using Shane.Church.StirlingMoney.Core.Services;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class PhoneUpgradeDBService : IUpgradeDBService
	{
		public PhoneUpgradeDBService()
		{

		}

		public async Task UpgradeDatabase(int dbVersion, string fileName)
		{
			var connectionStringPrefix = "Data Source=isostore:/";
			if (dbVersion == 2)
			{
				await Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeV2(connectionStringPrefix + fileName);
			}
			else if (dbVersion == 1)
			{
				var v2FileName = "v2.sdf";
				Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeV1(connectionStringPrefix + fileName, connectionStringPrefix + v2FileName);
				await Shane.Church.StirlingMoney.Data.Update.UpdateController.UpgradeV2(connectionStringPrefix + v2FileName);
				DeleteFile("v2.sdf");
			}
			DeleteFile(fileName);
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
	}
}
