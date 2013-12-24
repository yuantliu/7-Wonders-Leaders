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
                if (player.hand[i].colour == "Blue" && player.isCardBuildable(player.hand[i]) == 'T')
                {
                    gm.buildStructureFromHand(player.hand[i].id, player.nickname);
                    return;
                }
            }

            //look for buildable yellow cards that gives some resources
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if (player.hand[i].colour == "Yellow" && player.isCardBuildable(player.hand[i]) == 'T')
                {
                    if (player.hand[i].effect[0] == '4')
                    {
                        gm.buildStructureFromHand(player.hand[i].id, player.nickname);
                        return;
                    }
                }
            }

            //look for buildable resource cards
            for (int i = 0; i < player.numOfHandCards; i++)
            {
                if ((player.hand[i].colour == "Brown" || player.hand[i].colour == "Grey") && player.isCardBuildable(player.hand[i]) == 'T' && player.hand[i].effect[0] != '4')
                {
                    char resource = player.hand[i].effect[2];
                    int numOfResource = int.Parse(player.hand[i].effect[1] + "");

                    if (resource == 'B' && numOfResource + player.brick < maxResourcesRequired ) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }
                    else if (resource == 'O' && numOfResource + player.ore < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }
                    else if (resource == 'T' && numOfResource + player.stone < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }
                    else if (resource == 'W' && numOfResource + player.wood < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }
                    else if (resource == 'G' && numOfResource + player.glass < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }
                    else if (resource == 'L' && numOfResource + player.loom < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }
                    else if (resource == 'P' && numOfResource + player.papyrus < maxResourcesRequired) { gm.buildStructureFromHand(player.hand[i].id, player.nickname); }

                    return;
                }
            }

            //discard card[0]
            gm.discardCardForThreeCoins(player.hand[0].id, player.nickname);
        }
    }
}
