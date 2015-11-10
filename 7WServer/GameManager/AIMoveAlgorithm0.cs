using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    //Strategy 0: Always discard
    public class AIMoveAlgorithm0 : AIMoveBehaviour
    {
        public void makeMove(Player player, GameManager gm)
        {
            gm.discardCardForThreeCoins(player.hand[0].id, player.nickname);
        }
    }
}
