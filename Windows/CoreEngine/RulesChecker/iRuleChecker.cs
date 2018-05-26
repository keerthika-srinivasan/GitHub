using common.Model.RuleEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.RulesChecker
{
  public  interface iRuleChecker
    {
        decimal Validate();
    }
}
