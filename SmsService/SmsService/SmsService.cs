using Cipher;
using NLog.Fluent;
using SMSApi.Api;
using SmsSender;
using SmsService.Core;
using SmsService.Core.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SmsService
{
    public partial class SmsService : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const int MinuteToMiliSeconds = 60000;
        private readonly int _intervalInMinutes;
        private readonly Timer _timer;
        private static readonly string GUID = "{2D3EBC2F - 3812 - 4522 - B8D6 - 54ED0CADE1D7}";
        private const string NotEncryptedTokenPrefix = "encrypt:";
        private Sms _sms;
        private string[] _smsReceivers;
        private ErrorRepository _errorRepository = new ErrorRepository();
        private GenerateSms _plainTextSms = new GenerateSms();
        private StringCipher _stringCipher = new StringCipher(GUID);

        public SmsService()
        {
            InitializeComponent();

            try
            {
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinute"]);
                _timer = new Timer(_intervalInMinutes * MinuteToMiliSeconds);
                _smsReceivers = ConfigurationManager.AppSettings["SmsReceivers"]
                    .Split(',')
                    .Select(x => x.Trim())
                    .ToArray();

                _sms = new Sms(new SmsParams()
                {
                    Token = DecryptToken(),
                    ProxyAddress = (ProxyAddress)Enum.Parse(typeof(ProxyAddress), ConfigurationManager.AppSettings["ProxyAddress"]),
                    SenderName = ConfigurationManager.AppSettings["SenderName"]
                });

            }
            catch (SmsapiException ex)
            {
                Logger.Error(ex, ex.GetCode() + ": " + ex.Message);
                throw new ActionException(ex.Message, ex.GetCode());
            }
            catch(System.Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new System.Exception(ex.Message);
            }
        }

        private string DecryptToken()
        {
            var encryptedToken = ConfigurationManager.AppSettings["Token"];

            if (encryptedToken.StartsWith(NotEncryptedTokenPrefix))
            {
                encryptedToken = _stringCipher.Encrypt(encryptedToken.Replace(NotEncryptedTokenPrefix, ""));

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                configFile.AppSettings.Settings["Token"].Value = encryptedToken;

                configFile.Save();
            }

            return _stringCipher.Decrypt(encryptedToken);
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Sms Service started ...");
            _timer.Elapsed += DoWork;
            _timer.Start();
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendError();
            }
            catch (System.Exception ex)
            {
                Logger.Info(ex, ex.Message);
                throw new System.Exception(ex.Message);
            }
        }

        private void SendError()
        {
            var errors = _errorRepository.GetLastErrors(_intervalInMinutes);
            if (errors == null || !errors.Any())
                return;

            //send sms
            _sms.Send(_plainTextSms.GenerateErrors(errors, _intervalInMinutes), _smsReceivers, DateTime.Now);

            Logger.Info("Errors sent...");
        }

        protected override void OnStop()
        {
            Logger.Info("Sms Service stopped ...");
        }
    }
}
