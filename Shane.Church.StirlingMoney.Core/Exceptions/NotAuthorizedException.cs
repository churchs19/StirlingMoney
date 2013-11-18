using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Exceptions
{
	public class NotAuthorizedException : Exception
	{
		public NotAuthorizedException() 
			: base() 
		{ 
		
		}

		public NotAuthorizedException(string message)
			: base(message)
		{

		}

		public NotAuthorizedException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}
