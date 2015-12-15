using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{

    /// <summary>
    /// Strategy: just discard cards
    /// Recruit random leader during Leader phase
    /// </summary>
    public class AIMoveAlgorithmLeaders0 : LeadersAIMoveBehaviour
    {
        public void makeMove(Player p, LeadersGameManager gm)
        {
            if (gm.currentAge == 0)
            {
                gm.recruitLeader(p.nickname, p.hand[0].id);
            }
            else
            {
                gm.discardCardForThreeCoins(p.hand[0].id, p.nickname);
            }
        }
    }
}
