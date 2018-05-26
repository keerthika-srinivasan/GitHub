using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
   public class ProcessRecordRequest
    {
      public  ProcessRecordRequest()
        {

        }
        public string TransactionId { get; set; }
        public string IsFraud { get; set; }
    }
}
