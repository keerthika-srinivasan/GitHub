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
            con = new SqlConnection("Data Source=.;Initial Catalog=FraudDetector;Integrated Security=true;");
        }
        protected SqlConnection con { get; set; }
    }
}
