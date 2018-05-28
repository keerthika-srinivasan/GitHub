using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Model.RuleEngine
{
    public class SearchTransactionOnFraudResponse
    {
        public SearchTransactionOnFraudResponse()
        {
            IpLevelTransactions = new List<Records>();
            IpFradTransactions = new List<TransactionResponse>();
        }
        public decimal AverageTransAmount { get; set; }
        public decimal MaxTransAmount { get; set; }
        public decimal MaxTransCount { get; set; }
        public List<Records> IpLevelTransactions { get; set; }
        public List<TransactionResponse> IpFradTransactions { get; set; }
     
    }

    public class Records
    {
        public string IpAddress { get; set; }
        public DateTime LastDetected { get; set; }
        public int Count { get; set; }
    }



}
