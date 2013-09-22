using GalaSoft.MvvmLight;
using Shane.Church.StirlingMoney.Core.Data;
using System;
using System.Linq;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
	public class TransactionListItemViewModel : ObservableObject
	{
		private IRepository<Transaction> _transactionRepository;

		public TransactionListItemViewModel(IRepository<Transaction> transactionRepository)
		{
			if (transactionRepository == null) throw new ArgumentNullException("transactionRepository");
			_transactionRepository = transactionRepository;
		}

		private Guid _transactionId;
		public Guid TransactionId
		{
			get { return _transactionId; }
			set
			{
				Set(() => TransactionId, ref _transactionId, value);
			}
		}

		private string _location;
		public string Location
		{
			get { return _location; }
			set
			{
				Set(() => Location, ref _location, value);
			}
		}

		private double _amount;
		public double Amount
		{
			get { return _amount; }
			set
			{
				Set(() => Amount, ref _amount, value);
			}
		}

		private long? _checkNumber;
		public long? CheckNumber
		{
			get { return _checkNumber; }
			set
			{
				Set(() => CheckNumber, ref _checkNumber, value);
			}
		}

		private DateTime _transactionDate;
		public DateTime TransactionDate
		{
			get { return _transactionDate; }
			set
			{
				Set(() => TransactionDate, ref _transactionDate, value);
			}
		}

		private bool _posted;
		public bool Posted
		{
			get { return _posted; }
			set
			{
				if (Set(() => Posted, ref _posted, value))
				{
					var t = _transactionRepository.GetFilteredEntries(it => it.TransactionId == TransactionId).FirstOrDefault();
					if (t != null)
					{
						t.Posted = Posted;
						_transactionRepository.AddOrUpdateEntry(t);
					}
				}
			}
		}

		private string _memo;
		public string Memo
		{
			get { return _memo; }
			set
			{
				Set(() => Memo, ref _memo, value);
			}
		}

		private string _category;
		public string Category
		{
			get { return _category; }
			set
			{
				Set(() => Category, ref _category, value);
			}
		}

		public bool IsCheck
		{
			get
			{
				return CheckNumber.HasValue;
			}
		}

		public void LoadData(Guid transactionId)
		{
			var t = _transactionRepository.GetFilteredEntries(it => it.TransactionId == transactionId).FirstOrDefault();
			if (t != null)
			{
				TransactionId = transactionId;
				TransactionDate = t.TransactionDate;
				Location = t.Location;
				Amount = t.Amount;
				CheckNumber = t.CheckNumber;
				_posted = t.Posted;
				Memo = t.Note;
				Category = t.Category != null ? t.Category.CategoryName : null;
			}
		}

		public void LoadData(Transaction transaction)
		{
			TransactionId = transaction.TransactionId;
			TransactionDate = transaction.TransactionDate;
			Location = transaction.Location;
			Amount = transaction.Amount;
			CheckNumber = transaction.CheckNumber;
			_posted = transaction.Posted;
			Memo = transaction.Note;
			Category = transaction.Category != null ? transaction.Category.CategoryName : null;
		}
	}
}
