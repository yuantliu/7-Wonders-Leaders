using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace SevenWonders
{ 
    class Program
    {
        static GMCoordinator gmCoordinator;

        static void Main(string[] args)
        {
            gmCoordinator = new GMCoordinator();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
