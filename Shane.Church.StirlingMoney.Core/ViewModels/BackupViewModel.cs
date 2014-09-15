using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.Utility.Core.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class BackupViewModel : ObservableObject
	{
		IBackupService _backup;

		public BackupViewModel(IBackupService backup)
		{
			if (backup == null) throw new ArgumentNullException("backup");
			_backup = backup;
			_backup.ProgressChanged += _backup_ProgressChanged;

			BackupCommand = new AsyncRelayCommand(action: Backup, completed: BackupComplete, error: Error);
			RestoreCommand = new AsyncRelayCommand(action: Restore, completed: RestoreComplete, error: Error);
		}

		private void _backup_ProgressChanged(ProgressChangedArgs args)
		{
			if (BusyChanged != null)
			{
				if (_isDownload)
				{
					if (args.TotalBytes != 0)
						BusyChanged(new ProgressBusyEventArgs() { IsBusy = true, ProgressPercentage = args.ProgressPercentage, Message = String.Format(Strings.Resources.DownloadingProgressText, args.Bytes, args.TotalBytes) });
					else
						BusyChanged(new ProgressBusyEventArgs() { IsBusy = true, ProgressPercentage = args.ProgressPercentage, Message = String.Format(Strings.Resources.ProgressBarDownloadingText) });
				}
				else
				{
					if (args.TotalBytes != 0)
						BusyChanged(new ProgressBusyEventArgs() { IsBusy = true, ProgressPercentage = args.ProgressPercentage, Message = String.Format(Strings.Resources.UploadingProgressText, args.Bytes, args.TotalBytes) });
					else
						BusyChanged(new ProgressBusyEventArgs() { IsBusy = true, ProgressPercentage = args.ProgressPercentage, Message = String.Format(Strings.Resources.ProgressBarUploadingText) });
				}
			}
		}

		private bool _isDownload;

		public delegate void BusyProgressChangedHandler(ProgressBusyEventArgs args);
		public event BusyProgressChangedHandler BusyChanged;

		public ICommand BackupCommand { get; protected set; }

		public Task Backup(object param)
		{
			_isDownload = false;
			return _backup.BackupDatabase();
		}

		public Task BackupComplete()
		{
			return Task.Run(() =>
			{
				if (BusyChanged != null)
					BusyChanged(new ProgressBusyEventArgs() { IsBusy = false, Message = Strings.Resources.BackupSuccess });
			});
		}

		public void Error(Exception ex)
		{
			if (BusyChanged != null)
			{
				if (_isDownload)
					BusyChanged(new ProgressBusyEventArgs() { IsBusy = false, Error = ex, IsError = true, Message = string.Format(Strings.Resources.BackupError, ex.Message) });
				else
					BusyChanged(new ProgressBusyEventArgs() { IsBusy = false, Error = ex, IsError = true, Message = Strings.Resources.BackupRestoreError });
			}
		}

		public ICommand RestoreCommand { get; protected set; }

		public Task Restore(object param)
		{
			_isDownload = true;
			return _backup.RestoreDatabase();
		}

		public Task RestoreComplete()
		{
			return Task.Run(() =>
			{
				if (BusyChanged != null)
					BusyChanged(new ProgressBusyEventArgs() { IsBusy = false, Message = Strings.Resources.BackupRestoreSuccess });
			});
		}
	}
}
