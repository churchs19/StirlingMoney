using Shane.Church.StirlingMoney.Core.Data;
using System;
using System.Data.Linq.Mapping;

namespace Shane.Church.StirlingMoney.Data.v3
{
    [Table]
    public class Budget : ChangeTrackingObject
    {
        private Guid _budgetId;
        [Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid BudgetId
        {
            get { return _budgetId; }
            set
            {
                Set(() => BudgetId, ref _budgetId, value);
            }
        }

        private string _budgetName;
        [Column(CanBeNull = false)]
        public string BudgetName
        {
            get { return _budgetName; }
            set
            {
                Set(() => BudgetName, ref _budgetName, value);
            }
        }

        private double _budgetAmount;
        [Column(CanBeNull = false)]
        public double BudgetAmount
        {
            get { return _budgetAmount; }
            set
            {
                Set(() => BudgetAmount, ref _budgetAmount, value);
            }
        }

        private Guid? _categoryId;
        [Column]
        public Guid? CategoryId
        {
            get { return _categoryId; }
            set
            {
                Set(() => CategoryId, ref _categoryId, value);
            }
        }

        private PeriodType _budgetPeriod;
        [Column(CanBeNull = false, DbType = "INT NOT NULL")]
        public PeriodType BudgetPeriod
        {
            get { return _budgetPeriod; }
            set
            {
                Set(() => BudgetPeriod, ref _budgetPeriod, value);
            }
        }

        private DateTime _startDate;
        [Column(CanBeNull = false)]
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                Set(() => StartDate, ref _startDate, value);
            }
        }

        private DateTime? _endDate;
        [Column(CanBeNull = true)]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                Set(() => EndDate, ref _endDate, value);
            }
        }
    }
}
