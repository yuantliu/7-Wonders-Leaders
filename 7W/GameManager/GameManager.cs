using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public class GameManager
    {
        public int numOfPlayers { get; set; }
        public int numOfAI { get; set; }

        //the total number of boards that are in 7 Wonders
        protected int board_amount;

        public GMCoordinator gmCoordinator;


        //Current age
        public int currentAge;
        //Current turn
        public int currentTurn;

        public Player[] player;
        //All possible decks
        private IList<Board> board;

        public Deck[] deck;
        public Deck currentDeck { get; set; }

        public Card[] discardPile;
        int numDiscardPile;

        public bool esteban = false;

        public bool gameConcluded { get; set; }

        /// <summary>
        /// Shared constructor for GameManager and LeadersGameManager
        /// Common begin of game tasks that are shared amongst all versions of 7W
        /// </summary>
        /// <param name="gmCoordinator"></param>
        public GameManager(GMCoordinator gmCoordinator, int numOfPlayers, String []playerNicks, int numOfAI, char []AIStrats)
        {
            this.gmCoordinator = gmCoordinator;

            //set the maximum number of players in the game to numOfPlayers + numOfAI
            player = new Player[numOfPlayers + numOfAI];
            this.numOfPlayers = numOfPlayers;
            this.numOfAI = numOfAI;

            //set the discard pile
            discardPile = new Card[300];
            numDiscardPile = 0;

            //set the game to not finished, since we are just starting
            gameConcluded = false;

            //Vanilla only Initialisation tasks
            //check if the current class is LeadersGameManager or not
            //if not, then load the other vanilla only initilisation tasks
            if (this is LeadersGameManager == false)
            {
                vanillaGameManagerInitialisation(playerNicks, AIStrats);
            }
        }

        /// <summary>
        /// Set up the neighbours of each players
        /// </summary>
        /// <param name="numOfPlayers"></param>
        /// <param name="numOfAI"></param>
        protected void setPlayerPosition(int numOfPlayers, int numOfAI)
        {
            //set each Player's left and right neighbours
            //this determines player positioning
            //UC-19: R1
            //assign player positions
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                if (i == 0)
                {
                    player[i].setNeighbours(player[numOfPlayers + numOfAI - 1], player[1]);
                }
                else if (i == numOfPlayers + numOfAI - 1)
                {
                    player[i].setNeighbours(player[i - 1], player[0]);
                }
                else
                {
                    player[i].setNeighbours(player[i - 1], player[i + 1]);
                }
            }
        }

        /*
         * intialisation tasks for the vanilla manager
         */
        private void vanillaGameManagerInitialisation(string []playerNicks, char[] AIStrats)
        {
            //vanilla starts at age 1. It does not have the Leaders recruitment phase, which is age 0
            currentAge = 1;
            currentTurn = 1;

            //AI initialisation
            this.numOfAI = numOfAI;

            //player initialisation
            for (int i = 0; i < numOfPlayers; i++)
            {
                player[i] = new Player(playerNicks[i], false, this);
            }

            //vanilla has 14 boards
            board_amount = 14;

            //initialize the vanilla boards objects
            //does not assign the boards to players yet
            createBoards("boards.txt");

            //creating the vanilla AIs
            for (int i = numOfPlayers; i < numOfAI + numOfPlayers; i++)
            {
                player[i] = createAI("AI" + (i + 1), AIStrats[i-numOfPlayers]);
            }

            //set up the player positions
            setPlayerPosition(numOfPlayers, numOfAI);
        }

        /// <summary>
        /// Creating a vanilla AI controlled Player class
        /// </summary>
        /// <param name="name"></param>
        protected virtual Player createAI(String name, char strategy)
        {
            Player thisAI = new Player(name, true, this);
            switch (strategy)
            {
                case '0':
                    thisAI.AIBehaviour = new AIMoveAlgorithm0();
                    break;
                case '1':
                    thisAI.AIBehaviour = new AIMoveAlgorithm1();
                    break;
                case '2':
                    thisAI.AIBehaviour = new AIMoveAlgorithm2();
                    break;
                case '3':
                    thisAI.AIBehaviour = new AIMoveAlgorithm3();
                    break;
                case '4':
                    thisAI.AIBehaviour = new AIMoveAlgorithm4();
                    break;
            }

            return thisAI;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////

        



        /// <summary>
        /// Beginning of Session Actions for Vanilla game
        /// 1) Distribute a random board and 3 coins to all
        /// 2) Remove from all decks card that will not be used in the game
        /// </summary>
        /// <param name="numOfPlayers + numOfAI"></param>
        /// 
        public virtual void beginningOfSessionActions()
        {
            //distribute a random board and 3 coins to all and give the player their free resource
            //send the board display at this point
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                player[i].playerBoard = popRandomBoard();
                gmCoordinator.sendMessage(player[i], "b" + player[i].playerBoard.name);
                player[i].storeAction("13$");
                player[i].storeAction("11" + player[i].playerBoard.freeResource);
                player[i].executeAction(this);
            }

            //initialize, load, and remove unused cards
            //there are 3 decks, but load 4, deck 0 (leader phase) will not be used.
            deck = new Deck[4];
            //Read the textfile information and initialize the decks according to the information
            for (int i = 1; i < deck.Length; i++)
            {
                //deck[1] is age 1. deck[2] is age 2 ....
                deck[i] = new Deck("age" + i + "cards.txt", numOfAI + numOfPlayers);
                deck[i].removeUnusedCards();
            }

            //deal the cards for the first age to the players
            //currentAge not incremented?
            dealDeck(currentAge);
        }

        /// <summary>
        /// End of age actions
        /// 1) Calculate and distribute conflict tokens
        /// 2) Re-enable Olympia
        /// 3) Take player's remaining card and deposit it to the discard pile
        /// 4) Go to the next 
        /// </summary>
        public void endOfAgeActions()
        {
            //distribute tokens
            distributeConflictTokens();

            //reenable Olympia and Babylon
            //disable Halicarnassus
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                //Olympia always reactivate after every age.
                if (player[i].playerBoard.name == "OA" && player[i].currentStageOfWonder >= 2)
                {
                    player[i].olympiaPowerEnabled = true;
                }

                //always disables Halicarnassus after every age.
                player[i].usedHalicarnassus = true;

                //always reactivate Babylon after every age
                if (player[i].playerBoard.name == "BB" && player[i].currentStageOfWonder >= 2)
                {
                    player[i].usedBabylon = false;
                }
            }

            //take all player's remaining cards, deposit it to the discard pile
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                //if they still have a card
                if (player[i].numOfHandCards >= 1)
                {
                    //put it in the bin
                    discardPile[numDiscardPile++] = player[i].hand[0];
                    player[i].numOfHandCards = 0;
                }
            }
        }

        /// <summary>
        /// calculate and distribute conflict tokens
        /// </summary>
        public void distributeConflictTokens()
        {
            //go through each player and compare to the next person
            //remember the first player and keep follow the right
            //when first player is encountered again, all conflict tokens are distributed

            //the amount of conflict tokens one would get depends on the number of shields

            for(int i = 0; i < numOfPlayers + numOfAI; i++)
            {

                //if the current player's shield is greater than the next person, increase conflicttoken by the appropriate age
                //if less, get a losstoken
                if (player[i].shield > player[i].rightNeighbour.shield)
                {
                    if (currentAge == 1)
                    {
                        player[i].conflictTokenOne += 1;
                    }
                    else if (currentAge == 2)
                    {
                        player[i].conflictTokenTwo += 1;
                    }
                    else if (currentAge == 3)
                    {
                        player[i].conflictTokenThree += 1;
                    }

                    //check if player has played card 220: gain 2 coins for every victory point gained
                    //give 2 coins if so
                    if (player[i].hasIDPlayed(220))
                    {
                        player[i].coin += 2;
                    }

                    //check if right neighbour has played card 232: return conflict loss token received
                    //if no, receive lossToken
                    //if yes, do not get lossToken, instead, give lossToken to winner
                    if (player[i].rightNeighbour.hasIDPlayed(232) == false)
                    {
                        player[i].rightNeighbour.lossToken++;
                    }
                    else
                    {
                        //the loser is rightNeighbour
                        //the winner is current player. current player will get the loss token
                        player[i].lossToken++;
                    }
                }
                else if (player[i].shield < player[i].rightNeighbour.shield)
                {
                    if (currentAge == 1)
                    {
                        player[i].rightNeighbour.conflictTokenOne += 1;
                    }
                    else if (currentAge == 2)
                    {
                        player[i].rightNeighbour.conflictTokenTwo += 1;
                    }
                    else if (currentAge == 3)
                    {
                        player[i].rightNeighbour.conflictTokenThree += 1;
                    }

                    //check if player has played card 220: gain 2 coins for every victory point gained
                    //give 2 coins if so
                    if (player[i].rightNeighbour.hasIDPlayed(220))
                    {
                        player[i].rightNeighbour.coin += 2;
                    }

                    //check if I have played card 232: return conflict loss token received
                    //if no, receive lossToken
                    //if yes, do not get lossToken, instead, give lossToken to rightNeighbour
                    if (player[i].hasIDPlayed(232) == false)
                    {
                        player[i].lossToken++;
                    }
                    else
                    {
                        //the loser is rightNeighbour
                        //the winner is current player. current player will get the loss token
                        player[i].rightNeighbour.lossToken++;
                    }
                }

            }
        }

        /// <summary>
        /// End of game actions
        /// Calculate the score and proclaim the winner
        /// </summary>
        protected void endOfSessionActions()
        {
            int maxScore = 0;
            String winner = "ERROR";

            //execute the end of game actions for all players
            //find the maximum final score
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                player[i].executeEndOfGameActions();

                if (maxScore < player[i].finalScore())
                {
                    maxScore = player[i].finalScore();
                    winner = player[i].nickname;
                }
            }

            //broadcast the individual scores
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                for (int j = 0; j < numOfPlayers + numOfAI; j++)
                {
                    gmCoordinator.sendMessage(player[i], "#" + player[j].nickname + " scored " + player[j].finalScore() + " points.");
                }
            }

            //broadcast in chat the winner
            for(int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                gmCoordinator.sendMessage(player[i], "#" + winner + " is the winner with " + maxScore + " points!");
            }

            
        }

        ///////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Shuffle and Deal a deck to all Players
        /// </summary>
        /// <param name="d"></param>
        protected void dealDeck(int currentAge)
        {
            //shuffle the deck
            deck[currentAge].shuffle();

            //if the current deck is 0, then that means we are dealing with the leaders deck
            int numCardsToDeal;

            if (currentAge == 0)
            {
                numCardsToDeal = 4;
            }
            else
            {
                numCardsToDeal = 7;
            }

            //deal cards to each Player from Deck d
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                for (int j = 0; j < numCardsToDeal; j++)
                {
                    Card c = deck[currentAge].popRandomCard();
                    player[i].addHand(c);
                }
            }
        }

        /// <summary>
        /// Inherited class that will return to the caller class and subclasses a set of 14 or 16 boards
        /// Depending on the specified boardfile
        /// Vanilla: boards.txt
        /// Leaders: leadersboards.txt
        /// </summary>
        protected void createBoards(string filename)
        {
            board = new List<Board>();

            //read from the file
            String currentPath = Environment.CurrentDirectory;

            //open the board text file
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(currentPath + "\\boardText\\" + filename);

                if (file.ReadLine() != "This is The Board Script file!")
                {
                    throw new System.IO.IOException();
                }

                //start to parse the file
                for (int i = 0; i < board_amount; i++)
                {
                    string name = file.ReadLine();
                    int numOfStages = int.Parse(file.ReadLine());
                    char freeresource = file.ReadLine()[0];
                    string[] cost = new string[numOfStages];
                    string[] effect = new string[numOfStages];

                    for (int j = 0; j < numOfStages; j++)
                    {
                        cost[j] = file.ReadLine();
                        effect[j] = file.ReadLine();
                    }

                    board.Add(new Board(name, numOfStages, freeresource, cost, effect));

                    file.ReadLine();
                }

                file.Close();
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Cannot load board text file.");
            }
        }

        /// <summary>
        /// Return a random board, popping it from the array of Boards initialially created in createBoards(String filename)
        /// </summary>
        /// <returns></returns>
        protected Board popRandomBoard()
        {
            int index = (new Random()).Next(0, board.Count);

            Board randomBoard = board[index];

            board.RemoveAt(index);

            return randomBoard;
        }

        

        /// <summary>
        /// build a structure from hand, given the Card id number and the Player
        /// </summary>
        public void buildStructureFromHand(int id, string playerNickname)
        {
            //Find the Player object given the playerNickname
            Player p = playerFromNickname(playerNickname);

            //Find the card with the id number
            Card c = null;
            for (int i = 0; i < p.numOfHandCards; i++)
            {
                //found the right card
                if (p.hand[i].id == id)
                {
                    c = p.hand[i];

                    //remove it from hand
                    for (int j = i; j < p.numOfHandCards - 1; j++)
                    {
                        p.hand[j] = p.hand[j + 1];
                    }

                    p.numOfHandCards--;

                    break;
                }
            }

            //add the card to played card structure
            p.addPlayedCardStructure(c);
            //store the card's action
            p.storeAction(c.effect);

            //if the structure played costs money, deduct it
            //check if the Card costs money
            int costInCoins = 0;
            for (int i = 0; i < c.cost.Length; i++)
            {
                if (c.cost[i] == '$') costInCoins++;
            }

            //if player has card 217: free leaders, then leaders are free. add the appropriate amount of coins first to offset the deduction
            //OR
            //if player has Rome A, then leaders are free. (board has D resource (big discount))
            if ((p.hasIDPlayed(217) == true || p.playerBoard.freeResource == 'D') && c.colour == "White")
            {
                p.storeAction("1" + costInCoins + "$");
            }

            //if player has Rome B, then playing leaders will refund a 2 coin discount
            if (p.playerBoard.freeResource == 'd' && c.colour == "White")
            {
                //give 2 coins back if the card cost more than 2
                //else give less than 2 coins back
                if (c.cost.Length >= 2)
                {
                    p.storeAction("12$");
                }
                else
                {
                    p.storeAction("1" + c.cost.Length + "$");
                }
            }

            //if player's neighbour has Rome B, then refund a 1 coin discount instead
            else if ((p.leftNeighbour.playerBoard.freeResource == 'd' || p.rightNeighbour.playerBoard.freeResource == 'd') && c.colour == "White")
            {
                if (c.cost.Length >= 1)
                {
                    p.storeAction("11$");
                }
            }

            //store the deduction
            p.storeAction("$" + costInCoins);

            //determine if the player should get 2 coins for having those leaders (get 2 coins for playing a yellow and playing a pre-req
            giveCoinFromLeadersOnBuild(p, c);
        }

        /// <summary>
        /// Determines if 2 coins should be given for playing a card. Give 2 coins when the appropriate leader was played.
        /// </summary>
        private void giveCoinFromLeadersOnBuild(Player p, Card c)
        {
            for (int i = 0; i < p.numOfPlayedCards; i++)
            {
                //235 - 2 coin for yellow card played
                if (p.playedStructure[i].id == 235)
                {
                    if (c.colour == "Yellow")
                    {
                        p.storeAction("12$");
                        break;
                    }
                }
            }

            for (int i = 0; i < p.numOfPlayedCards; i++)
            {
                //234 - 2 coins if played from a pre-requisite
                if (p.playedStructure[i].id == 234 && c.id != 234)
                {
                    for (int j = 0; j < p.numOfPlayedCards; j++)
                    {
                        if (c.freePreq == p.playedStructure[j].name)
                        {
                            p.storeAction("12$");
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Player decides to buildStructure from Commerce WIndow
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="commerceInformation"></param>
        public void buildStructureFromCommerce(string nickname, string commerceInformation)
        {
            Player p = playerFromNickname(nickname);

            int index = 0;
            String idS = "";
            while (commerceInformation[index] != '_') idS += commerceInformation[index++];

            index++;
            int id = int.Parse(idS);


            //Find the card with the id number
            Card c = null;
            for (int i = 0; i < p.numOfHandCards; i++)
            {
                //found the right card
                if (p.hand[i].id == id)
                {
                    c = p.hand[i];

                    //remove it from hand
                    for (int j = i; j < p.numOfHandCards - 1; j++)
                    {
                        p.hand[j] = p.hand[j + 1];
                    }

                    p.numOfHandCards--;

                    break;
                }
            }

            //add the card to played card structure
            p.addPlayedCardStructure(c);
            //store the card's action
            p.storeAction(c.effect);


            //decrease the cost of the commerce from the players coins
            idS = "";
            while (commerceInformation[index] != '_') idS += commerceInformation[index++];

            index++;
            int decreaseCoins = int.Parse(idS);


            //store the deduction
            p.storeAction("$" + decreaseCoins);

            //give the coins that neigbours earned from commerce
            //first left neighbor

            idS = "";
            while (commerceInformation[index] != '_') idS += commerceInformation[index++];

            index++;
            int coinsForLeftNeigbour = int.Parse(idS);

            //store action
            p.leftNeighbour.storeAction("1" + coinsForLeftNeigbour + "$");


            //for right neighbor
            idS = "";
            while (commerceInformation[index] != '_') idS += commerceInformation[index++];

            index++;
            int coinsForRightNeigbour = int.Parse(idS);

            //storeAction
            p.rightNeighbour.storeAction("1" + coinsForRightNeigbour + "$");

            //determine if the player should get 2 coins for having those leaders (get 2 coins for playing a yellow and playing a pre-req
            giveCoinFromLeadersOnBuild(p, c);

            //Leaders: if Player has card 209 (gain 1 coin for using commerce per neighbouring player)
            //then gain 1 coin
            if (this is LeadersGameManager)
            {
                if (p.hasIDPlayed(209))
                {
                    if (coinsForLeftNeigbour != 0) p.storeAction("11$");
                    if (coinsForRightNeigbour != 0) p.storeAction("11$");
                }
            }
        }

        /// <summary>
        /// build a structure from a card in discard pile, given the Card id number and the Player
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        public void buildStructureFromDiscardPile(int id, string playerNickname)
        {
            //Find the Player object given the playerNickname
            Player p = playerFromNickname(playerNickname);

            //if the structure costed money, reimburse the money
            //check if the Card costs money
            int costInCoins = 0;

            for (int i = 0; i < numDiscardPile; i++)
            {
                //found the card
                if (discardPile[i].id == id)
                {
                    //count how many $ signs in the cost. Each $ means 1 coin cost
                    for (int j = 0; j < discardPile[i].cost.Length; j++)
                    {
                        if (discardPile[i].cost[j] == '$')
                        {
                            costInCoins++;
                        }
                    }

                    break;
                }
            }

            //store the reimbursement
            p.storeAction("1" + costInCoins + "$");

            //Find the card with the id number
            Card c = null;

            for (int i = 0; i < numDiscardPile; i++)
            {
                //found the right card
                if (discardPile[i].id == id)
                {
                    c = discardPile[i];
                    //remove it from the discard pile
                    for (int j = i; j < numDiscardPile - 1; j++)
                    {
                        discardPile[j] = discardPile[j + 1];
                    }

                    numDiscardPile--;

                    break;
                }
            }

            //add the card to played card structure
            p.addPlayedCardStructure(c);
            //store the card's action
            p.storeAction(c.effect);

            //determine if the player should get 2 coins for having those leaders (get 2 coins for playing a yellow and playing a pre-req
            giveCoinFromLeadersOnBuild(p, c);
        }

        /// <summary>
        /// build a stage of wonder, given the Player
        /// </summary>
        /// <param name="p"></param>
        public virtual void buildStageOfWonder(int id, String nickname)
        {
            Player p = playerFromNickname(nickname);

            //Player has less Stage of Wonder built than the max allowed on his board
            if (p.currentStageOfWonder < p.playerBoard.numOfStages)
            {
                //Find the card with the id number
                Card c = null;
                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    //found the right card
                    if (p.hand[i].id == id)
                    {
                        c = p.hand[i];

                        //remove it from hand
                        for (int j = i; j < p.numOfHandCards - 1; j++)
                        {
                            p.hand[j] = p.hand[j + 1];
                        }

                        p.numOfHandCards--;

                        break;
                    }
                }

                p.storeAction(p.playerBoard.effect[p.currentStageOfWonder++]);

            }
            else
            {
                //Player is attempting to build a Stage of Wonder when he has already built all of the Wonders. Something is wrong. This should never be reached.
                Console.WriteLine("GameManager.buildStageOfWonder(Player p) error");
                throw new System.Exception();
            }
        }

        /// <summary>
        /// discard a given card's id for three coins
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        public void discardCardForThreeCoins(int id, String nickname)
        {
            Player p = playerFromNickname(nickname);

            p.storeAction("13$");

            //Find the card with the id number and find its effects
            Card c = null;
            for (int i = 0; i < p.numOfHandCards; i++)
            {
                //found the right card
                if (p.hand[i].id == id)
                {
                    c = p.hand[i];

                    //remove it from hand
                    for (int j = i; j < p.numOfHandCards - 1; j++)
                    {
                        p.hand[j] = p.hand[j + 1];
                    }

                    p.numOfHandCards--;

                    break;
                }
            }

            //add the card to the discard pile
            discardPile[numDiscardPile++] = c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="information"></param>
        public virtual void buildStageOfWonderFromCommerce(String nickname, String information)
        {
            Player p = playerFromNickname(nickname);

            int index = 0;
            String idS = "";
            while (information[index] != '_') idS += information[index++];

            index++;
            int id = int.Parse(idS);

            //Player has less Stage of Wonder built than the max allowed on his board
            if (p.currentStageOfWonder < p.playerBoard.numOfStages)
            {
                //Find the card with the id number
                Card c = null;
                for (int i = 0; i < p.numOfHandCards; i++)
                {
                    //found the right card
                    if (p.hand[i].id == id)
                    {
                        c = p.hand[i];

                        //remove it from hand
                        for (int j = i; j < p.numOfHandCards - 1; j++)
                        {
                            p.hand[j] = p.hand[j + 1];
                        }

                        p.numOfHandCards--;

                        break;
                    }
                }

                p.storeAction(p.playerBoard.effect[p.currentStageOfWonder++]);

            }
            else
            {
                //Player is attempting to build a Stage of Wonder when he has already built all of the Wonders. Something is wrong. This should never be reached.
                throw new System.Exception();
            }


            //decrease the cost of the commerce from the players coins
            idS = "";
            while (information[index] != '_') idS += information[index++];

            index++;
            int decreaseCoins = int.Parse(idS);


            //store the deduction
            p.storeAction("$" + decreaseCoins);

            //give the coins that neigbours earned from commerce
            //first left neighbor

            idS = "";
            while (information[index] != '_') idS += information[index++];

            index++;
            int coinsForLeftNeigbour = int.Parse(idS);

            //store action
            p.leftNeighbour.storeAction("1" + coinsForLeftNeigbour + "$");

            //for right neighbor
            idS = "";
            while (information[index] != '_') idS += information[index++];

            index++;
            int coinsForRightNeigbour = int.Parse(idS);

            //storeAction
            p.rightNeighbour.storeAction("1" + coinsForRightNeigbour + "$");

            //Leaders: if Player has card 209 (gain 1 coin for using commerce per neighbouring player)
            //then gain 1 coin
            if (this is LeadersGameManager)
            {
                if (p.hasIDPlayed(209))
                {
                    if (coinsForLeftNeigbour != 0) p.storeAction("11$");
                    if (coinsForRightNeigbour != 0) p.storeAction("11$");
                }
            }
        }

        /// <summary>
        /// Pass remaining cards to neighbour
        /// </summary>
        public void passRemainingCardsToNeighbour()
        {
            if (currentAge % 2 == 1)
            {
                Card[] firstPlayerHand = player[0].hand;

                for (int i = 0; i < numOfPlayers + numOfAI - 1; i++)
                {
                    player[i].hand = player[i + 1].hand;
                }

                player[numOfPlayers + numOfAI - 1].hand = firstPlayerHand;
            }
            else
            {
                Card[] firstPlayerHand = player[numOfPlayers + numOfAI - 1].hand;

                for (int i = numOfPlayers + numOfAI - 1; i > 0; i--)
                {
                    player[i].hand = player[i - 1].hand;
                }

                player[0].hand = firstPlayerHand;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //Utility functions

        /// <summary>
        /// Return the Player object given the nickname
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public Player playerFromNickname(String nickname)
        {
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                if (player[i].nickname == nickname)
                {
                    return player[i];
                }
            }

            throw new Exception();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute the Action of All players
        /// </summary>
        public void executeActionsAtEndOfTurn()
        {
            //make AI moves
            executeAIActions();

            //execute the Actions for each players
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                player[i].executeAction(this);
            }
        }

        private void executeAIActions()
        {
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                if (player[i].isAI)
                {
                    player[i].makeMove(this);
                }
            }
        }

        /// <summary>
        /// Send the main display information for all players
        /// This is called at the beginning of the game and after each turn
        /// </summary>
        public virtual void updateGameUI(Player p)
        {
            //Update the Player Bar Panel
            //send the playerBarPanel information
            gmCoordinator.sendMessage(p, "B" + Marshaller.ObjectToString(new PlayerBarInformation(player)));
            //send the current stage of wonder information and tell it to start up the timer
            gmCoordinator.sendMessage(p, "s" + p.currentStageOfWonder);

            //if player has Olympia power, send the message to enable the Olympia button
            if(p.olympiaPowerEnabled)
            {
                //EO = "Enable Olympia"
                gmCoordinator.sendMessage(p, "EO");
            }
            //if player has Bilkis AND has at least 1 coin, then send the message to enable Bilkis button
            if (p.hasBilkis && p.coin > 0)
            {
                //EB = Enable Bilkis
                gmCoordinator.sendMessage(p, "EB");
            }

            //send current turn information
            gmCoordinator.sendMessage(p, "T" + currentTurn);

            //send the hand panel (action information) for regular ages (not the Recruitment phase i.e. Age 0)
            if (currentAge > 0)
            {
                //prepare to send the HandPanel information
                String handPanelInformationString = "U" + Marshaller.ObjectToString(new HandPanelInformation(p, currentAge));

                //send the Card Panel information to that player
                gmCoordinator.sendMessage(p, handPanelInformationString);
            }

            //send the timer signal if the current Age is less than 4 (i.e. game is still going)
            if (gameConcluded == false)
            {
                gmCoordinator.sendMessage(p, "t");
            }
            else
            {
                gmCoordinator.sendMessage(p, "e");
            }
        }

        /// <summary>
        /// Function to call for updating all player's UIs
        /// </summary>
        public virtual void updateAllGameUI()
        {
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                updateGameUI(player[i]);
            }
        }

        /// <summary>
        /// Whenever a card becomes "Played", UpdatePlayedCardPanel should be called
        /// Sends a message to the client so that a newly played card will show up in the PlayedCardPanel
        /// </summary>
        public void updatePlayedCardPanel(String nickname)
        {
            //get the player
            Player p = playerFromNickname(nickname);

            //create LastPlayedCardInformation from given Player
            String lastPlayedCardInformationString = "P" + Marshaller.ObjectToString(new LastPlayedCardInformation(p));

            gmCoordinator.sendMessage(p, lastPlayedCardInformationString);
        }

        /// <summary>
        /// Player hits the Olympia power button
        /// give the UI information.
        /// </summary>
        /// <param name="p"></param>
        public void sendOlympiaInformation(String nickname)
        {
            Player p = playerFromNickname(nickname);

            //information to be sent
            String information = "O";

            information += Marshaller.ObjectToString(new PlayForFreeInformation(p, 'O'));

            //send the information
            gmCoordinator.sendMessage(p, information);
        }

        /// <summary>
        /// Play a given card id in nickname for free with Olympia
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="id"></param>
        public void playCardForFreeWithOlympia(String nickname, int id)
        {
            Player p = playerFromNickname(nickname);

            //if the structure costed money, reimburse the money
            //check if the Card costs money
            int costInCoins = 0;

            for (int i = 0; i < p.numOfHandCards; i++)
            {
                //found the card
                if (p.hand[i].id == id)
                {
                    //count how many $ signs in the cost. Each $ means 1 coin cost
                    for (int j = 0; j < p.hand[i].cost.Length; j++)
                    {
                        if (p.hand[i].cost[j] == '$')
                        {
                            costInCoins++;
                        }
                    }

                    break;
                }
            }

            //store the reimbursement
            p.storeAction("1" + costInCoins + "$");

            //build the structure
            buildStructureFromHand(id, nickname);

            //disable Olympia
            p.olympiaPowerEnabled = false;
        }

        /// <summary>
        /// give the UI information for Halicarnassus
        /// </summary>
        /// <param name="p"></param>
        public void sendHalicarnassusInformation(String nickname)
        {
            Player p = playerFromNickname(nickname);
            
            //if there are no cards in the discard pile, send it H0, for nothing
            if (numDiscardPile == 0)
            {
                gmCoordinator.sendMessage(p, "H0");
                return;
            }

            

            //information to be sent
            String information = "H";

            //gather the necessary information
            //H_(num of cards)_(id1)&(name)_(id2)&(name)_...(id_last)&(name)|

            information += "_" + numDiscardPile;

            for (int i = 0; i < numDiscardPile; i++)
            {
                information += "_" + discardPile[i].id + "&" + discardPile[i].name;
            }

            information += "|";

            //send the information
            gmCoordinator.sendMessage(p, information);
        }

        /// <summary>
        /// play the card for free from discard pile with Halicarnassus
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="id"></param>
        public void playCardForFreeWithHalicarnassus(string nickname, int id)
        {
            Player p = playerFromNickname(nickname);
            //build from the discard pile
            buildStructureFromDiscardPile(id, nickname);
        }

        /// <summary>
        /// give the UI information for Babylon
        /// </summary>
        /// <param name="nickname"></param>
        public void sendBabylonInformation(string nickname)
        {
            Player p = playerFromNickname(nickname);

            //information to be sent
            String information = "A";

            //look at the last card in hand.
            //send the information about it
            
            //get the last card
            Card lastCard = p.hand[0];
            //get the id
            information += "_" + lastCard.id + "_";
            //get if the card is playable from hand
            information += p.isCardBuildable(lastCard);
            //get if stage is buildable from hand
            information += p.isStageBuildable();

            //send the information
            gmCoordinator.sendMessage(p, information);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nickname"></param>
        public void updateCommercePanel(int id, String nickname, Boolean isStageOfWonderCommerce)
        {

            Player p = playerFromNickname(nickname);

            //Find the card with the id number
            Card c = null;
            for (int i = 0; i < p.numOfHandCards; i++)
            {
                //found the right card
                if (p.hand[i].id == id)
                {
                    c = p.hand[i];
                    break;
                }
            }

            ///////////////////
            ////C(for commerce)_(id of the card)_(card or board cost)_//(current)_(resources)|(leftPlayer)_(resources)|(rightPlayer)_(resources)
            ////////////////

            //discount applicability
            bool hasDiscount;

            if ((c.colour == "Green" && p.hasIDPlayed(202)) ||
               (c.colour == "Blue" && p.hasIDPlayed(207)) ||
               (c.colour == "Red" && p.hasIDPlayed(216)) ||
               (isStageOfWonderCommerce == true && p.hasIDPlayed(212)))
            {
                hasDiscount = true;
            }
            else
            {
                hasDiscount = false;
            }

            //Yunus information
            string commerceInformation = id + "_";

            if (isStageOfWonderCommerce) commerceInformation += p.playerBoard.cost[p.currentStageOfWonder] + "_";
            else commerceInformation += c.cost + "_";

            commerceInformation += p.commerceInformation();


            //add this at the end to decide whether we a stage of wonder commerce or build structure commerce
            if (isStageOfWonderCommerce) commerceInformation += "&";

            //Package the Yunus information and hasDiscount information
            string commercePackageInformation = "C" + Marshaller.ObjectToString(new CommerceInformationPackage(commerceInformation, hasDiscount));

            gmCoordinator.sendMessage(p, commercePackageInformation);
        }

        //return the view detail UI information, given a player name
        public void sendViewDetailInformation(String requesterName, String requestedName)
        {
            Player requester = playerFromNickname(requesterName);
            Player requested = playerFromNickname(requestedName);
            String information = "V";

            ViewDetailsInformation info = new ViewDetailsInformation(requested);
            information += Marshaller.ObjectToString(info);

            gmCoordinator.sendMessage(requester, information);
        }

        protected int numOfPlayersThatHaveTakenTheirTurn = 0;

        /*
         * Player sends "t" message (turn complete) to the GameManager
         * Increment the amount of players that have taken their turn
         * If it is equal to the number of players, then go to the next turn
         */
        public virtual void turnTaken(String nickname)
        {
            Player p = playerFromNickname(nickname);

            //if player doesn't even have Halicarnassus or Babylon board, then no more action is required
            if (p.playerBoard.name != "BB" && p.playerBoard.name[0] != 'H')
            {
                numOfPlayersThatHaveTakenTheirTurn++;
            }
            //if current turn isn't 6, then no more action is required
            else if (currentTurn < 6)
            {
                numOfPlayersThatHaveTakenTheirTurn++;
            }
            //player has the Babylon board and has Babylon power built. Send the babylon information if it is currently the last turn of the Age.
            else if (p.usedBabylon == false)
            {
                //send babylon information here.
                sendBabylonInformation(nickname);

                p.usedBabylon = true;
            }
            //Player has the Halicarnassus board and has built the Halicarnassus power. Send the Halicarnassus information.
            else if (p.usedHalicarnassus == false)
            {
                //send halicarnassus information here.
                sendHalicarnassusInformation(nickname);

                p.usedHalicarnassus = true;
            }

            //Halicarnassus/Babylon action has been taken or not activated. Increment turn as normal.
            else if (p.usedHalicarnassus || p.usedBabylon)
            {
                numOfPlayersThatHaveTakenTheirTurn++;
            }

            //all players have completed their turn
            if (numOfPlayersThatHaveTakenTheirTurn == numOfPlayers)
            {
                //reset the number of players that have taken their turn
                numOfPlayersThatHaveTakenTheirTurn = 0;

                //execute every player's action
                executeActionsAtEndOfTurn();

                //pass the cards to the neighbour
                passRemainingCardsToNeighbour();

                //increment the turn
                currentTurn++;

                //if the current turn is last turn of Age, do end of Age calculation
                if (currentTurn == 7 && currentAge > 0)
                {
                    //perform end of Age actions
                    endOfAgeActions();
                    currentAge++;

                    //if game hasn't ended, then deal deck, reset turn
                    if (currentAge < 4)
                    {
                        dealDeck(currentAge);
                        currentTurn = 1;
                    }
                    //else do end of session actions
                    else
                    {
                        gameConcluded = true;
                        endOfSessionActions();
                    }
                }
                
                updateAllGameUI();
            }
        }

        /// <summary>
        /// A "virtual" function that is of no use to GameManager, and is meant for Leaders to implement
        /// Added here so that GMCoordinator is able to call the Leader's version of it.
        /// Program won't compile without the base class having this.
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="id"></param>
        public virtual void recruitLeader(String nickname, int id)
        {
            throw new NotImplementedException();
        }

        public virtual void bilkisPower(String nickname, byte resource)
        {
            throw new NotImplementedException();
        }

        public virtual void playLeaderForFreeWithRome(string nickname, string message)
        {
            throw new NotImplementedException();
        }

        public virtual void playCourtesansGuild(string nickname, string message)
        {
            throw new NotImplementedException();
        }

        
    }
}
