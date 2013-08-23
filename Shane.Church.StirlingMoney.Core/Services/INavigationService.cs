using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.Services
{
	/// <summary>
	/// The navigation service to 
	/// enable page navigation.
	/// For all our platforms.
	/// </summary>
	public interface INavigationService
	{
		/// <summary>
		/// Navigate to a specific page.
		/// Used for Windows phone.
		/// </summary>
		/// <param name="page">The absolute uri to the page to navigate to.</param>
		void NavigateTo(Uri page);

		/// <summary>
		/// Used for Windows 8.
		/// </summary>
		/// <param name="pageToNavigateTo"></param>
		void NavigateTo(Type pageToNavigateTo);


		/// <summary>
		/// Go back to
		/// the previous page.
		/// Used for Windows Phone and Windows 8.
		/// </summary>
		void GoBack();
	}
}
