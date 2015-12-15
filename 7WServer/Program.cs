﻿using System;
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
    struct CoinCost
    {
        int bank;
        int leftNeighbor;
        int rightNeighbor;
    };

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

            /*
            // Resources (single)
            ResourceEffect wood_1 = new ResourceEffect(true, "W");
            ResourceEffect stone_1 = new ResourceEffect(true, "S");
            ResourceEffect clay_1 = new ResourceEffect(true, "B");
            ResourceEffect ore_1 = new ResourceEffect(true, "O");

            // Resources (either/or)
            ResourceEffect wood_clay = new ResourceEffect(true, "WB");
            ResourceEffect stone_clay = new ResourceEffect(true, "SB");
            ResourceEffect clay_ore = new ResourceEffect(true, "BO");
            ResourceEffect stone_wood = new ResourceEffect(true, "SW");
            ResourceEffect wood_ore = new ResourceEffect(true, "WO");
            ResourceEffect stone_ore = new ResourceEffect(true, "OS");

            // Resources (double)
            ResourceEffect wood_2 = new ResourceEffect(true, "WW");
            ResourceEffect stone_2 = new ResourceEffect(true, "SS");
            ResourceEffect clay_2 = new ResourceEffect(true, "BB");
            ResourceEffect ore_2 = new ResourceEffect(true, "OO");

            // Goods
            ResourceEffect cloth = new ResourceEffect(true, "C");
            ResourceEffect glass = new ResourceEffect(true, "G");
            ResourceEffect papyrus = new ResourceEffect(true, "P");

            // Choose any, but can only be used by player, not neighbors
            ResourceEffect forum = new ResourceEffect(false, "CGP");
            ResourceEffect caravansery = new ResourceEffect(false, "WSBO");

            // Discount effects
            CommercialDiscountEffect east_trading_post = new CommercialDiscountEffect("RR");
            CommercialDiscountEffect west_trading_post = new CommercialDiscountEffect("LR");
            CommercialDiscountEffect marketplace = new CommercialDiscountEffect("BG");

            ResourceManager testResMan = new ResourceManager();

            testResMan.add(wood_1);

            // CoinCost cc;

            if (testResMan.canAfford(new Cost("W")) != true)
                throw new Exception();

            if (testResMan.canAfford(new Cost("O")) != false)
                throw new Exception();

            testResMan.add(clay_ore);

            if (testResMan.canAfford(new Cost("O")) != true)
                throw new Exception();

            testResMan.add(stone_2);

            if (testResMan.canAfford(new Cost("SSW")) != true)
                throw new Exception();

            if (testResMan.canAfford(new Cost("SSWW")) != false)
                throw new Exception();

            testResMan.add(wood_ore);

            if (testResMan.canAfford(new Cost("SSWW")) != true)
                throw new Exception();

            if (testResMan.canAfford(new Cost("SSOWWB")) != false)
                throw new Exception();

            if (testResMan.canAfford(new Cost("SSOWB")) != true)
                throw new Exception();
                */


            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
