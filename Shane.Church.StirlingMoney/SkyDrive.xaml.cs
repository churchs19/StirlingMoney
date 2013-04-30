using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Live;
using Shane.Church.StirlingMoney.ViewModels;
using System.IO.IsolatedStorage;
#if !PERSONAL
using Shane.Church.StirlingMoney.Data;
using Shane.Church.StirlingMoney.Data.Update;
#else
using Shane.Church.StirlingMoney.Data.Sync;
#endif
using Microsoft.Phone.Shell;

namespace Shane.Church.StirlingMoney
{
	public partial class SkyDrive : PhoneApplicationPage
	{
		LiveConnectSession _session = null;
		SkyDriveViewModel _model = null;
		LiveConnectClient _client = null;
		string _stirlingMoneyFolderId = null;
		ProgressIndicator _progress = null;

		public SkyDrive()
		{
			InitializeComponent();

			InitializeAdControl();

			_progress = new ProgressIndicator();
			_progress.IsVisible = false;
			_progress.IsIndeterminate = true;
			SystemTray.SetProgressIndicator(this, _progress);

			_model = new SkyDriveViewModel();
			this.DataContext = _model;
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
			{
				AdControl.ApplicationId = "test_client";
				AdControl.AdUnitId = "Image480_80";
			}
			else
			{
				AdControl.ApplicationId = "081af108-c899-401e-a44a-b54e303f12dc";
				AdControl.AdUnitId = "92173";
			}
#if PERSONAL
			AdControl.IsEnabled = false;
			AdControl.Height = 0;
#endif
		}

		private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
		{
			AdControl.Height = 0;
		}

		private void AdControl_AdRefreshed(object sender, EventArgs e)
		{
			AdControl.Height = 80;
		}
		#endregion

		private void buttonSkyDriveSignIn_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
		{
#if !PERSONAL
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                _session = e.Session;
				_model.IsSignedIn = true;
				_client = new LiveConnectClient(_session);
				_client.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(_client_GetCompleted);
				_client.PostCompleted += new EventHandler<LiveOperationCompletedEventArgs>(_client_PostCompleted);
				_client.UploadCompleted += new EventHandler<LiveOperationCompletedEventArgs>(_client_UploadCompleted);
				_client.DownloadCompleted += new EventHandler<LiveDownloadCompletedEventArgs>(_client_DownloadCompleted);
				_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarText;
				_progress.IsVisible = true;
				_client.GetAsync("me/skydrive/files?filter=folders", "me/skydrive/files?filter=folders");				
                _model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupSignedIn;
            }
            else
             {
				 _client = null;
				 _model.IsSignedIn = false;
				 _model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupNotSignedIn;
             }
#endif
		}

		private void buttonBackup_Click(object sender, RoutedEventArgs e)
		{
#if !PERSONAL
			if (!string.IsNullOrEmpty(_stirlingMoneyFolderId))
			{
				_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarUploadingText;
				_progress.IsVisible = true;
				IsolatedStorageFileStream fileStream = null;
				try
				{
					using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
					{
						fileStream = store.OpenFile(Data.v2.StirlingMoneyDataContext.DBFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
					}
					_client.UploadAsync(_stirlingMoneyFolderId, Data.v2.StirlingMoneyDataContext.DBFileName, true, fileStream, null);
				}
				catch (Exception ex)
				{
					_progress.IsVisible = false;
					_model.InfoBoxText = string.Format(Shane.Church.StirlingMoney.Resources.AppResources.BackupError, ex.Message);
				}
			}
#endif
		}

		private void buttonRestore_Click(object sender, RoutedEventArgs e)
		{
#if !PERSONAL
			if (!string.IsNullOrEmpty(_stirlingMoneyFolderId))
			{
				_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarDownloadingText;
				_progress.IsVisible = true;
				_client.GetAsync(_stirlingMoneyFolderId + "/files", _stirlingMoneyFolderId + "/files");
			}
#endif
		}

		void _client_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
		{
#if !PERSONAL
			if (e.Error == null)
			{
				string folderFiles = _stirlingMoneyFolderId + "/files";
				if(e.UserState.ToString() == "me/skydrive/files?filter=folders")
				{
					List<object> folders = e.Result["data"] as List<object>;
					if (folders != null)
					{
						foreach (Dictionary<string, object> item in folders)
						{
							if (item["name"].ToString().ToLower() == "stirling money")
							{
								_stirlingMoneyFolderId = item["id"].ToString();
								break;
							}
						}
					}
					if (string.IsNullOrEmpty(_stirlingMoneyFolderId))
					{
						Dictionary<string, object> folderData = new Dictionary<string, object>();
						folderData.Add("name", "Stirling Money");
						_client.PostAsync("me/skydrive", folderData, "me/skydrive");
					}
					else
					{
						_progress.IsVisible = false;
					}
				}
				else if(e.UserState.ToString() == folderFiles)
				{
					List<object> files = e.Result["data"] as List<object>;
					if (files != null)
					{
						string v1FileId = null;
						string v2FileId = null;
						foreach (Dictionary<string, object> item in files)
						{
							if (item["name"].ToString() == (Data.v1.StirlingMoneyDataContext.DBFileName + ".txt"))
							{
								//Old backup file
								v1FileId = item["id"].ToString();
							}
							else if (item["name"].ToString() == Data.v2.StirlingMoneyDataContext.DBFileName)
							{
								//v2 backup file
								v2FileId = item["id"].ToString();
							}
						}
						if (!string.IsNullOrEmpty(v2FileId))
						{
							//A v2 backup exists so only download v2
							_client.DownloadAsync(v2FileId + "/content", Data.v2.StirlingMoneyDataContext.DBFileName);
							return;
						}
						else if (!string.IsNullOrEmpty(v1FileId))
						{
							_client.DownloadAsync(v1FileId + "/content", Data.v1.StirlingMoneyDataContext.DBFileName);
							return;
						}

					}
					_progress.IsVisible = false;
					_model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupRestoreNoBackupError;
				}
			}
			else
			{
				_progress.IsVisible = false;
				_model.InfoBoxText = string.Format(Shane.Church.StirlingMoney.Resources.AppResources.BackupSkyDriveError, e.Error.Message);
			}
#endif
		}

		void _client_PostCompleted(object sender, LiveOperationCompletedEventArgs e)
		{
#if !PERSONAL
			_progress.IsVisible = false;
			if (e.Error == null)
			{
				switch (e.UserState.ToString())
				{
					case "me/skydrive":
						_stirlingMoneyFolderId = e.Result["id"].ToString();
						break;
				}
			}
			else
			{
				_model.InfoBoxText = string.Format(Shane.Church.StirlingMoney.Resources.AppResources.BackupSkyDriveError, e.Error.Message);
			}
#endif
		}

		void _client_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
		{
#if !PERSONAL
			if (e.Error == null)
			{
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					try
					{
						IsolatedStorageFileStream stream = store.CreateFile("Temp.sdf");
						e.Result.CopyTo(stream);
						stream.Flush();
						stream.Close();

						if (e.UserState.ToString() == Data.v1.StirlingMoneyDataContext.DBFileName)
						{
							_progress.Text = Shane.Church.StirlingMoney.Resources.AppResources.ProgressBarUpdatingText;
							_progress.IsVisible = true;
							//Restoring a v1 backup
							using (Data.v1.StirlingMoneyDataContext testContext = new Data.v1.StirlingMoneyDataContext("Data Source=isostore:/Temp.sdf"))
							{
								if (!testContext.DatabaseExists())
								{
									_model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupRestoreError;
									return;
								}

								store.DeleteFile(Data.v2.StirlingMoneyDataContext.DBFileName);
								using (Data.v2.StirlingMoneyDataContext _context = new Data.v2.StirlingMoneyDataContext(Data.v2.StirlingMoneyDataContext.DBConnectionString))
								{
									UpdateController.CreateContext(_context);
									UpdateController.UpgradeV1(testContext, _context);
								}
							}
							store.DeleteFile("Temp.sdf");
						}
						else 
						{
							//Restoring a v2 backup
							using (Data.v2.StirlingMoneyDataContext testContext = new Data.v2.StirlingMoneyDataContext("Data Source=isostore:/Temp.sdf"))
							{
								if (!testContext.DatabaseExists())
								{
									_model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupRestoreError;
									return;
								}
								else
								{
									UpdateController.UpdateContext(testContext);
								}
							} 
							store.DeleteFile(Data.v2.StirlingMoneyDataContext.DBFileName);
							store.MoveFile("Temp.sdf", Data.v2.StirlingMoneyDataContext.DBFileName);

						}
						App.ViewModel.LoadData();

						_model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupRestoreSuccess;
						_progress.IsVisible = false;
					}
					catch (Exception ex)
					{
						_progress.IsVisible = false;
						_model.InfoBoxText = string.Format(Shane.Church.StirlingMoney.Resources.AppResources.BackupRestoreException, ex.Message);
					}						
				}
			}
			else
			{
				_progress.IsVisible = false;
				_model.InfoBoxText = string.Format(Shane.Church.StirlingMoney.Resources.AppResources.BackupRestoreException, e.Error.Message);
			}
#endif
		}

		void _client_UploadCompleted(object sender, LiveOperationCompletedEventArgs e)
		{
#if !PERSONAL
			_progress.IsVisible = false;
			if (e.Error == null)
			{				
				_model.InfoBoxText = Shane.Church.StirlingMoney.Resources.AppResources.BackupSuccess;
			}
			else
			{
				_model.InfoBoxText = string.Format(Shane.Church.StirlingMoney.Resources.AppResources.BackupError, e.Error.Message);
			}
#endif
		}
	}
}