using Apache.Ignite.Core.Cache;
using System;
using Dulle.Education.StringComparison;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Model
{
    [Serializable]
    public class PersonFilter : ICacheEntryFilter<int, Person>
    {
        private string searchFor;

        public PersonFilter( string personName )
        {
            searchFor = personName;
        }

        public bool Invoke(ICacheEntry<int, Person> entry)
        {
            return RabinKarp.Calculate(entry.Value.FirstName, searchFor) > 80;
        }
    }
}
