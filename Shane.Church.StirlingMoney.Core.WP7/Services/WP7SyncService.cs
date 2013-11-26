#if !AGENT
using Microsoft.Live;
using System.Windows;
using Shane.Church.StirlingMoney.Core.WP7.Extensions;
#endif
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Strings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.WP7.Services
{
	public class WP7SyncService : SyncService
	{
		SterlingEngine _engine;
#if !AGENT
		MicrosoftLiveUtils _liveUtils;
#endif

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
#if !AGENT
			_liveUtils = new MicrosoftLiveUtils();
#endif
		}

		public override async Task AuthenticateUserSilent()
		{
#if !AGENT
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
#endif
		}

		public override void Disconnect()
		{
			User = null;
#if !AGENT
			_liveUtils.Disconnect();
#endif
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
#if !AGENT
				if (_liveUtils.Session != null)
				{
					LiveConnectClient client = new LiveConnectClient(_liveUtils.Session);
					LiveOperationResult meResult = await client.GetAsyncTask("me");
					JObject token = JObject.Parse("{\"authenticationToken\": \"" + _liveUtils.Session.AuthenticationToken + "\"}");
					var emails = meResult.Result["emails"] as Dictionary<string, object>;
					_settingsService.SaveSetting<JObject>(token, "AuthenticationToken");
					if (emails != null)
					{
						this.Email = emails["account"].ToString();
						_settingsService.SaveSetting<string>(this.Email, "Email");
					}
					this.FirstName = meResult.Result["first_name"].ToString();
					_settingsService.SaveSetting<string>(this.FirstName, "FirstName");
#else
				JObject token = _settingsService.LoadSetting<JObject>("AuthenticationToken");
				this.Email = _settingsService.LoadSetting<string>("Email");
				this.FirstName = _settingsService.LoadSetting<string>("FirstName");
#endif
					this.User = await this.Client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount, token);
#if !AGENT
				}
#endif
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Exception");
#if !AGENT
				Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					MessageBox.Show(Resources.SyncUnknownError);
				});
#endif
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
