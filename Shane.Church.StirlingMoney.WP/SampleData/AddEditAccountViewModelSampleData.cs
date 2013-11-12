using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.WP.SampleData
{
	public class SampleAccountRepository : IRepository<Account, Guid>
	{
		public IQueryable<Account> GetAllEntries(bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<IQueryable<Account>> GetAllEntriesAsync(bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public IQueryable<Account> GetFilteredEntries(System.Linq.Expressions.Expression<Func<Account, bool>> filter, bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<IQueryable<Account>> GetFilteredEntriesAsync(System.Linq.Expressions.Expression<Func<Account, bool>> filter, bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public void DeleteEntry(Account entry, bool hardDelete = false)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task DeleteEntryAsync(Account entry, bool hardDelete = false)
		{
			throw new NotImplementedException();
		}

		public Account AddOrUpdateEntry(Account entry)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<Account> AddOrUpdateEntryAsync(Account entry)
		{
			throw new NotImplementedException();
		}

		public void BatchUpdateEntries(System.Collections.Generic.ICollection<Account> entries)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task BatchUpdateEntriesAsync(System.Collections.Generic.ICollection<Account> entries)
		{
			throw new NotImplementedException();
		}


		public Task<Account> GetEntryAsync(Guid key)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}


		public Task Commit()
		{
			throw new NotImplementedException();
		}


		public Task<IQueryable<Account>> GetUpdatedEntries(DateTimeOffset date)
		{
			throw new NotImplementedException();
		}


		public Task<int> GetEntriesCount(bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public Task<IQueryable<Account>> GetIndexFilteredEntriesAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public Task<int> GetIndexFilteredEntriesCountAsync<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

#pragma warning disable 1066
		int IRepository<Account, Guid>.GetEntriesCount(bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}
#pragma warning restore 1066

		public Task<int> GetEntriesCountAsync(bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public int GetIndexFilteredEntriesCount<TIndex>(string indexName, TIndex indexValue, bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public IQueryable<Guid> GetAllKeys(bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.Dictionary<Guid, TIndex> GetAllIndexKeys<TIndex>(string indexName, bool includeDeleted = false)
		{
			throw new NotImplementedException();
		}
	}
	public class SampleNavService : INavigationService
	{
		public bool CanGoBack
		{
			get { return false; }
		}

		public void GoBack()
		{

		}

		public void Navigate<TDestinationViewModel>(object parameter = null)
		{

		}


		public Uri NavigationUri<TDestinationViewModel>(object parameter = null)
		{
			throw new NotImplementedException();
		}
	}
	public class SampleAddEditAccountViewModel : AddEditAccountViewModel
	{
		public SampleAddEditAccountViewModel(IRepository<Account, Guid> account, INavigationService navService)
			: base(account, navService)
		{

		}

		public override async Task LoadData(Guid accountId)
		{
			await base.LoadData(accountId);
		}
	}
	public class AddEditAccountViewModelSampleData
	{
		public AddEditAccountViewModel SampleData
		{
			get
			{
				var model = new SampleAddEditAccountViewModel(new SampleAccountRepository(), new SampleNavService());
				LoadImages(model);
				return model;
			}
		}

		private void LoadImages(SampleAddEditAccountViewModel model)
		{
			var assembly = Assembly.Load(new AssemblyName("Shane.Church.StirlingMoney.Core.WP"));
			string resourcePath = "Shane.Church.StirlingMoney.Core.WP.Images";
			var resources = assembly.GetManifestResourceNames().Where(it => it.StartsWith(resourcePath));

			foreach (var r in resources)
			{
				var key = r.Substring(42);
				using (var resourceStream = assembly.GetManifestResourceStream(r))
				{
					var data = new byte[resourceStream.Length];
					using (var ms = new MemoryStream(data))
					{
						resourceStream.CopyTo(ms);
					}
					model.AvailableImages.Add(new ImageData() { Name = key, Data = data });
				}
			}

		}
	}
}
