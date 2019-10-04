using System;
using System.Collections.Generic;
using System.Text;

namespace AltSourceCore
{
    public class Transaction
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccurredAt { get; set; }
        public int? RelatedID { get; set; }
    }

    public enum TransactionType
    {
        Debit,
        Credit
    }
}
