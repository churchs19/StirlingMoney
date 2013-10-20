using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Shane.Church.Utility.Core.WP
{
	public static class BindingHelper
	{
		public static void UpdateBindings(params TextBox[] items)
		{
			foreach (var item in items)
			{
				var bind = item.GetBindingExpression(TextBox.TextProperty);

				if (bind != null)
					bind.UpdateSource();
			}
		}
	}
}
