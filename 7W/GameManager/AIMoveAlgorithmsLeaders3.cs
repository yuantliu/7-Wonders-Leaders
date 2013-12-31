using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    /// <summary>
    /// Strategy: go for Red cards to keep up with Shields.
    /// Try to build blue cards and Leader cards that help with blue cards whenever possible
    /// Avoid blacklisted cards.
    /// </summary>
    public class AIMoveAlgorithmLeaders3 : LeadersAIMoveBehaviour
    {
        int[] favouredLeaders = { 216, 220, 222, 232, 200, 208, 205 };

        public void makeMove(Player p, LeadersGameManager gm)
        {
            //recruitment phase
            if (gm.currentAge == 0)
            {
                int highestIndex = 100;

                //try to find the highest rated card in hand
                //start looking for the highest rated card, then go down to the next highest, etc.
                foreach (int favouredLeaderID in favouredLeaders)
                {
                    for (int i = 0; i < p.numOfHandCards; i++)
                    {
                        if (p.hand[i].id == favouredLeaderID)
                        {
                            highestIndex = i;
                            break;
                        }
                    }

                    if (highestIndex != 10)
                        break;
                }

                //Found a favoured leader. Try to recruit it
                if (highestIndex < 100)
                {
                    gm.recruitLeader(p.nickname, p.hand[highestIndex].id);
                }
                //No favoured leader found
                //Recruit first leader
                else
                {
                    gm.recruitLeader(p.nickname, p.hand[0].id);
                }
            }
            //During leaders phase, try to play the favoured cards, if able
            else if (gm.currentTurn == 0)
            {
                //find a favoured card
                foreach (int favouredLeaderID in favouredLeaders)
                {
                    for (int i = 0; i < p.numOfHandCards; i++)
                    {
                        //found one
                        if (p.hand[i].id == favouredLeaderID && p.isCardBuildable(p.hand[i]) == 'T')
                        {
                            gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                            return;
                        }
                    }
                }
                //no favoured card found
                //discard a random card then
                gm.discardCardForThreeCoins(p.hand[0].id, p.nickname);
            }
        }

    }
}
