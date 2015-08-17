#if !AGENT
using Microsoft.Live;
using System.Windows;
#endif
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Threading.Tasks;
using Wintellect.Sterling.Core;

namespace Shane.Church.StirlingMoney.Core.WP.Services
{
	public class WP8SyncService : SyncService
	{
		//SterlingEngine _engine;
#if !AGENT
		MicrosoftLiveUtils _liveUtils;
#endif

		public WP8SyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IDataRepository<Account, Guid> accounts,
							IDataRepository<AppSyncUser, string> users,
							IDataRepository<Budget, Guid> budgets,
							IDataRepository<Goal, Guid> goals,
							IDataRepository<Category, Guid> categories,
							IDataRepository<Transaction, Guid> transactions,
							ILicensingService licensing)
			: base(client, settings, log, accounts, users, budgets, goals, categories, transactions, licensing)
		{
#if !AGENT
			_liveUtils = new MicrosoftLiveUtils();
#endif
		}

		public override async Task AuthenticateUserSilent()
		{
			try
			{
#if !AGENT
				if (await _liveUtils.LiveLoginSilent())
				{
#endif
				await SetUserData();
#if !AGENT
				}
#endif
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "WP7SyncService AuthenticateUserSilent");
			}
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
						MessageBox.Show(Strings.Resources.SyncLoginError);
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
					LiveOperationResult meResult = await client.GetAsync("me");
					JObject token = JObject.Parse("{\"authenticationToken\": \"" + _liveUtils.Session.AuthenticationToken + "\"}");
					_settingsService.SaveSetting<string>(token.ToString(), "AuthenticationToken");
					dynamic result = meResult.Result;

					if (result.emails != null)
					{
						this.Email = result.emails.account;
						_settingsService.SaveSetting<string>(this.Email, "Email");
					}
					this.FirstName = meResult.Result["first_name"].ToString();
					_settingsService.SaveSetting<string>(this.FirstName, "FirstName");
#else
				JObject token = JObject.Parse(_settingsService.LoadSetting<string>("AuthenticationToken"));
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
					MessageBox.Show(Strings.Resources.SyncUnknownError);
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
