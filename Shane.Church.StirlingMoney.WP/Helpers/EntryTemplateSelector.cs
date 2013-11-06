using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Telerik.Windows.Controls;

namespace Shane.Church.StirlingMoney.WP.Helpers
{
	public class EntryTemplateSelectorWrapper
	{
		public bool IsSelected { get; set; }
		public object Model { get; set; }
	}

	public class EntryTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var visualContainer = container as RadDataBoundListBoxItem;
			if (visualContainer != null)
			{
				var isSelected = item is EntryTemplateSelectorWrapper ? ((EntryTemplateSelectorWrapper)item).IsSelected : visualContainer.IsSelected;
				if (isSelected)
					return SelectedTemplate;
			}
			return DefaultTemplate;
		}

		public DataTemplate SelectedTemplate { get; set; }
		public DataTemplate DefaultTemplate { get; set; }
	}
}
