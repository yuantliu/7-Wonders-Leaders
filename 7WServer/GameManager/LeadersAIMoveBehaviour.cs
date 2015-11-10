using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public interface LeadersAIMoveBehaviour
    {
        void makeMove(Player p, LeadersGameManager gm);
    }
}
