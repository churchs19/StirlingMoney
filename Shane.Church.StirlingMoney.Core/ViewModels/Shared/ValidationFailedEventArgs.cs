using System;
using System.Collections.Generic;

namespace Shane.Church.StirlingMoney.Core.ViewModels.Shared
{
	public class ValidationFailedEventArgs : EventArgs
	{
		public ValidationFailedEventArgs()
			: base()
		{
			Errors = new List<string>();
		}

		public ValidationFailedEventArgs(IList<string> errors)
			: base()
		{
			if (errors == null) throw new ArgumentNullException("errors");
			Errors = errors;
		}

		public IList<string> Errors { get; set; }
	}
}
