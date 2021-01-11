using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZSI.Projekt.Models.NewNeural
{
    public static class RandomWeights
    {
        private static Random _random = new Random();
        private static readonly object _sync = new object();

        public static double NextDouble(double minimum, double maximum)
        {
            lock(_sync) {
                return _random.NextDouble() * (maximum - minimum) + minimum;
            }
        }

        public static int Next(int min, int max)
        {
            lock (_sync) {
                return _random.Next(min, max);
            }
        }

    }
}
