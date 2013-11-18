using Microsoft.Live;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Exceptions;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.SterlingDb.Services;
using Shane.Church.StirlingMoney.Core.WP7.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;
using Wintellect.Sterling.WP7.IsolatedStorage;

namespace Shane.Church.StirlingMoney.Core.WP7.Services
{
	public class WP7BackupService : SterlingBackupService
	{
		private IsoStorageHelper _helper;
		private MicrosoftLiveUtils _liveUtils;
		private IUpgradeDBService _upgradeDb;
		private readonly string _fileName = "Backup.db";
		private readonly string _v2FileName = "StirlingMoney.v2.sdf";
		private readonly string _v1FileName = "StirlingMoney.sdf";

		public WP7BackupService(SterlingEngine engine, IUpgradeDBService upgradeDb)
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
				try
				{
					var folderId = await GetFolderId(client);

					using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
					{
						using (var fileStream = store.OpenFile(_fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None))
						{
							client.UploadProgressChanged += client_UploadProgressChanged;
							var results = await client.UploadAsyncTask(folderId, _fileName, fileStream, OverwriteOption.Overwrite);
						}
					}
				}
				finally
				{
					try
					{
						client.UploadProgressChanged -= client_UploadProgressChanged;
					}
					catch { }
				}
			}
			else
			{
				throw new BackupException();
			}

			await RemoveDatabaseBackup();
		}

		public override event ProgressChangedHandler ProgressChanged;

		async void client_UploadProgressChanged(object sender, LiveUploadProgressChangedEventArgs e)
		{
			if (ProgressChanged != null)
				ProgressChanged(new ProgressChangedArgs() { ProgressPercentage = e.ProgressPercentage, Bytes = e.BytesSent, TotalBytes = e.TotalBytesToSend });

			await TaskEx.Yield();
		}

		private async Task<string> GetFolderId(LiveConnectClient client)
		{
			var folderResult = await client.GetAsyncTask("me/skydrive/files?filter=folders");
			List<object> folders = folderResult.Result["data"] as List<object>;
			if (folders != null)
			{
				foreach (Dictionary<string, object> item in folders)
				{
					if (item["name"].ToString().ToLower() == "stirling money")
					{
						return item["id"].ToString();
					}
				}
			}
			Dictionary<string, object> folderData = new Dictionary<string, object>();
			folderData.Add("name", "Stirling Money");
			var postResult = await client.PostAsyncTask("me/skydrive", folderData);
			return postResult["id"].ToString();
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

					client.DownloadProgressChanged += client_DownloadProgressChanged;

					Stream downloadStream = await client.DownloadAsyncTask(fileId.Item2 + "/content");

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
						await _upgradeDb.UpgradeDatabase(fileId.Item1, _fileName);
					}
					DeleteFile(_fileName);
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					try
					{
						client.DownloadProgressChanged -= client_DownloadProgressChanged;
					}
					catch { }
				}
			}
			else
			{
				//Throw exception here
				throw new BackupException();
			}

		}

		void client_DownloadProgressChanged(object sender, LiveDownloadProgressChangedEventArgs e)
		{
			if (ProgressChanged != null)
				ProgressChanged(new ProgressChangedArgs() { ProgressPercentage = e.ProgressPercentage, Bytes = e.BytesReceived, TotalBytes = e.TotalBytesToReceive });
		}

		private async Task<Tuple<int, string>> GetDownloadFileId(LiveConnectClient client, string folderId)
		{
			var results = await client.GetAsyncTask(folderId + "/files");
			List<object> files = results.Result["data"] as List<object>;
			string v1FileId = null;
			string v2FileId = null;
			string v3FileId = null;
			if (files != null)
			{
				foreach (Dictionary<string, object> item in files)
				{
					if (item["name"].Equals(_fileName))
						v3FileId = item["id"].ToString();
					else if (item["name"].Equals(_v2FileName))
						v2FileId = item["id"].ToString();
					else if (item["name"].Equals(_v1FileName))
						v1FileId = item["id"].ToString();
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
	}
}
