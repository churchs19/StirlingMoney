using Microsoft.Live;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Exceptions;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;
using Wintellect.Sterling.WP8.IsolatedStorage;
namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class WP8BackupService : SterlingBackupService, IProgress<LiveOperationProgress>
	{
		private IsoStorageHelper _helper;
		private MicrosoftLiveUtils _liveUtils;
		private IUpgradeDBService _upgradeDb;
		private readonly string _fileName = "Backup.db";
		private readonly string _v2FileName = "StirlingMoney.v2.sdf";
		private readonly string _v1FileName = "StirlingMoney.sdf";

		public WP8BackupService(SterlingEngine engine, IUpgradeDBService upgradeDb)
			: base(engine)
		{
			if (upgradeDb == null) throw new ArgumentNullException("upgradeDb");
			_upgradeDb = upgradeDb;
			_helper = new IsoStorageHelper();
			_liveUtils = new MicrosoftLiveUtils();
		}

		public override async Task BackupDatabase()
		{
			if (ProgressChanged != null)
				ProgressChanged(new ProgressChangedArgs() { ProgressPercentage = 0, Bytes = 0, TotalBytes = 0 });

			await TaskEx.Yield();

			await base.BackupDatabase();

			if (await _liveUtils.LiveLogin())
			{
				LiveConnectClient client = new LiveConnectClient(_liveUtils.Session);
				var folderId = await GetFolderId(client);

				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var fileStream = store.OpenFile(_fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
					{
						var results = await client.UploadAsync(folderId, _fileName, fileStream, OverwriteOption.Overwrite, Task.Factory.CancellationToken, this);
					}
				}
			}
			else
			{
				throw new BackupException();
			}

			await RemoveDatabaseBackup();
		}

		public override event ProgressChangedHandler ProgressChanged;

		private async Task<string> GetFolderId(LiveConnectClient client)
		{
			var folderResult = await client.GetAsync("me/skydrive/files?filter=folders");
			dynamic folders = folderResult.Result;
			if (folders.data != null)
			{
				foreach (dynamic item in folders.data)
				{
					if (item.name.ToString().ToLower() == "stirling money")
					{
						return item.id.ToString();
					}
				}
			}
			Dictionary<string, object> folderData = new Dictionary<string, object>();
			folderData.Add("name", "Stirling Money");
			var postResult = await client.PostAsync("me/skydrive", folderData);
			return postResult.Result["id"].ToString();
		}

		public override async Task RestoreDatabase()
		{
			if (ProgressChanged != null)
				ProgressChanged(new ProgressChangedArgs() { ProgressPercentage = 0, Bytes = 0, TotalBytes = 0 });

			await TaskEx.Yield();

			if (await _liveUtils.LiveLogin())
			{
				LiveConnectClient client = new LiveConnectClient(_liveUtils.Session);
				try
				{
					var folderId = await GetFolderId(client);
					var fileId = await GetDownloadFileId(client, folderId);

					if (fileId == null) throw new BackupException();

					LiveDownloadOperationResult downloadResult = await client.DownloadAsync(fileId.Item2 + "/content", Task.Factory.CancellationToken, this);
					var downloadStream = downloadResult.Stream;

					using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
					{
						using (IsolatedStorageFileStream stream = store.OpenFile(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
						{
							await (downloadStream.CopyToAsync(stream));
							stream.Flush();
						}
						downloadStream.Close();
					}

					if (fileId.Item1 == 3)
					{
						await base.RestoreDatabase();
					}
					else
					{
						var sdfFileName = _fileName.Substring(0, _fileName.Length - 2) + "sdf";
						using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
						{
							store.MoveFile(_fileName, sdfFileName);
						}
						await _upgradeDb.UpgradeDatabase(fileId.Item1, sdfFileName);
					}
					DeleteFile(_fileName);
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			else
			{
				//Throw exception here
				throw new BackupException();
			}

		}

		private async Task<Tuple<int, string>> GetDownloadFileId(LiveConnectClient client, string folderId)
		{
			var results = await client.GetAsync(folderId + "/files");
			dynamic files = results.Result;
			string v1FileId = null;
			string v2FileId = null;
			string v3FileId = null;
			if (files.data != null)
			{
				foreach (dynamic item in files.data)
				{
					if (item.name.Equals(_fileName))
						v3FileId = item.id.ToString();
					else if (item.name.Equals(_v2FileName))
						v2FileId = item.id.ToString();
					else if (item.name.Equals(_v1FileName))
						v1FileId = item.id.ToString();
				}
			}

			if (!string.IsNullOrEmpty(v3FileId))
				return new Tuple<int, string>(3, v3FileId);
			else if (!string.IsNullOrEmpty(v2FileId))
				return new Tuple<int, string>(2, v2FileId);
			else if (!string.IsNullOrEmpty(v1FileId))
				return new Tuple<int, string>(1, v1FileId);
			else
				return null;
		}

		protected override System.IO.BinaryWriter GetWriter()
		{
			return _helper.GetWriter(_fileName);
		}

		protected override System.IO.BinaryReader GetReader()
		{
			return _helper.GetReader(_fileName);
		}

		public override Task RemoveDatabaseBackup()
		{
			return TaskEx.Run(() => DeleteFile(_fileName));
		}

		private void DeleteFile(string fileName)
		{
			_helper.Purge(fileName);
		}

		public void Report(LiveOperationProgress value)
		{
			if (ProgressChanged != null)
				ProgressChanged(new ProgressChangedArgs() { ProgressPercentage = Convert.ToInt32(value.ProgressPercentage), Bytes = value.BytesTransferred, TotalBytes = value.TotalBytes });
		}
	}
}
