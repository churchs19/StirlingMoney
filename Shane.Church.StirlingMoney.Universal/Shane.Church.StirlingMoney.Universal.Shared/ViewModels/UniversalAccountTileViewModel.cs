using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Shane.Church.StirlingMoney.Universal.ViewModels
{
    public class UniversalAccountTileViewModel : AccountTileViewModel
    {
		private IRepository<Account, Guid> _accountRepository;
        private ILoggingService _logService;

		public UniversalAccountTileViewModel(ITileService<Account, Guid> tileService, IRepository<Account, Guid> accountRepository, INavigationService navService, ILoggingService logService)
			: base(tileService, accountRepository, navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
            if (logService == null) throw new ArgumentNullException("logService");
            _logService = logService;
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

		private async void GetImage(string ImageName)
		{
			if (string.IsNullOrWhiteSpace(ImageName)) ImageName = "Book-Open.png";
			
            try
            {
                var installedLocation = Package.Current.InstalledLocation;
                
                var iconFile = await installedLocation.GetFileAsync("Assets\\AccountIcons\\" + ImageName);

                using (var stream = await iconFile.OpenStreamForReadAsync())
                {
                    var data = new byte[stream.Length];
                    using (var ms = new MemoryStream(data))
                    {
                        stream.CopyTo(ms);
                    }
                    this.Image = data;
                }
            }
            catch(Exception ex)
            {
                _logService.LogMessage("Error loading account image: " + ex.Message);
            }
		}
	}    
}
