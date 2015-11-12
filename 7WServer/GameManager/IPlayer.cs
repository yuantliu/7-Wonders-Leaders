using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SevenWonders
{
    public interface IPlayer
    {
        bool GetIsAI();

        String GetNickName();

        String GetBoardName();

        //current Stage of wonder
        int GetCurrentStageOfWonder();

        //resources
        int GetBrick();

        int GetOre();

        int GetStone();

        int GetWood();

        int GetGlass();

        int GetLoom();

        int GetPapyrus();

        int GetCoin();

        //science
        int GetBearTrap();
        int GetTablet();
        int GetSextant();

        //Points and stuff
        int GetVictoryPoint();

        int GetShield();

        int GetLossToken();

        int GetConflictTokenOne();

        int GetConflictTokenTwo();

        int GetConflictTokenThree();

        int GetNumCardsInHand();

        Card GetCard(int i);

        char isCardBuildable(int i);

        char isStageBuildable();

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
