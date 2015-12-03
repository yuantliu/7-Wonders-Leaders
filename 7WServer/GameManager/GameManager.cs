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

        public GMCoordinator gmCoordinator;

        public int currentAge;

        public int currentTurn;

        // JDF I think player should be a dictionary rather than an array.
        // public Dictionary<string, Player> player;

        public Dictionary<string, Player> player = new Dictionary<string, Player>();

        // A list would be better here.
        private Dictionary<Board.Wonder, Board> board;

        // I'd prefer to use a dictionary, but because there may be 2 (or even 3) of the same card,
        // I'll stay with a List container.
        List<Card> fullCardList = new List<Card>();

        public List<Deck> deckList = new List<Deck>();

        public List<Card> discardPile = new List<Card>();

        public bool esteban = false;

        bool gettingBabylonExtraCard = false;
        bool playingCardFromDiscardPile = false;
        List<Card> savedHandWhenPlayingFromDiscardPile;

        string[] playerNicks;

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
            this.numOfPlayers = numOfPlayers;
            this.numOfAI = numOfAI;
            this.playerNicks = playerNicks;

            //set the game to not finished, since we are just starting
            gameConcluded = false;

            //Vanilla only Initialisation tasks
            //check if the current class is LeadersGameManager or not
            //if not, then load the other vanilla only initilisation tasks
            // if (this is LeadersGameManager == false)
            // {
                vanillaGameManagerInitialisation(AIStrats);
            // }
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
            for (int i = 0; i < player.Count; i++)
            {
                if (i == 0)
                {
                    player[playerNicks[i]].setNeighbours(player[playerNicks[numOfPlayers + numOfAI - 1]], player[playerNicks[i + 1]]);
                }
                else if (i == numOfPlayers + numOfAI - 1)
                {
                    player[playerNicks[i]].setNeighbours(player[playerNicks[i - 1]], player[playerNicks[0]]);
                }
                else
                {
                    player[playerNicks[i]].setNeighbours(player[playerNicks[i - 1]], player[playerNicks[i + 1]]);
                }
            }
        }

        /*
         * intialisation tasks for the vanilla manager
         */
        private void vanillaGameManagerInitialisation(char[] AIStrats)
        {
            //vanilla starts at age 1. It does not have the Leaders recruitment phase, which is age 0
            currentAge = 1;
            currentTurn = 1;

            //AI initialisation
            this.numOfAI = numOfAI;

            //player initialisation
            for (int i = 0; i < numOfPlayers; i++)
            {
                player.Add(playerNicks[i], new Player(playerNicks[i], false, this));
            }

            // load the card list
            using (System.IO.StreamReader file = new System.IO.StreamReader(System.Reflection.Assembly.Load("GameManager").
                GetManifestResourceStream("GameManager.7 Wonders Card list.csv")))
            {
                // skip the header line
                file.ReadLine();

                String line = file.ReadLine();

                while (line != null && line != String.Empty)
                {
                    fullCardList.Add(new Card(line.Split(',')));
                    line = file.ReadLine();
                }
            }

            //initialize the vanilla boards objects
            //does not assign the boards to players yet
            createBoards();

            //creating the vanilla AIs
            for (int i = numOfPlayers; i < numOfAI + numOfPlayers; i++)
            {
                playerNicks[i] = "AI" + (i + 1);
                player.Add(playerNicks[i], createAI(playerNicks[i], AIStrats[i-numOfPlayers]));
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



        public virtual void sendBoardNames()
        {
            string strMsg = string.Empty;

            foreach (Player p in player.Values)
            {
                strMsg += string.Format("&{0}={1}/{2}", p.nickname, p.playerBoard.numOfStages, p.playerBoard.name);
            }

            foreach (Player p in player.Values)
            {
                gmCoordinator.sendMessage(p, "SetBoard" + strMsg);

                p.executeAction(this);
            }
        }

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
            foreach (Player p in player.Values)
            {
                p.playerBoard = popRandomBoard();
                // gmCoordinator.sendMessage(player[i], "b" + player[i].playerBoard.name);
                // player[i].storeAction("13$");
                p.storeAction(new CoinsAndPointsEffect(CoinsAndPointsEffect.CardsConsidered.None, StructureType.Constant, 3, 0));
                // player[i].storeAction("11" + player[i].playerBoard.freeResource);
                p.storeAction(p.playerBoard.freeResource);

                // deferred until the client is ready to accept UI information
                // player[i].executeAction(this);
            }

            //initialize, load, and remove unused cards
            //there are 3 decks, but load 4, deck 0 (leader phase) will not be used.
            //Read the textfile information and initialize the decks according to the information
            for (int i = 1; i < 4; i++)
            {
                //deck[1] is age 1. deck[2] is age 2 ....
                deckList.Add(new Deck(fullCardList, i, numOfAI + numOfPlayers));
            }

            deckList.Find(x => x.age == 3).removeAge3Guilds(numOfAI + numOfPlayers);

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

            string strUpdateMilitaryTokens = "Military";

            foreach (Player p in player.Values)
            {
                int nVictoryTokens = 0;

                switch(currentAge)
                {
                    case 1: nVictoryTokens = p.conflictTokenOne; break;
                    case 2: nVictoryTokens = p.conflictTokenTwo; break;
                    case 3: nVictoryTokens = p.conflictTokenThree; break;
                }

                // this string has the format: Current Age/Victories (0, 1, 2) for the *current* age/total loss tokens so far (0 or more)
                strUpdateMilitaryTokens += string.Format("&{0}={1}/{2}/{3}", p.nickname, currentAge, nVictoryTokens, p.lossToken);
            }

            foreach (Player p in player.Values)
            {
                gmCoordinator.sendMessage(p, strUpdateMilitaryTokens);
            }

            /*
        //reenable Olympia and Babylon
        //disable Halicarnassus
        foreach (Player p in player.Values)
        {
            //Olympia always reactivate after every age.
            if (p.playerBoard.name == "OA" && p.currentStageOfWonder >= 2)
            {
                p.olympiaPowerEnabled = true;
            }

            //always disables Halicarnassus after every age.
            // p.usedHalicarnassus = true;

            //always reactivate Babylon after every age
            if (p.playerBoard.name == "Babylon (B)" && p.currentStageOfWonder >= 2)
            {
                // p.usedBabylon = false;
            }
        }
            */

            // take all player's remaining cards, deposit it to the discard pile
            foreach (Player p in player.Values)
            {
                if (p.olympiaPowerEnabled)
                {
                    p.olympiaPowerAvailable = true;
                    gmCoordinator.sendMessage(p, "EnableFB&Olympia=true");
                }

                //if they still have a card
                if (p.hand.Count >= 1)
                {
                    //put it in the bin
                    discardPile.Add(p.hand[0]);
                }

                p.hand.Clear();
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

            foreach (Player p in player.Values)
            {

                //if the current player's shield is greater than the next person, increase conflicttoken by the appropriate age
                //if less, get a losstoken
                if (p.shield > p.rightNeighbour.shield)
                {
                    if (currentAge == 1)
                    {
                        p.conflictTokenOne += 1;
                    }
                    else if (currentAge == 2)
                    {
                        p.conflictTokenTwo += 1;
                    }
                    else if (currentAge == 3)
                    {
                        p.conflictTokenThree += 1;
                    }

                    //check if player has played card 220: gain 2 coins for every victory point gained
                    //give 2 coins if so
                    if (p.playedStructure.Exists(x => x.name == "Nero"))
                    {
                        throw new Exception();
                        // fix this.  Likely by making the player.GiveConflictToken into a function and
                        // having that function check whether the current player has Nero.
                        // p.coin += 2;
                    }

                    //check if right neighbour has played card 232: return conflict loss token received
                    //if no, receive lossToken
                    //if yes, do not get lossToken, instead, give lossToken to winner
                    if (p.rightNeighbour.playedStructure.Exists(x => x.name == "Tomyris") == false)
                    {
                        p.rightNeighbour.lossToken++;
                    }
                    else
                    {
                        //the loser is rightNeighbour
                        //the winner is current player. current player will get the loss token
                        p.lossToken++;
                    }
                }
                else if (p.shield < p.rightNeighbour.shield)
                {
                    if (currentAge == 1)
                    {
                        p.rightNeighbour.conflictTokenOne += 1;
                    }
                    else if (currentAge == 2)
                    {
                        p.rightNeighbour.conflictTokenTwo += 1;
                    }
                    else if (currentAge == 3)
                    {
                        p.rightNeighbour.conflictTokenThree += 1;
                    }

                    /*
                    //check if player has played card 220: gain 2 coins for every victory point gained
                    //give 2 coins if so
                    if (p.rightNeighbour.playedStructure.Exists(x => x.name == "Nero"))
                    {
                        p.rightNeighbour.coin += 2;
                    }
                    */

                    //check if I have played card 232: return conflict loss token received
                    //if no, receive lossToken
                    //if yes, do not get lossToken, instead, give lossToken to rightNeighbour
                    if (p.playedStructure.Exists(x => x.name == "Tomyris") == false)
                    {
                        p.lossToken++;
                    }
                    else
                    {
                        //the loser is rightNeighbour
                        //the winner is current player. current player will get the loss token
                        p.rightNeighbour.lossToken++;
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
            string strFinalScoreMsg = string.Empty;
            List<Player> playerScores = new List<Player>(numOfPlayers + numOfAI);

            //execute the end of game actions for all players
            //find the maximum final score
            foreach (Player p in player.Values)
            {
                p.executeEndOfGameActions();
                playerScores.Add(p);
            }

            // sort the scores into lowest to highest
            playerScores.Sort(delegate (Player p1, Player p2)
            {
                int victoryPointDiff = p1.victoryPoint - p2.victoryPoint;

                if (victoryPointDiff != 0)
                    return victoryPointDiff;
                else
                    return p1.coin - p2.coin;
            });

            //broadcast the individual scores
            foreach (Player p in playerScores)
            {
                foreach (Player pl in player.Values)
                {
                    gmCoordinator.sendMessage(pl, string.Format("# {0}: {1} points", p.nickname, p.victoryPoint));
                }
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
            Deck deck = deckList.Find(x => x.age == currentAge);
            deck.shuffle();

            //if the current deck is 0, then that means we are dealing with the leaders deck
            int numCardsToDeal;

            numCardsToDeal = currentAge == 0 ? 4 : 7;

            //deal cards to each Player from Deck d
            foreach (Player p in player.Values)
            {
                for (int j = 0; j < numCardsToDeal; j++)
                {
                    Card c = deck.GetTopCard();
                    p.hand.Add(c);
                }
            }
        }

        /// <summary>
        /// Inherited class that will return to the caller class and subclasses a set of 14 or 16 boards
        /// Depending on the specified boardfile
        /// Vanilla: boards.txt
        /// Leaders: leadersboards.txt
        /// </summary>
        protected void createBoards()
        {
            board = new Dictionary<Board.Wonder, Board>(14)
            {
                { Board.Wonder.Alexandria_A, new Board(Board.Wonder.Alexandria_B, "Alexandria (A)", new ResourceEffect(true, "G"), 3) },
                { Board.Wonder.Alexandria_B, new Board(Board.Wonder.Alexandria_A, "Alexandria (B)", new ResourceEffect(true, "G"), 3) },
                { Board.Wonder.Babylon_A, new Board(Board.Wonder.Babylon_B, "Babylon (A)", new ResourceEffect(true, "B"), 3) },
                { Board.Wonder.Babylon_B, new Board(Board.Wonder.Babylon_A, "Babylon (B)", new ResourceEffect(true, "B"), 3) },
                { Board.Wonder.Ephesos_A, new Board(Board.Wonder.Ephesos_B, "Ephesos (A)", new ResourceEffect(true, "P"), 3) },
                { Board.Wonder.Ephesos_B, new Board(Board.Wonder.Ephesos_A, "Ephesos (B)", new ResourceEffect(true, "P"), 3) },
                { Board.Wonder.Giza_A, new Board(Board.Wonder.Giza_B, "Giza (A)", new ResourceEffect(true, "S"), 3) },
                { Board.Wonder.Giza_B, new Board(Board.Wonder.Giza_A, "Giza (B)", new ResourceEffect(true, "S"), 4) },
                { Board.Wonder.Halikarnassos_A, new Board(Board.Wonder.Halikarnassos_B, "Halikarnassos (A)", new ResourceEffect(true, "C"), 3) },
                { Board.Wonder.Halikarnassos_B, new Board(Board.Wonder.Halikarnassos_A, "Halikarnassos (B)", new ResourceEffect(true, "C"), 3) },
                { Board.Wonder.Olympia_A, new Board(Board.Wonder.Olympia_B, "Olympia (A)", new ResourceEffect(true, "W"), 3) },
                { Board.Wonder.Olympia_B, new Board(Board.Wonder.Olympia_A, "Olympia (B)", new ResourceEffect(true, "W"), 3) },
                { Board.Wonder.Rhodos_A, new Board(Board.Wonder.Rhodos_B, "Rhodos (A)", new ResourceEffect(true, "O"), 3) },
                { Board.Wonder.Rhodos_B, new Board(Board.Wonder.Rhodos_A, "Rhodos (B)", new ResourceEffect(true, "O"), 2) },
                /*
                { Board.Wonder.Roma_A, new Board("Roma (A)", null, 3) },
                { Board.Wonder.Roma_B, new Board("Roma (B)", null, 3) },
                */
            };

            // Take the board effects from the card list.

            foreach (Board b in board.Values)
            {
                b.stageCard = new List<Card>(b.numOfStages);

                for (int i = 0; i < b.numOfStages; ++i)
                {
                    Card card = fullCardList.Find(c => c.structureType == StructureType.WonderStage && c.name == b.name && c.wonderStage == i + 1);

                    fullCardList.Remove(card);
                    b.stageCard.Add(card);
                }
            }
        }

        /// <summary>
        /// Return a random board, popping it from the array of Boards initialially created in createBoards(String filename)
        /// </summary>
        /// <returns></returns>
        protected Board popRandomBoard()
        {
            // int index = (new Random()).Next(0, board.Count);
            int index = 11;

            KeyValuePair<Board.Wonder, Board> randomBoard = board.ElementAt(index);

            while(board[randomBoard.Key].inPlay == true)
            {
                ++index;

                if (index > 13)
                    index = 0;

                randomBoard = board.ElementAt(index);
            }

            // Remove the other side (i.e. if we returned the Babylon A, remove Babylon B from
            // the board list)
            board[randomBoard.Key].inPlay = true;
            board[randomBoard.Value.otherSide].inPlay = true;

            return randomBoard.Value;
        }

        public void buildStructureFromHand(string playerNickname, string cardName, string strWonderStage, string strFreeBuild, string strLeftCoins, string strRightCoins)
        {
            Player p = player[playerNickname];

            Card c = p.hand.Find(x => x.name == cardName);

            if (c == null)
                throw new Exception("Received a message from the client to build a card that wasn't in the player's hand.");

            int nLeftCoins = 0, nRightCoins = 0;

            if (strLeftCoins != null)
                nLeftCoins = int.Parse(strLeftCoins);

            if (strRightCoins != null)
                nRightCoins = int.Parse(strRightCoins);

            bool freeBuild = strFreeBuild != null && strFreeBuild == "True";

            buildStructureFromHand(p, c, strWonderStage == "1", freeBuild, nLeftCoins, nRightCoins);
        }

        /// <summary>
        /// build a structure from hand, given the Card id number and the Player
        /// </summary>
        public void buildStructureFromHand(Player p, Card c, bool wonderStage, bool freeBuild = false, int nLeftCoins = 0, int nRightCoins = 0)
        {
            p.hand.Remove(c);

            if (wonderStage)
            {
                if (p.currentStageOfWonder >= p.playerBoard.numOfStages)
                {
                    //Player is attempting to build a Stage of Wonder when he has already built all of the Wonders. Something is wrong. This should never be reached.
                    throw new Exception("GameManager.buildStageOfWonder(Player p) error");
                }

                c = p.playerBoard.stageCard[p.currentStageOfWonder];
                p.currentStageOfWonder++;
            }

            //add the card to played card structure
            p.addPlayedCardStructure(c);
            //store the card's action
            p.storeAction(c.effect);

            if (freeBuild)
            {
                // check that the player
                if (p.olympiaPowerAvailable)
                {
                    p.olympiaPowerAvailable = false;
                }
                else
                {
                    // the player is cheating
                    throw new Exception("You do not have the ability to build a free structure");
                }
            }

            //if the structure played costs money, deduct it
            //check if the Card costs money and add the coins paid to the neighbors for their resources
            int costInCoins = c.cost.coin + nLeftCoins + nRightCoins;

            /*
            for (int i = 0; i < c.cost.Length; i++)
            {
                if (c.cost[i] == '$') costInCoins++;
            }
            */

            //if player has card 217: free leaders, then leaders are free. add the appropriate amount of coins first to offset the deduction
            //OR
            //if player has Rome A, then leaders are free. (board has D resource (big discount))
            if ((p.playedStructure.Exists(x => x.name == "Maecenas") == true /* || p.playerBoard.freeResource == 'D'*/) && c.structureType == StructureType.Leader)
            {
                costInCoins -= c.cost.coin;
            }

            //if player has Rome B, then playing leaders will refund a 2 coin discount
            if (/*p.playerBoard.freeResource == 'd' && */c.structureType == StructureType.Leader)
            {
                //give 2 coins back if the card cost more than 2
                //else give less than 2 coins back
                int coins = c.cost.coin;
                if (c.cost.coin >= 2)
                {
                    coins = 2;
                    // p.storeAction("12$");
                }
                else
                {
                    // JDF.  Not sure this is correct.  Will need to test it.
                    // p.storeAction("1" + c.cost.Length + "$");
                }
                p.storeAction(new CoinEffect(coins));
            }

            /*
            //if player's neighbour has Rome B, then refund a 1 coin discount instead
            else if ((p.leftNeighbour.playerBoard.freeResource == 'd' || p.rightNeighbour.playerBoard.freeResource == 'd') && c.structureType == StructureType.Leader)
            {
                if (c.cost.coin >= 1)
                {
                    p.storeAction(new SimpleEffect(1, '$'));
                    // p.storeAction("11$");
                }
            }
            */

            if (costInCoins != 0)
            {
                p.storeAction(new CoinEffect(-costInCoins));
            }

            if (nLeftCoins != 0)
                p.leftNeighbour.storeAction(new CoinEffect(nLeftCoins));

            if (nRightCoins != 0)
                p.rightNeighbour.storeAction(new CoinEffect(nRightCoins));

            //determine if the player should get 2 coins for having those leaders (get 2 coins for playing a yellow and playing a pre-req
            giveCoinFromLeadersOnBuild(p, c);
        }

        /// <summary>
        /// Determines if 2 coins should be given for playing a card. Give 2 coins when the appropriate leader was played.
        /// </summary>
        private void giveCoinFromLeadersOnBuild(Player p, Card c)
        {
            /*
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
            */
        }

#if FALSE
        /// <summary>
        /// Player finishes conducting commerce to pay for a card
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="commerceInformation"></param>
        public void buildStructureFromCommerce(string nickname, string structureName, int leftcoins, int rightcoins)//string commerceInformation)
        {
            Player p = player[nickname];

            /*
            CommerceClientToServerResponse response = (CommerceClientToServerResponse)Marshaller.StringToObject(commerceInformation);

            string structureName = response.structureName;
            int leftcoins = response.leftCoins;
            int rightcoins = response.rightCoins;
            */

            //Find the card with the id number
            Card c = p.hand.Find(x => x.name == structureName);

            if (c == null)
                throw new Exception("Unexpected card name");

            p.hand.Remove(c);

            //add the card to played card structure
            p.addPlayedCardStructure(c);
            //store the card's action
            p.storeAction(c.effect);

            //charge the player the appropriate amount of coins
            int commerceCost = leftcoins + rightcoins;

            //store the deduction
            // p.storeAction("$" + commerceCost);
            p.storeAction(new CostEffect(commerceCost));

            //give the coins that neigbours earned from commerce
            // p.leftNeighbour.storeAction("1" + leftcoins + "$");
            // p.rightNeighbour.storeAction("1" + rightcoins + "$");
            p.leftNeighbour.storeAction(new SimpleEffect(leftcoins, '$'));
            p.rightNeighbour.storeAction(new SimpleEffect(rightcoins, '$'));


            //determine if the player should get 2 coins for having those leaders (get 2 coins for playing a yellow and playing a pre-req
            giveCoinFromLeadersOnBuild(p, c);

            //Leaders: if Player has card 209 (gain 1 coin for using commerce per neighbouring player)
            //then gain 1 coin
            /*
            if (this is LeadersGameManager)
            {
                if (p.hasIDPlayed(209))
                {
                    if (leftcoins != 0) p.storeAction("11$");
                    if (rightcoins != 0) p.storeAction("11$");
                }
            }
            */
        }

        /// <summary>
        /// Player finishes conducting commerce to pay for a stage of wonder
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="information"></param>
        public virtual void buildStageOfWonderFromCommerce(string nickname, string structureName, int leftcoins, int rightcoins)
        {
            Player p = player[nickname];

            /*
            CommerceClientToServerResponse response = (CommerceClientToServerResponse)Marshaller.StringToObject(information);

            int leftcoins = response.leftCoins;
            int rightcoins = response.rightCoins;
            string structureName = response.structureName;
            */

            //build the stage of wonder
            buildStageOfWonder(structureName, nickname);

            //charge the player the appropriate amount of coins
            int commerceCost = leftcoins + rightcoins;

            //store the deduction
            // p.storeAction("$" + commerceCost);
            p.storeAction(new CostEffect(commerceCost));

            //give the coins that neigbours earned from commerce
            // p.leftNeighbour.storeAction("1" + leftcoins + "$");
            // p.rightNeighbour.storeAction("1" + rightcoins + "$");
            p.leftNeighbour.storeAction(new SimpleEffect(leftcoins, '$'));
            p.rightNeighbour.storeAction(new SimpleEffect(rightcoins, '$'));

            //Leaders: if Player has card 209 (gain 1 coin for using commerce per neighbouring player)
            //then gain 1 coin
            /*
            if (this is LeadersGameManager)
            {
                if (p.hasIDPlayed(209))
                {
                    if (leftcoins != 0) p.storeAction("11$");
                    if (rightcoins != 0) p.storeAction("11$");
                }
            }
            */
        }

        /// <summary>
        /// build a structure from a card in discard pile, given the Card id number and the Player
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        public void buildStructureFromDiscardPile(string name, string playerNickname)
        {
            //Find the Player object given the playerNickname
            Player p = player[playerNickname];

            //if the structure costed money, reimburse the money
            //check if the Card costs money
            int costInCoins = 0;

            throw new NotImplementedException();

            for (int i = 0; i < discardPile.Count; i++)
            {
                //found the card
                if (discardPile[i].name == name)
                {
                    // I think the loop below shoudl be replaced with:
                    costInCoins = discardPile[i].cost.coin;

                    // TODO: figure this out.  Not really sure what they were doing before.
                    throw new NotImplementedException();

                    /*
                    //count how many $ signs in the cost. Each $ means 1 coin cost
                    for (int j = 0; j < discardPile[i].cost.Length; j++)
                    {
                        if (discardPile[i].cost[j] == '$')
                        {
                            costInCoins++;
                        }
                    }
                    */

                    break;
                }
            }

            //store the reimbursement
            p.storeAction(new CoinEffect(costInCoins));

            //Find the card with the id number
            Card c = discardPile.Find(x => x.name == name);
            discardPile.Remove(c);

            //add the card to played card structure
            p.addPlayedCardStructure(c);
            //store the card's action
            p.storeAction(c.effect);

            //determine if the player should get 2 coins for having those leaders (get 2 coins for playing a yellow and playing a pre-req
            giveCoinFromLeadersOnBuild(p, c);
        }
#endif
        public void discardCardForThreeCoins(string nickname, string name)
        {
            Player p = player[nickname];

            discardCardForThreeCoins(p, p.hand.Find(x => x.name == name));
        }

        /// <summary>
        /// discard a given card's id for three coins
        /// </summary>
        /// <param name="id"></param>
        /// <param name="p"></param>
        public void discardCardForThreeCoins(Player p, Card c)
        {
            p.storeAction(new CoinEffect(3));

            //Find the card with the id number and find its effects
            p.hand.Remove(c);

            //add the card to the discard pile
            discardPile.Add(c);
        }

        

        /// <summary>
        /// Pass remaining cards to neighbour
        /// </summary>
        public void passRemainingCardsToNeighbour()
        {
            Player p = player.Values.First();
            List<Card> p1hand = p.hand;

            do
            {
                if (currentAge % 2 == 1)
                {
                    // First and third age the cards are passed to each player's left neighbor
                    p.hand = p.rightNeighbour.hand;
                    p = p.rightNeighbour;
                }
                else
                {
                    p.hand = p.leftNeighbour.hand;
                    p = p.leftNeighbour;
                }

            } while (p != player.Values.First());

            if (currentAge % 2 == 1)
            {
                p.leftNeighbour.hand = p1hand;
            }
            else
            {
                p.rightNeighbour.hand = p1hand;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        //Utility functions

        /// <summary>
        /// Execute the Action of All players
        /// </summary>
        public void executeActionsAtEndOfTurn()
        {
            if (!gettingBabylonExtraCard && !playingCardFromDiscardPile)
            {
                //make AI moves
                executeAIActions();
            }

            //execute the Actions for each players
            foreach (Player p in player.Values)
            {
                p.executeAction(this);
            }
        }

        private void executeAIActions()
        {
            foreach (Player p in player.Values)
            {
                if (p.isAI)
                {
                    p.makeMove(this);
                }
            }
        }

        /// <summary>
        /// Send the main display information for all players
        /// This is called at the beginning of the game and after each turn
        /// </summary>
        public virtual void updateAllGameUI()
        {
            string strCardsPlayed = "CardPlay";
            string strUpdateCoinsMessage = "SetCoins";

            foreach (Player p in player.Values)
            {
                if (p.bUIRequiresUpdating)
                {
                    // TODO: update this to send built Wonder stage updates as well as the cards played panel.
                    Card card = p.playedStructure.Last();

                    if (card.structureType == StructureType.WonderStage)
                    {
                        strCardsPlayed += string.Format("&{0}=WonderStage{1}", p.nickname, card.wonderStage);
                    }
                    else
                    {
                        strCardsPlayed += string.Format("&{0}={1}", p.nickname, card.name);
                    }
                    p.bUIRequiresUpdating = false;
                }
                else
                {
                    strCardsPlayed += string.Format("&{0}={1}", p.nickname, "Discarded");
                }

                strUpdateCoinsMessage += string.Format("&{0}={1}", p.nickname, p.coin);
            }

            foreach (Player p in player.Values)
            {
                gmCoordinator.sendMessage(p, strCardsPlayed);
                gmCoordinator.sendMessage(p, strUpdateCoinsMessage);

                //Update the Player Bar Panel
                //send the playerBarPanel information
                // Replaced with "SetCoins" message
                // gmCoordinator.sendMessage(p, "B" + Marshaller.ObjectToString(new PlayerBarInformation(player)));
                //send the current stage of wonder information and tell it to start up the timer
                // gmCoordinator.sendMessage(p, "s" + p.currentStageOfWonder);

                /*
                //if player has Bilkis AND has at least 1 coin, then send the message to enable Bilkis button
                if (p.hasBilkis && p.coin > 0)
                {
                    //EB = Enable Bilkis
                    gmCoordinator.sendMessage(p, "EB");
                }

                //send current turn information
                gmCoordinator.sendMessage(p, "T" + currentTurn);
                */

                // Check if we're in a special state - extra turn for Babylon (B)
                if (gettingBabylonExtraCard && p.playerBoard.name != "Babylon (B)")
                    continue;

                // Check if we're in a special state - playing a card from the discard pile
                // only the player (or players) who are getting the extra turn can proceed here.
                if (playingCardFromDiscardPile && !p.playCardFromDiscardPile)
                    continue;

                //send the hand panel (action information) for regular ages (not the Recruitment phase i.e. Age 0)
                if (currentAge > 0)
                {
                    string strHand = "SetPlyrH";

                    if (playingCardFromDiscardPile)
                    {
                        savedHandWhenPlayingFromDiscardPile = p.hand;   // save the player's hand
                        p.hand = discardPile;                           // the player's hand now points to the discard pile.

                        foreach (Card card in discardPile)
                        {
                            // Filter out structures that have already been built in the players' city.
                            if (p.isCardBuildable(card) != Buildable.StructureAlreadyBuilt)
                                strHand += string.Format("&{0}={1}", card.name, Buildable.True.ToString());
                        }

                        // The free build for Halikarnassos/Solomon requires the card be put in play.
                        // It cannot be used to build a wonder stage, nor can it be discarded for 3
                        // coins.
                        strHand += string.Format("&WonderStage{0}={1}&CanDiscard=False", p.currentStageOfWonder, Buildable.InsufficientResources.ToString());
                    }
                    else
                    {
                        foreach (Card card in p.hand)
                        {
                            strHand += string.Format("&{0}={1}", card.name, p.isCardBuildable(card).ToString());
                        }

                        strHand += string.Format("&WonderStage{0}={1}", p.currentStageOfWonder, p.isStageBuildable().ToString());
                    }

                    //send the Card Panel information to that player
                    gmCoordinator.sendMessage(p, strHand);
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
        }

#if FALSE
        /// <summary>
        /// Whenever a card becomes "Played", UpdatePlayedCardPanel should be called
        /// Sends a message to the client so that a newly played card will show up in the PlayedCardPanel
        /// </summary>
        public void updatePlayedCardPanel(String nickname)
        {
            
            // bUIRequiresUpdating = true;

            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                Player p = player[i];

                if (p.GetNumberOfPlayedCards() > 0)
                {
                    string lastPlayedCardInformationString = string.Format("P{0}{1}", i, Marshaller.ObjectToString(p.GetCardPlayed(p.GetNumberOfPlayedCards() - 1)));

                    gmCoordinator.sendMessage(p, lastPlayedCardInformationString);
                }
            }
    }


        /// <summary>
        /// Player hits the Olympia power button
        /// give the UI information.
        /// </summary>
        /// <param name="p"></param>
        public void sendOlympiaInformation(String nickname)
        {
            /*
            Player p = player[nickname];

            //information to be sent
            String information = "O";

            information += Marshaller.ObjectToString(new PlayForFreeInformation(p, 'O'));

            //send the information
            gmCoordinator.sendMessage(p, information);
            */
        }

        /// <summary>
        /// Play a given card id in nickname for free with Olympia
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="id"></param>
        public void playCardForFreeWithOlympia(String nickname, string structureName)
        {
            Player p = player[nickname];

            //if the structure costed money, reimburse the money
            //check if the Card costs money
            int costInCoins = 0;

            Card c = p.hand.Find(x => x.name == structureName);
            costInCoins = c.cost.coin;

            //store the reimbursement
            // p.storeAction("1" + costInCoins + "$");
            p.storeAction(new CoinEffect(c.cost.coin));

            //build the structure
            buildStructureFromHand(structureName, nickname, null, null, null, null);

            //disable Olympia
            p.olympiaPowerEnabled = false;
        }

        /// <summary>
        /// give the UI information for Halicarnassus
        /// </summary>
        /// <param name="p"></param>
        public void sendHalicarnassusInformation(String nickname)
        {
            // test this.
            throw new NotImplementedException();

            Player p = player[nickname];

            //if there are no cards in the discard pile, send it H0, for nothing
            if (discardPile.Count == 0)
            {
                gmCoordinator.sendMessage(p, "H0");
                return;
            }

            //information to be sent
            String information = "H";

            //gather the necessary information
            //H_(num of cards)_(id1)&(name)_(id2)&(name)_...(id_last)&(name)|

            information += "_" + discardPile.Count;

            foreach (Card c in discardPile)
            {
                // this used to be the Card ID and name.  Now it should just be the card name
                information += "_" + c.name + "&" + c.name;
            }

            information += "|";

            //send the information
            gmCoordinator.sendMessage(p, information);
        }
#endif
        /*
        /// <summary>
        /// play the card for free from discard pile with Halicarnassus
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="id"></param>
        public void playCardForFreeWithHalicarnassus(string nickname, string structureName)
        {
            //build from the discard pile
            buildStructureFromDiscardPile(structureName, nickname);
        }

        /// <summary>
        /// give the UI information for Babylon
        /// </summary>
        /// <param name="nickname"></param>
        public void sendBabylonInformation(string nickname)
        {
            Player p = player[nickname];

            /*

            //information to be sent
            String information = "A";

            //look at the last card in hand.
            //send the information about it
            
            //get the last card
            Card lastCard = p.hand[0];
            //get the id
            information += "_" + lastCard.name + "_";
            //get if the card is playable from hand
            information += p.isCardBuildable(lastCard);
            //get if stage is buildable from hand
            information += p.isStageBuildable();

            Card lastCard = p.hand[0];

            string babylonBmsg = "BabylonB";

            babylonBmsg += string.Format("&{0}={1}", lastCard.name, p.isCardBuildable(lastCard).ToString());

            babylonBmsg += string.Format("&WonderStage{0}={1}", p.currentStageOfWonder, p.isStageBuildable().ToString());

            //send the information
            gmCoordinator.sendMessage(p, babylonBmsg);
        }

        Attempting to build the commerce send string from played cards rather than effects.
        string BuildResourceString(string who, Player plyr, bool isSelf)
        {
            string strRet = string.Format("&{0}Resources=", who);

            foreach (Card c in plyr.playedStructure.Where(x => x.effect is SimpleEffect || (())
            {
                strRet += string.Format("{0},", c.name);
            }

            if (isSelf)
            {
                if (plyr.playedStructure.Where(x => x.effect is ResourceChoiceEffect))
                { }


            }

            foreach (ResourceChoiceEffect e in plyr.dag.getChoiceStructures(isSelf))
            {
                ResourceChoiceEffect rce = e as ResourceChoiceEffect;

                strRet += string.Format("{0},", rce.strChoiceData);
            }

            // remove the trailing comma, if necessary
            if (strRet.EndsWith(","))
                strRet = strRet.Remove(strRet.Length - 1);

            return strRet;
        }
        */

        string BuildResourceString(string who, Player plyr, bool isSelf)
        {
            string strRet = string.Format("&{0}Resources=", who);

            foreach (Effect e in plyr.dag.getResourceList(isSelf))
            {
                ResourceEffect se = e as ResourceEffect;

                strRet += se.resourceTypes;

                strRet += ",";
            }

            // remove the trailing comma, if necessary
            if (strRet.EndsWith(","))
                strRet = strRet.Remove(strRet.Length - 1);

            return strRet;
        }

        /// <summary>
        /// Server to Client message consisting of Commerce UI updates
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nickname"></param>
//        public void updateCommercePanel(string structureName, string nickname, bool isStage)
        public void updateCommercePanel(string nickname, string strctureName, string wonderStage)
        {
            Player p = player[nickname];

            // Leaders who give discounts on certain card types:
            // "Imhotep" (Wonder stages)
            // "Archimedes" (Science cards)
            // "Hammurabi" (Civilian)
            // "Leonidas" (Military)

            // old structure included structure name, cost, whether a Wonder stage is being constructed, and leader discount
            // I'm going to skip the name and whether it is a wonder stage (client already knows these) and the cost (client knows all card info) and the leaders, for now.

            string strCommerce = "CommData";

            strCommerce += string.Format("&coin={0}", p.coin);
            strCommerce += string.Format("&resourceDiscount={0}", p.rawMaterialsDiscount.ToString());
            strCommerce += string.Format("&goodsDiscount={0}", p.goodsDiscount.ToString());

            strCommerce += string.Format("&wonderInfo={0}/{1}", p.currentStageOfWonder, p.playerBoard.name);

            strCommerce += string.Format("&Structure={0}&WonderStage={1}", strctureName, wonderStage);

            strCommerce += BuildResourceString("Player", p, true);
            strCommerce += BuildResourceString("Left", p.leftNeighbour, false);
            strCommerce += BuildResourceString("Right", p.rightNeighbour, false);

            gmCoordinator.sendMessage(p, strCommerce);
        }

        protected int numOfPlayersThatHaveTakenTheirTurn = 0;

        /*
         * Player sends "t" message (turn complete) to the GameManager
         * Increment the amount of players that have taken their turn
         * If it is equal to the number of players, then go to the next turn
         */
        public virtual void turnTaken(String nickname)
        {
            Player p = player[nickname];

            /*
            //if player doesn't even have Halicarnassus or Babylon board, then no more action is required
            if (p.playerBoard.name != "Babylon (B)" && p.playerBoard.name.Substring(0, 13) != "Halikarnassos")
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
            */

            numOfPlayersThatHaveTakenTheirTurn++;

            if ((numOfPlayersThatHaveTakenTheirTurn == numOfPlayers) || gettingBabylonExtraCard || playingCardFromDiscardPile)
            {
                //reset the number of players that have taken their turn
                numOfPlayersThatHaveTakenTheirTurn = 0;

                //execute every player's action
                    // any other turn, execute everyone's actions.
                executeActionsAtEndOfTurn();

                if (gettingBabylonExtraCard && currentTurn == 6 && p.playerBoard.name == "Babylon (B)" && p.currentStageOfWonder >= 2)
                {
                    gettingBabylonExtraCard = false;
                }
                else if (currentTurn == 6 && p.playerBoard.name == "Babylon (B)" && p.currentStageOfWonder >= 2)
                {
                    gettingBabylonExtraCard = true;
                }

                if (playingCardFromDiscardPile && p.playCardFromDiscardPile)
                {
                    playingCardFromDiscardPile = false;
                    p.playCardFromDiscardPile = false;

                    // restore the player's original hand
                    p.hand = savedHandWhenPlayingFromDiscardPile;
                    savedHandWhenPlayingFromDiscardPile = null;
                }
                else if (p.playCardFromDiscardPile)
                {
                    playingCardFromDiscardPile = true;
                }

                //all players have completed their turn
                if (!gettingBabylonExtraCard && !playingCardFromDiscardPile)
                {
                    // gettingBabylonExtraCard = false;
                    // playingCardFromDiscardPile = false;

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
