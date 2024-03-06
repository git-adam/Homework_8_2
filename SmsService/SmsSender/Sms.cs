using SMSApi.Api;
using System;

namespace SmsSender
{
    public class Sms
    {

        private IClient _client;
        private SMSFactory _smsApi;
        private SMSApi.Api.Response.Status _sendResponseStatus;

        private string _token;
        private ProxyAddress _proxyAddress;
        private string _senderName;

        public Sms(SmsParams smsParams)
        {
            _token = smsParams.Token;
            _proxyAddress = smsParams.ProxyAddress;
            _senderName = smsParams.SenderName;
        }

        public void Send(string body, string[] to, DateTime date)
        {
            _client = new ClientOAuth(_token);
            _smsApi = new SMSFactory(_client, _proxyAddress);

            _sendResponseStatus = _smsApi.ActionSend()
                .SetDateSent(date)
                .SetSender(_senderName)
                .SetTo(to)
                .SetText(body)       
                .Execute();

            var ids = GetSendSmsIds();
            GetResponseStatus(ids);
        }

        private string[] GetSendSmsIds()
        {
            string[] ids = new string[_sendResponseStatus.Count];

            for (int i = 0, l = 0; i < _sendResponseStatus.List.Count; i++)
            {
                if (!_sendResponseStatus.List[i].isError())
                {
                    if (!_sendResponseStatus.List[i].isFinal())
                    {
                        ids[l] = _sendResponseStatus.List[i].ID;
                        l++;
                    }
                }
            }

            return ids;
        }

        private void GetResponseStatus(string[] ids)
        {

            var result = _smsApi.ActionGet()
                    .Ids(ids)
                    .Execute();

            foreach (var status in result.List)
            {
                System.Console.WriteLine("ID: " + status.ID + " Number: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
            }
        }
    }
}
