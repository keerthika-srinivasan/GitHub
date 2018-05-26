using common.Model.RuleEngine;
using CoreEngine.RulesChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine
{
    public class RuleExecutor
    {
        public decimal RunRules(string TransactionId)
        {
            Repository.SelectOperation selectRepo = new Repository.SelectOperation();
            var transDetails = selectRepo.LoadTransaction(TransactionId);
            
            //Distance Scoring
            DistanceFinder disFinder = new DistanceFinder(transDetails);
            var res = disFinder.Validate();

            IPAddressChecker ipValidator = new IPAddressChecker(transDetails);
            var ipRes = ipValidator.Validate();
            
            HistoryChecker his = new HistoryChecker(transDetails);
            var hisRes = his.Validate();

            var totScore= res + ipRes + hisRes;

            Repository.InsertOperation insRepo = new Repository.InsertOperation();
            insRepo.SetFraudValidation(TransactionId, totScore, (totScore > 1.1M));


            return totScore;
        }
    }
}
