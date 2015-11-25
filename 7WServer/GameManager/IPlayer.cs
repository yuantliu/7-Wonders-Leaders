using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SevenWonders
{
    public interface IPlayer
    {
        String GetNickName();

        int GetCoin();

        Buildable isCardBuildable(Card card);

        Buildable isStageBuildable();

        bool GetLeftRaw();

        bool GetLeftManu();

        bool GetRightRaw();

        bool GetRightManu();

        DAG GetDAG();
    }
}
