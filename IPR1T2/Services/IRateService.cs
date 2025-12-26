using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IPR1_2.Models;

namespace IPR1_2.Services
{
    public interface IRateService
    {
        Task<IEnumerable<Rate>> GetDailyRates(DateTime date);
    }
}