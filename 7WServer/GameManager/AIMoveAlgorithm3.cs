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
                if (player.hand[i].structureType == StructureType.Military && player.isCardBuildable(i) == Buildable.True)
                {
                    gm.buildStructureFromHand(player.hand[i].name, player.nickname);
                    Console.WriteLine(player.nickname + " Just Bought a [Red Army] Card..");
                    return;
                    
                }
            }

            //look for buildable resource cards that give more than one resource ...
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial || player.hand[i].structureType == StructureType.Goods) && player.isCardBuildable(i) == Buildable.True && player.hand[i].effect is ResourceChoiceEffect)
                {
                    string resource = ((ResourceChoiceEffect)player.hand[i].effect).strChoiceData;

                    if (resource.Contains('B') && player.brick < maxOBS){ gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource.Contains('O') && player.ore < maxOBS){ gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource.Contains('S') && player.stone < maxOBS){ gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource.Contains('W') && player.wood < maxWood){ gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource.Contains('L') && player.loom < maxLoom){ gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                }
            }
         
            
            //look for buildable resource cards that only give one ..
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial || player.hand[i].structureType == StructureType.Goods) && player.isCardBuildable(i) == Buildable.True && player.hand[i].effect is SimpleEffect)
                {
                    SimpleEffect e = player.hand[i].effect as SimpleEffect;

                    char resource = e.type;
                    int numOfResource = e.multiplier;

                    if (resource == 'B' && numOfResource + player.brick < maxOBS) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'O' && numOfResource + player.ore < maxOBS) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'T' && numOfResource + player.stone < maxOBS) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'W' && numOfResource + player.wood < maxWood) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; }
                    else if (resource == 'L' && numOfResource + player.loom < maxLoom) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); return; } 
                }
            }

            //Discard the non-buildable Red cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].structureType == StructureType.Military && (player.isCardBuildable(i) == Buildable.False) |player.isCardBuildable(i) == Buildable.CommerceRequired))
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
