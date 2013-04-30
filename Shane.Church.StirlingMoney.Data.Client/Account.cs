using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Data.Sync
{
	public partial class Account : Microsoft.Synchronization.ClientServices.IsolatedStorage.IsolatedStorageOfflineEntity
	{
		public double AccountBalance
		{
			get
			{
				try
				{
					var transactionTotal = (from t in ContextInstance.Context.TransactionCollection
											where t._accountId == this.AccountId
											select t.Amount).Sum();
					return InitialBalance + transactionTotal;
				}
				catch { return InitialBalance; }
			}
		}

		public double PostedBalance
		{
			get
			{
				try
				{
					var transactionTotal = (from t in ContextInstance.Context.TransactionCollection
											where t._accountId == this.AccountId && t.Posted
											select t.Amount).Sum();
					return InitialBalance + transactionTotal;
				}
				catch { return InitialBalance; }
			}
		}
	}
}
