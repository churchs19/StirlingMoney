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

		public static void UpdatePasswordBindings(params PasswordBox[] items)
		{
			foreach (var item in items)
			{
				var bind = item.GetBindingExpression(PasswordBox.PasswordProperty);

				if (bind != null)
					bind.UpdateSource();
			}
		}
	}
}
