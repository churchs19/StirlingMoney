using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP7.Extensions
{
	public static class LiveConnectClientExtensions
	{
		public static Task<LiveOperationResult> GetAsyncTask(this LiveConnectClient client, string path, object userState = null)
		{
			TaskCompletionSource<LiveOperationResult> tcs =
			  new TaskCompletionSource<LiveOperationResult>();
			client.GetCompleted += (sender, args) =>
			{
				try
				{
					if (args.Error != null) tcs.SetException(args.Error);
					else if (args.Cancelled) tcs.SetCanceled();
					else tcs.SetResult(new LiveOperationResult(args.Result, args.RawResult));
				}
				catch (InvalidOperationException) { }
			};
			if (userState == null)
				client.GetAsync(path);
			else
				client.GetAsync(path, userState);
			return tcs.Task;
		}

		public static Task<IDictionary<string, object>> UploadAsyncTask(this LiveConnectClient client, string path, string fileName, Stream inputStream, OverwriteOption option = OverwriteOption.DoNotOverwrite)
		{
			TaskCompletionSource<IDictionary<string, object>> tcs = new TaskCompletionSource<IDictionary<string, object>>();
			client.UploadCompleted += (sender, args) =>
			{
				if (args.Cancelled) tcs.TrySetCanceled();
				if (args.Error != null) tcs.TrySetException(args.Error);

				tcs.TrySetResult(args.Result);
			};
			client.UploadAsync(path, fileName, inputStream, option);
			return tcs.Task;
		}

		public static Task<IDictionary<string, object>> PutAsyncTask(this LiveConnectClient client, string path, IDictionary<string, object> body)
		{
			TaskCompletionSource<IDictionary<string, object>> tcs = new TaskCompletionSource<IDictionary<string, object>>();
			client.PutCompleted += (sender, args) =>
			{
				if (args.Cancelled) tcs.TrySetCanceled();
				if (args.Error != null) tcs.TrySetException(args.Error);

				tcs.TrySetResult(args.Result);
			};
			client.PutAsync(path, body);
			return tcs.Task;
		}

		public static Task<IDictionary<string, object>> PostAsyncTask(this LiveConnectClient client, string path, IDictionary<string, object> body)
		{
			TaskCompletionSource<IDictionary<string, object>> tcs = new TaskCompletionSource<IDictionary<string, object>>();
			client.PostCompleted += (sender, args) =>
			{
				if (args.Cancelled) tcs.TrySetCanceled();
				if (args.Error != null) tcs.TrySetException(args.Error);

				tcs.TrySetResult(args.Result);
			};
			client.PostAsync(path, body);
			return tcs.Task;
		}

		public static Task<Stream> DownloadAsyncTask(this LiveConnectClient client, string path, object userState = null)
		{
			TaskCompletionSource<Stream> tcs = new TaskCompletionSource<Stream>();
			client.DownloadCompleted += (sender, args) =>
			{
				if (args.Cancelled) tcs.TrySetCanceled();
				if (args.Error != null) tcs.TrySetException(args.Error);

				tcs.TrySetResult(args.Result);
			};
			if (userState == null)
				client.DownloadAsync(path);
			else
				client.DownloadAsync(path, userState);
			return tcs.Task;
		}
	}
}
