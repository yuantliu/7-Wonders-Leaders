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
        // Used by the server
        public static string ObjectToString(object obj)
        {
            MemoryStream ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, obj);
            return Convert.ToBase64String(ms.ToArray());
        }

        // Used on the client side
        public static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;
            return new BinaryFormatter().Deserialize(ms);
        }
    }

    /// <summary>
    /// Serializable object representing only ids of current hands. No associated available actions are here.
    /// This is only used by the Recruitment phase (Age 0)
    /// </summary>
    [Serializable]
    public class RecruitmentPhaseInformation
    {
        public string[] ids;

        public RecruitmentPhaseInformation(IPlayer p)
        {
            /*
            ids = new string[p.GetNumCardsInHand()];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = p.GetCard(i).name;
            }
            */
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
        // public Tuple<string, int>[] cards;
        public Tuple<string, string>[] cards;       // TODO: get rid of this mapping.  It's not needed any more

        public PlayForFreeInformation(IPlayer p, char mode)
        {
            this.mode = mode;

            //Olympia information
            //get hand information to play hand cards for free
            if (mode == 'O')
            {
                throw new Exception();
                /*
                // cards = new Tuple<string, int>[p.GetNumCardsInHand()];
                cards = new Tuple<string, string>[p.GetNumCardsInHand()];

                for (int i = 0; i < p.GetNumCardsInHand(); i++)
                {
                    cards[i] = new Tuple<string, string>(p.GetCard(i).name, p.GetCard(i).name);
                }
                */
            }
            /*
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
            */
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Class sent for CourtesanUI to process
    /// Contains name and id of neighbouring played Leader cards, represented in a List of string-int Tuples
    /// </summary>
    [Serializable]
    public class CourtesanGuildInformation
    {
        /*
        // TODO: remove this mapping (no longer needed)
        public List<Tuple<string, string>> card;

        public CourtesanGuildInformation(IPlayer p)
        {
            card = new List<Tuple<string, string>>();

            //get the left neighbours Leaders cards
            for (int i = 0; i < p.GetLeftNeighbour().GetNumberOfPlayedCards(); i++)
            {
                if (p.GetLeftNeighbour().GetCardPlayed(i).structureType == StructureType.Leader)
                {
                    card.Add(new Tuple<string, string>(p.GetLeftNeighbour().GetCardPlayed(i).name, p.GetLeftNeighbour().GetCardPlayed(i).name));
                }
            }

            //get the right neighbours Leader cards
            for (int i = 0; i < p.GetRightNeighbour().GetNumberOfPlayedCards(); i++)
            {
                if (p.GetRightNeighbour().GetCardPlayed(i).structureType == StructureType.Leader)
                {
                    card.Add(new Tuple<string, string>(p.GetRightNeighbour().GetCardPlayed(i).name, p.GetRightNeighbour().GetCardPlayed(i).name));
                }
            }
        }
        */
    }

#if FALSE

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
        public Cost cardCost;

        //[0] represents left player
        //[1] represents mid player (the user)
        //[2] represents right player
        public PlayerCommerceInfo[] playerCommerceInfo = new PlayerCommerceInfo[3];

        //how many coins does the (middle) player have?
        public int playerCoins;

        //what card is being played/used
        public string structureName;

        public bool isStage = false;

        public CommerceInformation(IPlayer left, IPlayer centre, IPlayer right, bool discountApplies, string structureName, Cost cardCost, bool isStage)
        {
            this.hasDiscount = discountApplies;
            this.cardCost = cardCost;
            leftRawMarket = centre.GetLeftRaw();
            leftManuMarket = centre.GetLeftManu();
            rightRawMarket = centre.GetRightRaw();
            rightManuMarket = centre.GetRightManu();

            playerCoins = centre.GetCoin();
            this.structureName = structureName;
            this.isStage = isStage;

            //fill the PlayercommerceInfo for all 3 relevant players
            playerCommerceInfo[0] = new PlayerCommerceInfo(left);
            playerCommerceInfo[1] = new PlayerCommerceInfo(centre);
            playerCommerceInfo[2] = new PlayerCommerceInfo(right);
        }
    }
#endif
}
