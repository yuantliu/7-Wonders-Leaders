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

        int GetNumCardsInHand();

        Card GetCard(int i);

        Buildable isCardBuildable(int i);

        Buildable isStageBuildable();

        Card GetCardPlayed(int index);

        int GetNumberOfPlayedCards();

        IPlayer GetLeftNeighbour();

        IPlayer GetRightNeighbour();

        List<Card> GetLeadersPile();

        bool GetLeftRaw();

        bool GetLeftManu();

        bool GetRightRaw();

        bool GetRightManu();

        DAG GetDAG();
    }
}
