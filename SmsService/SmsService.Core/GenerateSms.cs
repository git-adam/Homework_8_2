using SmsService.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmsService.Core
{
    public class GenerateSms
    {
        public string GenerateErrors(List<Error> errors, int interval)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            if (!errors.Any())
                return string.Empty;

            var sms = $"Błędy z ostatnich {interval} minut.\n";

            foreach (var error in errors)
            {
                sms += $"Wiadomość - {error.Message} <--> Data - {error.Date.ToString("dd-MM-yyyy HH:mm:ss")}.\n";
            }

            sms += "Wiadomość wygenerowana automatycznie z aplikacji SmsService.";

            return sms;
        }
        public string GenerateErrorsWithoutPolishSigns(List<Error> errors, int interval)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            if (!errors.Any())
                return string.Empty;

            var sms = $"Bledy z ostatnich {interval} minut.\n";

            foreach (var error in errors)
            {
                sms += $"Wiadomosc - {error.Message} <--> Data - {error.Date.ToString("dd-MM-yyyy HH:mm:ss")}.\n";
            }

            sms += "Wiadomosc wygenerowana automatycznie z aplikacji SmsService.";

            return sms;
        }

    }
}
