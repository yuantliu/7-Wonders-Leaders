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

            string strOutput = string.Format("{0} hand: [ ", player.nickname);

            foreach (Card card in player.hand)
            {
                strOutput += card.name;
                strOutput += " ";
            }

            strOutput += "]";

            Console.WriteLine(strOutput);

            Card c = null;

            //look for buildable blue cards at the third age ..
            for (int i = 0; i < player.hand.Count && c == null; i++)
            {
                if (player.hand[i].structureType == StructureType.Civilian && player.isCardBuildable(i) == Buildable.True && player.hand[i].age == 3)
                {
                    c = player.hand[i];
                }
            }


            //look for buildable green cards
            for (int i = 0; i < player.hand.Count && c == null; i++)
            {
                if (player.hand[i].structureType == StructureType.Science && player.isCardBuildable(i) == Buildable.True)
                {
                    c = player.hand[i];
                }
            }

            //look for buildable resource cards that give more than one manufactory resources ...
            for (int i = 0; i < player.hand.Count && c == null; i++)
            {
                if ((player.hand[i].structureType == StructureType.Commerce && player.isCardBuildable(i) == Buildable.True) && player.hand[i].effect is ResourceChoiceEffect)
                {
                    // char resource = player.hand[i].effect[2];        // hunh?
                    string resource = ((ResourceChoiceEffect)player.hand[i].effect).strChoiceData;

                    if (resource.Contains("C") && player.loom < maxLPG * 2) { c = player.hand[i];  }
                    else if (resource.Contains("P") && player.papyrus < maxLPG * 2) { c = player.hand[i]; }
                    else if (resource.Contains("G") && player.glass < maxLPG * 2) { c = player.hand[i]; }

                    // not sure what's going on here.  I think there may have been a bug in the original implementation.
                }
            }

            //look for buildable resource cards that give more than one resource ...
            for (int i = 0; i < player.hand.Count && c == null; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial && player.isCardBuildable(i) == Buildable.True) && player.hand[i].effect is ResourceChoiceEffect)
                {
                    string resource = ((ResourceChoiceEffect)player.hand[i].effect).strChoiceData;

                    if (player.brick < maxOBW && (resource[0] == 'B' || resource[1] == 'B') ) { c = player.hand[i]; }
                    else if (player.ore < maxOBW && (resource[0] == 'O' || resource[1] == 'O') ) { c = player.hand[i]; }
                    else if (player.stone < maxStone && (resource[0] == 'S' || resource[1] == 'S') ) { c = player.hand[i]; }
                    else if (player.wood < maxOBW && (resource[0] == 'W' || resource[1] == 'W') ) { c = player.hand[i]; }
                }
            }


            //look for buildable resource cards that only give one and the manufactory resources ..
            for (int i = 0; i < player.hand.Count && c == null; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial || player.hand[i].structureType == StructureType.Goods) && player.isCardBuildable(i) == Buildable.True && player.hand[i].effect is SimpleEffect)
                {
                    char resource = ((SimpleEffect)player.hand[i].effect).type;
                    int numOfResource = ((SimpleEffect)player.hand[i].effect).multiplier;

                    if (resource == 'C' && player.loom < maxLPG) { c = player.hand[i]; }
                    else if (resource == 'G' && player.glass < maxLPG) { c = player.hand[i]; }
                    else if (resource == 'P' && player.papyrus < maxLPG) { c = player.hand[i]; }
                    else if (resource == 'B' && numOfResource + player.brick < maxOBW) { c = player.hand[i]; }
                    else if (resource == 'O' && numOfResource + player.ore < maxOBW) { c = player.hand[i]; }
                    else if (resource == 'S' && numOfResource + player.stone < maxStone) { c = player.hand[i]; }
                    else if (resource == 'W' && numOfResource + player.wood < maxOBW) { c = player.hand[i]; }
                }
            }

            //look for buildable Red cards
            for (int i = 0; i < player.hand.Count && c == null; i++)
            {
                if (player.hand[i].structureType == StructureType.Military && player.isCardBuildable(i) == Buildable.True)
                {
                    c = player.hand[i]; 
                }
            }

            //Discard the non-buildable Red cards
            for (int i = 0; (i < player.hand.Count) && (c == null); i++)
            {
                if (player.hand[i].structureType == StructureType.Military && player.isCardBuildable(i) != Buildable.True)
                {
                    Console.WriteLine(player.nickname + " Action: Discard {0}", player.hand[i].name);
                    gm.discardCardForThreeCoins(player.hand[i].name, player.nickname);
                    return;
                }
            }

            if (c != null)
            {
                Console.WriteLine(player.nickname + " Action: Constuct {0}", c.name);
                gm.buildStructureFromHand(c.name, player.nickname);
            }
            else
            {
                c = player.hand[0];
                Console.WriteLine(player.nickname + " Action: Discard {0}", c.name);
                gm.discardCardForThreeCoins(c.name, player.nickname);
            }
        } 
    }
}
