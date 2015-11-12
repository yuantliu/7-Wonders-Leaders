using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{

    /// <summary>
    /// Strategy: build anything that is buildable
    /// First thing that is found to be buildable is built
    /// If no suitable cards are found, then discard the card at 0th position (i.e. discard random card
    /// Certain cards are avoided.
    /// Namely, 
    /// 12, 13, 24, 25, 46, 47 (Market place effects)
    /// 101, 102, 103, 107, 201, 212, 231 (involves Stages)
    /// 204, 209, 237, 229 (Problematic Leaders)
    /// 301 (Courtesan's Guild)
    /// </summary>
    public class AIMoveAlgorithmLeaders1 : LeadersAIMoveBehaviour
    {
        public void makeMove(Player p, LeadersGameManager gm)
        {
            //Recruitment phase
            //Just recruit any random guy
            if (gm.currentAge == 0)
            {
                gm.recruitLeader(p.nickname, p.hand[0].id);
            }

            //Other turns
            //try to sequentially see if there are cards to build, avoiding the blacklisted cards
            //if no eligible cards are found, then discard card at 0th position
            else 
            {
                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    //found a playable card that is not blacklisted
                    //build it
                    if((AI_LEADERS_BLACKLIST.Contains(p.hand[i].id) == false) && (p.isCardBuildable(i) == 'T'))
                    {
                        gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                        return;
                    }
                }

                //no suitable card has been found
                //discard a random card then
                gm.discardCardForThreeCoins(p.hand[0].id, p.nickname);
            }
        }
    }
}
