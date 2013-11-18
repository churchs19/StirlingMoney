
using System;
namespace Shane.Church.StirlingMoney.Core.Exceptions
{
	public class BackupException : Exception
	{
		public BackupException()
			: base()
		{

		}

		public BackupException(string message)
			: base(message)
		{

		}

		public BackupException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}
