using Grace;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Data
{
	public class Transaction
	{
		private IDataRepository<Account, Guid> _accountRepository;
		private IDataRepository<Category, Guid> _categoryRepository;

		public Transaction(IDataRepository<Account, Guid> accountRepo, IDataRepository<Category, Guid> categoryRepo)
		{
			if (accountRepo == null) throw new ArgumentNullException("accountRepo");
			_accountRepository = accountRepo;
			if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
			_categoryRepository = categoryRepo;

			CheckNumber = 0;
			Location = "";
		}

		public Transaction()
			: this(ContainerService.Container.Locate<IDataRepository<Account, Guid>>(), ContainerService.Container.Locate<IDataRepository<Category, Guid>>())
		{

		}

		public Guid TransactionId { get; set; }
		public DateTimeOffset TransactionDate { get; set; }
		public double Amount { get; set; }
		public string Location { get; set; }
		public string Note { get; set; }
		public bool Posted { get; set; }
		public long CheckNumber { get; set; }
		public DateTimeOffset EditDateTime { get; set; }
		public bool IsDeleted { get; set; }

		public Guid AccountId { get; set; }
		public Guid CategoryId { get; set; }

		public async Task<Account> GetAccount()
		{
			try
			{
				return await _accountRepository.GetEntryAsync(AccountId);
			}
			catch
			{
				return null;
			}
		}

		public async Task<Category> GetCategory()
		{
			try
			{
				if (!CategoryId.Equals(Guid.Empty)) return null;
				return await ContainerService.Container.Locate<IDataRepository<Category, Guid>>().GetEntryAsync(CategoryId);
			}
			catch
			{
				return null;
			}
		}
	}
}
