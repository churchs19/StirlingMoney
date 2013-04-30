using System;
using Coding4Fun.Phone.Controls;

namespace Shane.Church.Utility
{
	public static class ToastMessage
	{
		public static void Show(string title, string message)
		{
			ToastPrompt prompt = new ToastPrompt()
			{
				MillisecondsUntilHidden = 4000,
				Title = title,
				Message = message,
				TextOrientation = System.Windows.Controls.Orientation.Vertical
			};
			prompt.Show();
		}

	}
}
