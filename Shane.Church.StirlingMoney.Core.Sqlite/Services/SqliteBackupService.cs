using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Sqlite.Services
{
    public class SqliteBackupService : IBackupService
    {
        public event ProgressChangedHandler ProgressChanged;

        public Task BackupDatabase()
        {
            throw new NotImplementedException();
        }

        public Task RemoveDatabaseBackup()
        {
            throw new NotImplementedException();
        }

        public Task RestoreDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
