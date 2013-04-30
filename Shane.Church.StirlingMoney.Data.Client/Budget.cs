using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Data.Sync
{
	public partial class Budget : Microsoft.Synchronization.ClientServices.IsolatedStorage.IsolatedStorageOfflineEntity
	{
		public DateTime CurrentPeriodStart
		{
			get
			{
				if (BudgetPeriod == 3)
				{
					return StartDate;
				}
				else
				{
					int day = StartDate.Day;
					DateTime start = DateTime.Today;
					if (start.Day < day)
					{
						start = start.AddMonths(-1);
					}
					if (start.Month == 2 && start.Month > 28)
					{
						if (DateTime.IsLeapYear(start.Year))
						{
							return new DateTime(start.Year, start.Month, 29);
						}
						else
						{
							return new DateTime(start.Year, start.Month, 28);
						}
					}
					else if ((start.Month == 4 || start.Month == 6 || start.Month == 9 || start.Month == 11) && day == 31)
					{
						return new DateTime(start.Year, start.Month, 30);
					}
					else
					{
						return new DateTime(start.Year, start.Month, day);
					}
				}
			}
		}

		public DateTime CurrentPeriodEnd
		{
			get
			{
				switch (BudgetPeriod)
				{
					case 0:
						return CurrentPeriodStart.AddDays(7);
					case 1:
					default:
						return CurrentPeriodStart.AddMonths(1);
					case 2:
						return CurrentPeriodStart.AddYears(1);
				}
			}
		}
	}
}
