using System;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public interface ITransactionSum
	{
		double GetSumByAccount(Guid accountId);
		double GetPostedSumByAccount(Guid accountId);
	}
}
