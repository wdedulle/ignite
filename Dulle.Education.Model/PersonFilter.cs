using Apache.Ignite.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Model
{
    [Serializable]
    public class PersonFilter : ICacheEntryFilter<int, Person>
    {
        public bool Invoke(ICacheEntry<int, Person> entry)
        {
            return entry.Value.Age > 80;
        }
    }
}
