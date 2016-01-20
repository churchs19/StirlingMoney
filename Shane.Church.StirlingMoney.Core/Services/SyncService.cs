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
		protected IDataRepository<Account, Guid> _accounts;
		protected IDataRepository<AppSyncUser, string> _users;
		protected IDataRepository<Budget, Guid> _budgets;
		protected IDataRepository<Goal, Guid> _goals;
		protected IDataRepository<Category, Guid> _categories;
		protected IDataRepository<Transaction, Guid> _transactions;
		protected ILicensingService _licensing;

		public SyncService(IMobileServiceClient client,
							ISettingsService settings,
							ILoggingService log,
							IDataRepository<Account, Guid> accounts,
							IDataRepository<AppSyncUser, string> users,
							IDataRepository<Budget, Guid> budgets,
							IDataRepository<Goal, Guid> goals,
							IDataRepository<Category, Guid> categories,
							IDataRepository<Transaction, Guid> transactions,
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

                            var filterString = string.Format("[EditDateTime] >= {0}", lastSuccessfulSyncDate.UtcTicks);

                            var localCategories = await _categories.GetFilteredEntriesAsync(filterString, true);
							var localAccounts = await _accounts.GetFilteredEntriesAsync(filterString, true);
							var localUsers = await _users.GetFilteredEntriesAsync(filterString, true);
							var localBudgets = await _budgets.GetFilteredEntriesAsync(filterString, true);
							var localGoals = await _goals.GetFilteredEntriesAsync(filterString, true);
							var localTransactions = await _transactions.GetFilteredEntriesAsync(filterString, true);

							List<SyncItem> objects = new List<SyncItem>();
							JsonSerializer serializer = JsonSerializer.Create(Client.SerializerSettings);
							JArray arrCategories = JArray.FromObject(localCategories.ToList(), serializer);
							objects.Add(new SyncItem() { TableName = "Categories", KeyField = "categoryId", Values = arrCategories });
							JArray arrAccounts = JArray.FromObject(localAccounts.ToList(), serializer);
							objects.Add(new SyncItem() { TableName = "Accounts", KeyField = "accountId", Values = arrAccounts });
							JArray arrUsers = JArray.FromObject(localUsers.ToList(), serializer);
							objects.Add(new SyncItem() { TableName = "AppSyncUsers", KeyField = "userEmail", Values = arrUsers });
							JArray arrBudgets = JArray.FromObject(localBudgets.ToList(), serializer);
							objects.Add(new SyncItem() { TableName = "Budgets", KeyField = "budgetId", Values = arrBudgets });
							JArray arrGoals = JArray.FromObject(localGoals.ToList(), serializer);
							objects.Add(new SyncItem() { TableName = "Goals", KeyField = "goalId", Values = arrGoals });
							JArray arrTransactions = JArray.FromObject(localTransactions.ToList(), serializer);
							objects.Add(new SyncItem() { TableName = "Transactions", KeyField = "transactionId", Values = arrTransactions });

							JObject body = new JObject();
							body.Add("email", JToken.FromObject(this.Email));
							body.Add("items", JArray.FromObject(objects, serializer));
							body.Add("lastSyncDate", lastSuccessfulSyncDate);

							var results = await Client.InvokeApiAsync("sync", body);

							try
							{
//								var updateTasks = new List<Task>();
								foreach (var item in results.OrderBy(it => it["tableName"].ToString(), new TableNameSortComparer()))
								{

									switch (item["tableName"].ToString())
									{
										case "Categories":
											var categoryChanges = item["changes"].ToObject<List<Category>>();
                                            //											updateTasks.Add(_categories.BatchUpdateEntriesAsync(categoryChanges));
                                            _categories.BatchUpdateEntries(categoryChanges);
											break;
										case "Accounts":
											var accountChanges = item["changes"].ToObject<List<Account>>();
                                            //											updateTasks.Add(_accounts.BatchUpdateEntriesAsync(accountChanges));
                                            _accounts.BatchUpdateEntries(accountChanges);
											break;
										case "AppSyncUsers":
											var userChanges = item["changes"].ToObject<List<AppSyncUser>>().Where(it => it.UserEmail.ToLower() != this.Email.ToLower()).ToList();
                                            //											updateTasks.Add(_users.BatchUpdateEntriesAsync(userChanges));
                                            _users.BatchUpdateEntries(userChanges);
											break;
										case "Budgets":
											var budgetChanges = item["changes"].ToObject<List<Budget>>();
                                            //											updateTasks.Add(_budgets.BatchUpdateEntriesAsync(budgetChanges));
                                            _budgets.BatchUpdateEntries(budgetChanges);
											break;
										case "Goals":
											var goalChanges = item["changes"].ToObject<List<Goal>>();
                                            //											updateTasks.Add(_goals.BatchUpdateEntriesAsync(goalChanges));
                                            _goals.BatchUpdateEntries(goalChanges);
											break;
                                        case "Transactions":
                                            var transactionChanges = item["changes"].ToObject<List<Transaction>>();
                                            //                                            updateTasks.Add(_transactions.BatchUpdateEntriesAsync(transactionChanges));
                                            _transactions.BatchUpdateEntries(transactionChanges);
                                            break;
                                    }
								}
								//Task.WaitAll(updateTasks.ToArray());
							}
							catch
							{
								throw;
							}

							foreach (var t in _transactions.GetFilteredEntries("[IsDeleted] = 1", true).Select(it=>it.TransactionId))
								_transactions.DeleteEntry(t, true);
							foreach (var t in _goals.GetFilteredEntries("[IsDeleted] = 1", true).Select(it=>it.GoalId))
								_goals.DeleteEntry(t, true);
							foreach (var t in _budgets.GetFilteredEntries("[IsDeleted] = 1", true).Select(it=>it.BudgetId))
								_budgets.DeleteEntry(t, true);
							foreach (var t in _users.GetFilteredEntries("[IsDeleted] = 1", true).Select(it=>it.UserId))
								_users.DeleteEntry(t, true);
							foreach (var t in _accounts.GetFilteredEntries("[IsDeleted] = 1", true).Select(it=>it.AccountId))
                                _accounts.DeleteEntry(t, true);
							foreach (var t in _categories.GetFilteredEntries("[IsDeleted] = 1", true).Select(it=>it.CategoryId))
                                _categories.DeleteEntry(t, true);

							_settingsService.SaveSetting<DateTimeOffset>(DateTimeOffset.Now, "LastSuccessfulSync");
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
