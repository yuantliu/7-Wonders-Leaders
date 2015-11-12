using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    /// <summary>
    /// Strategy: go for Blue cards (VP) victory.
    /// Try to build blue cards and Leader cards that help with blue cards whenever possible
    /// Avoid blacklisted cards.
    /// </summary>
    public class AIMoveAlgorithmLeaders2 : LeadersAIMoveBehaviour
    {
        int[] favouredLeaders = { 207, 219, 214, 221, 236, 230, 238, 232, 218 };

        public void makeMove(Player p, LeadersGameManager gm)
        {
            //During recruitment phase, favour 207, 219, 214, 221, 236, 230, 238, 232, 218 in that order 
            if (gm.currentAge == 0)
            {
                int highestIndex = 10;

                //try to find the highest rated card in hand
                //start looking for the highest rated card, then go down to the next highest, etc.
                foreach(int favouredLeaderID in favouredLeaders)
                {
                    for(int i = 0; i < p.numOfHandCards; i++)
                    {
                        if(p.hand[i].id == favouredLeaderID)
                        {
                            highestIndex = i;
                            break;
                        }
                    }

                    if(highestIndex != 10)
                        break;
                }

                //Found a favoured leader. Try to recruit it
                if (highestIndex < 10)
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
                        if (p.hand[i].id == favouredLeaderID && p.isCardBuildable(i) == 'T')
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
            //The normal phases
            //prioritise Blue cards
            //if no blue cards available, then try to build any available card not in blacklist
            else
            {
                //Search for and build Blue cards if able
                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    if (p.hand[i].colour == "Blue" && p.isCardBuildable(i) == 'T')
                    {
                        gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                        return;
                    }
                }
                //No suitable blue card found
                //try to stockpile some resources then
                //Look for brown/grey cards
                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    if ((p.hand[i].colour == "Brown" || p.hand[i].colour == "Grey") && p.isCardBuildable(i) == 'T')
                    {
                        gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                        return;
                    }
                }
                //All options exhausted. Discard card for some money
                gm.discardCardForThreeCoins(p.hand[0].id, p.nickname);

            }

        }
    }
}
