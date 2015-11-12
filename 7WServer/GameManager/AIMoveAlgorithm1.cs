using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    //Strategy 1: If first card is buildable, build it. Otherwise, discard it.
    public class AIMoveAlgorithm1 : AIMoveBehaviour
    {
        public void makeMove(Player p, GameManager gm)
        {
            //if first card is buildable, do it.
            //otherwise, discard it
            for(int i = 0; i < p.numOfHandCards; i++){
                if (p.isCardBuildable(i) == 'T')
                {
                    gm.buildStructureFromHand(p.hand[i].id, p.nickname);
                    return;
                }
            }
           
            gm.discardCardForThreeCoins(p.hand[0].id, p.nickname);
            
        }
    }
}
