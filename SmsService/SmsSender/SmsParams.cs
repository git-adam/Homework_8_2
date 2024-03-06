using SMSApi.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsSender
{
    public class SmsParams
    {
        public string Token { get; set; }
        public string SenderName { get; set; }
        public ProxyAddress ProxyAddress { get; set; }
    }
}
