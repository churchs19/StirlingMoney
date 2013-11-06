using Microsoft.Live;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.WP;
using Shane.Church.StirlingMoney.Core.WP7.Extensions;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Shane.Church.StirlingMoney.Core.WP7.Services
{
	public class WP7SyncService : SyncService
	{
		LiveAuthClient _liveIdClient = new LiveAuthClient(LiveConfig.ClientId);

		public WP7SyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IRepository<Account> accounts,
							IRepository<AppSyncUser> users,
							IRepository<Budget> budgets,
							IRepository<Goal> goals,
							IRepository<Category> categories,
							IRepository<Transaction> transactions)
			: base(client, settings, log, accounts, users, budgets, goals, categories, transactions)
		{

		}

		private LiveConnectSession _session;
		public LiveConnectSession Session
		{
			get { return _session; }
			set
			{
				_session = value;
			}
		}

		public override async Task AuthenticateUserSilent()
		{
			try
			{
				if (await LiveLoginSilent())
				{
					await SetUserData();
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "WP7SyncService AuthenticateUserSilent");
			}
		}

		private async Task<bool> LiveLoginSilent()
		{
			LiveLoginResult result;
			try
			{
				if (_session == null || _session.Expires.CompareTo(DateTimeOffset.Now) < 0)
				{
					result = await _liveIdClient.InitializeAsyncTask(LiveConfig.Scopes);
					if (result.Status == LiveConnectSessionStatus.Connected)
					{
						_session = result.Session;
						return true;
					}
					else
					{
						_session = null;
						return false;
					}
				}
				else
					return true;
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Exception");
				return false;
			}
		}

		public override void Disconnect()
		{
			_session = null;
			User = null;
			_liveIdClient.Logout();
		}

		public override async Task AuthenticateUser()
		{
			if (_session == null || _session.Expires.CompareTo(DateTimeOffset.Now) < 0)
			{
				LiveLoginResult result;
				try
				{
					if (!(await LiveLoginSilent()))
					{
						result = await _liveIdClient.LoginAsyncTask(LiveConfig.Scopes);
						if (result.Status == LiveConnectSessionStatus.Connected)
						{
							_session = result.Session;
						}
						else
						{
							_session = null;
							Deployment.Current.Dispatcher.BeginInvoke(() =>
							{
								MessageBox.Show(Resources.SyncLoginError);
							});
						}
					}
				}
				catch (Exception ex)
				{
					FlurryWP8SDK.Api.LogError("WP7SyncService Authenticate", ex);
				}
			}
			await SetUserData();
		}

		private async Task SetUserData()
		{
			try
			{
				if (_session != null)
				{
					LiveConnectClient client = new LiveConnectClient(_session);
					LiveOperationResult meResult = await client.GetAsyncTask("me");
					JObject token = JObject.Parse("{\"authenticationToken\": \"" + _session.AuthenticationToken + "\"}");
					var emails = meResult.Result["emails"] as Dictionary<string, object>;
					if (emails != null)
					{
						this.Email = emails["account"].ToString();
					}
					this.FirstName = meResult.Result["first_name"].ToString();
					this.User = await this.Client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, token);
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Exception");
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					MessageBox.Show(Resources.SyncUnknownError);
				});
			}
		}

		public override Task<bool> IsNetworkConnected()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Task.Factory.StartNew(() =>
			{
				try
				{
					var currentNetworkType = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;

					tcs.SetResult(currentNetworkType != Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
				}
				catch (Exception ex)
				{
					_log.LogException(ex, "IsNetworkConnected Exception");
					tcs.SetResult(false);
				}
			});
			return tcs.Task;
		}
	}
}
