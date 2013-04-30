using System;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;

namespace Shane.Church.Utility.Colors
{
	public class ColorHarmonyCalculator
	{
		public static List<Color> GetColorHarmonies(Color c)
		{
			List<Color> colors = new List<Color>();
			colors.Add(c);
			colors.AddRange(GetTriadic(c));
			var temp = colors.ToList();
			for (int i = 1; i<=3; i++ )
			{
				foreach(Color clr in temp)
				{
					colors.Add(GetMonochromaticCompliment(clr, -0.20 * i));
				}
			}
			return colors;
		}

		private static Color[] GetTriadic(Color c)
		{
			Color[] retArray = new Color[2];
			CIELch color = ColorSpaceHelper.RGBtoLch(c);
			var plus120 = color.H + 120;
			if (plus120 >= 360) plus120 = 360 - plus120;
			var minus120 = color.H - 120;
			if (minus120 < 0) minus120 = minus120 + 360;
			retArray[0] = ColorSpaceHelper.LchtoColor(new CIELch(color.L, color.C, plus120));
			retArray[1] = ColorSpaceHelper.LchtoColor(new CIELch(color.L, color.C, minus120));
			return retArray;
		}

		private static Color[] GetSplitCompliments(Color c)
		{
			Color[] retArray = new Color[2];
			CIELch color = ColorSpaceHelper.RGBtoLch(c);
			var plus150 = color.H + 150;
			if (plus150 >= 360) plus150 = 360 - plus150;
			var minus150 = color.H - 150;
			if (minus150 < 0) minus150 = minus150 + 360;
			retArray[0] = ColorSpaceHelper.LchtoColor(new CIELch(color.L, color.C, plus150));
			retArray[1] = ColorSpaceHelper.LchtoColor(new CIELch(color.L, color.C, minus150));
			return retArray;
		}

		private static Color GetTetradPair(Color c)
		{
			CIELch pair = ColorSpaceHelper.RGBtoLch(c);
			var h = pair.H + 30;
			if (h >= 360) h = h - 360;
			pair.H = h;
			return ColorSpaceHelper.LchtoColor(pair);
		}

		private static Color GetMonochromaticCompliment(Color c, double saturationDelta)
		{
			CIELch compliment = ColorSpaceHelper.RGBtoLch(c);
			compliment.C = (1.0 + saturationDelta) * compliment.C;
			return ColorSpaceHelper.LchtoColor(compliment);
		}

		private static Color GetCompliment(Color c)
		{
			CIELch compliment = ColorSpaceHelper.RGBtoLch(c);
			var h = compliment.H + 180;
			if (h >= 360) h = h - 360;
			compliment.H = h;
			return ColorSpaceHelper.LchtoColor(compliment);
		}
	}
}
