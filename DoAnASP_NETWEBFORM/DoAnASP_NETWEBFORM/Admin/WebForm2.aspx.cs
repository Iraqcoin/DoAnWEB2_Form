﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DoAnASP_NETWEBFORM.Admin
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var order = new Order(); // this is something I have defined in order to save the order in the database

            // Receive IPN request from PayPal and parse all the variables returned
            var formVals = new Dictionary<string, string>();
            formVals.Add("cmd", "_notify-synch"); //notify-synch_notify-validate
            formVals.Add("at", "this is a long token found in Buyers account"); // this has to be adjusted
            formVals.Add("tx", Request["tx"]);

            // if you want to use the PayPal sandbox change this from false to true
            string response = GetPayPalResponse(formVals, false);

            if (response.Contains("SUCCESS"))
            {
                string transactionID = GetPDTValue(response, "txn_id"); // txn_id //d
                string sAmountPaid = GetPDTValue(response, "mc_gross"); // d
                string deviceID = GetPDTValue(response, "custom"); // d
                string payerEmail = GetPDTValue(response, "payer_email"); // d
                string Item = GetPDTValue(response, "item_name");

                //validate the order
                Decimal amountPaid = 0;
                Decimal.TryParse(sAmountPaid, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out amountPaid);

                if (amountPaid == 9)  // you might want to have a bigger than or equal to sign here!
                {
                    //if (orders.Count(d => d.PayPalOrderRef == transactionID) < 1)
                    //{
                    //    //if the transactionID is not found in the database, add it
                    //    //then, add the additional features to the user account
                    //}
                    //else
                    //{
                    //    //if we are here, the user must have already used the transaction ID for an account
                    //    //you might want to show the details of the order, but do not upgrade it!
                    //}
                    // take the information returned and store this into a subscription table
                    // this is where you would update your database with the details of the tran

                    //return View();
                    string message = "Hello! Mudassar.";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<script type = 'text/javascript'>");

                    sb.Append("window.onload=function(){");

                    sb.Append("alert('");

                    sb.Append(message);

                    sb.Append("')};");

                    sb.Append("</script>");

                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());

                }
                else
                {
                    //// let fail - this is the IPN so there is no viewer
                    //// you may want to log something here
                    //order.Comments = "User did not pay the right ammount.";

                    //// since the user did not pay the right amount, we still want to log that for future reference.

                    //_db.Orders.Add(order); // order is your new Order
                    //_db.SaveChanges();
                    string message = "Hello! baobao.";

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<script type = 'text/javascript'>");

                    sb.Append("window.onload=function(){");

                    sb.Append("alert('");

                    sb.Append(message);

                    sb.Append("')};");

                    sb.Append("</script>");

                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
                }

            }
            else
            {
                //error
            }

        }

        string GetPayPalResponse(Dictionary<string, string> formVals, bool useSandbox)
        {

            string paypalUrl = useSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr"
                : "https://www.paypal.com/cgi-bin/webscr";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://urlort#");
            //req.Proxy = proxy;
            //Send the request to PayPal and get the response
            string response = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {

                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            return response;
        }
        string GetPDTValue(string pdt, string key)
        {

            string[] keys = pdt.Split('\n');
            string thisVal = "";
            string thisKey = "";
            foreach (string s in keys)
            {
                string[] bits = s.Split('=');
                if (bits.Length > 1)
                {
                    thisVal = bits[1];
                    thisKey = bits[0];
                    if (thisKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
            }
            return thisVal;

        }
    }
}