using Grace;
using Grace.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public static class ContainerService
	{
		private static IDependencyInjectionContainer _kernel;

		public static IDependencyInjectionContainer Container
		{
			get
			{
				return _kernel;
			}
			set
			{
				if (_kernel == null)
				{
					_kernel = value;
				}
			}
		}
	}
}
