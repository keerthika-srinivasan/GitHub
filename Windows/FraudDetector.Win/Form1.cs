using common.Model;
using common.Model.RuleEngine;
using CoreEngine;
using CoreEngine.Repository;
using CoreEngine.RulesChecker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FraudDetector.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //TransactionId	CardNo	IPAddress	Latitude	Longitude	TransactionDT	TransactionAmount
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            var file = openFileDialog1.FileName;

            var lines = File.ReadLines(file);
            List<TransactionRequest> input = new List<TransactionRequest>();
            //To Avoid Header
            var _transIdToAppend = DateTime.Now.ToString("ddMMyyHHMMss");
            foreach (var item in lines.Skip(1))
            {
                TransactionRequest request = new TransactionRequest();
                var split = item.Split(',');
                request.Transactionid = string.Format("{0}{1}", _transIdToAppend, split[0]);
                request.CardNo = split[1];
                request.IpAddress = split[2];
                request.Latitude = split[3];
                request.Longitude = split[4];
                request.TransactionDT = DateTime.Parse(split[5]);
                request.Amount = decimal.Parse(split[6]);
                input.Add(request);

            }

            var maxDt = input.Max(d => d.TransactionDT);
            var timeDiff = (DateTime.Now - maxDt).TotalSeconds;

          
            input.ForEach(f =>
            {
                f.TransactionDT = f.TransactionDT.AddSeconds(timeDiff);
                ProcessTransaction(f);
            }
            );
            
            MessageBox.Show("Upload Completed");
        }

        /// <summary>
        /// To Post Individual Transaction to Processing Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            TransactionRequest request = new TransactionRequest()
            {
                Transactionid = txttransid.Text,
                Amount = Convert.ToDecimal(string.IsNullOrEmpty(txtamount.Text) ? "1" : txtamount.Text),
                CardNo = txtcardno.Text,
                IpAddress = txtipaddress.Text,
                Latitude = txtlatitude.Text,
                Longitude = txtlongitude.Text,
                TransactionDT = DateTime.Now
            };

            txtResponse.Text = "Fraud Transaction Risk score Out of 6 is" + ProcessTransaction(request);
        }

      private decimal ProcessTransaction(TransactionRequest request)
        {
            //To insert fresh Record
            InsertOperation repo = new InsertOperation();
            repo.InsertFreshTransaction(request);

            RuleExecutor rule = new RuleExecutor();

            var score = rule.RunRules(request.Transactionid);

            return score;
        }
    }
}
