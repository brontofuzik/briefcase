using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILRepack.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            ILRepacking.ILRepack repack = new ILRepacking.ILRepack(new ILRepacking.RepackOptions(new string[0]));
            repack.Repack();
        }
    }
}
