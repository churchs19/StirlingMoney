using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListParams
	{
		public Guid Id { get; set; }
		public bool PinnedTile { get; set; }
	}
}
