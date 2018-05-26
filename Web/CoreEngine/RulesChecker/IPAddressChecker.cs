using common.Model.RuleEngine;
using CoreEngine.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.RulesChecker
{
    public class IPAddressChecker : iRuleChecker
    {
        public IPAddressChecker(string ipAddress)
        {
            this.ipAddress = ipAddress;
        }

        public IPAddressChecker(TransactionResponse transDetails)
        {
            this.ipAddress = transDetails.IpAddress;
        }
        public string ipAddress { get; set; }

        public decimal Validate()
        {
            SelectOperation SoP = new SelectOperation();
            var count = SoP.FindFraudIpExists(ipAddress);
            if (count > 0)
            {
                if (count > 3)
                    return 1; 
                if (count > 2)
                    return 0.9M;
                if (count > 1)
                    return 0.8M;
            }


            var maxMindAPI = "https://www.maxmind.com/geoip/v2.1/city/{0}?use-downloadable-db=1&demo=1";

            var http = new HttpClient();
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var url = String.Format(maxMindAPI, ipAddress);
            try
            {
                var response = http.GetAsync(url).Result;
                //HttpResponseMessage res1 = http.GetAsync("https://ebanking.bankofmaldives.com.mv/xe/").Result;
                var result = response.Content.ReadAsStringAsync().Result;
                IpAddressValidatorResponse obj = JsonConvert.DeserializeObject<IpAddressValidatorResponse>(result);

                if (obj != null)
                {
                    //Error Response or Not India
                    if (!string.IsNullOrEmpty(obj.error) || (obj.country != null && !obj.country.iso_code.Equals("IN")))                        
                    {
                        InsertOperation op = new InsertOperation();
                        op.InsertFraudRecord(ipAddress);

                        //new Record
                        return 0.5M;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                //Write Exception
            }
            return 0.1M;
        }
    }
}
