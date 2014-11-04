using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalBackupService : IBackupService
    {
        public async Task BackupDatabase()
        {
            //throw new NotImplementedException();
        }

        public async Task RestoreDatabase()
        {
            //throw new NotImplementedException();
        }

        public async Task RemoveDatabaseBackup()
        {
            throw new NotImplementedException();
        }

#pragma warning disable 0067
        public event ProgressChangedHandler ProgressChanged;
#pragma warning restore 0067
    }
}
