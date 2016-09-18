using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dulle.Education.StringComparison
{
    public class ChapmanLengthDeviation
    {
        public static double Calculate(string source, string target)
        {
            // degenerate cases
            if (source == target) return 100;
            if (source.Length == 0) return target.Length;
            if (target.Length == 0) return source.Length;

            double distance = 0;

            if (source.Length >= target.Length)
            {
                distance = (double)target.Length / (double)source.Length;
            }
            else
            {
                distance = (double)source.Length / (double)target.Length;
            }

            return (1.0 - (distance / (double)Math.Max(source.Length, target.Length))) * 100;
        }

    }
}
