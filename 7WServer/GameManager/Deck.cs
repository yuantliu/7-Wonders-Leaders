using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public class Deck
    {
        //array of cards, which will represent the cards in the deck
        private List<Card> card { get; set; }

        private int numPlayers;

        /// <summary>
        /// Load the cards by reading the File.
        /// Add Card objects to the card array
        /// </summary>
        /// <param name="cardFile"></param>
        public Deck(String cardFile, int numOfPlayers)
        {
            String currentPath = Environment.CurrentDirectory;

            numPlayers = numOfPlayers;

            //open the given cardFile
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(currentPath + @"\Resources\cardText\" + cardFile);
                //check that it is a file containing 7 Wonders card information by checking if the first line says "7W"
                //if it is not, then throw an IO Exception
                if (file.ReadLine() != "Yes this is a 7W Card File!")
                {
                    throw new System.IO.IOException();
                }

                //initialise the final card List
                card = new List<Card>();

                String line = file.ReadLine();
                //Read the file, create cards, until we have reached the END

                while(true)
                {
                    int id = int.Parse(file.ReadLine());

                    String name = file.ReadLine();

                    int age = int.Parse(file.ReadLine());

                    int numberOfPlayers = int.Parse(file.ReadLine());

                    String cost = file.ReadLine();

                    String freePreq = file.ReadLine();

                    String colour = file.ReadLine();

                    String effect = file.ReadLine();

                    //ignore the path
                    //we actually don't need it
                    file.ReadLine();

                    //create the card with the extracted parameters

                    if(numberOfPlayers <= numPlayers)
                        card.Add(new Card(id, name, age, numberOfPlayers, cost, freePreq, colour, effect));
                    

                    //now I should either have a - or an END
                    //extract the next line and recheck the loop condition
                    line = file.ReadLine();

                    if (String.Equals(line, "END"))
                    {
                        //END reached. Close the file.
                        file.Close();
                        break;
                    }
                }
            }
            catch( System.IO.IOException )
            {
                Console.WriteLine("There was an error reading " + cardFile + ". Does it exist and is it in the right format?");
            }
        }


        //find and remove all unused cards Guild cards
        public void removeUnusedCards()
        {
            //if the current deck is not Age 3, then it is necessary to perform this operation
            if (card[0].age != 3)
                return;

            //shuffle first
            shuffle();

            //find and remove the appropriate amount of guild cards, based on how many players there are
            int numOfPurpleCardsToRemove = 7 - numPlayers;
            int numRemoved = 0;

            if (numOfPurpleCardsToRemove == 0) return;

            for(int i = 0; i < card.Count; i++)
            {
                if (card[i].colour == "Purple")
                {
                    card.RemoveAt(i);
                    numRemoved++;
                    if (numRemoved == numOfPurpleCardsToRemove)
                    {
                        return;
                    }
                }
            }
        }

        public int numOfCards()
        {
            return card.Count;
        }

        //shuffle the cards in the deck
        public void shuffle()
        {
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
        }

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
    }
}
