using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public static class Extnesions
    {
        public static K KeyByValue<K, V>(this Dictionary<K, V> dict, V value)
        {
            foreach (var pair in dict)
            {
                if (pair.Value.Equals(value))
                    return pair.Key;
            }
            return default(K);
        }
    }
}
