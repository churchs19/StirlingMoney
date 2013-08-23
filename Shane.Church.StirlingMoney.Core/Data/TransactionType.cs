using System;
using System.Net;
using System.Windows;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public enum TransactionType
	{
		Unknown,
		Withdrawal,
		Deposit,
		Check,
		Transfer
	}
}
