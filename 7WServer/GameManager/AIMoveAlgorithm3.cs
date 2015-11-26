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
            Card c = player.hand.Find(x => x.structureType == StructureType.Military && player.isCardBuildable(x) == Buildable.True);

            if (c == null)
            {
                //look for buildable resource cards that give more than one resource ...
                foreach (Card card in player.hand)
                {
                    if ((card.structureType == StructureType.RawMaterial || card.structureType == StructureType.Goods) && player.isCardBuildable(card) == Buildable.True && card.effect is ResourceChoiceEffect)
                    {
                        string resource = ((ResourceChoiceEffect)card.effect).strChoiceData;

                        if (resource.Contains('B') && player.brick < maxOBS) { c = card; return; }
                        else if (resource.Contains('O') && player.ore < maxOBS) { c = card; return; }
                        else if (resource.Contains('S') && player.stone < maxOBS) { c = card; return; }
                        else if (resource.Contains('W') && player.wood < maxWood) { c = card; return; }
                        else if (resource.Contains('L') && player.loom < maxLoom) { c = card; return; }
                    }
                }
            }

            if (c == null)
            {
                //look for buildable resource cards that only give one ..
                foreach (Card card in player.hand)
                {
                    if ((card.structureType == StructureType.RawMaterial || card.structureType == StructureType.Goods) && player.isCardBuildable(card) == Buildable.True && card.effect is SimpleEffect)
                    {
                        SimpleEffect e = card.effect as SimpleEffect;

                        char resource = e.type;
                        int numOfResource = e.multiplier;

                        if (resource == 'B' && numOfResource + player.brick < maxOBS) { c = card; return; }
                        else if (resource == 'O' && numOfResource + player.ore < maxOBS) { c = card; return; }
                        else if (resource == 'S' && numOfResource + player.stone < maxOBS) { c = card; return; }
                        else if (resource == 'W' && numOfResource + player.wood < maxWood) { c = card; return; }
                        else if (resource == 'L' && numOfResource + player.loom < maxLoom) { c = card; return; } 
                    }
                }
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
