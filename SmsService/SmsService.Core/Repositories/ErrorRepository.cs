using SmsService.Core.Domains;
using System;
using System.Collections.Generic;

namespace SmsService.Core.Repositories
{
    public class ErrorRepository
    {
        public List<Error> GetLastErrors(int intervalInMinutes)
        {
            //pobieranie z bazy danych

            return new List<Error>()
            {
                new Error() {Message = "Błąd testowy 1", Date = DateTime.Now},
                new Error() {Message = "Błąd testowy 3", Date = new DateTime(2022, 1, 1, 12, 12, 12)},
                new Error() {Message = "Błąd testowy 4", Date = new DateTime(2020, 11, 21, 10, 11, 12)},
            };
        }
    }
}
