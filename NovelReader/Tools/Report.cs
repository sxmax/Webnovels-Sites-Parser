using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelReader
{
    public static class Report
    {
        public static void Progress(int value,int previous)
        {
            if (previous > 0) {
                string del = Convert.ToString(previous);
                for (int i = 0; i < del.Length; i++)
                {
                    Console.Write('\b');
                }
            }
            string val = Convert.ToString(value);
            Console.Write(val);
        }
    }
}
