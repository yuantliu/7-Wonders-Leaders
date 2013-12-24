using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    /// <summary>
    /// Strategy: go for Red cards to keep up with .
    /// Try to build blue cards and Leader cards that help with blue cards whenever possible
    /// Avoid blacklisted cards.
    /// </summary>
    public class AIMoveAlgorithmLeaders3 : LeadersAIMoveBehaviour
    {
        int[] favouredLeaders = { 216,  };

        public void makeMove(Player p, LeadersGameManager gm)
        {

        }
    }
}
