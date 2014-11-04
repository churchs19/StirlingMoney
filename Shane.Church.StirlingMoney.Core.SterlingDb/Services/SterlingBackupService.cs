using Grace;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.SterlingDb.Services
{
	public abstract class SterlingBackupService : IBackupService
	{
		private SterlingEngine _engine;

		public SterlingBackupService(SterlingEngine engine)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
		}

		public virtual async Task BackupDatabase()
		{
			using (var writer = GetWriter())
			{
				await _engine.SterlingDatabase.BackupAsync<StirlingMoneyDatabaseInstance>(writer);
				writer.Flush();
			}
		}

		public virtual async Task RestoreDatabase()
		{
			using (var reader = GetReader())
			{
				await _engine.SterlingDatabase.RestoreAsync<StirlingMoneyDatabaseInstance>(reader);
			}

			_engine = ContainerService.Container.Locate<SterlingEngine>();

			_engine.Activate();

			_engine.SterlingDatabase.RegisterDatabase<StirlingMoneyDatabaseInstance>("Money", ContainerService.Container.Locate<ISterlingDriver>());

			_engine.SterlingDatabase.GetDatabase("Money").RefreshAsync().Wait(1000);
		}

		public abstract Task RemoveDatabaseBackup();

		protected abstract BinaryWriter GetWriter();

		protected abstract BinaryReader GetReader();

#pragma warning disable 0067
		public virtual event ProgressChangedHandler ProgressChanged;
#pragma warning restore 0067
	}
}
