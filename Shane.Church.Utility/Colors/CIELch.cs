using System;
using System.ComponentModel;

namespace Shane.Church.Utility.Colors
{
	/// <summary>
	/// Structure to define CIE L*C*Hº.
	/// </summary>
	public struct CIELch
	{
		/// <summary>
		/// Gets an empty CIELab structure.
		/// </summary>
		public static readonly CIELch Empty = new CIELch();

		#region Fields
		private double l;
		private double c;
		private double h;

		#endregion

		#region Operators
		public static bool operator ==(CIELch item1, CIELch item2)
		{
			return (
				item1.L == item2.L 
				&& item1.C == item2.C
				&& item1.H == item2.H
				);
		}

		public static bool operator !=(CIELch item1, CIELch item2)
		{
			return (
				item1.L != item2.L 
				|| item1.C != item2.C 
				|| item1.H != item2.H
				);
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets or sets L component.
		/// </summary>
		public double L
		{
			get
			{
				return this.l;
			}
			set
			{
				this.l = value;
			}
		}

		/// <summary>
		/// Gets or sets a component.
		/// </summary>
		public double C
		{
			get
			{
				return this.c;
			}
			set
			{
				this.c = value;
			}
		}

		/// <summary>
		/// Gets or sets a component.
		/// </summary>
		public double H
		{
			get
			{
				return this.h;
			}
			set
			{
				this.h = value;
			}
		}

		#endregion

		public CIELch(double l, double c, double h) 
		{
			this.l = l;
			this.c = c;
			this.h = h;
		}

		#region Methods
		public override bool Equals(Object obj) 
		{
			if(obj==null || GetType()!=obj.GetType()) return false;

			return (this == (CIELch)obj);
		}

		public override int GetHashCode() 
		{
			return L.GetHashCode() ^ c.GetHashCode() ^ h.GetHashCode();
		}

		#endregion
	} 
}
