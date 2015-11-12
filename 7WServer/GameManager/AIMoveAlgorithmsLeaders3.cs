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
        int[] favouredLeaders = { 216, 220, 222, 232, 200, 208, 205, 221, 214, 236, 213 };

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
            //prioritise Red cards IF falling behind on shields. Always try to keep at least a 2 shield lead on neighbours.
            //if no red cards available, then try to build blue or green cards. If not, then any available card not in blacklist
            else
            {
                if ((p.shield < p.leftNeighbour.shield + 2) || (p.shield < p.rightNeighbour.shield + 2))
                {
                    //Search for and build Red cards if able
                    for (int i = 0; i < p.numOfHandCards; i++)
                    {
                        if (p.hand[i].colour == "Red" && p.isCardBuildable(i) == 'T')
                        {
                            gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                            return;
                        }
                    }
                }
                //look for either blue or green cards then
                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    if ((p.hand[i].colour == "Blue" || p.hand[i].colour == "Green") && p.isCardBuildable(i) == 'T')
                    {
                        gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                        return;
                    }
                }
                //No suitable red card found
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
