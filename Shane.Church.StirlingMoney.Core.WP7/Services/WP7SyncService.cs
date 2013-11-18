using Microsoft.Live;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.WP7.Extensions;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.WP7.Services
{
	public class WP7SyncService : SyncService
	{
		SterlingEngine _engine;
		MicrosoftLiveUtils _liveUtils;

		public WP7SyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IRepository<Account, Guid> accounts,
							IRepository<AppSyncUser, string> users,
							IRepository<Budget, Guid> budgets,
							IRepository<Goal, Guid> goals,
							IRepository<Category, Guid> categories,
							IRepository<Transaction, Guid> transactions,
							SterlingEngine engine,
							ILicensingService licensing)
			: base(client, settings, log, accounts, users, budgets, goals, categories, transactions, licensing)
		{
			if (engine == null) throw new ArgumentNullException("engine");
			_engine = engine;
			_liveUtils = new MicrosoftLiveUtils();
		}

		public override async Task AuthenticateUserSilent()
		{
			try
			{
				if (await _liveUtils.LiveLoginSilent())
				{
					await SetUserData();
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "WP7SyncService AuthenticateUserSilent");
			}
		}

		public override void Disconnect()
		{
			User = null;
			_liveUtils.Disconnect();
		}

		public override async Task AuthenticateUser()
		{
#if AGENT
			throw new InvalidOperationException();
#else
			try
			{
				var result = await _liveUtils.LiveLogin();
				if (!result)
				{
					Deployment.Current.Dispatcher.BeginInvoke(() =>
					{
						MessageBox.Show(Resources.SyncLoginError);
					});
				}
				await SetUserData();
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "WP7SyncService Authenticate");
			}
#endif
		}

		private async Task SetUserData()
		{
			try
			{
				if (_liveUtils.Session != null)
				{
					LiveConnectClient client = new LiveConnectClient(_liveUtils.Session);
					LiveOperationResult meResult = await client.GetAsyncTask("me");
					JObject token = JObject.Parse("{\"authenticationToken\": \"" + _liveUtils.Session.AuthenticationToken + "\"}");
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
