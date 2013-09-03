using System;
using System.Data.Linq;

namespace Shane.Church.StirlingMoney.Data.v2
{
	public class StirlingMoneyDataContext : DataContext
	{
		// Specify the connection string as a static, used in main page and app.xaml.
		public static string DBConnectionString = "Data Source=isostore:/StirlingMoney.v2.sdf";
		public static string DBFileName = "StirlingMoney.v2.sdf";

		public StirlingMoneyDataContext()
			: base(StirlingMoneyDataContext.DBConnectionString)
		{ }

		// Pass the connection string to the base class.
		public StirlingMoneyDataContext(string connectionString)
			: base(connectionString)
		{ }

		public Table<Account> Accounts;
		public Table<Transaction> Transactions;
		public Table<Category> Categories;
		public Table<Budget> Budgets;
		public Table<Goal> Goals;
	}
}
