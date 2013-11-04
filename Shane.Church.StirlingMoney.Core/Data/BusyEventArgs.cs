using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class BusyEventArgs : EventArgs
	{
		public BusyEventArgs()
		{
			IsBusy = false;
			IsError = false;
			Error = null;
			Message = Strings.Resources.ProgressBarText;
			AnimationType = 2;
		}

		public bool IsBusy { get; set; }
		public string Message { get; set; }
		public int AnimationType { get; set; }
		public bool IsError { get; set; }
		public Exception Error { get; set; }
	}
}
