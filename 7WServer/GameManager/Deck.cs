using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.IO;
//using System.Reflection;

namespace SevenWonders
{
    public class Deck
    {
        //array of cards, which will represent the cards in the deck
        List<Card> card = new List<Card>();

        /// <summary>
        /// Load the cards by reading the File.
        /// Add Card objects to the card array
        /// </summary>
        /// <param name="cardFile"></param>
        public Deck(List<Card> cardList, int age, int numOfPlayers)
        {
            // Create the card list for this age & number of players
            foreach (Card c in cardList)
            {
                int nToAdd = c.GetNumCardsAvailble(age, numOfPlayers);

                for (int i = 0; i < nToAdd; ++i)
                {
                    card.Add(c);
                }
            }
        }

        //find and remove all unused cards Guild cards
        public void removeAge3Guilds(int nPlayers)
        {
            //shuffle first to randomize the locations of the guild cards in the deck
            shuffle();

            //find and remove the appropriate amount of guild cards, based on how many players there are
            // int numOfPurpleCardsToRemove = 7 - numPlayers;
            int nGuildsToRemove = 8 - nPlayers;      // should be *8* - numPlayers, no?  Old code wasn't removing enough Guild structures.

            for (int i = card.Count - 1; i >= 0 && nGuildsToRemove > 0; --i)
            {
                if (card[i].structureType == StructureType.Guild)
                {
                    card.RemoveAt(i);
                    --nGuildsToRemove;
                }
            }

            /*
            // card.RemoveAll(item => item.structureType == Card.StructureType.Guild);

            // a forward-iterator doesn't work
            foreach (Card c in card)
            {
                if (c.structureType == Card.StructureType.Guild)
                {
                    card.Remove(c);
                    numRemoved++;
                    if (numRemoved == numOfPurpleCardsToRemove)
                    {
                        return;
                    }
                }
            }

            for (int i = 0; i < card.Count; i++)
            {
                if (card[i].structureType == Card.StructureType.Guild)
                {
                    card.RemoveAt(i);
                    numRemoved++;
                    if (numRemoved == numOfPurpleCardsToRemove)
                    {
                        return;
                    }
                }
            }
            */
        }

        public int numOfCards()
        {
            return card.Count;
        }

        //shuffle the cards in the deck
        public void shuffle()
        {
            var c = Enumerable.Range(0, card.Count);
            var shuffledcards = c.OrderBy(a => Guid.NewGuid()).ToArray();

            List<Card> d = new List<Card>(card.Count);

            for (int i = 0; i < card.Count; ++i)
            {
                d.Add(card[shuffledcards[i]]);
            }

            card = d;
            /*
            JDF - old code
            Random random = new Random();

            //swap the position of 2 random cards 300 times
            for (int i = 0; i < 300; i++)
            {
                int random1 = random.Next(0, card.Count);
                int random2 = random.Next(0, card.Count);
                Card temp = card[random1];
                card[random1] = card[random2];
                card[random2] = temp;
            }
            */
        }

        /*
        /// <summary>
        /// pop a Random card from the Card array
        /// </summary>
        /// <returns></returns>
        public Card popRandomCard()
        {
            Random random = new Random();

            //get a random number
            int randomNum = random.Next(0, card.Count);

            //get the random card
            Card randomCard = card[randomNum];

            //remove the random card
            card.RemoveAt(randomNum);

            //return the random card
            return randomCard;
        }
        */

        public Card GetTopCard()
        {
            Card topCard = card.First();

            //remove the random card
            card.RemoveAt(0);

            //return the random card
            return topCard;
        }

        /*
        /// <summary>
        /// Return Deck that have all cards in Player's  removed from unusedDeck
        /// Used by Rome B stage 1
        /// </summary>
        /// <param name="usedDeck"></param>
        /// <param name="unusedDeck"></param>
        public static Deck removeDuplicate(Deck usedDeck, Deck unusedDeck)
        {
            Deck newDeck = unusedDeck;

            for (int i = 0; i < usedDeck.numOfCards(); i++)
            {
                for (int j = 0; j < newDeck.numOfCards(); j++)
                {
                    if (usedDeck.card[i].id == newDeck.card[j].id)
                    {
                        newDeck.card.RemoveAt(j);
                        break;
                    }
                }
            }

            return newDeck;
        }
        */
        }
    }
