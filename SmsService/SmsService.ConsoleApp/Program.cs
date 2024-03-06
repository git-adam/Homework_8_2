using SMSApi.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsService.ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            return;

            try
            {
                string _token = "";
                SMSApi.Api.IClient client = new SMSApi.Api.ClientOAuth($"{_token}");

                var smsApi = new SMSApi.Api.SMSFactory(client);

                var result = smsApi.ActionSend()
                    .SetTo("").SetText("").SetSender("")
                    .Execute();

                System.Console.WriteLine("Send: " + result.Count);

                string[] ids = new string[result.Count];

                for (int i = 0, l = 0; i < result.List.Count; i++)
                {
                    if (!result.List[i].isError())
                    {
                        if (!result.List[i].isFinal())
                        {
                            ids[l] = result.List[i].ID;
                            l++;
                        }
                    }
                }

                System.Console.WriteLine("Get:");
                result =
                    smsApi.ActionGet()
                        .Ids(ids)
                        .Execute();

                foreach (var status in result.List)
                {
                    System.Console.WriteLine("ID: " + status.ID + " Number: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
