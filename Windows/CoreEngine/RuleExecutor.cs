using common.Model.RuleEngine;
using CoreEngine.RulesChecker;
using System;
using System.Linq;

namespace CoreEngine
{
    public class RuleExecutor
    {
        public decimal RunRules(string TransactionId)
        {
            Repository.SelectOperation selectRepo = new Repository.SelectOperation();
            var transDetails = selectRepo.LoadTransaction(TransactionId);

            var totScore = 0M;

            #region Execute Rule Engine On Request
            Type type1 = typeof(iRuleChecker);
            var lookupTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type1.IsAssignableFrom(p) && !p.IsInterface).ToList();
            

            foreach (var item in lookupTypes)
            {
                Type[] consPara = new Type[] { typeof(TransactionResponse) };
                var emptyConstructor = item.GetConstructor(consPara);
                var newStringCustomer = (iRuleChecker)emptyConstructor.Invoke(new object[] { transDetails });

                totScore += newStringCustomer.Validate();
            }

            #endregion

            Repository.InsertOperation insRepo = new Repository.InsertOperation();
            insRepo.SetFraudValidation(TransactionId, totScore, (totScore > 1.1M));


            return totScore;
        }
    }
}
