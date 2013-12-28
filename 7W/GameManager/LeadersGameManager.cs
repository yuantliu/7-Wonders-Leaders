using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    /*
     * The GameManager with Leaders expansion enabled
     */
    public class LeadersGameManager : GameManager
    {

        public LeadersGameManager(GMCoordinator gmCoordinator, int numOfPlayers, String []playerNicks, int numOfAI, char []AIStrats) 
            : base (gmCoordinator, numOfPlayers, playerNicks, numOfAI, AIStrats)
        {
            //for possible future Cities expansion
            //if(this is CitiesGameManager == true)
            leadersGameManagerInitialisation(playerNicks, AIStrats);

            //set the current Age to 0, which is the recruitment phase
            currentAge = 0;
        }

        /*
         * initialisation tasks specifically for leaders expansion
         */
        private void leadersGameManagerInitialisation(string []playerNicks, char[] AIStrats)
        {
            //create the new leaders AIs
            for (int i = 0; i < numOfAI; i++)
            {
                player[i] = createAI("L_" + (i + 1), AIStrats[i]);
            }

            //leaders start at age 0
            currentAge = 0;
            currentTurn = 1;

            //AI initialisation
            this.numOfAI = numOfAI;

            //player initialisation
            for (int i = 0; i < numOfPlayers; i++)
            {
                player[i] = new Player(playerNicks[i], false, this);
            }

            //leaders has 16 boards (2 new boards from rome)
            board_amount = 14;
            //board_amount = 16;

            //initialise the leaders boards
            createBoards("leadersboards.txt");

            //creating the leader AIs
            for (int i = numOfPlayers; i < numOfAI + numOfPlayers; i++)
            {
                player[i] = createAI("AI" + (i + 1), AIStrats[i - numOfPlayers]);
            }

            //set up the player positions
            setPlayerPosition(numOfPlayers, numOfAI);
        }

        /// <summary>
        /// Beginning of session actions for Leaders game
        /// 1) Distribute a random board and 6 coins to all
        /// </summary>
        public override void beginningOfSessionActions()
        {
            //UC-19: R2
            //6 coins and random board
            //send the board display at this point
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                player[i].playerBoard = popRandomBoard();
                gmCoordinator.sendMessage(player[i], "b" + player[i].playerBoard.name);
                player[i].storeAction("16$");
                player[i].storeAction("11" + player[i].playerBoard.freeResource);
                player[i].executeAction((GameManager)(this));
            }

            //UC-19 R3: load decks and remove all unused cards

            //initialize, load, and remove unused cards
            deck = new Deck[4];
            
            //Read the textfile information and initialize the decks according to the information
            
            //load Leaders deck. No need to remove unused cards. Pop random card will work fine.
            deck[0] = new Deck("leaders.txt", numOfAI + numOfPlayers);
            
            //load the regular age 1 and 2 decks
            for (int i = 1; i < 3; i++)
            {
                //deck[0] is age 1. deck[1] is age 2 ....
                deck[i] = new Deck("age" + i + "cards.txt", numOfAI + numOfPlayers);
            }

            //load the age 3 decks, which have the new guild cards
            deck[3] = new Deck("age3cardsleaders.txt", numOfAI + numOfPlayers);
            deck[3].removeUnusedCards();

           

            //deal four deck 0 to players
            dealDeck(currentAge);
        }

        /// <summary>
        /// During the leaders phase (age 0)
        /// Player p will recruit card id (which is in his hand) and add it to his recruited leaders pile
        /// </summary>
        /// <param name="player"></param>
        /// <param name="id"></param>
        public override void recruitLeader(String nickname, int id)
        {
            Player p = playerFromNickname(nickname);

            //Look into the player's hand
            //and extract the chosen Leader card from it
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

            //add the card to that player's unplayed leaders pile
            p.leadersPile.Add(c);
        }

        /// <summary>
        /// Leaders specific response for turn taken by a player
        /// </summary>
        public override void turnTaken(String nickname)
        {
            //same as original GameManager
            Player p = playerFromNickname(nickname);

            //The leader special powers are checked here

            //check if Stevie is played
            if (p.hasStevie() == true)
            {
                //has played Stevie
                //build the next stage of wonder
                buildStageOfWonder(0, nickname);
            }

            //check for Salomon (aka Halicarnassus) power
            if (p.hasSalomon() == true)
            {
                //send the Halicarnassus screen
                sendHalicarnassusInformation(nickname);
            }

            //check if Player has played Courtesan's guild
            else if (p.hasCourtesan() == true)
            {
                sendCourtesanInformation(p);

            }

            //Regular board powers from vanilla are checked here
            //if player doesn't even have Halicarnassus or Babylon board, then no more action is required
            else if (p.playerBoard.name != "BB" && p.playerBoard.name[0] != 'H')
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
            //all players taken their turn
            if (numOfPlayersThatHaveTakenTheirTurn == numOfPlayers)
            {

                    //reset the number of players that have taken their turn
                    numOfPlayersThatHaveTakenTheirTurn = 0;

                    //execute every player's action
                    executeActionsAtEndOfTurn();

                    //reset all Bilkis resources
                    for (int i = 0; i < numOfAI + numOfPlayers; i++)
                    {
                        if (player[i].hasBilkis)
                        {
                            //remove the appropriate temporary resource
                            if (player[i].bilkis == 1) player[i].ore--;
                            else if (player[i].bilkis == 2) player[i].stone--;
                            else if (player[i].bilkis == 3) player[i].glass--;
                            else if (player[i].bilkis == 4) player[i].papyrus--;
                            else if (player[i].bilkis == 5) player[i].loom--;
                            else if (player[i].bilkis == 6) player[i].wood--;
                            else if (player[i].bilkis == 7) player[i].brick--;

                            player[i].bilkis = 0;
                            break;
                        }
                    }

                    //increment the turn
                    currentTurn++;

                    //if esteban is enabled, then disable esteban, and skip the card passing
                    if (esteban == true && !(currentTurn == 1 && currentAge > 0))
                    {
                        esteban = false;
                    }
                    //if current turn is not turn 1 of a normal age (i.e. not play leader turn)
                    //then pass the cards
                    else if (!(currentTurn == 1 && currentAge > 0))
                    {
                        //pass the cards to the neighbour
                        passRemainingCardsToNeighbour();
                    }
                    //otherwise, put the hand cards into the leaders pile
                    //receive 7 cards as normal
                    else
                    {
                        //first, place the remaining Leader cards from the hand to the Leaders pile
                        for (int i = 0; i < numOfAI + numOfPlayers; i++)
                        {
                            for (int j = 0; j < player[i].numOfHandCards; j++)
                            {
                                player[i].leadersPile.Add(player[i].hand[j]);
                            }
                        }

                        //then, remove all the leader cards from the hand by setting the numOfHandCards to 0
                        //the newly dealt normal cards will replace the current hand, which has been transferred to the Leaders pile once again.
                        for (int i = 0; i < numOfAI + numOfPlayers; i++)
                        {
                            player[i].numOfHandCards = 0;
                        }

                        //deal the cards as normal
                        dealDeck(currentAge);
                    }
                    
                    //if the current turn is last turn of the recruitment phase, then get ready for Age 1
                    if (currentTurn == 5 && currentAge == 0)
                    {
                        currentAge++;
                        currentTurn = 0;
                        //go to recruitment turn
                        startOfRecruitmentTurn();
                    }
                    //if the current turn is last turn of a regular Age, do end of Age calculation
                    else if (currentTurn == 7 && currentAge > 0)
                    {
                        //perform end of Age actions
                        endOfAgeActions();
                        currentAge++;

                        if (currentAge < 4)
                        {
                            currentTurn = 0;
                            startOfRecruitmentTurn();
                        }
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
        /// Recruitment turn at the beginning of each age
        /// 1) get the cards from leaders pile
        /// 2) calculate available actions
        /// 3) present it to player
        /// </summary>
        public void startOfRecruitmentTurn()
        {
            //UC-22 R1
            //get the cards from the Leaders pile, get the avaliable actions, then display it

            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                Player p = player[i];

                int numOfLeaderCards = p.leadersPile.Count;

                //put each card from leadersPile to p's hand
                for (int j = 0; j < numOfLeaderCards; j++)
                {
                    Card c = p.leadersPile[0];

                    p.leadersPile.RemoveAt(0);

                    //231 Stevie Leader card: change the coin cost to be equal to the number of resources on the next wonder stage
                    //I.e. if next stage of wonder costs 2 ores, then the cost of Steview is 2 coins.
                    //Change to 100 coins (i.e. unaffordable) if all wonder stages have been maxed out
                    if (c.id == 231)
                    {
                        if (p.currentStageOfWonder < p.playerBoard.numOfStages)
                        {
                            //change cost to the coin amount of the next stage's cost
                            int newCost = p.playerBoard.cost[p.currentStageOfWonder].Length;
                            string newCostString = "";

                            for (int k = 0; k < newCost; k++)
                            {
                                newCostString += "$";
                            }

                            c.cost = newCostString;
                        }
                        else //can't build any more stages
                        {
                            c.cost = "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$";
                        }    
                    }

                    p.addHand(c);
                }
            }

            //UC-22 R2
            //calculate available actions

            //this is done in updateGameUI automatically
        }

        /// <summary>
        /// Essentially the same as the base class' updateAllGameUI, but calling that would have called the base class' updategameUI
        /// which is not what I wanted. This class' updateGameUI needs to be called, not the base's
        /// </summary>
        public override void updateAllGameUI()
        {
            for (int i = 0; i < numOfPlayers + numOfAI; i++)
            {
                updateGameUI(player[i]);
            }
        }

        /// <summary>
        /// Leaders updated to support Age 0
        /// </summary>
        /// <param name="p"></param>
        public override void updateGameUI(Player p)
        {
            //send the rest of the non-hand information
            //hand information will be seperate, because we are in Age 0
            base.updateGameUI(p);

            //send the hand display for Age 0
            if (currentAge == 0)
            {
                //prepare to send the Age 0 hand panel information
                String handPanelInformationString = "r" + Marshaller.ObjectToString(new RecruitmentPhaseInformation(p));

                //send the information
                gmCoordinator.sendMessage(p, handPanelInformationString);
            }
        }

        /// <summary>
        /// Player used Bilkis to obtain a temporary resource
        /// (0 is nothing, 1 is ore, 2 is stone, 3 is glass, 4 is papyrus, 5 is loom, 6 is wood, 7 is brick
        /// </summary>
        /// <param name="resource"></param>
        public override void bilkisPower(String nickname, byte resource)
        {
            Player p = playerFromNickname(nickname);

            //if a previous resource was selected, then undo the previous selection
            if (p.bilkis != 0)
            {
                p.coin++;
                if (p.bilkis == 1) p.ore--;
                else if (p.bilkis == 2) p.stone--;
                else if (p.bilkis == 3) p.glass--;
                else if (p.bilkis == 4) p.papyrus--;
                else if (p.bilkis == 5) p.loom--;
                else if (p.bilkis == 6) p.wood--;
                else if (p.bilkis == 7) p.brick--;
            }

            //set the resource
            p.bilkis = resource;

            //charge money
            p.coin--;
            //add the appropriate resource temporarily
            if (resource == 1) p.ore++;
            else if (resource == 2) p.stone++;
            else if (resource == 3) p.glass++;
            else if (resource == 4) p.papyrus++;
            else if (resource == 5) p.loom++;
            else if (resource == 6) p.wood++;
            else if (resource == 7) p.brick++;

            //Update the Player Bar Panel
            gmCoordinator.sendMessage(p, "B" + Marshaller.ObjectToString(new PlayerBarInformation(player)));

            //Update the available actions panel as well
            gmCoordinator.sendMessage(p, "U" + Marshaller.ObjectToString(new HandPanelInformation(p, currentAge)));
        }

        /// <summary>
        /// Special build stage of wonder for dealing with Rome B's "play leader for free" ability
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nickname"></param>
        public override void buildStageOfWonder(int id, string nickname)
        {

            base.buildStageOfWonder(id, nickname);

            //Rome Side B (LB)
            //Send to client the Window asking which Leader should be played for free
            handleRomeBoard(playerFromNickname(nickname));
        }


        public override void buildStageOfWonderFromCommerce(string nickname, string information)
        {
            base.buildStageOfWonderFromCommerce(nickname, information);

            //Rome Side B (LB)
            //Send to client the Window asking which Leader should be played for free
            handleRomeBoard(playerFromNickname(nickname));
        }

        /// <summary>
        /// Build stage of wonder and Commerce build stage call this to handle 
        /// </summary>
        /// <param name="p"></param>
        private void handleRomeBoard(Player p)
        {
            //Stage 1 of Rome B: Give p 4 random cards
            if (p.playerBoard.effect[p.currentStageOfWonder - 1] == "7LB1")
            {
                for (int i = 0; i < 4; i++)
                {
                    p.leadersPile.Add(deck[0].popRandomCard());
                }
            }
            //Only applicable to Stage 2 and 3 of Rome B
            else if (p.playerBoard.effect[p.currentStageOfWonder - 1] == "7LB2")
            {
                //lock (romeLocked)
                {
                    //decrement numOfPlayersThatHaveTakenTheirturn to offset the end the turn signal the client sends with the build stage order
                    numOfPlayersThatHaveTakenTheirTurn--;
                }

                //send the Rome UI to the client
                sendRomeInformation(p);
            }
        }

        private void sendCourtesanInformation(Player p)
        {
            //send the Courtesan's guild screen
            gmCoordinator.sendMessage(p, "c" + Marshaller.ObjectToString(new CourtesanGuildInformation(p)));
        }

        private void sendRomeInformation(Player p)
        {
            if (currentTurn > 0)
            {
                gmCoordinator.sendMessage(p, "O" + Marshaller.ObjectToString(new PlayForFreeInformation(p, 'R')));
            }
            else
            {
                //Player uses Rome power on Recruitment Turn, look into hands instead
                //in other words, do the same thing as Olympia power for that turn.
                gmCoordinator.sendMessage(p, "O" + Marshaller.ObjectToString(new PlayForFreeInformation(p, 'O')));
            }
        }

        /// <summary>
        /// Play a Leader card for free
        /// Called in response to playing Stage 2 and 3 of Rome B board.
        /// </summary>
        /// <param name="p"></param>
        public override void playLeaderForFreeWithRome(string nickname, string message)
        {
            Player p = playerFromNickname(nickname);

            int id = int.Parse(message);

            //find the id in the Leader pile if we are in a "regular" turn
            if (currentTurn > 0)
            {
                for (int i = 0; i < p.leadersPile.Count; i++)
                {
                    //found the card
                    if (p.leadersPile[i].id == id)
                    {
                        //add to the played card pile
                        p.addPlayedCardStructure(p.leadersPile[i]);

                        //add the effect
                        p.storeAction(p.leadersPile[i].effect);

                        //remove the Leader card
                        p.leadersPile.RemoveAt(i);
                    }
                }
            }
            //current turn is in the recruitment stage
            //leader cards won't be in the Leader pile
            //its in the player's hands instead. search there.
            else
            {
                Card c;

                for (int i = 0; i < p.hand.Length; i++)
                {
                    //found the card
                    if (p.hand[i].id == id)
                    {
                        //add to played card pile
                        c = p.hand[i];

                        //remove it from hand
                        for (int j = i; j < p.numOfHandCards - 1; j++)
                        {
                            p.hand[j] = p.hand[j + 1];
                        }

                        p.numOfHandCards--;

                        //add to the played card pile
                        p.addPlayedCardStructure(c);

                        //add the effect
                        p.storeAction(c.effect);

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Player sends information selected in Courtesan's guild
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="message"></param>
        public override void playCourtesansGuild(string nickname, string message)
        {
            Player p = playerFromNickname(nickname);

            int id = int.Parse(message);

            //look for this id in the neighbours and build a copy of this card
            Card c = null;

            for (int i = 0; i < p.leftNeighbour.numOfPlayedCards; i++)
            {
                if (p.leftNeighbour.playedStructure[i].id == id)
                {
                    c = p.leftNeighbour.playedStructure[i].copy();
                    //This card still counts as Purple
                    c.colour = "Purple";
                }
            }

            for (int i = 0; i < p.rightNeighbour.numOfPlayedCards; i++)
            {
                if (p.rightNeighbour.playedStructure[i].id == id)
                {
                    c = p.rightNeighbour.playedStructure[i].copy();
                    c.colour = "Purple";
                }
            }

            //if a card hasn't been copied, something is wrong.
            if (c == null)
            {
                throw new NotImplementedException();
            }

            //add this cloned card to the hand
            p.addHand(c);

            //play this card
            buildStructureFromHand(c.id, nickname);
        }

        /// <summary>
        /// Leaders expansion AI creation
        /// Not implemented yet
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strategy"></param>
        /// <returns></returns>
        protected override Player createAI(String name, char strategy)
        {
            Player thisAI = new Player(name, true, this);
            switch (strategy)
            {
                case '0':
                    thisAI.LeadersAIBehaviour = new AIMoveAlgorithmLeaders0();
                    break;
                case '1':
                    thisAI.LeadersAIBehaviour = new AIMoveAlgorithmLeaders1();
                    break;
                case '2':
                    thisAI.LeadersAIBehaviour = new AIMoveAlgorithmLeaders2();
                    break;
            }

            return thisAI;
        }
    }
}
