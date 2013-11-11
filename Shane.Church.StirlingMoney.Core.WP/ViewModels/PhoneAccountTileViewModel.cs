﻿using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.WP.ViewModels
{
	public class PhoneAccountTileViewModel : AccountTileViewModel
	{
		private IRepository<Account, Guid> _accountRepository;

		public PhoneAccountTileViewModel(ITileService<Account> tileService, IRepository<Account, Guid> accountRepository, INavigationService navService)
			: base(tileService, accountRepository, navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
		}

		public override void LoadData(Account a)
		{
			base.LoadData(a);
			GetImage(_imageUri);
		}

		public override async Task LoadData(Guid accountId)
		{
			await base.LoadData(accountId);
			GetImage(_imageUri);
		}

		private void GetImage(string ImageName)
		{
			if (string.IsNullOrWhiteSpace(ImageName)) ImageName = "Book-Open.png";
			// The resource name will correspond to the namespace and path in the file system.
			// Have a look at the resources collection in the debugger to figure out the name.
			string resourcePath = "Shane.Church.StirlingMoney.Core.WP.Images." + ImageName;
			Assembly assembly = Assembly.GetExecutingAssembly();

			using (var resourceStream = assembly.GetManifestResourceStream(resourcePath))
			{
				var data = new byte[resourceStream.Length];
				using (var ms = new MemoryStream(data))
				{
					resourceStream.CopyTo(ms);
				}
				this.Image = data;
			}
		}
	}
}
