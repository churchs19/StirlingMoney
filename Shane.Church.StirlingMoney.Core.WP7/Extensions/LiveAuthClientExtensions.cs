using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP7.Extensions
{
	public static class LiveAuthClientExtensions
	{
		public static Task<LiveLoginResult> LoginAsyncTask(this LiveAuthClient client, IEnumerable<string> scopes, object userState = null)
		{
			TaskCompletionSource<LiveLoginResult> tcs =
			  new TaskCompletionSource<LiveLoginResult>();
			client.LoginCompleted += (sender, args) =>
			{
				try
				{
					if (args.Error != null) tcs.SetException(args.Error);
					else if (args.Cancelled) tcs.SetCanceled();
					else tcs.SetResult(new LiveLoginResult(args.Status, args.Session));
				}
				catch (InvalidOperationException) { }
			};
			if (userState == null)
				client.LoginAsync(scopes);
			else
				client.LoginAsync(scopes, userState);
			return tcs.Task;
		}

		public static Task<LiveLoginResult> InitializeAsyncTask(this LiveAuthClient client, IEnumerable<string> scopes, object userState = null)
		{
			TaskCompletionSource<LiveLoginResult> tcs =
			  new TaskCompletionSource<LiveLoginResult>();
			client.InitializeCompleted += (sender, args) =>
			{
				try
				{
					if (args.Error != null) tcs.SetException(args.Error);
					else if (args.Cancelled) tcs.SetCanceled();
					else tcs.SetResult(new LiveLoginResult(args.Status, args.Session));
				}
				catch (InvalidOperationException) { }
			};
			if (userState == null)
				client.InitializeAsync(scopes);
			else
				client.InitializeAsync(scopes, userState);
			return tcs.Task;
		}
	}

}
