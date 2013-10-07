using Shane.Church.StirlingMoney.Core.Data;
using System;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AddEditTransactionParams
	{
		public AddEditTransactionParams()
		{
			AccountId = Guid.Empty;
			TransactionId = Guid.Empty;
			Type = TransactionType.Unknown;
		}

		public Guid AccountId { get; set; }
		public Guid TransactionId { get; set; }
		public TransactionType Type { get; set; }
	}
}
