using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Shane.Church.StirlingMoney.Universal.ViewModels
{
    public class UniversalAddEditAccountViewModel : AddEditAccountViewModel
    {
		private IRepository<Account, Guid> _accountRepository;
        private ILoggingService _logService;

		public UniversalAddEditAccountViewModel(IRepository<Account, Guid> accountRepository, INavigationService navService, ILoggingService logService)
			: base(accountRepository, navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
            if (logService == null) throw new ArgumentNullException("logService");
            _logService = logService;
		}

		public override async Task LoadData(Guid accountId)
		{
			await base.LoadData(accountId);
			await GetAvailableImages();
			if (!accountId.Equals(Guid.Empty))
			{
				var acct = await _accountRepository.GetEntryAsync(accountId);
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

		private async Task GetAvailableImages()
		{
            try
            {
                this.AvailableImages.Clear();

                var installedLocation = Package.Current.InstalledLocation;

                var iconFolder = await installedLocation.GetFolderAsync("Assets\\AccountIcons");

                var iconFiles = await iconFolder.GetFilesAsync();

                foreach(var file in iconFiles)
                {
                    using (var stream = await file.OpenStreamForReadAsync())
                    {
                        var data = new byte[stream.Length];
                        using (var ms = new MemoryStream(data))
                        {
                            stream.CopyTo(ms);
                        }

                        var img = new ImageData() { Name = file.Name, Data = data };
                        this.AvailableImages.Add(img);
                    }                    
                }
            }
            catch(Exception ex)
            {
                _logService.LogMessage("Error loading available account images: " + ex.Message);
            }
		}    
    }
}
