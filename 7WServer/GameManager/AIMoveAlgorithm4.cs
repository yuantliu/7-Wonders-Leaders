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
            for (int i = 0; i < player.hand.Count; i++)
            {
                if (player.hand[i].structureType == StructureType.Civilian && player.isCardBuildable(i) == Buildable.True && player.hand[i].age == 3)
                {
                    gm.buildStructureFromHand(player.hand[i].name, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a Blue Card at the third age..");
                    return;

                }
            }

            //look for buildable green cards
            for (int i = 0; i < player.hand.Count; i++)
            {
                if (player.hand[i].structureType == StructureType.Science && player.isCardBuildable(i) == Buildable.True)
                {
                    gm.buildStructureFromHand(player.hand[i].name, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a green Card..");
                    return;

                }
            }

            //look for buildable resource cards that give more than one manufactory resources ...
            for (int i = 0; i < player.hand.Count; i++)
            {
                if ((player.hand[i].structureType == StructureType.Commerce && player.isCardBuildable(i) == Buildable.True) && player.hand[i].effect is ResourceChoiceEffect)
                {
                    // char resource = player.hand[i].effect[2];        // hunh?
                    string resource = ((ResourceChoiceEffect)player.hand[i].effect).strChoiceData;

                    if (resource.Contains("L") && player.loom < maxLPG * 2) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource.Contains("P") && player.papyrus < maxLPG * 2) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource.Contains("G") && player.glass < maxLPG * 2) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }

                    // not sure what's going on here.  I think there may have been a bug in the original implementation.
                }
            }

            //look for buildable resource cards that give more than one resource ...
            for (int i = 0; i < player.hand.Count; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial && player.isCardBuildable(i) == Buildable.True) && player.hand[i].effect is ResourceChoiceEffect)
                {
                    string resource = ((ResourceChoiceEffect)player.hand[i].effect).strChoiceData;

                    if (player.brick < maxOBW && (resource[0] == 'B' || resource[1] == 'B') ) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (player.ore < maxOBW && (resource[0] == 'O' || resource[1] == 'O') ) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (player.stone < maxStone && (resource[0] == 'S' || resource[1] == 'S') ) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (player.wood < maxOBW && (resource[0] == 'W' || resource[1] == 'W') ) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                }
            }


            //look for buildable resource cards that only give one and the manufactory resources ..
            for (int i = 0; i < player.hand.Count; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial || player.hand[i].structureType == StructureType.Goods) && player.isCardBuildable(i) == Buildable.True && player.hand[i].effect is SimpleEffect)
                {
                    char resource = ((SimpleEffect)player.hand[i].effect).type;
                    int numOfResource = ((SimpleEffect)player.hand[i].effect).multiplier;

                    if (resource == 'L' && player.loom < maxLPG) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'G' && player.glass < maxLPG) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'P' && player.papyrus < maxLPG) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'B' && numOfResource + player.brick < maxOBW) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'O' && numOfResource + player.ore < maxOBW) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'T' && numOfResource + player.stone < maxStone) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'W' && numOfResource + player.wood < maxOBW) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                }
            }

            //look for buildable Red cards
            for (int i = 0; i < player.hand.Count; i++)
            {
                if (player.hand[i].structureType == StructureType.Military && player.isCardBuildable(i) == Buildable.True)
                {
                    gm.buildStructureFromHand(player.hand[i].name, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a [Red Army] Card..");
                    return;

                }
            }

            //Discard the non-buildable Red cards
            for (int i = 0; i < player.hand.Count; i++)
            {
                if (player.hand[i].structureType == StructureType.Military && player.isCardBuildable(i) != Buildable.True)
                {
                    gm.discardCardForThreeCoins(player.hand[i].name, player.nickname);
                    Console.WriteLine(player.nickname + " Just Discard A (Red) Card for 3 Coins..");
                    return;
                }
            } 

            gm.discardCardForThreeCoins(player.hand[0].name, player.nickname);
            Console.WriteLine(player.nickname + " Just Discard A Random Card for 3 Coins..");
        } 
    }
}
