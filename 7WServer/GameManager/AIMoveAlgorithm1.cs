using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    //Strategy 1: Build first in the hand that is buildable.  If there are no buildable cards, discard the first one
    public class AIMoveAlgorithm1 : AIMoveBehaviour
    {
        public void makeMove(Player p, GameManager gm)
        {
            //if first card is buildable, do it.
            //otherwise, discard it
            for(int i = 0; i < p.numOfHandCards; i++){
                if (p.isCardBuildable(i) == Buildable.True)
                {
                    gm.buildStructureFromHand(p.hand[i].name, p.nickname);
                    return;
                }
            }

            gm.discardCardForThreeCoins(p.hand[0].name, p.nickname);
        }
    }
}
