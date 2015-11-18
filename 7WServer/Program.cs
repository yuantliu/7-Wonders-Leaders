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

            // TODO: test whether we can use other names, such as "James", "Mike", "Greg", "Ricky", "John", "Kevin"
            StatusChangedEventArgs cmd = new StatusChangedEventArgs("Host", "");

            cmd.message = "JHost"; gmCoordinator.receiveMessage(null, cmd);     // Host joins the table
            cmd.message = "aa3"; gmCoordinator.receiveMessage(null, cmd);       // Add AI (Prefer military cards)
            cmd.message = "aa4"; gmCoordinator.receiveMessage(null, cmd);       // Add AI (Difficult AI)
            cmd.message = "R"; gmCoordinator.receiveMessage(null, cmd);         // Player is ready.  After all non-AI players send this, the game begins.
            cmd.message = "U"; gmCoordinator.receiveMessage(null, cmd);         // UI is ready to accept the first update
            cmd.message = "r"; gmCoordinator.receiveMessage(null, cmd);         // ready for the first hand of cards

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
