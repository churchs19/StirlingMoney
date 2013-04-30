using System;
using System.Collections.Generic;
using System.Windows.Media;
using Shane.Church.Utility;
using Shane.Church.Utility.Colors;
using System.Windows;

namespace Shane.Church.StirlingMoney
{
	public class ChartColors
	{
		public ChartColors()
		{
		}

		private static List<Brush> _chartBrushes;

		public List<Brush> ChartBrushes
		{
			get
			{
				if (_chartBrushes == null)
				{
					Color currentAccentColorHex = (Color)Application.Current.Resources["PhoneAccentColor"];
					var colors = ColorHarmonyCalculator.GetColorHarmonies(currentAccentColorHex);
					_chartBrushes = new List<Brush>();
					foreach (Color c in colors)
					{
						_chartBrushes.Add(new SolidColorBrush(c));
					}
				}
				return _chartBrushes;
			}
		}

		public Brush SecondaryChartBrush
		{
			get { return ChartBrushes[1]; }
		}
	}
}
