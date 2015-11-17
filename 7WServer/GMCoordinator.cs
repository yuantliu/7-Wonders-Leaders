using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public class GMCoordinator
    {
        GameManager gameManager;

        public Server host;

        int numOfPlayers;
        int numOfAI;

        int numOfCountdownsFinished;
        int numOfReadyPlayers;

        String[] playerNicks = new String[7];
        char[] AIStrats = new char[6];

        int numOfPlayersThatHaveTakenTheirTurn { get; set; }

        private enum GameMode { Vanilla, Leaders }

        GameMode currentMode;

        /// <summary>
        /// Create a new server.
        /// Have the server start listening for requests.
        /// Part of UC-01 R01
        /// </summary>
        public GMCoordinator()
        {
            //create server
            host = new Server();
            host.StartServer();
            host.StatusChanged += new StatusChangedEventHandler(receiveMessage);

            //keep track of information at table UI
            numOfAI = 0;
            numOfPlayers = 0;
            numOfReadyPlayers = 0;

            //default mode is Vanilla
            currentMode = GameMode.Vanilla;
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        /// Networking functionalities

        /// <summary>
        /// Send to Player p a message
        /// </summary>
        /// <param name="p"></param>
        /// <param name="message"></param>
        public void sendMessage(Player p, String message)
        {
            if(!p.isAI)
                host.sendMessageToUser(p.nickname, message);
        }

        /// <summary>
        /// Receives a String from a user
        /// Parse the String and call the appropriate function in GameManager
        /// </summary>
        /// <param name="m"></param>
        public void receiveMessage(object sender, StatusChangedEventArgs e)
        {
            //lock makes sure that only one message is being processed at a time
            //and to ensure that data are not corrupted
            
            //an all encompassing lock statement like this hurts performance
            //therefore could use some optimisation
            lock (typeof(GMCoordinator))
            {

                //This is the nickname of the player that sent the message
                String nickname = e.nickname;

                //This is the string received from Server
                String message = e.message;

                Console.WriteLine("In receiveMessage.  Nickname: {0}, Message={1}", nickname, message);

                //#: Chat string.
                if (message[0] == '#')
                {
                    host.sendMessageToAll("#" + nickname + ": " + message.Substring(1));
                }
                //J: Player joins the game
                //increment the numOfPlayers
                else if (message[0] == 'J')
                {
                    //store the player's nickname and increase the number of players
                    playerNicks[numOfPlayers++] = nickname;

                    host.sendMessageToAll("#" + nickname + " has joined the table.");
                }
                //R: Player hits the Ready button
                //increment the numOfReadyPlayers
                //if all players are ready then send the Start signal
                else if (message[0] == 'R')
                {

                    //server returns an error in the chat if there are not enough players in the game
                    //Sends the signal to re-enable the Ready buttons
                    if (numOfPlayers + numOfAI < 3)
                    {
                        host.sendMessageToAll("#Not enough players at the table. Need at least " + (3 - numOfPlayers) + " more participants.");
                        host.sendMessageToAll("S0");
                    }
                    else
                    {
                        //Increase the number of ready players
                        numOfReadyPlayers++;

                        //inform all that the player is ready
                        host.sendMessageToAll("#" + nickname + " is ready.");

                        //if all players are ready, then initialise the GameManager
                        if (numOfReadyPlayers == numOfPlayers)
                        {
                            //Do not accept any more players
                            host.acceptClient = false;

                            Console.WriteLine("All players have hit Ready.  Game is starting now with {0} AI players", numOfAI);

                            if (currentMode == GameMode.Leaders)
                            {
                                //tell the GameManager to start on the beginning of session operations
                                gameManager = new LeadersGameManager(this, numOfPlayers, playerNicks, numOfAI, AIStrats);
                                //have to upcast to LeadersGameManager because polymorphism doesn't work in C#
                                gameManager = (LeadersGameManager)gameManager;
                            }
                            else if (currentMode == GameMode.Vanilla)
                            {
                                gameManager = new GameManager(this, numOfPlayers, playerNicks, numOfAI, AIStrats);
                            }
                            else throw new NotImplementedException();
                            
                            //S[n], n = number of players in this game
                            for (int i = 0; i < gameManager.numOfAI + gameManager.numOfPlayers; i++)
                            {
                                sendMessage(gameManager.player[i], string.Format("S{0}", gameManager.numOfAI + gameManager.numOfPlayers));
                            }
                            
                            //set up the game, send information on boards to players, etc.
                            gameManager.beginningOfSessionActions();

                            //set the number of countdowns finished
                            numOfCountdownsFinished = 0;
                            
                        }
                    }
                }
                else if (message[0] == 'U')
                {
                    // main UI on client-side is ready; send board information to each one and initial actions
                    gameManager.sendBoardInformation();
                }
                //m: game mode options
                //changed by TableUI
                else if (message[0] == 'm')
                {
                    //leaders mode checkbox enabled by host
                    //clear all the AIs
                    numOfAI = 0;

                    if (message[1] == 'L')
                    {
                        host.sendMessageToAll("#Host changed game mode to Leaders.");
                        currentMode = GameMode.Leaders;
                    }
                    else if (message[1] == 'V')
                    {
                        host.sendMessageToAll("#Host changed game mode to Vanilla.");
                        currentMode = GameMode.Vanilla;
                    }

                    host.sendMessageToAll("#All AIs from previous mode cleared.");
                }
                //r: all player's countdowns are 
                //tell the GameManager to update each player's game UI
                else if (message[0] == 'r')
                {
                    //increase the number of players with countdowns finished
                    numOfCountdownsFinished++;
                    //everyone's countdown is finished
                    //display the first table UI for the first turn
                    if (numOfCountdownsFinished == numOfPlayers)
                    {
                        gameManager.updateAllGameUI();
                    }
                }
                //B: player decides to build structure on the card
                else if (message[0] == 'B')
                {
                    //get the id of the card
                    int id = int.Parse(message.Substring(1));

                    //Tell the GameManager that Player has decided to build structure with the Card
                    gameManager.buildStructureFromHand(id, nickname);

                    //Update the Played card panel
                    gameManager.updatePlayedCardPanel(nickname);
                }
                //S: player decides to build stage of wonder
                else if (message[0] == 'S')
                {
                    //get the id of the card
                    int id = int.Parse(message.Substring(1));

                    //Tell the GameManager that Player has decided to build stage of wonder with card
                    gameManager.buildStageOfWonder(id, nickname);
                }
                //D: player decides to discard card for three coins
                else if (message[0] == 'D')
                {
                    //get the id of the card
                    int id = int.Parse(message.Substring(1));

                    //Tell the GM that player has decided to discard the card for 3 coins
                    gameManager.discardCardForThreeCoins(id, nickname);
                }
                //O: player hits the Olympia power button
                else if (message[0] == 'O')
                {
                    //handle the Olympia button
                    //prepare to send Olympia UI information to the player
                    gameManager.sendOlympiaInformation(nickname);
                }
                //o: player makes a selection in the Olympia UI
                else if (message[0] == 'o')
                {
                    //o(id)
                    //play the card for free from hand
                    gameManager.playCardForFreeWithOlympia(nickname, int.Parse(message.Substring(1)));
                    //Update the Played card panel
                    gameManager.updatePlayedCardPanel(nickname);
                }
                //h: player asks for halicarnassus information
                else if (message[0] == 'h')
                {
                    //get the halicarnassus information and send it to the player
                    gameManager.sendHalicarnassusInformation(nickname);
                }
                //H: player makes selection in Halicarnassus UI
                else if (message[0] == 'H')
                {
                    //H(id)
                    //play the card for free from discard pile
                    gameManager.playCardForFreeWithHalicarnassus(nickname, int.Parse(message.Substring(1)));
                    //Update the Played card panel
                    gameManager.updatePlayedCardPanel(nickname);
                }
                //b: player asks for babylon information
                else if (message[0] == 'b')
                {
                    gameManager.sendBabylonInformation(nickname);
                }
                //C player decides to open the commerce window
                else if (message[0] == 'C')
                {
                    //b for buildStructure Commerce
                    if (message[1] == 'b')
                    {
                        //get the id of the card
                        int id = int.Parse(message.Substring(2));
                        gameManager.updateCommercePanel(id, nickname, false);
                    }

                    //s for build Stage Of Wonder Commerce
                    else if (message[1] == 's')
                    {
                        int id = int.Parse(message.Substring(2));
                        gameManager.updateCommercePanel(id, nickname, true);
                    }

                    //player built the card from commerce window
                    else if (message[1] == 'B')
                    {
                        gameManager.buildStructureFromCommerce(nickname, message.Substring(2));
                        //Update the Played card panel
                        gameManager.updatePlayedCardPanel(nickname);
                    }

                    //player build stage of wonder from commerce window
                    else if (message[1] == 'S')
                    {
                        gameManager.buildStageOfWonderFromCommerce(nickname, message.Substring(2));

                    }
                }
                //t: player has taken an action for the turn
                else if (message[0] == 't')
                {
                    gameManager.turnTaken(nickname);
                }
                /*
                // JDF commented-out as the main window now has all the view data and this is therefore no longer needed.
                //V(player nickname)
                //return the view detail UI information of a given player's nickname
                else if (message[0] == 'V')
                {
                    gameManager.sendViewDetailInformation(nickname, message.Substring(1));
                }
                */

                //"L" for leave a game
                else if (message[0] == 'L')
                {
                    //Server.sendMessageToAll("#" + nickname + " has left the table.");
                    host.sendMessageToAll("#" + nickname + " has left the table.");
                    host.sendMessageToAll("#Game has stopped.");
                    host.sendMessageToAll("e");
                    // host.stopListening();
                }
                //"a": AI management
                else if (message[0] == 'a')
                {
                    //"aa": add AI in the GameManager
                    if (message[1] == 'a')
                    {
                        //increase the number of players
                        if ((numOfPlayers + numOfAI) < 7)
                        {
                            AIStrats[numOfAI++] = message[2];
                            host.updateAIPlayer(true);
                            host.sendMessageToAll("#AI added. There are currently " + numOfAI + " AI(s).");
                        }
                        else
                        {
                            host.sendMessageToAll("#There are " + numOfPlayers + " human players and " + numOfAI + " AI(s) already at the table.");
                        }
                    }
                    //"ar": remove AI in the GameManager
                    else if (message[1] == 'r')
                    {
                        if (numOfAI > 0)
                        {
                            numOfAI--;
                            host.updateAIPlayer(false);
                            host.sendMessageToAll("#An AI has been removed. There are " + numOfAI + " AI(s) remaining.");
                        }
                        else
                        {
                            //send back to the host, telling him AI cannot be removed, since there are none
                            host.sendMessageToUser(nickname, "#No AI is currently at the table.");
                        }
                    }
                }

                /////////////////////////////////////////////////////////////////////////////////
                //Leaders only

                //recruit the leader during age 0
                else if (message[0] == 'l')
                {
                    //id of the selected Leader card
                    int id = int.Parse(message.Substring(1));

                    gameManager.recruitLeader(nickname, id);
                }

                //someone uses Esteban power
                else if (message == "Esteban")
                {
                    //if user attempts to use Esteban power during recruitment turn, then re-enable their button, because Esteban power can't be used
                    if (gameManager.currentTurn == 0)
                    {
                        sendMessage(gameManager.playerFromNickname(nickname), "EE");
                    }
                    //otherwise, toggle the Esteban variable in the gameManager
                    else
                    {
                        gameManager.esteban = true;
                    }
                }

                //bilkis power is used
                else if (message[0] == 'k')
                {
                    //Bilkis power format
                    //(0 is nothing, 1 is ore, 2 is stone, 3 is glass, 4 is papyrus, 5 is loom, 6 is wood, 7 is brick

                    gameManager.bilkisPower(nickname, (byte)(int.Parse(message.Substring(1))));
                }

                //Rome power is used to play a Leader for free
                else if (message[0] == 'M')
                {
                    gameManager.playLeaderForFreeWithRome(nickname, message.Substring(1));
                    gameManager.updatePlayedCardPanel(nickname);
                }

                //Courtesan's guild
                else if (message[0] == 'c')
                {
                    gameManager.playCourtesansGuild(nickname, message.Substring(1));
                    gameManager.updatePlayedCardPanel(nickname);
                }
            }
        }
    }
}
