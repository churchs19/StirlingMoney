using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Exceptions;
using Shane.Church.StirlingMoney.Core.Repositories;
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
		protected IRepository<Account, Guid> _accounts;
		protected IRepository<AppSyncUser, string> _users;
		protected IRepository<Budget, Guid> _budgets;
		protected IRepository<Goal, Guid> _goals;
		protected IRepository<Category, Guid> _categories;
		protected IRepository<Transaction, Guid> _transactions;
		protected ILicensingService _licensing;

		public SyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IRepository<Account, Guid> accounts,
							IRepository<AppSyncUser, string> users,
							IRepository<Budget, Guid> budgets,
							IRepository<Goal, Guid> goals,
							IRepository<Category, Guid> categories,
							IRepository<Transaction, Guid> transactions,
							ILicensingService licensing)
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
			if (licensing == null) throw new ArgumentNullException("licensing");
			_licensing = licensing;
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
				if (_licensing.IsSyncLicensed())
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
							//						await BackupDatabase();

							DateTimeOffset lastSuccessfulSyncDate = _settingsService.LoadSetting<DateTimeOffset>("LastSuccessfulSync");

							var localCategories = _categories.GetUpdatedEntries(lastSuccessfulSyncDate);
							var localAccounts = _accounts.GetUpdatedEntries(lastSuccessfulSyncDate);
							var localUsers = _users.GetUpdatedEntries(lastSuccessfulSyncDate);
							var localBudgets = _budgets.GetUpdatedEntries(lastSuccessfulSyncDate);
							var localGoals = _goals.GetUpdatedEntries(lastSuccessfulSyncDate);
							var localTransactions = _transactions.GetUpdatedEntries(lastSuccessfulSyncDate);

							Task.WaitAll(localCategories, localAccounts, localBudgets, localUsers, localGoals, localTransactions);

							List<SyncItem> objects = new List<SyncItem>();
							JsonSerializer serializer = JsonSerializer.Create(Client.SerializerSettings);
							JArray arrCategories = JArray.FromObject(localCategories.Result, serializer);
							objects.Add(new SyncItem() { TableName = "Categories", KeyField = "categoryId", Values = arrCategories });
							JArray arrAccounts = JArray.FromObject(localAccounts.Result, serializer);
							objects.Add(new SyncItem() { TableName = "Accounts", KeyField = "accountId", Values = arrAccounts });
							JArray arrUsers = JArray.FromObject(localUsers.Result, serializer);
							objects.Add(new SyncItem() { TableName = "AppSyncUsers", KeyField = "userEmail", Values = arrUsers });
							JArray arrBudgets = JArray.FromObject(localBudgets.Result, serializer);
							objects.Add(new SyncItem() { TableName = "Budgets", KeyField = "budgetId", Values = arrBudgets });
							JArray arrGoals = JArray.FromObject(localGoals.Result, serializer);
							objects.Add(new SyncItem() { TableName = "Goals", KeyField = "goalId", Values = arrGoals });
							JArray arrTransactions = JArray.FromObject(localTransactions.Result, serializer);
							objects.Add(new SyncItem() { TableName = "Transactions", KeyField = "transactionId", Values = arrTransactions });

							JObject body = new JObject();
							body.Add("email", JToken.FromObject(this.Email));
							body.Add("items", JArray.FromObject(objects, serializer));
							body.Add("lastSyncDate", lastSuccessfulSyncDate);

							var results = await Client.InvokeApiAsync("sync", body);

							try
							{
								var updateTasks = new List<Task>();
								foreach (var item in results.OrderBy(it => it["tableName"].ToString(), new TableNameSortComparer()))
								{

									switch (item["tableName"].ToString())
									{
										case "Categories":
											var categoryChanges = item["changes"].ToObject<List<Category>>();
											updateTasks.Add(_categories.BatchUpdateEntriesAsync(categoryChanges));
											break;
										case "Accounts":
											var accountChanges = item["changes"].ToObject<List<Account>>();
											updateTasks.Add(_accounts.BatchUpdateEntriesAsync(accountChanges));
											break;
										case "AppSyncUsers":
											var userChanges = item["changes"].ToObject<List<AppSyncUser>>().Where(it => it.UserEmail.ToLower() != this.Email.ToLower()).ToList();
											updateTasks.Add(_users.BatchUpdateEntriesAsync(userChanges));
											break;
										case "Budgets":
											var budgetChanges = item["changes"].ToObject<List<Budget>>();
											updateTasks.Add(_budgets.BatchUpdateEntriesAsync(budgetChanges));
											break;
										case "Goals":
											var goalChanges = item["changes"].ToObject<List<Goal>>();
											updateTasks.Add(_goals.BatchUpdateEntriesAsync(goalChanges));
											break;
										case "Transactions":
											var transactionChanges = item["changes"].ToObject<List<Transaction>>();
											updateTasks.Add(_transactions.BatchUpdateEntriesAsync(transactionChanges));
											break;
									}
								}
								Task.WaitAll(updateTasks.ToArray());
							}
							catch
							{
								throw;
							}

							await _accounts.Commit();

							_settingsService.SaveSetting<DateTimeOffset>(DateTimeOffset.Now, "LastSuccessfulSync");

							//						await RemoveDatabaseBackup();
						}
					}
					if (SyncCompleted != null)
						SyncCompleted();
				}
				else
				{
					throw new NotAuthorizedException();
				}
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Sync Error");
				//RestoreDatabase().Wait(5000);
				//RemoveDatabaseBackup().Wait(5000);
				throw;
			}
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
