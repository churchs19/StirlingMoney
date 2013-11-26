using Microsoft.Phone.Controls;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Shane.Church.StirlingMoney.Core.WP.Extensions
{
	public static class TombstoneExtensions
	{
		public static void DeactivatePage(this PhoneApplicationPage phonePage, ITombstoneFriendly viewModel)
		{
			viewModel.Deactivate();
		}

		public static void ActivatePage(this PhoneApplicationPage phonePage, ITombstoneFriendly viewModel)
		{
			RoutedEventHandler loaded = null;
			loaded = (o, e) =>
			{
				((PhoneApplicationPage)o).Loaded -= loaded;
				viewModel.Activate();
			};
			phonePage.Loaded += loaded;
		}
	}
}
