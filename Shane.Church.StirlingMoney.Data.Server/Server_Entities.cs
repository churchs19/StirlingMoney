//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5456
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shane.Church.StirlingMoney.Data.Sync
{
    
    
    public class Server_OfflineEntityBase : object, Microsoft.Synchronization.Services.IOfflineEntity {
        
        private Microsoft.Synchronization.Services.OfflineEntityMetadata _serviceMetadata;
        
        public Server_OfflineEntityBase() {
            ServiceMetadata = new Microsoft.Synchronization.Services.OfflineEntityMetadata();
        }
        
        public virtual Microsoft.Synchronization.Services.OfflineEntityMetadata ServiceMetadata {
            get {
                return _serviceMetadata;
            }
            set {
                if ((value == null)) {
                    throw new System.ArgumentNullException("value");
                }
                _serviceMetadata = value;
            }
        }
    }
    
    [Microsoft.Synchronization.Services.SyncEntityTypeAttribute(TableGlobalName="Account", TableLocalName="[Account]", KeyFields="AccountId")]
    public partial class Account : Server_OfflineEntityBase {
        
        private System.Guid _AccountId;
        
        private string _AccountName;
        
        private double _InitialBalance;
        
        public System.Guid AccountId {
            get {
                return _AccountId;
            }
            set {
                _AccountId = value;
            }
        }
        
        public string AccountName {
            get {
                return _AccountName;
            }
            set {
                _AccountName = value;
            }
        }
        
        public double InitialBalance {
            get {
                return _InitialBalance;
            }
            set {
                _InitialBalance = value;
            }
        }
    }
    
    [Microsoft.Synchronization.Services.SyncEntityTypeAttribute(TableGlobalName="Category", TableLocalName="[Category]", KeyFields="CategoryId")]
    public partial class Category : Server_OfflineEntityBase {
        
        private System.Guid _CategoryId;
        
        private string _CategoryName;
        
        public System.Guid CategoryId {
            get {
                return _CategoryId;
            }
            set {
                _CategoryId = value;
            }
        }
        
        public string CategoryName {
            get {
                return _CategoryName;
            }
            set {
                _CategoryName = value;
            }
        }
    }
    
    [Microsoft.Synchronization.Services.SyncEntityTypeAttribute(TableGlobalName="Transaction", TableLocalName="[Transaction]", KeyFields="TransactionId")]
    public partial class Transaction : Server_OfflineEntityBase {
        
        private System.Guid @__accountId;
        
        private System.Guid _TransactionId;
        
        private System.DateTime _TransactionDate;
        
        private double _Amount;
        
        private string _Location;
        
        private string _Note;
        
        private bool _Posted;
        
        private System.Nullable<long> _CheckNumber;
        
        private System.Nullable<System.Guid> _CategoryId;
        
        public System.Guid _accountId {
            get {
                return @__accountId;
            }
            set {
                @__accountId = value;
            }
        }
        
        public System.Guid TransactionId {
            get {
                return _TransactionId;
            }
            set {
                _TransactionId = value;
            }
        }
        
        public System.DateTime TransactionDate {
            get {
                return _TransactionDate;
            }
            set {
                _TransactionDate = value;
            }
        }
        
        public double Amount {
            get {
                return _Amount;
            }
            set {
                _Amount = value;
            }
        }
        
        [Microsoft.Synchronization.Services.SyncEntityPropertyIsNullableAttribute()]
        public string Location {
            get {
                return _Location;
            }
            set {
                _Location = value;
            }
        }
        
        [Microsoft.Synchronization.Services.SyncEntityPropertyIsNullableAttribute()]
        public string Note {
            get {
                return _Note;
            }
            set {
                _Note = value;
            }
        }
        
        public bool Posted {
            get {
                return _Posted;
            }
            set {
                _Posted = value;
            }
        }
        
        [Microsoft.Synchronization.Services.SyncEntityPropertyIsNullableAttribute()]
        public System.Nullable<long> CheckNumber {
            get {
                return _CheckNumber;
            }
            set {
                _CheckNumber = value;
            }
        }
        
        [Microsoft.Synchronization.Services.SyncEntityPropertyIsNullableAttribute()]
        public System.Nullable<System.Guid> CategoryId {
            get {
                return _CategoryId;
            }
            set {
                _CategoryId = value;
            }
        }
    }
    
    [Microsoft.Synchronization.Services.SyncEntityTypeAttribute(TableGlobalName="Budget", TableLocalName="[Budget]", KeyFields="BudgetId")]
    public partial class Budget : Server_OfflineEntityBase {
        
        private System.Guid _BudgetId;
        
        private string _BudgetName;
        
        private double _BudgetAmount;
        
        private System.Nullable<System.Guid> _CategoryId;
        
        private long _BudgetPeriod;
        
        private System.DateTime _StartDate;
        
        private System.Nullable<System.DateTime> _EndDate;
        
        public System.Guid BudgetId {
            get {
                return _BudgetId;
            }
            set {
                _BudgetId = value;
            }
        }
        
        public string BudgetName {
            get {
                return _BudgetName;
            }
            set {
                _BudgetName = value;
            }
        }
        
        public double BudgetAmount {
            get {
                return _BudgetAmount;
            }
            set {
                _BudgetAmount = value;
            }
        }
        
        [Microsoft.Synchronization.Services.SyncEntityPropertyIsNullableAttribute()]
        public System.Nullable<System.Guid> CategoryId {
            get {
                return _CategoryId;
            }
            set {
                _CategoryId = value;
            }
        }
        
        public long BudgetPeriod {
            get {
                return _BudgetPeriod;
            }
            set {
                _BudgetPeriod = value;
            }
        }
        
        public System.DateTime StartDate {
            get {
                return _StartDate;
            }
            set {
                _StartDate = value;
            }
        }
        
        [Microsoft.Synchronization.Services.SyncEntityPropertyIsNullableAttribute()]
        public System.Nullable<System.DateTime> EndDate {
            get {
                return _EndDate;
            }
            set {
                _EndDate = value;
            }
        }
    }
    
    [Microsoft.Synchronization.Services.SyncEntityTypeAttribute(TableGlobalName="Goal", TableLocalName="[Goal]", KeyFields="GoalId")]
    public partial class Goal : Server_OfflineEntityBase {
        
        private System.Guid @__accountId;
        
        private System.Guid _GoalId;
        
        private string _GoalName;
        
        private double _Amount;

		private double _InitialBalance;

		private System.DateTime _TargetDate;
        
        public System.Guid _accountId {
            get {
                return @__accountId;
            }
            set {
                @__accountId = value;
            }
        }
        
        public System.Guid GoalId {
            get {
                return _GoalId;
            }
            set {
                _GoalId = value;
            }
        }
        
        public string GoalName {
            get {
                return _GoalName;
            }
            set {
                _GoalName = value;
            }
        }
        
        public double Amount {
            get {
                return _Amount;
            }
            set {
                _Amount = value;
            }
        }

		public double InitialBalance
		{
			get
			{
				return _InitialBalance;
			}
			set
			{
				_InitialBalance = value;
			}
		}        

        public System.DateTime TargetDate {
            get {
                return _TargetDate;
            }
            set {
                _TargetDate = value;
            }
        }
    }
    
    [Microsoft.Synchronization.Services.SyncScopeAttribute()]
    public class Server_OfflineEntities {
        
        private System.Collections.Generic.ICollection<Account> _Accounts;
        
        private System.Collections.Generic.ICollection<Category> _Categorys;
        
        private System.Collections.Generic.ICollection<Transaction> _Transactions;
        
        private System.Collections.Generic.ICollection<Budget> _Budgets;
        
        private System.Collections.Generic.ICollection<Goal> _Goals;
    }
}
