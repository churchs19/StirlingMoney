using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP
{
	public class MicrosoftLiveUtils
	{
		LiveAuthClient _liveIdClient = new LiveAuthClient(LiveConfig.ClientId);

		internal LiveConnectSession Session { get; set; }

		internal async Task<bool> LiveLoginSilent()
		{
			LiveLoginResult result;
				if (Session == null || Session.Expires.CompareTo(DateTimeOffset.Now) < 0)
				{
					result = await _liveIdClient.InitializeAsync(LiveConfig.Scopes);
					if (result.Status == LiveConnectSessionStatus.Connected)
					{
						Session = result.Session;
						return true;
					}
					else
					{
						Session = null;
						return false;
					}
				}
				else
					return true;
		}

		internal async Task<bool> LiveLogin()
		{
#if AGENT
			throw new InvalidOperationException();
#else
			if (Session == null || Session.Expires.CompareTo(DateTimeOffset.Now) < 0)
			{
				LiveLoginResult result;
				if (!(await LiveLoginSilent()))
				{
					result = await _liveIdClient.LoginAsync(LiveConfig.Scopes);
					if (result.Status == LiveConnectSessionStatus.Connected)
					{
						Session = result.Session;
						return true;
					}
					else
					{
						Session = null;
						return false;
					}
				}
				else
				{
					return true;
				}
			}
			return true;		
#endif
		}

		internal void Disconnect()
		{
			Session = null;
			_liveIdClient.Logout();
		}
	}
}
