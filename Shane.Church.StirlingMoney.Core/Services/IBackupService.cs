using Shane.Church.StirlingMoney.Core.Data;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public delegate void ProgressChangedHandler(ProgressChangedArgs args);

	public interface IBackupService
	{
		Task BackupDatabase();
		Task RestoreDatabase();
		Task RemoveDatabaseBackup();

		event ProgressChangedHandler ProgressChanged;
	}
}
