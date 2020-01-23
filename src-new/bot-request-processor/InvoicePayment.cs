using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotRequestProcessor
{
    public class InvoicePayment
    {
        public string GetPaymentStatus()
        {
            //Thread.Sleep(5000);  // fake processing

            return "Status is PAID";
        }
    }
}
