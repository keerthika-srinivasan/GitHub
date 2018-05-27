using common.Model.RuleEngine;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.Repository
{
    public class SelectOperation : BaseRepository
    {

        public int FindFraudIpExists(string ipAddress)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "FindFraudIpAddress";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                cmd.Parameters["@ipAddress"].Direction = ParameterDirection.Input;

                var count = 0;
                cmd.Parameters.AddWithValue("@Count", count);
                cmd.Parameters["@Count"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                con.Close();

                return Int32.Parse(Convert.ToString(cmd.Parameters["@Count"].Value));
            }
        }

        public TransactionResponse LoadTransaction(string transactionId)
        {
            string query = "select top 1 *  FROM [Transactions] where TransactionId={0} order by id desc";
            TransactionResponse response = GetTransaction(string.Format(query, transactionId));

            var oldTransId = LoadPreviousImmediateTransasction(transactionId);

            if (string.IsNullOrEmpty(oldTransId))
                response.ImmediateLastRes = new TransactionResponse();
            else
            {
                response.ImmediateLastRes = GetTransaction(string.Format(query, oldTransId));
            }

            return response;
        }


        public string LoadPreviousImmediateTransasction(string TransactionId)
        {

            var sqlQuery = string.Format("select transactionid from Transactions where CardNo in(SELECT CardNo FROM[dbo].[Transactions] where TransactionId = {0}) and TransactionId!={0} order by id desc", TransactionId);
            SqlDataAdapter datrans = new SqlDataAdapter(sqlQuery, con);
            DataSet dsPubs = new DataSet("transaction");
            datrans.FillSchema(dsPubs, SchemaType.Source, "trans");
            datrans.Fill(dsPubs, "trans");

            DataTable tbltrans;
            tbltrans = dsPubs.Tables["trans"];

            foreach (DataRow drCurrent in tbltrans.Rows)
            {
                return drCurrent["transactionid"].ToString();
            }

            return null;
        }

        private TransactionResponse GetTransaction(string sqlQuery)
        {
            TransactionResponse response = new TransactionResponse();
            SqlDataAdapter datrans = new SqlDataAdapter(sqlQuery, con);
            DataSet dsPubs = new DataSet("transaction");
            datrans.FillSchema(dsPubs, SchemaType.Source, "trans");
            datrans.Fill(dsPubs, "trans");

            DataTable tbltrans;
            tbltrans = dsPubs.Tables["trans"];

            foreach (DataRow drCurrent in tbltrans.Rows)
            {
                response = ReadRow(drCurrent);
                break;
            }

            return response;
        }

        private TransactionResponse ReadRow(DataRow row)
        {
            TransactionResponse response = new TransactionResponse();
            response.Transactionid = row["TransactionId"].ToString();
            response.CardNo = row["CardNo"].ToString();
            response.IpAddress = row["IPAddress"].ToString();
            response.Latitude = row["Latitude"].ToString();
            response.Longitude = row["Longitude"].ToString();
            response.TransactionDT = DateTime.Parse(row["TransactionDT"].ToString());
            response.Amount = decimal.Parse(row["TransactionAmount"].ToString());

            var val = Convert.ToString(row["IsFradulant"]);
            if (!string.IsNullOrEmpty(val))
                response.IsFradulant = bool.Parse(val);

            val = Convert.ToString(row["IsFalsePositive"]);
            if (!string.IsNullOrEmpty(val))
                response.isFalsePositive = bool.Parse(val);

            val = Convert.ToString(row["FalsePositiveChangeDT"]);
            if (!string.IsNullOrEmpty(val))
                response.FalsePositiveChangeDT = DateTime.Parse(val);

            return response;
        }

        public SearchTransactionOnFraudResponse SearchTransactionOnFraud(string cardNo, string transactionId, DateTime FilteringTimeOnwards, string currentIpAddress)
        {
            SearchTransactionOnFraudResponse response = new SearchTransactionOnFraudResponse();
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "SearchTransactionOnFraud";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();

                cmd.Parameters.AddWithValue("@cardNo", cardNo).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@transactionId", transactionId).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@TransDateFilterFrom", FilteringTimeOnwards).Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@IpAddress", currentIpAddress).Direction = ParameterDirection.Input;

                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();

                var tab = ds.Tables[0];
                if (tab != null)
                {
                    var avg = Convert.ToString(tab.Rows[0]["Avg"]);
                    if (!string.IsNullOrEmpty(avg))
                        response.AverageTransAmount = decimal.Parse(avg);

                    var max = Convert.ToString(tab.Rows[0]["max"]);
                    if (!string.IsNullOrEmpty(max))
                        response.MaxTransAmount = decimal.Parse(max);
                }
                tab = ds.Tables[1];
                if (tab != null)
                {
                    var cnt = Convert.ToString(tab.Rows[0]["cnt"]);
                    if (!string.IsNullOrEmpty(cnt))
                        response.MaxTransCount = decimal.Parse(cnt);
                }

                tab = ds.Tables[2];
                if (tab != null)
                {
                    foreach (DataRow existingRow in tab.Rows)
                    {
                        var record = new Records();
                        record.IpAddress = Convert.ToString(existingRow["IPAddress"]);
                        record.Count = int.Parse(Convert.ToString(existingRow["cnt"]));
                        response.IpLevelTransactions.Add(record);
                    }
                }

                tab = ds.Tables[3];
                if (tab != null)
                {
                    foreach (DataRow existingRow in tab.Rows)
                    {
                        response.IpFradTransactions.Add(ReadRow(existingRow));
                    }
                }
            }

            return response;
        }

        public List<DashBoardResponse> GetPendingTransactions(string Id)
        {

            List<DashBoardResponse> response = new List<DashBoardResponse>();
            using (SqlCommand cmd = new SqlCommand())
            {
                con.Open();
                cmd.Connection = con;

                cmd.CommandText = "GetPendingFradulentTransaction";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();

                cmd.Parameters.AddWithValue("@ID", Id).Direction = ParameterDirection.Input;

                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();

                var tab = ds.Tables[0];
                if (tab != null)
                {
                    foreach (DataRow existingRow in tab.Rows)
                    {
                        DashBoardResponse model = new DashBoardResponse();
                        model.TransactionId = Convert.ToString(existingRow["TransactionId"]);
                        model.UniqueId = Convert.ToString(existingRow["id"]);
                        model.TransDt = Convert.ToString(existingRow["TransactionDT"]);
                        model.CardNumber = Convert.ToString(existingRow["CardNo"]);
                        response.Add(model);
                    }
                }

                return response;
            }

        }
    }
}
