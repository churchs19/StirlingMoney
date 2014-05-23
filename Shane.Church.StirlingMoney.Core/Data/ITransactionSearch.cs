using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.StirlingMoney.Core.Data
{
    public interface ITransactionSearch
    {
        List<Transaction> GetSearchResults(Guid AccountId, string searchText);
        Task<List<Transaction>> GetSearchResultsAsync(Guid AccountId, string searchText);
    }
}
