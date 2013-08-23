using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public interface IWebNavigationService
	{
		/// <summary>
		/// Navigate to a specific URI.
		/// </summary>
		/// <param name="page">The absolute uri to the page to navigate to.</param>
		void NavigateTo(Uri page);
	}
}
