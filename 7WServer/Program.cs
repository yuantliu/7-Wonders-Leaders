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

            /*
            // TODO: test whether we can use other names, such as "James", "Mike", "Greg", "Ricky", "John", "Kevin"
            StatusChangedEventArgs cmd = new StatusChangedEventArgs("James", "");

            cmd.message = "JJames"; gmCoordinator.receiveMessage(null, cmd);    // James joins the table
            cmd.message = "aa4"; gmCoordinator.receiveMessage(null, cmd);       // Add AI player
            cmd.message = "aa4"; gmCoordinator.receiveMessage(null, cmd);       // Add AI player
            cmd.message = "R"; gmCoordinator.receiveMessage(null, cmd);         // Player is ready.  After all non-AI players send this, the game begins.
            cmd.message = "U"; gmCoordinator.receiveMessage(null, cmd);         // UI is ready to accept the first update
            cmd.message = "r"; gmCoordinator.receiveMessage(null, cmd);         // ready for the first hand of cards
            cmd.message = "BldStrct&WonderStage=0&Structure=Clay Pit"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "BldStrct&WonderStage=0&Structure=East Trading Post"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "BldStrct&WonderStage=0&Structure=Marketplace"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            */
            /*
            cmd.message = "Discards&Baths"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "BWorkshop"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "BStone Pit"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "SWest Trading Post"; gmCoordinator.receiveMessage(null, cmd);
            cmd.message = "t"; gmCoordinator.receiveMessage(null, cmd);
            */

            ResourceManager testResMag = new ResourceManager();



            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
