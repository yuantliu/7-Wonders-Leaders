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
    public class Serializer
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

        public PlayerBarInformation(Player[] player)
        {
            numOfPlayers = player.Length;

            playerInfo = new PlayerBarInformationIndividual[numOfPlayers];
            
            for (int i = 0; i < player.Length; i++)
            {
                playerInfo[i] = new PlayerBarInformationIndividual();
                playerInfo[i].nickname = player[i].nickname;
                playerInfo[i].brick = player[i].brick;
                playerInfo[i].ore = player[i].ore;
                playerInfo[i].stone = player[i].stone;
                playerInfo[i].wood = player[i].wood;
                playerInfo[i].glass = player[i].glass;
                playerInfo[i].loom = player[i].loom;
                playerInfo[i].papyrus = player[i].papyrus;
                playerInfo[i].bear = player[i].bearTrap;
                playerInfo[i].sextant = player[i].sextant;
                playerInfo[i].tablet = player[i].tablet;
                playerInfo[i].victory = player[i].victoryPoint;
                playerInfo[i].shield = player[i].shield;
                playerInfo[i].coin = player[i].coin;
                playerInfo[i].conflict = player[i].conflictTokenOne + (player[i].conflictTokenTwo * 3) + (player[i].conflictTokenThree * 5);
                playerInfo[i].conflictTokensCount = player[i].conflictTokenOne + player[i].conflictTokenTwo + player[i].conflictTokenThree;
                playerInfo[i].loss = player[i].lossToken;
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

        public HandPanelInformation(Player p, int currentAge)
        {
            this.currentAge = currentAge;
            informationSize = p.numOfHandCards;
            id_buildable = new Tuple<int, char>[informationSize];

            //add the IDs and their buildability into array of pair-Tuples
            for (int i = 0; i < informationSize; i++)
            {
                id_buildable[i] = new Tuple<int, char>(p.hand[i].id, p.isCardBuildable(p.hand[i]));
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

        public RecruitmentPhaseInformation(Player p)
        {
            ids = new int[p.numOfHandCards];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = p.hand[i].id;
            }
        }
    }

    /*
     * Object that encapsulate information about last played card, which is used to add to the Played Card Panel
     * Whenever a card becomes "Played", UpdatePlayedCardPanel should be called
     * Created by GameManager.UpdatePlayedCardPanel
     * Read by 
     */

    [Serializable]
    public class LastPlayedCardInformation
    {
        public string colour;
        public string name;
        public int id;

        public LastPlayedCardInformation(Player p)
        {
            colour = p.getLastPlayedCard().colour;
            name = p.getLastPlayedCard().name;
            id = p.getLastPlayedCard().id;
        }
    }

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

        public ViewDetailsInformation(Player p)
        {
            numOfStagesBuilt = p.currentStageOfWonder;

            //set the board name
            boardname = p.playerBoard.name;

            for (int i = 0; i < p.numOfPlayedCards; i++)
            {
                if (p.playedStructure[i].colour == "Blue")
                {
                    blueCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "Brown")
                {
                    brownCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "Green")
                {
                    greenCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "Grey")
                {
                    greyCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "Purple")
                {
                    purpleCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "Red")
                {
                    redCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "Yellow")
                {
                    yellowCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }

                else if (p.playedStructure[i].colour == "White")
                {
                    whiteCards.Add(new Tuple<string, int>(p.playedStructure[i].name, p.playedStructure[i].id));
                }
            }
        }
    }

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

        public PlayForFreeInformation(Player p, char mode)
        {
            this.mode = mode;

            //Olympia information
            //get hand information to play hand cards for free
            if (mode == 'O')
            {
                cards = new Tuple<string, int>[p.numOfHandCards];

                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    cards[i] = new Tuple<string, int>(p.hand[i].name, p.hand[i].id);
                }
            }
            //Rome information
            //get Leader pile information
            else if (mode == 'R')
            {
                cards = new Tuple<string, int>[p.leadersPile.Count];

                for (int i = 0; i < p.leadersPile.Count; i++)
                {
                    cards[i] = new Tuple<string, int>(p.leadersPile[i].name, p.leadersPile[i].id);
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

        public CourtesanGuildInformation(Player p)
        {
            card = new List<Tuple<string, int>>();

            //get the left neighbours Leaders cards
            for (int i = 0; i < p.leftNeighbour.numOfPlayedCards; i++)
            {
                if (p.leftNeighbour.playedStructure[i].colour == "White")
                {
                    card.Add(new Tuple<string, int>(p.leftNeighbour.playedStructure[i].name, p.leftNeighbour.playedStructure[i].id));
                }
            }

            //get the right neighbours Leader cards
            for (int i = 0; i < p.rightNeighbour.numOfPlayedCards; i++)
            {
                if (p.rightNeighbour.playedStructure[i].colour == "White")
                {
                    card.Add(new Tuple<string, int>(p.rightNeighbour.playedStructure[i].name, p.rightNeighbour.playedStructure[i].id));
                }
            }
        }
    }
}
