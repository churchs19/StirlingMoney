using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Data;
using System;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class AccountTileViewModel : ObservableObject
	{
		public AccountTileViewModel()
		{

		}

		public AccountTileViewModel(Account a)
		{
			AccountId = a.AccountId;
			AccountName = a.AccountName;
			AccountBalance = a.AccountBalance;
			PostedBalance = a.PostedBalance;
		}

		private string _accountName;
		public string AccountName
		{
			get
			{
				return _accountName;
			}
			set
			{
				Set(() => AccountName, ref _accountName, value);
			}
		}

		private double _accountBalance;
		public double AccountBalance
		{
			get
			{
				return _accountBalance;
			}
			set
			{
				Set(() => AccountBalance, ref _accountBalance, value);
			}
		}

		private double _postedBalance;
		public double PostedBalance
		{
			get { return _postedBalance; }
			set
			{
				Set(() => PostedBalance, ref _postedBalance, value);
			}
		}

		private Guid _accountId;
		public Guid AccountId
		{
			get
			{
				return _accountId;
			}
			set
			{
				Set(() => AccountId, ref _accountId, value);
			}
		}

		public string PinMenuText
		{
			get
			{
				//if (!TileUtility.TileExists(AccountId))
				//{
				//	return Shane.Church.StirlingMoney.ViewModels.Resources.ViewModelResources.PinToStart;
				//}
				//else
				//{
				//	return Shane.Church.StirlingMoney.ViewModels.Resources.ViewModelResources.UnpinFromStart;
				//}
				throw new NotImplementedException();
			}
		}



		public void LoadData(Guid AccountId)
		{
		}
	}
}
