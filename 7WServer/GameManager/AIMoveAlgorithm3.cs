using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    class AIMoveAlgorithm3 : AIMoveBehaviour
    { 
        int maxOBS = 3;
        int maxWood = 2;
        int maxLoom = 1;

        public void makeMove(Player player, GameManager gm)
        {
            //go for Red cards whenever you can
            //if not, Discard Red Cards
            //otherwise, discard first card

            //look for buildable Red cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].colour == "Red" && player.isCardBuildable(player.hand[i]) == 'T')
                {
                    gm.buildStructureFromHand(player.hand[i].id, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a [Red Army] Card..");
                    return;
                    
                }
            }

            //look for buildable resource cards that give more than one resource ...
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Brown" || player.hand[i].colour == "Grey") && player.isCardBuildable(player.hand[i]) == 'T')
                {
                    char resource = player.hand[i].effect[2];

                    if (resource == 'B' && player.brick < maxOBS){ gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'O' && player.ore < maxOBS){ gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'T' && player.stone < maxOBS){ gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'W' && player.wood < maxWood){ gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'L' && player.loom < maxLoom){ gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                }
            }
         
            
            //look for buildable resource cards that only give one ..
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Brown" || player.hand[i].colour == "Grey") && player.isCardBuildable(player.hand[i]) == 'T' && player.hand[i].effect[0] != '4')
                {
                    char resource = player.hand[i].effect[2];
                    int numOfResource = int.Parse(player.hand[i].effect[1] + "");

                    if (resource == 'B' && numOfResource + player.brick < maxOBS) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'O' && numOfResource + player.ore < maxOBS) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'T' && numOfResource + player.stone < maxOBS) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'W' && numOfResource + player.wood < maxWood) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'L' && numOfResource + player.loom < maxLoom) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; } 
                }
            }
         
           
            
           //Discard the non-buildable Red cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Red" && player.isCardBuildable(player.hand[i]) == 'F') || (player.hand[i].colour == "Red" && player.isCardBuildable(player.hand[i]) == 'C'))
                {
                    gm.discardCardForThreeCoins(player.hand[i].id, player.nickname);
                    Console.WriteLine(player.nickname + " Just Discard A (Red) Card for 3 Coins..");
                    return;
                }
            } 
           
            
            gm.discardCardForThreeCoins(player.hand[0].id, player.nickname);
            Console.WriteLine(player.nickname + " Just Discard A Random Card for 3 Coins..");

         
        } 
    }
}
