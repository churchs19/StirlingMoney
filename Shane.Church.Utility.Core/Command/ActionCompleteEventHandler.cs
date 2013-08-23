using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.Utility.Core.Command
{
	public delegate void ActionCompleteEventHandler(object sender, EventArgs args);

	public class ValidationResultEventArgs : EventArgs
	{
		public bool IsValid { get; set; }

		public ValidationResultEventArgs(bool isValid = true)
		{
			IsValid = isValid;
		}
	}
}
