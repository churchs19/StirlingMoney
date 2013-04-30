using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Shane.Church.Utility
{
	public static class RadianMathExtensions
	{
		public static double DegreeToRadian(this double angle)
		{
			return Math.PI * angle / 180.0;
		}

		public static double RadianToDegree(this double angle)
		{
			return angle * (180.0 / Math.PI);
		}
	}
}
