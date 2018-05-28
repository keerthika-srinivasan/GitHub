using common.Model.RuleEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.Repository
{
  public  class InsertOperation: BaseRepository
    {       
        public InsertOperation()
            :base()
        {

        }

        public void InsertFraudRecord(string ipAddress)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "InsertFraudIpAddress";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                cmd.Parameters["@ipAddress"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
        }

        public void InsertFreshTransaction(TransactionRequest request)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "InsertTransaction";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TransactionId", request.Transactionid).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@CardNo", request.CardNo).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@IPAddress", request.IpAddress).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@Latitude", request.Latitude).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@Longitude", request.Longitude).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@TransactionAmount", request.Amount).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@TransactionDt", request.TransactionDT).Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
        }

        public bool UpdateFraudStatus(string transactionid, bool status)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "UpdateFraudStatusFromUI";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@transactionId", transactionid);
                cmd.Parameters["@transactionId"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@status", status ? "1" : "0");
                cmd.Parameters["@status"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                con.Close();
            }

            return true;
        }

        public bool SetFraudValidation(string transactionid, decimal score,bool isFraud)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "UpdateFraudStatus";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@transactionId", transactionid);
                cmd.Parameters["@transactionId"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@score", score);
                cmd.Parameters["@score"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@isFrad", isFraud ? "1" : "0");
                cmd.Parameters["@isFrad"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                con.Close();
            }

            return true;
        }
    }
}
