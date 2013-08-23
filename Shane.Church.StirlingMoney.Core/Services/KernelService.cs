using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public static class KernelService
	{
		private static IKernel _kernel;

		public static IKernel Kernel
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
