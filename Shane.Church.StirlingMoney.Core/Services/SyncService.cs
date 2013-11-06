using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shane.Church.StirlingMoney.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Services
{
	public abstract class SyncService
	{
		protected ISettingsService _settingsService;
		protected ILoggingService _log;
		protected IRepository<Account> _accounts;
		protected IRepository<AppSyncUser> _users;
		protected IRepository<Budget> _budgets;
		protected IRepository<Goal> _goals;
		protected IRepository<Category> _categories;
		protected IRepository<Transaction> _transactions;

		public SyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IRepository<Account> accounts,
							IRepository<AppSyncUser> users,
							IRepository<Budget> budgets,
							IRepository<Goal> goals,
							IRepository<Category> categories,
							IRepository<Transaction> transactions)
		{
			if (settings == null) throw new ArgumentNullException("settings");
			_settingsService = settings;
			if (client == null) throw new ArgumentNullException("client");
			Client = client;
			if (log == null) throw new ArgumentNullException("log");
			_log = log;
			if (accounts == null) throw new ArgumentNullException("accounts");
			_accounts = accounts;
			if (users == null) throw new ArgumentNullException("users");
			_users = users;
			if (budgets == null) throw new ArgumentNullException("budgets");
			_budgets = budgets;
			if (goals == null) throw new ArgumentNullException("goals");
			_goals = goals;
			if (categories == null) throw new ArgumentNullException("categories");
			_categories = categories;
			if (transactions == null) throw new ArgumentNullException("transactions");
			_transactions = transactions;
		}

		private IMobileServiceClient _client;
		public IMobileServiceClient Client
		{
			get { return _client; }
			set { _client = value; }
		}

		private MobileServiceUser _user;
		public MobileServiceUser User
		{
			get { return _user; }
			set
			{
				_user = value;
			}
		}

		private string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		private string _email;
		public string Email
		{
			get { return _email; }
			set
			{
				_email = value;
			}
		}

		public bool IsConnected
		{
			get { return User != null; }
		}

		public abstract void Disconnect();

		public async Task Authenticate()
		{
			if (User == null)
				await AuthenticateUser();
		}

		public abstract Task<bool> IsNetworkConnected();

		public abstract Task AuthenticateUserSilent();

		public abstract Task AuthenticateUser();

		public delegate void SyncCompletedHandler();
		public event SyncCompletedHandler SyncCompleted;

		public async Task Sync(bool silent = false)
		{
			try
			{
				if (await IsNetworkConnected())
				{
					if (!silent)
					{
						await Authenticate();
					}
					else
					{
						await AuthenticateUserSilent();
					}

					if (User != null)
					{
						DateTimeOffset lastSuccessfulSyncDate = _settingsService.LoadSetting<DateTimeOffset>("LastSuccessfulSync");

						var localCategories = _categories.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).ToList();
						var localAccounts = _accounts.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).ToList();
						var localUsers = _users.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).ToList();
						var localBudgets = _budgets.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).ToList();
						var localGoals = _goals.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).ToList();
						var localTransactions = _transactions.GetFilteredEntries(it => it.EditDateTime > lastSuccessfulSyncDate, true).ToList();

						List<SyncItem> objects = new List<SyncItem>();
						JsonSerializer serializer = JsonSerializer.Create(Client.SerializerSettings);
						JArray arrCategories = JArray.FromObject(localCategories, serializer);
						objects.Add(new SyncItem() { TableName = "Categories", KeyField = "categoryId", Values = arrCategories });
						JArray arrAccounts = JArray.FromObject(localAccounts, serializer);
						objects.Add(new SyncItem() { TableName = "Accounts", KeyField = "accountId", Values = arrAccounts });
						JArray arrUsers = JArray.FromObject(localUsers, serializer);
						objects.Add(new SyncItem() { TableName = "AppSyncUsers", KeyField = "userEmail", Values = arrUsers });
						JArray arrBudgets = JArray.FromObject(localBudgets, serializer);
						objects.Add(new SyncItem() { TableName = "Budgets", KeyField = "budgetId", Values = arrBudgets });
						JArray arrGoals = JArray.FromObject(localGoals, serializer);
						objects.Add(new SyncItem() { TableName = "Goals", KeyField = "goalId", Values = arrGoals });
						JArray arrTransactions = JArray.FromObject(localTransactions, serializer);
						objects.Add(new SyncItem() { TableName = "Transactions", KeyField = "transactionId", Values = arrTransactions });

						JObject body = new JObject();
						body.Add("email", JToken.FromObject(this.Email));
						body.Add("items", JArray.FromObject(objects, serializer));
						body.Add("lastSyncDate", lastSuccessfulSyncDate);

						var results = await Client.InvokeApiAsync("sync", body);

						foreach (var item in results.OrderBy(it=>it["tableName"].ToString(), new TableNameSortComparer()))
						{
							try
							{
								switch (item["tableName"].ToString())
								{
									case "Categories":
										var categoryChanges = item["changes"].ToObject<List<Category>>();
										foreach (var c in categoryChanges)
											await _categories.AddOrUpdateEntryAsync(c);
										break;
									case "Accounts":
										var accountChanges = item["changes"].ToObject<List<Account>>();
										foreach (var a in accountChanges)
											await _accounts.AddOrUpdateEntryAsync(a);
										break;
									case "AppSyncUsers":
										var userChanges = item["changes"].ToObject<List<AppSyncUser>>();
										foreach (var u in userChanges.Where(it => it.UserEmail.ToLower() != this.Email.ToLower()))
											await _users.AddOrUpdateEntryAsync(u);
										break;
									case "Budgets":
										var budgetChanges = item["changes"].ToObject<List<Budget>>();
										foreach (var b in budgetChanges)
											await _budgets.AddOrUpdateEntryAsync(b);
										break;
									case "Goals":
										var goalChanges = item["changes"].ToObject<List<Goal>>();
										foreach (var g in goalChanges)
											await _goals.AddOrUpdateEntryAsync(g);
										break;
									case "Transactions":
										var transactionChanges = item["changes"].ToObject<List<Transaction>>();
										foreach (var t in transactionChanges)
											await _transactions.AddOrUpdateEntryAsync(t);
										break;
								}
							}
							catch
							{
								throw;
							}
						}

						_settingsService.SaveSetting<DateTimeOffset>(DateTimeOffset.Now, "LastSuccessfulSync");
					}
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Error");
				//TODO: Do I need a message here?
				throw;
			}
			if (SyncCompleted != null)
				SyncCompleted();
		}
	}

	public class TableNameSortComparer : IComparer<string>
	{
		private Dictionary<string, int> tableOrder = new Dictionary<string, int>();

		public TableNameSortComparer()
		{
			tableOrder.Add("Categories", 0);
			tableOrder.Add("Accounts", 1);
			tableOrder.Add("AppSyncUsers", 2);
			tableOrder.Add("Budgets", 3);
			tableOrder.Add("Goals", 4);
			tableOrder.Add("Transactions", 5);
		}

		public int Compare(string x, string y)
		{
			return tableOrder[x].CompareTo(tableOrder[y]);
		}
	}
}
