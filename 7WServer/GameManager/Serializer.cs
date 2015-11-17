using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace SevenWonders
{
    public class Marshaller
    {
        public static string ObjectToString(object obj)
        {
            MemoryStream ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, obj);
            return Convert.ToBase64String(ms.ToArray());
        }

        public static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            return new BinaryFormatter().Deserialize(ms);
        }
    }

    /*
     * Player Bar Information
     * used by showPlayerBarPanel() in MainWindow
     */
    [Serializable]
    public class PlayerBarInformation
    {
        public PlayerBarInformationIndividual[] playerInfo;
        public int numOfPlayers;

        public PlayerBarInformation(IPlayer[] player)
        {
            numOfPlayers = player.Length;

            playerInfo = new PlayerBarInformationIndividual[numOfPlayers];
            
            for (int i = 0; i < player.Length; i++)
            {
                playerInfo[i] = new PlayerBarInformationIndividual();
                playerInfo[i].nickname = player[i].GetNickName();
                playerInfo[i].brick = player[i].GetBrick();
                playerInfo[i].ore = player[i].GetOre();
                playerInfo[i].stone = player[i].GetStone();
                playerInfo[i].wood = player[i].GetWood();
                playerInfo[i].glass = player[i].GetGlass();
                playerInfo[i].loom = player[i].GetLoom();
                playerInfo[i].papyrus = player[i].GetPapyrus();
                playerInfo[i].bear = player[i].GetBearTrap();
                playerInfo[i].sextant = player[i].GetSextant();
                playerInfo[i].tablet = player[i].GetTablet();
                playerInfo[i].victory = player[i].GetVictoryPoint();
                playerInfo[i].shield = player[i].GetShield();
                playerInfo[i].coin = player[i].GetCoin();
                playerInfo[i].conflict = player[i].GetConflictTokenOne() + (player[i].GetConflictTokenTwo() * 3) + (player[i].GetConflictTokenThree() * 5);
                playerInfo[i].conflictTokensCount = player[i].GetConflictTokenOne() + player[i].GetConflictTokenTwo() + player[i].GetConflictTokenThree();
                playerInfo[i].loss = player[i].GetLossToken();
            }
        }
    }

    [Serializable]
    public class PlayerBarInformationIndividual
    {
        public String nickname;
        public int brick, ore, stone, wood, glass, loom, papyrus, bear, sextant, tablet, victory, shield, coin, conflict, conflictTokensCount, loss;
    }

    /*
     * showHandPanel
     * Information needed for by the showHandPanel() function in MainWindow. It is used to display the cards
     */
    [Serializable]
    public class HandPanelInformation
    {
        //use tuples to represent the ID-buildable pair
        //the char can be T, F, or C (for buildable with commerce)
        //Tuples would make sure that and id would always be correctly paired with its corresponding buildability status
        public Tuple<int, char> []id_buildable;
        public int informationSize;
        public int currentAge;
        public char stageBuildable;

        public HandPanelInformation(IPlayer p, int currentAge)
        {
            this.currentAge = currentAge;
            informationSize = p.GetNumCardsInHand();
            id_buildable = new Tuple<int, char>[informationSize];

            //add the IDs and their buildability into array of pair-Tuples
            for (int i = 0; i < informationSize; i++)
            {
                id_buildable[i] = new Tuple<int, char>(p.GetCard(i).id, p.isCardBuildable(i));
            }

            stageBuildable = p.isStageBuildable();
        }
    }

    /// <summary>
    /// Serializable object representing only ids of current hands. No associated available actions are here.
    /// This is only used by the Recruitment phase (Age 0)
    /// </summary>
    [Serializable]
    public class RecruitmentPhaseInformation
    {
        public int[] ids;

        public RecruitmentPhaseInformation(IPlayer p)
        {
            ids = new int[p.GetNumCardsInHand()];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = p.GetCard(i).id;
            }
        }
    }

    /*
    /// <summary>
    /// Information to be sent when player wishes to view the details of a particular player
    /// </summary>
    [Serializable]
    public class ViewDetailsInformation
    {
        //Cards that the player has played
        //pairs of tuples will represent each name and id
        //there needs to be 8 lists of tuples. One set for each colour.
        public List<Tuple<string, int>> blueCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> brownCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> greenCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> greyCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> purpleCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> redCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> yellowCards = new List<Tuple<string, int>>();
        public List<Tuple<string, int>> whiteCards = new List<Tuple<string, int>>();

        public String boardname;

        public int numOfStagesBuilt;

        public ViewDetailsInformation(IPlayer p)
        {
            numOfStagesBuilt = p.GetCurrentStageOfWonder();

            //set the board name
            boardname = p.GetBoardName();

            for (int i = 0; i < p.GetNumberOfPlayedCards(); i++)
            {
                if (p.GetCardPlayed(i).colour == "Blue")
                {
                    blueCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "Brown")
                {
                    brownCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "Green")
                {
                    greenCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "Grey")
                {
                    greyCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "Purple")
                {
                    purpleCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "Red")
                {
                    redCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "Yellow")
                {
                    yellowCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }

                else if (p.GetCardPlayed(i).colour == "White")
                {
                    whiteCards.Add(new Tuple<string, int>(p.GetCardPlayed(i).name, p.GetCardPlayed(i).id));
                }
            }
        }
    }
    */
    /// <summary>
    /// Information for dialogues that involve playing cards for free
    /// For example, Olympia (for current hand cards) and Rome (for Leader cards)
    /// O is Olympia, R is Rome
    /// </summary>
    [Serializable]
    public class PlayForFreeInformation
    {
        public char mode;
        //string: name, int: id
        public Tuple<string, int>[] cards;

        public PlayForFreeInformation(IPlayer p, char mode)
        {
            this.mode = mode;

            //Olympia information
            //get hand information to play hand cards for free
            if (mode == 'O')
            {
                cards = new Tuple<string, int>[p.GetNumCardsInHand()];

                for (int i = 0; i < p.GetNumCardsInHand(); i++)
                {
                    cards[i] = new Tuple<string, int>(p.GetCard(i).name, p.GetCard(i).id);
                }
            }
            //Rome information
            //get Leader pile information
            else if (mode == 'R')
            {
                cards = new Tuple<string, int>[p.GetLeadersPile().Count];

                for (int i = 0; i < p.GetLeadersPile().Count; i++)
                {
                    cards[i] = new Tuple<string, int>(p.GetLeadersPile()[i].name, p.GetLeadersPile()[i].id);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Class to be sent to form the commerce window and containing commerce information
    /// Contains information made by Yunus
    /// As well as whether the player has the appropriate discount or not
    /// </summary>
    [Serializable]
    public class CommerceInformationPackage
    {
        public string information;
        public bool hasDiscount;

        public CommerceInformationPackage(string information, bool hasDiscount)
        {
            this.information = information;
            this.hasDiscount = hasDiscount;
        }
    }

    /// <summary>
    /// Class sent for CourtesanUI to process
    /// Contains name and id of neighbouring played Leader cards, represented in a List of string-int Tuples
    /// </summary>
    [Serializable]
    public class CourtesanGuildInformation
    {
        public List<Tuple<string, int>> card;

        public CourtesanGuildInformation(IPlayer p)
        {
            card = new List<Tuple<string, int>>();

            //get the left neighbours Leaders cards
            for (int i = 0; i < p.GetLeftNeighbour().GetNumberOfPlayedCards(); i++)
            {
                if (p.GetLeftNeighbour().GetCardPlayed(i).colour == "White")
                {
                    card.Add(new Tuple<string, int>(p.GetLeftNeighbour().GetCardPlayed(i).name, p.GetLeftNeighbour().GetCardPlayed(i).id));
                }
            }

            //get the right neighbours Leader cards
            for (int i = 0; i < p.GetRightNeighbour().GetNumberOfPlayedCards(); i++)
            {
                if (p.GetRightNeighbour().GetCardPlayed(i).colour == "White")
                {
                    card.Add(new Tuple<string, int>(p.GetRightNeighbour().GetCardPlayed(i).name, p.GetRightNeighbour().GetCardPlayed(i).id));
                }
            }
        }
    }

    /// <summary>
    /// Contains information needed to construct the NewCommerce UIs. Sent from Server to Client
    /// </summary>
    [Serializable]
    public class CommerceInformation
    {
        //Whether there is a Leader that is giving a 1 resource discount on this card
        public bool hasDiscount = false;
        //whether the player (centre, aka you) have market effect for left and right
        public bool leftRawMarket, leftManuMarket, rightRawMarket, rightManuMarket;

        //cost of card
        public string cardCost = "";
        //[0] represents left player
        //[1] represents mid player (the user)
        //[2] represents right player
        public PlayerCommerceInfo[] playerCommerceInfo = new PlayerCommerceInfo[3];

        //how many coins does the (middle) player have?
        public int playerCoins;
        //what card is being played/used
        public int id;

        public bool isStage = false;

        public CommerceInformation(IPlayer left, IPlayer centre, IPlayer right, bool discountApplies, int id, string cardCost, bool isStage)
        {
            this.hasDiscount = discountApplies;
            this.cardCost = cardCost;
            leftRawMarket = centre.GetLeftRaw();
            leftManuMarket = centre.GetLeftManu();
            rightRawMarket = centre.GetRightRaw();
            rightManuMarket = centre.GetRightManu();

            playerCoins = centre.GetCoin();
            this.id = id;
            this.isStage = isStage;

            //fill the PlayercommerceInfo for all 3 relevant players
            playerCommerceInfo[0] = new PlayerCommerceInfo(left);
            playerCommerceInfo[1] = new PlayerCommerceInfo(centre);
            playerCommerceInfo[2] = new PlayerCommerceInfo(right);
        }

    }

    /// <summary>
    /// Used by CommerceInformation to store each player's commerce info
    /// </summary>
    [Serializable]
    public class PlayerCommerceInfo
    {
        public string name;
        public DAG dag;

        public PlayerCommerceInfo(IPlayer player)
        {
            this.name = player.GetNickName();
            dag = player.GetDAG();
        }
    }

    /// <summary>
    /// Used by Client to send commerce response to Server
    /// if id == 0, then we know that we are building the current stage of wonder
    /// </summary>
    [Serializable]
    public class CommerceClientToServerResponse
    {
        public int leftCoins, rightCoins, id;
    }
}
