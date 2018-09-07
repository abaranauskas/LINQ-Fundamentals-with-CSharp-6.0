using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.MyLinq
{
    public static class MyLinq
    {
        public static int Count<T>(this IEnumerable<T> employee)
        {
            int count = 0;
            foreach (var item in employee)
            {
                count++;
            }
            return count;
        }
    }
}
