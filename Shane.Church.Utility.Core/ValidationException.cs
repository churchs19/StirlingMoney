using Shane.Church.Utility.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.Utility.Core
{
	public class ValidationException : Exception
	{
		public ValidationException(string field)
			: base(string.Format(UtilityResources.ValidationFailedErrorMessage, field))
		{

		}

		public ValidationException(string field, Exception innerException)
			: base(string.Format(UtilityResources.ValidationFailedErrorMessage, field), innerException)
		{

		}
	}
}
