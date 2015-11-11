using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.IO;
using System.Data;
using System.Timers;

namespace SevenWonders
{
    public class Coordinator
    {
        //Is the client connected to an ongoing game?
        public bool hasGame;

        //The various UI that Coordinator keeps track of
        public MainWindow gameUI;
        TableUI tableUI;
        JoinTableUI joinTableUI;
        OlympiaUI olympiaUI;
        HalicarnassusUI halicarnassusUI;
        BabylonUI babylonUI;

        //If this application will be the server, then gmCoordinator and gameManager must be initialized
        GMCoordinator gmCoordinator = null;

        //The client that the application will use to interact with the server.
        public Client client { get; set;  }

        //User's nickname
        public string nickname;

        //Timer
        int timeElapsed;
        private const int MAX_TIME = 120;
        private System.Windows.Threading.DispatcherTimer timer;

        //current turn
        int currentTurn;

        //Leaders
        BilkisUI bilkisUI;

        public Coordinator(MainWindow gameUI)
        {
            this.gameUI = gameUI;
            nickname = "";

            hasGame = false;

            //prepare the timer
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        /// <summary>
        /// Return whether this client is also the server
        /// </summary>
        /// <returns></returns>
        public bool isServer()
        {
            if (gmCoordinator == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void stopServer()
        {
            if (isServer() == true)
            {
                // gmCoordinator.host.stopListening();
            }
        }
        
        //Update the 100 Second timer field
        public void timer_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                gameUI.timerTextBox.Text = (MAX_TIME - timeElapsed) + "";
                timeElapsed++;   

                if (timeElapsed == MAX_TIME+1)
                {
                    discardRandomCard();
                    //close all open windows.
                    for(int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                        App.Current.Windows[intCounter].Close();

                    timer.Stop();
                }
            }));
        }

        /// <summary>
        /// Discard card in first hand position
        /// </summary>
        public void discardRandomCard()
        {
            //grab the id number of the first Card to the right by accessing the name
            if (gameUI.buildStructureButton[0].Name.StartsWith("BuildCommerce_"))
            {
                sendToHost("D" + gameUI.buildStructureButton[0].Name.Substring(14));
            }
            else
            {
                sendToHost("D" + gameUI.buildStructureButton[0].Name.Substring(6));
            }

            endTurn();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update the current stage of wonder label
        /// Start up the timer at this point
        /// </summary>
        /// <param name="message"></param>
        private void updateCurrentStageLabelAndStartTimer(string message)
        {
            //get the current stage
            int currentAge = int.Parse(message[1] + "");

            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                string content = "Current Stage: " + currentAge;
                gameUI.currentStageLabel.Content = content;
            }));
        }

        /// <summary>
        /// Start the 100 second timer
        /// </summary>
        private void startTimer()
        {
            //start up the timer
            timeElapsed = 0;
            timer.Start();
        }

        public void updateCardImage(string s)
        {
            gameUI.showCardImage(s);
        }

        public void updatePlayedCardsPanel(string s)
        {
            gameUI.showPlayedCardsPanel(s);
        }

        /// <summary>
        /// Update the Chat logs
        /// </summary>
        /// <param name="s"></param>
        public void updateChatTextBox(string s)
        {
            s = s + "\n";

            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                gameUI.chatTextBox.Text += s;
                tableUI.chatTextBox.Text += s;

                gameUI.scroll.ScrollToEnd();
                tableUI.scroll.ScrollToEnd();
            }));

        }

        /// <summary>
        /// User quits the Client program
        /// </summary>
        public void quit()
        {
            //If the client is not a server, then send to the host the close connection signal.
            if (gmCoordinator != null)
            {
                sendToHost("L");
                client.CloseConnection();
            }
            //If the client is a server, send the 
        }

        public void endTurn()
        {
            timer.Stop();
            sendToHost("t");
        }

        public void olympiaButtonClicked()
        {
            //send the request for Olympia power
            sendToHost("O");
        }

        /// <summary>
        /// Sends a message to the server telling it to start a game of vanilla or leaders
        /// send Rv for vanilla
        /// send Rl for leaders
        /// </summary>
        /// <param name="gameMode"></param>
        public void iAmReady()
        {
            //Grey out the Ready button
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                tableUI.readyButton.IsEnabled = false;
            }));

            initGameUI();
            
            sendToHost("R");
        }

        /// <summary>
        /// All players are ready. Initialise the game UI elements.
        /// </summary>
        private void initGameUI()
        {
            string currentPath = Environment.CurrentDirectory;
            ImageBrush back = new ImageBrush();
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(currentPath + @"\Resources\Images\background.jpg");
            source.EndInit();
            back.ImageSource = source;

            gameUI.mainGrid.Background = null;
            gameUI.mainGrid.Background = back;
            gameUI.chatTextField.Visibility = Visibility.Visible;
            gameUI.sendButton.Visibility = Visibility.Visible;
            gameUI.scroll.Visibility = Visibility.Visible;
            gameUI.timerTextBox.Visibility = Visibility.Visible;
            gameUI.olympiaButton.Visibility = Visibility.Visible;
            gameUI.estebanButton.Visibility = Visibility.Visible;
            gameUI.bilkisButton.Visibility = Visibility.Visible;
            gameUI.currentAgeLabel.Visibility = Visibility.Visible;
            gameUI.currentAge.Visibility = Visibility.Visible;
            gameUI.stackPanel1.Visibility = Visibility.Visible;
            gameUI.canvas1.Visibility = Visibility.Hidden;
            gameUI.helpButton.Visibility = Visibility.Visible;
        }

        public void sendChat()
        {
            string message;

            //determine the textfield that is non-empty and send that
            //this should not be necessary later on, when the UI is better
            if (gameUI.chatTextField.Text.Length != 0)
            {
                message = gameUI.chatTextField.Text;
            }
            else
            {
                message = tableUI.chatTextField.Text;
            }

            if (message.Length > 0) //check that the string is valid
            {
                //Tell the coordinator to send this a chat message to the server
                //# means chat message
                sendToHost("#" + message);
            }

            //reset the textfields
            gameUI.chatTextField.Text = "";
            tableUI.chatTextField.Text = "";
        }

        public void removeAI()
        {
            //tell the GMCoordinator, which in turn tells the GameManager, to remove an AI
            sendToHost("ar");
        }

        //tell the GMCoordinator, which in turn tells the GameManager, to add AI
        //UC-03 R01
        public void newAIUI(char mode)
        {
            AIStrategy aistrategy = new AIStrategy(mode, this);
            aistrategy.ShowDialog();
        }

        //display the Bilkis UI
        public void bilkisUIActivate()
        {
            //create new Olympia UI
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                this.bilkisUI = new BilkisUI(this);
                bilkisUI.ShowDialog();
            }));
        }

        /*
         * Send the Join Game request to the Server.
         */
        public void joinGame(string nickname, IPAddress serverIP)
        {
            this.nickname = nickname;

            client = new Client(this, nickname);
            client.InitializeConnection(serverIP);

            //set hasGame to true
            hasGame = true;

            //Display the non-host player version of TableUI
            sendToHost("J" + nickname);

            //display the tableUI
            //UC-02 R06

            tableUI = new TableUI(this);
            tableUI.addAIButton.IsEnabled = false;
            tableUI.removeAIButton.IsEnabled = false;
            tableUI.leaders_Checkbox.IsEnabled = false;
            tableUI.ShowDialog();
        }

        /*
         * Display the join table UI
         * UC-02 R02
         */
        public void displayJoinGameUI()
        {
            joinTableUI = new JoinTableUI(this);
            joinTableUI.ShowDialog();
        }


        /**
         * Function called by MainWindow for creating a new game
         * UC-01 R02
         */
        public void createGame()
        {
            //create a GM Coordinator
            // JDF - this shouldn't be needed any more, the GMCoordinator has moved to a separate process.
            // gmCoordinator = new GMCoordinator();

            hasGame = true;

            //automatically set the nickname to Host if the nickname is currently blank (which is the default)
            if (nickname == "")
            {
                nickname = "Host";
            }

            //get my IP
            IPAddress myIP = myIPAddress();

            //create the TCP Client
            client = new Client(this, nickname);

            client.InitializeConnection(myIP);
            
            //display the TableUI
            tableUI = new TableUI(this);
            //join the game as a player
            //UC-01 R03

            sendToHost("J" + nickname);
            tableUI.ShowDialog();
        }

        /// <summary>
        /// Invoke the halicarnassus screen at the end of the Age
        /// </summary>
        /// <param name="information"></param>
        private void receiveHalicarnassus(string information)
        {
            //open the halicarnssus window
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                halicarnassusUI = new HalicarnassusUI(this, information);
                halicarnassusUI.ShowDialog();
            }));
        }

        /// <summary>
        /// Invoke the Babylon screen at the end of the Age
        /// </summary>
        /// <param name="information"></param>
        private void receiveBabylon(string information)
        {
            //open the babylon window
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                babylonUI = new BabylonUI(this, information);
                babylonUI.ShowDialog();
            }));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Utility Classes
        /// </summary>
        /// <returns></returns>


        private IPAddress myIPAddress()
        {
            /*
            IPAddress localIP = null;
            IPHostEntry host;

            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip;
                }
            }

            return localIP;
             */

            return IPAddress.Loopback;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Networking functionalities
        /// only interpretUIAction will call these
        /// </summary>
        /// <param name="s"></param>

        /// <summary>
        /// Send string s to the Server
        /// The nickname will automatically be sent to the Server in the form of
        /// nickname_(message)
        /// </summary>
        /// <param name="s"></param>
        public void sendToHost(string s)
        {
            if(client != null)
                client.SendMessageToServer(s);
        }

        /// <summary>
        /// Client has received a message
        /// Call the appropriate action based on the first character
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void receiveMessage(string message)
        {
            //First character | Meaning
            //# --------------- chat message. E.g. "#John: I need some coins!" would print the string on the Chat
            //# is also used to display a Player joining a table.
            //J --------------- a player has joined the table. Add a player to the Game Manager
            //S --------------- the all ready signal. 5 second count down, then the join table window closes.

            //chat
            if (message[0] == '#')
            {
                updateChatTextBox(message.Substring(1));
            }
            //Receives signal from GMCoordinator on whether the game can start or not
            //S0 means game can start
            //S1 means game cannot (because of insufficient players)
            else if (message[0] == 'S')
            {
                //Handle when game cannot start
                if (message[1] == '1')
                {
                    //re-enable the ready button
                    Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        tableUI.readyButton.IsEnabled = true;
                    }));
                }
                //game is starting
                else if (message[1] == '0')
                {
                    //tell the server UI initialisation is done
                    sendToHost("r");

                    //close the TableUI
                    Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        tableUI.Close();
                    }));
                }
            }
            //update the Hand cards and Action panel
            else if (message[0] == 'U')
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    //update the hand panel with the information
                    gameUI.showHandPanel(message.Substring(1));
                }));
            }
            //update the Player Bar panel
            //also start up the timer
            else if (message[0] == 'B')
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.showPlayerBarPanel(message.Substring(1));
                }));
            }
            //update the current stage of wonder information
            else if (message[0] == 's')
            {
                updateCurrentStageLabelAndStartTimer(message);
            }
            //update the board panel
            else if (message[0] == 'b')
            {
                //"b_(name)"
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.showBoardImage(message.Substring(1));
                }));
            }
            //update the played cards panel
            else if (message[0] == 'P')
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.showPlayedCardsPanel(message.Substring(1));
                }));
            }
            //indicate to client to start timer
            else if (message[0] == 't')
            {
                startTimer();
            }
            //create the commerce if necessary 
            else if (message[0] == 'C')
            {
                createAndUpdateCommerce(message.Substring(1));
            }
            //enable the Olympia button
            else if (message == "EO")
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.olympiaButton.IsEnabled = true;
                }));
            }
            //enable Olympia power OR Rome power
            //activate the Olympia UI
            else if (message[0] == 'O')
            {
                //create new Olympia UI
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    olympiaUI = new OlympiaUI(this, message.Substring(1));
                }));
            }
            //receive the information on the current turn
            else if (message[0] == 'T')
            {
                //get the current turn information
                currentTurn = int.Parse(message[1] + "");
            }

            //receive halicarnassus or babylon information
            else if (message[0] == 'H' || message[0] == 'A')
            {
                if (message[0] == 'A')
                {
                    receiveBabylon(message);
                }
                else
                {
                    //No card in the discard pile, therefore Halicarnassus cannot be played
                    //just send back the end turn signal
                    if (message == "H0")
                    {
                        endTurn();
                    }
                    else
                    {
                        receiveHalicarnassus(message);
                    }
                }
            }
            //receive the information on view details for player
            else if (message[0] == 'V')
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    //pass this information to handleViewDetails in GameUI
                    gameUI.handleViewDetails(message);
                }));
            }
            //received an unable to join message from the server
            //UC-02 R07
            else if (message[0] == '0')
            {
                MessageBox.Show(message.Substring(2));

                tableUI.Close();

                displayJoinGameUI();
            }

            //leaders: received Recruitment phase turn display (Age 0 turn)
            else if (message[0] == 'r')
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.showHandPanelLeadersPhase(message.Substring(1));
                }));
            }
            //enable the Esteban button
            else if (message == "EE")
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.estebanButton.IsEnabled = true;
                }));
            }
            //enable the Bilkis Button...forever
            else if (message == "EB")
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    gameUI.bilkisButton.IsEnabled = true;
                }));
            }
            //receive Courtesan's guild information
            else if (message[0] == 'c')
            {
                Application.Current.Dispatcher.Invoke(new Action(delegate
                {
                    CourtesanUI courtUI = new CourtesanUI(this, message.Substring(1));
                    courtUI.ShowDialog();
                }));
            }
            //receive the end of game signal
            else if (message[0] == 'e')
            {
                timer.Stop();
            }
        }

        public void createAndUpdateCommerce(string s)
        {
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                //gameUI.showCommerceUI(s);
                NewCommerce commerce = new NewCommerce(this, s);

                commerce.ShowDialog();
            }));
        }

        /// <summary>
        /// Return back if the current turn is the last turn of the age
        /// </summary>
        public bool currentTurnIsLastTurnOfAge()
        {
            if (currentTurn > 4) return true;
            else return false;
        }
    }
}
