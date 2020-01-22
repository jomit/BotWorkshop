using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using System.Linq;

namespace Microsoft.BotBuilderSamples
{
    public partial class AccountsPayable
    {
        public (string InvoiceNumber, string VendorName) StatusEntities
        {
            get
            {
                //var invoiceNumber = Entities?._instance?.InvoiceNumber?.FirstOrDefault()?.Text;
                //var vendorName = Entities?._instance?.VendorName?.FirstOrDefault()?.Text;
                var invoiceNumber = Entities.InvoiceNumber.FirstOrDefault();
                var vendorName = Entities.VendorName .FirstOrDefault();
                return (invoiceNumber, vendorName);
            }
        }
    }
}
