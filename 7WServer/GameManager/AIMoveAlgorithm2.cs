using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public class AIMoveAlgorithm2 : AIMoveBehaviour
    {
        int maxResourcesRequired = 3;

        public void makeMove(Player player, GameManager gm)
        {
            //go for blue cards whenever you can
            //if not, go for resources
            //otherwise, discard first card

            //look for buildable blue cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].structureType == StructureType.Civilian && player.isCardBuildable(i) == Buildable.True)
                {
                    gm.buildStructureFromHand(player.hand[i].name, player.nickname);
                    return;
                }
            }

            //look for buildable yellow cards that gives some resources
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].structureType == StructureType.Commerce && player.isCardBuildable(i) == Buildable.True)
                {
                    if (player.hand[i].effect is ResourceChoiceEffect)
                    {
                        gm.buildStructureFromHand(player.hand[i].name, player.nickname);
                        return;
                    }
                }
            }

            //look for buildable resource cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].structureType == StructureType.RawMaterial || player.hand[i].structureType == StructureType.Goods) && player.isCardBuildable(i) == Buildable.True && player.hand[i].effect is SimpleEffect)
                {
                    SimpleEffect e = player.hand[i].effect as SimpleEffect;
                    char resource = e.type;
                    int numOfResource = e.multiplier;

                    if (resource == 'B' && numOfResource + player.brick < maxResourcesRequired ) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }
                    else if (resource == 'O' && numOfResource + player.ore < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }
                    else if (resource == 'T' && numOfResource + player.stone < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }
                    else if (resource == 'W' && numOfResource + player.wood < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }
                    else if (resource == 'G' && numOfResource + player.glass < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }
                    else if (resource == 'L' && numOfResource + player.loom < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }
                    else if (resource == 'P' && numOfResource + player.papyrus < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].name, player.nickname); }

                    return;
                }
            }

            //discard card[0]
            gm.discardCardForThreeCoins(player.hand[0].name, player.nickname);
        }
    }
}
