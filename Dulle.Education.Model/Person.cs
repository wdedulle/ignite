using Apache.Ignite.Core.Cache.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.Model
{
    [Serializable]
    public class Person
    {
        [QuerySqlField(IsIndexed = true)]
        public string Name { get; set; }

        [QuerySqlField(IsIndexed = true)]
        public string FirstName { get; set; }

        [QuerySqlField]
        public int Age { get; set; }

        public override string ToString()
        {
            return $"Person [Name={Name + " " + FirstName}, Age={Age}]";
        }
    }
}
