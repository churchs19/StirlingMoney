using GalaSoft.MvvmLight;
using Grace;
using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Repositories;
using Shane.Church.StirlingMoney.Core.Services;
using Shane.Church.StirlingMoney.Core.ViewModels.Shared;
using Shane.Church.StirlingMoney.Strings;
using Shane.Church.Utility.Core.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Shane.Church.StirlingMoney.Core.ViewModels
{
    public class AddEditTransactionViewModel : ObservableObject
    {
        private TransactionType _transactionType = TransactionType.Unknown;
        private IDataRepository<Account, Guid> _accountRepository;
        private IDataRepository<Transaction, Guid> _transactionRepository;
        private IDataRepository<Category, Guid> _categoryRepository;
        private INavigationService _navService;

        public AddEditTransactionViewModel(IDataRepository<Account, Guid> accountRepo, IDataRepository<Transaction, Guid> transactionRepo, IDataRepository<Category, Guid> categoryRepo, INavigationService navService)
        {
            if (accountRepo == null) throw new ArgumentNullException("accountRepo");
            _accountRepository = accountRepo;
            if (transactionRepo == null) throw new ArgumentNullException("transactionRepo");
            _transactionRepository = transactionRepo;
            if (categoryRepo == null) throw new ArgumentNullException("categoryRepo");
            _categoryRepository = categoryRepo;
            if (navService == null) throw new ArgumentNullException("navService");
            _navService = navService;

            _categories = new ObservableCollection<string>();
            _categories.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(() => Categories);
            };
            _transferAccounts = new ObservableCollection<string>();
            _transferAccounts.CollectionChanged += (s, e) =>
            {
                RaisePropertyChanged(() => TransferAccounts);
            };
            SaveCommand = new AsyncRelayCommand(o => SaveTransaction());
            Amount = 0;
        }

        public AddEditTransactionViewModel()
            : this(ContainerService.Container.Locate<IDataRepository<Account, Guid>>(),
                ContainerService.Container.Locate<IDataRepository<Transaction, Guid>>(),
                ContainerService.Container.Locate<IDataRepository<Category, Guid>>(),
                ContainerService.Container.Locate<INavigationService>())
        {
        }

        private bool _isDeleted;

        private Guid _transactionId;
        public Guid TransactionId
        {
            get { return _transactionId; }
            set
            {
                Set(() => TransactionId, ref _transactionId, value);
            }
        }

        private Guid _accountId;
        public Guid AccountId
        {
            get { return _accountId; }
            set
            {
                Set(() => AccountId, ref _accountId, value);
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

        private double _amount;
        public double Amount
        {
            get { return _amount; }
            set
            {
                Set(() => Amount, ref _amount, value);
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

        private string _note;
        public string Note
        {
            get { return _note; }
            set
            {
                Set(() => Note, ref _note, value);
            }
        }

        private bool _posted;
        public bool Posted
        {
            get { return _posted; }
            set
            {
                Set(() => Posted, ref _posted, value);
            }
        }

        private long _checkNumber;
        public long CheckNumber
        {
            get { return _checkNumber; }
            set
            {
                Set(() => CheckNumber, ref _checkNumber, value);
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

        private ObservableCollection<string> _categories;
        public ObservableCollection<string> Categories
        {
            get { return _categories; }
        }

        private string _transferAccount;
        public string TransferAccount
        {
            get { return _transferAccount; }
            set
            {
                Set(() => TransferAccount, ref _transferAccount, value);
            }
        }

        private ObservableCollection<string> _transferAccounts;
        public ObservableCollection<string> TransferAccounts
        {
            get { return _transferAccounts; }
        }

        public bool IsCheck
        {
            get { return _transactionType == TransactionType.Check; }
        }

        public bool IsTransfer
        {
            get { return _transactionType == TransactionType.Transfer; }
        }

        public bool IsDeposit
        {
            get { return _transactionType == TransactionType.Deposit; }
        }

        public string TitleText
        {
            get
            {
                switch (_transactionType)
                {
                    case TransactionType.Check:
                        return Resources.CheckTitle;
                    case TransactionType.Deposit:
                        return Resources.DepositTitle;
                    case TransactionType.Transfer:
                        return Resources.TransferTitle;
                    case TransactionType.Withdrawal:
                        return Resources.WithdrawalTitle;
                    default:
                        return Resources.UnknownTitle;
                }
            }
        }

        public bool IsLocationReadOnly
        {
            get
            {
                return (_transactionType == TransactionType.Transfer && TransactionId != Guid.Empty);
            }
        }

        public bool AccountVisible
        {
            get
            {
                return _transactionType == TransactionType.Transfer && TransactionId == Guid.Empty;
            }
        }

        public bool LocationVisible
        {
            get
            {
                return !AccountVisible;
            }
        }

        public bool CategoryVisible
        {
            get
            {
                return _transactionType != TransactionType.Transfer;
            }
        }

        public async Task LoadData(AddEditTransactionParams param)
        {
            this._transactionType = param.Type;
            AccountId = param.AccountId;

            if (param.TransactionId == Guid.Empty)
            {
                TransactionDate = DateTime.Today;
                if (_transactionType == TransactionType.Check)
                {
                    //var checks = _transactionRepository.GetAllIndexKeys<Tuple<Guid, long>>("TransactionAccountIdCheckNumber").Where(it => it.Value.Item1 == AccountId).Select(it => it.Value.Item2);
                    var checks = _transactionRepository.GetFilteredEntries(it => it.AccountId == AccountId && it.CheckNumber > 0).Select(it => it.CheckNumber);
                    if (checks.Any())
                    {
                        CheckNumber = checks.Max() + 1;
                    }
                    else
                    {
                        CheckNumber = 1;
                    }
                }
                else
                {
                    CheckNumber = 0;
                }
            }
            else
            {
                Transaction t = await _transactionRepository.GetEntryAsync(param.TransactionId);
                if (t != null)
                {
                    _isDeleted = t.IsDeleted;
                    TransactionId = t.TransactionId;
                    TransactionDate = DateTime.SpecifyKind(t.TransactionDate.Date, DateTimeKind.Utc);
                    Amount = t.Amount;
                    Location = t.Location;
                    Note = t.Note;
                    Posted = t.Posted;
                    if (!t.CategoryId.Equals(Guid.Empty))
                    {
                        var cat = await _categoryRepository.GetEntryAsync(t.CategoryId);
                        Category = cat.CategoryName;
                    }
                    else
                    {
                        Category = "";
                    }

                    CheckNumber = t.CheckNumber;
                }
            }
            if (param.Type == TransactionType.Unknown)
            {
                Transaction t = await _transactionRepository.GetEntryAsync(param.TransactionId);
                if (t.CheckNumber > 0)
                    _transactionType = TransactionType.Check;
                else if (t.Location != null && (t.Location.Contains(Resources.TransferFromComparisonString) || t.Location.Contains(Resources.TransferToComparisonString)))
                    _transactionType = TransactionType.Transfer;
                else if (t.Amount >= 0)
                    _transactionType = TransactionType.Deposit;
                else
                    _transactionType = TransactionType.Withdrawal;
            }
            if (_transactionType == TransactionType.Check || _transactionType == TransactionType.Withdrawal)
                Amount = -Amount;

            Categories.Clear();
            var catQuery = _categoryRepository.GetAllEntries().Select(it => it.CategoryName);
            foreach (var c in catQuery)
                Categories.Add(c);

            TransferAccounts.Clear();
            var acctQuery = _accountRepository.GetAllEntries().Select(it => it.AccountName);
            foreach (var a in acctQuery)
                TransferAccounts.Add(a);
        }

        public IList<string> Validate()
        {
            List<string> validationErrors = new List<string>();

            if (_transactionType == TransactionType.Transfer)
            {
                if (TransactionId == Guid.Empty)
                {
                    if (string.IsNullOrWhiteSpace(TransferAccount))
                        validationErrors.Add(Resources.AccountRequiredError);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Location))
                    validationErrors.Add(Resources.LocationRequiredError);
            }

            return validationErrors;
        }

        public ICommand SaveCommand { get; private set; }

        public delegate void ValidationFailedHandler(object sender, ValidationFailedEventArgs args);
        public event ValidationFailedHandler ValidationFailed;

        public async Task SaveTransaction()
        {
            var errors = Validate();
            if (errors.Count == 0)
            {
                Transaction transaction = new Transaction();
                transaction.IsDeleted = _isDeleted;
                transaction.TransactionId = TransactionId;
                transaction.TransactionDate = new DateTimeOffset(DateTime.SpecifyKind(TransactionDate, DateTimeKind.Utc));
                transaction.Note = Note;
                transaction.Posted = Posted;
                transaction.AccountId = AccountId;
                Guid categoryId = Guid.Empty;
                if (!string.IsNullOrEmpty(Category))
                {
                    var catQuery = await _categoryRepository.GetFilteredEntriesAsync(it => it.CategoryName == Category);
                    categoryId = catQuery.Select(it => it.CategoryId).FirstOrDefault();
                    if (categoryId.Equals(Guid.Empty))
                    {
                        var cat = await _categoryRepository.AddOrUpdateEntryAsync(new Category() { CategoryName = Category });
                        categoryId = cat.CategoryId;
                    }
                }
                else
                    categoryId = Guid.Empty;
                switch (_transactionType)
                {
                    case TransactionType.Check:
                        transaction.Location = Location;
                        transaction.CategoryId = categoryId;
                        transaction.CheckNumber = CheckNumber;
                        transaction.Amount = -Amount;
                        break;
                    case TransactionType.Deposit:
                        transaction.Location = Location;
                        transaction.CategoryId = categoryId;
                        transaction.Amount = Amount;
                        break;
                    case TransactionType.Transfer:
                        if (TransactionId != Guid.Empty)
                        {
                            //Editing an existing transfer
                            if (Location.Contains(Resources.TransferFromComparisonString))
                            {
                                transaction.Amount = Math.Abs(Amount);
                            }
                            else
                            {
                                transaction.Amount = -Math.Abs(Amount);
                            }
                        }
                        else
                        {
                            //New transfer
                            transaction.Amount = -Amount;
                            transaction.Location = string.Format(Resources.TransferToLocation, TransferAccount);

                            if (TransactionId == Guid.Empty)
                            {
                                transaction = await _transactionRepository.AddOrUpdateEntryAsync(transaction);
                            }

                            Transaction destTransaction = new Transaction();
                            destTransaction.TransactionDate = new DateTimeOffset(DateTime.SpecifyKind(TransactionDate, DateTimeKind.Utc));
                            destTransaction.Note = Note;
                            destTransaction.Posted = Posted;
                            var transAcctQuery = await _accountRepository.GetFilteredEntriesAsync(it => it.AccountName == this.TransferAccount);
                            destTransaction.AccountId = transAcctQuery.Select(it => it.AccountId).FirstOrDefault();
                            destTransaction.Amount = Amount;
                            var sourceAccount = await _accountRepository.GetEntryAsync(transaction.AccountId);

                            destTransaction.Location = string.Format(Resources.TransferFromLocation, sourceAccount != null ? sourceAccount.AccountName : "");

                            destTransaction = await _transactionRepository.AddOrUpdateEntryAsync(destTransaction);
                        }
                        break;
                    case TransactionType.Withdrawal:
                        transaction.Location = Location;
                        transaction.CategoryId = categoryId;
                        transaction.Amount = -Amount;
                        break;
                    default:
                        break;
                }

                if (_transactionType != TransactionType.Transfer)
                {
                    var t = await _transactionRepository.AddOrUpdateEntryAsync(transaction);
                    TransactionId = t.TransactionId;
                    _isDeleted = t.IsDeleted;
                }

                if (_navService.CanGoBack)
                    _navService.GoBack();
            }
            else
            {
                if (ValidationFailed != null)
                    ValidationFailed(this, new ValidationFailedEventArgs(errors));
            }
        }
    }
}
