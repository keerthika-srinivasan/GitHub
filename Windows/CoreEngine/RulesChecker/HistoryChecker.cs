using common.Model.RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.RulesChecker
{
    public class HistoryChecker : iRuleChecker
    {
        public HistoryChecker(TransactionResponse transDetails)
        {
            Request = transDetails;

        }
        public TransactionResponse Request = new TransactionResponse();


        public decimal Validate()
        {
            //To validate against last 5 minutes records only
            var filterRecordsTimeDiff = DateTime.Now.AddMinutes(-5);
            Repository.SelectOperation selectRepo = new Repository.SelectOperation();
            var data = selectRepo.SearchTransactionOnFraud(Request.CardNo, Request.Transactionid, filterRecordsTimeDiff, Request.IpAddress);

            decimal score = 0;
            if (data != null)
            {
                var transAmount = Request.Amount;

                score += validateAvgMaxTransAmount(transAmount, data);


                #region To Check No Of Transaction on Card in Time Interval
                if (data.MaxTransCount > 10)
                {
                    score += 1;
                }
                else if (data.MaxTransCount > 5)
                {
                    score += 0.5M;
                }
                else if (data.MaxTransCount > 3)
                {
                    score += 0.2M;
                }
                else
                    score += 0.1M;
                #endregion

                #region To Check Transactions on same Ip address for time Interval
                if (data.IpLevelTransactions != null && data.IpLevelTransactions.Any(f =>
                         f.IpAddress == Request.IpAddress))
                {
                    var ipTransCount = data.IpLevelTransactions.FirstOrDefault(f =>
                      f.IpAddress == Request.IpAddress).Count;

                    if (ipTransCount > 10)
                    {
                        score += 1;
                    }
                    else if (ipTransCount > 5)
                    {
                        score += 0.5M;
                    }
                    else if (ipTransCount > 3)
                    {
                        score += 0.2M;
                    }
                    else
                        score += 0.1M;
                }
                #endregion

                #region To Check Any Frad Trans Went on the IP
                if (data.IpFradTransactions != null && data.IpFradTransactions.Any())
                {
                    if (data.IpFradTransactions.Count > 5)
                        score += 1;
                    else if (data.IpFradTransactions.Count > 1)
                        score += 0.7M;
                }
                #endregion
            }

            return score;
        }


        /// <summary>
        /// To Check the Average & max Transaction Amount with history
        /// </summary>
        /// <param name="transAmount"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private decimal validateAvgMaxTransAmount(decimal transAmount, SearchTransactionOnFraudResponse data)
        {
            int score = 0;

            if (data.AverageTransAmount != 0 && transAmount <= data.AverageTransAmount)
            {
                score += 2;
            }
            else if (data.AverageTransAmount != 0 && transAmount > data.AverageTransAmount)
            {
                score += 10;
            }
            else if (data.AverageTransAmount == 0 && data.MaxTransAmount == 0)
            {
                score += 5;
            }

            return score / 10;
        }
    }
}
