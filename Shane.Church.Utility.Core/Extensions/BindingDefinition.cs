using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.Utility.Core.Extensions
{
	public class BindingDefinition
	{
		public BindingDefinition(
			InterfaceTypeDefinition definition, Type openGenericType)
		{
			Implementation = definition.Implementation;
			Service = definition.GetService(openGenericType);
		}

		public Type Implementation { get; private set; }

		public Type Service { get; private set; }
	}
}
