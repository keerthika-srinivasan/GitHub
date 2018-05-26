using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.Repository
{
    public class BaseRepository
    {
        public BaseRepository()
        {
            con = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=FraudDetector;Integrated Security=true;");
        }
        protected SqlConnection con { get; set; }
        protected SqlDataAdapter adapt { get; set; }
        //ID variable used in Updating and Deleting Record  
        private int ID = 0;
    }
}
