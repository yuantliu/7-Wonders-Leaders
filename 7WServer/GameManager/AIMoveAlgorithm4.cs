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

            //look for buildable blue cards at the third age ..
            Card c = player.hand.Find(x => x.structureType == StructureType.Civilian && player.isCardBuildable(x) == Buildable.True && x.age == 3);

            if (c == null)
            {
                //look for buildable green cards
                c = player.hand.Find(x => x.structureType == StructureType.Science && player.isCardBuildable(x) == Buildable.True);
            }

            if (c == null)
            {
                //look for buildable resource cards that give more than one manufactory resources ...
                foreach (Card card in player.hand)
                {
                    if ((card.structureType == StructureType.Commerce && player.isCardBuildable(card) == Buildable.True) && card.effect is ResourceChoiceEffect)
                    {
                        // char resource = player.hand[i].effect[2];        // hunh?
                        string resource = ((ResourceChoiceEffect)card.effect).strChoiceData;

                        if (resource.Contains("C") && player.loom < maxLPG * 2) { c = card; }
                        else if (resource.Contains("P") && player.papyrus < maxLPG * 2) { c = card; }
                        else if (resource.Contains("G") && player.glass < maxLPG * 2) { c = card; }

                        // not sure what's going on here.  I think there may have been a bug in the original implementation.
                    }
                }
            }

            if (c == null)
            {
                //look for buildable resource cards that give more than one resource ...
                foreach (Card card in player.hand)
                {
                    if ((card.structureType == StructureType.RawMaterial && player.isCardBuildable(card) == Buildable.True) && card.effect is ResourceChoiceEffect)
                    {
                        string resource = ((ResourceChoiceEffect)card.effect).strChoiceData;

                        if (player.brick < maxOBW && resource.Contains('B') ) { c = card; }
                        else if (player.ore < maxOBW && resource.Contains('O') ) { c = card; }
                        else if (player.stone < maxStone && resource.Contains('S') ) { c = card; }
                        else if (player.wood < maxOBW && resource.Contains('W') ) { c = card; }
                    }
                }
            }


            if (c == null)
            {
                //look for buildable resource cards that only give one and the manufactory resources ..
                foreach (Card card in player.hand)
                {
                    if ((card.structureType == StructureType.RawMaterial || card.structureType == StructureType.Goods) && player.isCardBuildable(card) == Buildable.True && card.effect is SimpleEffect)
                    {
                        char resource = ((SimpleEffect)card.effect).type;
                        int numOfResource = ((SimpleEffect)card.effect).multiplier;

                        if (resource == 'C' && player.loom < maxLPG) { c = card; }
                        else if (resource == 'G' && player.glass < maxLPG) { c = card; }
                        else if (resource == 'P' && player.papyrus < maxLPG) { c = card; }
                        else if (resource == 'B' && numOfResource + player.brick < maxOBW) { c = card; }
                        else if (resource == 'O' && numOfResource + player.ore < maxOBW) { c = card; }
                        else if (resource == 'S' && numOfResource + player.stone < maxStone) { c = card; }
                        else if (resource == 'W' && numOfResource + player.wood < maxOBW) { c = card; }
                    }
                }
            }

            if (c == null)
            {
                //look for buildable Red cards
                c = player.hand.Find(x => x.structureType == StructureType.Military && player.isCardBuildable(x) == Buildable.True);
            }

            if (c == null)
            {
                //Discard the non-buildable Red cards
                foreach (Card card in player.hand)
                {
                    if (card.structureType == StructureType.Military && player.isCardBuildable(card) != Buildable.True)
                    {
                        Console.WriteLine(player.nickname + " Action: Discard {0}", card.name);
                        gm.discardCardForThreeCoins(card, player);
                        return;
                    }
                }
            }

            if (c != null)
            {
                Console.WriteLine(player.nickname + " Action: Constuct {0}", c.name);
                gm.buildStructureFromHand(c, player, false);
            }
            else
            {
                c = player.hand[0];
                Console.WriteLine(player.nickname + " Action: Discard {0}", c.name);
                gm.discardCardForThreeCoins(c, player);
            }
        } 
    }
}
