using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Shane.Church.StirlingMoney.WP.ViewModels
{
	public class PhoneAddEditAccountViewModel : AddEditAccountViewModel
	{
		private IRepository<Account, Guid> _accountRepository;

		public PhoneAddEditAccountViewModel(IRepository<Account, Guid> accountRepository, INavigationService navService)
			: base(accountRepository, navService)
		{
			if (accountRepository == null) throw new ArgumentNullException("accountRepository");
			_accountRepository = accountRepository;
		}

		public override async Task LoadData(Guid accountId)
		{
			await base.LoadData(accountId);
			GetAvailableImages();
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

		private void GetAvailableImages()
		{
			var names = new string[] 
			{
				"Accounts-Book.png",
				"Aeroplane.png",
				"American Express.png",
				"ATM.png",
				"Bank.png",
				"Baseball-Bat.png",
				"Basket.png",
				"Basketball.png",
				"Bid-01.png",
				"Bid-02.png",
				"Bike-Loan.png",
				"Bill.png",
				"Book-Open.png",
				"Caduceus.png",
				"Car-Loan.png",
				"Cash.png",
				"Cat.png",
				"Charity.png",
				"Cheque.png",
				"Children.png",
				"CitiBank.png",
				"Counting-Machine.png",
				"Debit-Card.png",
				"Dice.png",
				"Discover.png",
				"Dog.png",
				"Dollar-Tag.png",
				"Euro-Tag.png",
				"Football-01.png",
				"Fund.png",
				"Gift.png",
				"Golf-Ball.png",
				"Graduate-02.png",
				"Home-Loan.png",
				"HSBC.png",
				"Jewel-Loan.png",
				"Market.png",
				"Master-Card.png",
				"Money-Bag.png",
				"Money.png",
				"Paid-Invoice.png",
				"Payment-01.png",
				"Payment-02.png",
				"Paypal.png",
				"Pending-Invoice.png",
				"Piggy Bank.png",
				"RBS.png",
				"Sales-Order.png",
				"Savings-01.png",
				"Savings-02.png",
				"Settled credit record.png",
				"Shopping-Bag.png",
				"Soccer.png",
				"Ticket.png",
				"Travel-Luggage.png",
				"University.png",
				"Visa.png"
			};
			StreamResourceInfo sri = Application.GetResourceStream(new Uri("Images/AccountIcons.zip", UriKind.Relative));
			if (sri != null)
			{
				foreach (var name in names)
				{
					var resourceStream = Application.GetResourceStream(sri, new Uri(name, UriKind.Relative));
					if (resourceStream != null)
					{
						var data = new byte[resourceStream.Stream.Length];
						using (var ms = new MemoryStream(data))
						{
							resourceStream.Stream.CopyTo(ms);
						}
						AvailableImages.Add(new ImageData() { Name = name, Data = data });
					}
				}
			}
		}
	}
}
