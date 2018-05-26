namespace common.Model.RuleEngine
{
    public class IpAddressValidatorResponse
    {
        public string code { get; set; }
        public string error { get; set; }
       public Country country { get; set; }
       
    }
  
    public class Country
    {
        public string iso_code { get; set; }
      
    } 

}