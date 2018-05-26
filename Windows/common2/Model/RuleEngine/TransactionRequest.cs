using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Model.RuleEngine
{
    public class TransactionRequest
    {
        public string Transactionid { get; set; }
        public string CardNo { get; set; }
        public string IpAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDT { get; set; }


    }

    public class TransactionResponse : TransactionRequest
    {
        
        public bool? IsFradulant { get; set; }
        public bool? isFalsePositive { get; set; }
        public DateTime? FalsePositiveChangeDT { get; set; }
        public decimal FraudScore { get; set; }

        public TransactionResponse ImmediateLastRes { get; set; }

    }
}