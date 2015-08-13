using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Shane.Church.StirlingMoney.WP.ViewModels
{
	public class PhoneAccountTileViewModel : AccountTileViewModel
	{
		private IDataRepository<Account, Guid> _accountRepository;

		public PhoneAccountTileViewModel(ITileService<Account, Guid> tileService, IDataRepository<Account, Guid> accountRepository, INavigationService navService)
			: base(tileService, accountRepository, navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
		}

		public override void LoadData(Account a, bool updateTile = false)
		{
			base.LoadData(a, updateTile);
			GetImage(_imageUri);
		}

		public override async Task LoadData(Guid accountId, bool updateTile = false)
		{
			await base.LoadData(accountId, updateTile);
			GetImage(_imageUri);
		}

		private void GetImage(string ImageName)
		{
			if (string.IsNullOrWhiteSpace(ImageName)) ImageName = "Book-Open.png";
			StreamResourceInfo sri = Application.GetResourceStream(new Uri("Images/AccountIcons.zip", UriKind.Relative));
			if (sri != null)
			{
				var resourceStream = Application.GetResourceStream(sri, new Uri(ImageName, UriKind.Relative));
				if (resourceStream != null)
				{
					var data = new byte[resourceStream.Stream.Length];
					using (var ms = new MemoryStream(data))
					{
						resourceStream.Stream.CopyTo(ms);
					}
					this.Image = data;
				}
			}
		}
	}
}
