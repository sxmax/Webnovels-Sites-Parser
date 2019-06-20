using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelReader
{
    class Commands
    {
        public static void Execute(string command,ref bool flag)
        {
            switch(command.ToLower())
            {
                case "exit":
                    flag = false;
                    break;
                case "clear":
                    Console.Clear();
                    Console.ResetColor();
                    break;
            }
        }
    }
}
