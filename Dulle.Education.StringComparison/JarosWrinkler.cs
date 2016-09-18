using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.StringComparison
{
    public class JarosWrinkler
    {
        public static double Calculate(string s, string t)
        {                         
            // degenerate cases
            if (s == t) return 100;
            if (s.Length == 0) return t.Length;
            if (t.Length == 0) return s.Length;

            return 0;
        }
    }
}
