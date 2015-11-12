using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    class AIMoveAlgorithm4 : AIMoveBehaviour
    {
        int maxOBW = 2;
        int maxStone = 3;
        int maxLPG = 1;

        public void makeMove(Player player, GameManager gm)
        {
            //go for blue cards only on the third age
            //if not, Discard Red Cards
            //otherwise, discard first card

            //look for buildable blue cards at the third age ..
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].colour == "Blue" && player.isCardBuildable(i) == 'T' && player.hand[i].age == 3)
                {
                    gm.buildStructureFromHand(player.hand[i].id, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a Blue Card at the third age..");
                    return;

                }
            }

            //look for buildable green cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].colour == "Green" && player.isCardBuildable(i) == 'T')
                {
                    gm.buildStructureFromHand(player.hand[i].id, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a green Card..");
                    return;

                }
            }

            //look for buildable resource cards that give more than one manufactory resources ...
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Yellow" && player.isCardBuildable(i) == 'T') && player.hand[i].effect[0] == '4')
                {
                    char resource = player.hand[i].effect[2];

                    if (resource == 'L' && player.brick < maxLPG * 2) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'P' && player.ore < maxLPG * 2) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'G' && player.stone < maxLPG * 2) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                }
            }

            //look for buildable resource cards that give more than one resource ...
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Brown" && player.isCardBuildable(i) == 'T') && player.hand[i].effect[0] == '4')
                {
                    char resource = player.hand[i].effect[2];

                    if (resource == 'B' && player.brick < maxOBW) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'O' && player.ore < maxOBW) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'T' && player.stone < maxStone) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'W' && player.wood < maxOBW) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    
                }
            }


            //look for buildable resource cards that only give one and the manufactory resources ..
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Brown" || player.hand[i].colour == "Grey") && player.isCardBuildable(i) == 'T' && player.hand[i].effect[0] != '4')
                {
                    char resource = player.hand[i].effect[2];
                    int numOfResource = int.Parse(player.hand[i].effect[1] + "");

                    
                    if (resource == 'L' && player.loom < maxLPG) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'G' && player.glass < maxLPG) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'P' && player.papyrus < maxLPG) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'B' && numOfResource + player.brick < maxOBW) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'O' && numOfResource + player.ore < maxOBW) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'T' && numOfResource + player.stone < maxStone) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                    else if (resource == 'W' && numOfResource + player.wood < maxOBW) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); return; }
                   
                }
            }

            //look for buildable Red cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].colour == "Red" && player.isCardBuildable(i) == 'T')
                {
                    gm.buildStructureFromHand(player.hand[i].id, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a [Red Army] Card..");
                    return;

                }
            }

            //Discard the non-buildable Red cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Red" && player.isCardBuildable(i) == 'F') || (player.hand[i].colour == "Red" && player.isCardBuildable(i) == 'C'))
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
