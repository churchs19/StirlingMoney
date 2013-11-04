﻿using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Shane.Church.StirlingMoney.Core.WP.ViewModels
{
	public class PhoneAddEditAccountViewModel : AddEditAccountViewModel
	{
		private IRepository<Account> _accountRepository;

		public PhoneAddEditAccountViewModel(IRepository<Account> accountRepository, INavigationService navService)
			: base(accountRepository, navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
		}

		public override void LoadData(Guid accountId)
		{
			base.LoadData(accountId);
			GetAvailableImages();
			if (accountId != null && !accountId.Equals(Guid.Empty))
			{
				var acct = _accountRepository.GetFilteredEntries(it => it.AccountId == accountId).FirstOrDefault();
				if (acct != null)
				{
					Image = AvailableImages.Where(it => it.Name == acct.ImageUri).FirstOrDefault();
				}
			}
			if (Image == null)
			{
				Image = AvailableImages.Where(it => it.Name == "Book-Open.png").FirstOrDefault();
			}
		}

		private void GetAvailableImages()
		{
			// The resource name will correspond to the namespace and path in the file system.
			// Have a look at the resources collection in the debugger to figure out the name.
			string resourcePath = "Shane.Church.StirlingMoney.Core.WP.Images";
			Assembly assembly = Assembly.GetExecutingAssembly();
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
					AvailableImages.Add(new ImageData() { Name = key, Data = data });
				}
			}
		}
	}
}