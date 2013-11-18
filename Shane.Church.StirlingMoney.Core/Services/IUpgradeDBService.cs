using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public interface IUpgradeDBService
	{
		Task UpgradeDatabase(int dbVersion, string fileName);
	}
}
