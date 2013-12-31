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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SevenWonders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //dimensions for the icons at the Player bars
        public const int ICON_WIDTH = 25;
        public const int CARD_WIDTH = 112;
        public const int CARD_HEIGHT = 206;

        //Client's coordinator
        Coordinator coordinator;

        //Current directory
        String currentPath = Environment.CurrentDirectory;

        //Individual Player bar panel
        StackPanel[] playerBarPanel;

        //Buttons
        public Button[] buildStructureButton;
        Button[] buildStageButton;
        Button[] discardButton;

        public bool playerPlayedHisTurn = false;
        //variable that represent the button that was pressed in the cardActionPanel
        Button playedButton = new Button();


        //constructor: create the UI. create the Coordinator object
        public MainWindow()
        {
            //create the coordinator
            coordinator = new Coordinator(this);

            InitializeComponent();

            //make graphics better
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Menu UI event handlers

        //Event handlers for clicking the Create Table button
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            //tell the coordinator that create game Button is pressed
            //UC-01 R01
            coordinator.createGame();
        }

        //Event handler for clicking the Join Table button
        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            //tell the coordinator that join game Button is pressed
            //UC-02 R01
            coordinator.displayJoinGameUI();
        }

        private void NickNameButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

     

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Updates
        // Receives Strings from Coordinator
        // to update various UI information

        /// <summary>
        /// Display the Player Bar Panel information, given the String from Coordinator
        /// </summary>
        /// <param name="playerBarPanelInformation"></param>
        public void showPlayerBarPanel(String playerBarPanelInformation)
        {          

            //clear the PlayerPanel of its current contents
            playerPanel.Children.Clear();

            PlayerBarInformation playerBarInformation = (PlayerBarInformation)Marshaller.StringToObject(playerBarPanelInformation);

            //if Player has no money, then disable Bilkis

            //get the number of players
            int numberOfPlayers = playerBarInformation.numOfPlayers;            

            //initialize the icons
            //Static icon images used in Player Bar Panel
            Image[] brickIcon, oreIcon, stoneIcon, woodIcon, glassIcon, loomIcon, papyrusIcon, bearTrapIcon, sextantIcon, tabletIcon,
                victoryIcon, shieldIcon, coinIcon, conflictIcon, conflictTokensCountIcon, lossIcon;
            brickIcon = new Image[numberOfPlayers];
            oreIcon = new Image[numberOfPlayers];
            stoneIcon = new Image[numberOfPlayers];
            woodIcon = new Image[numberOfPlayers];
            glassIcon = new Image[numberOfPlayers];
            loomIcon = new Image[numberOfPlayers];
            papyrusIcon = new Image[numberOfPlayers];
            bearTrapIcon = new Image[numberOfPlayers];
            sextantIcon = new Image[numberOfPlayers];
            tabletIcon = new Image[numberOfPlayers];
            victoryIcon = new Image[numberOfPlayers];
            shieldIcon = new Image[numberOfPlayers];
            coinIcon = new Image[numberOfPlayers];
            conflictIcon = new Image[numberOfPlayers];
            conflictTokensCountIcon = new Image[numberOfPlayers];
            lossIcon = new Image[numberOfPlayers];


            //display the information
            //create the appropriate amount of bars and add the elements to each bar
            playerBarPanel = new StackPanel[numberOfPlayers];

            Button[] viewDetails = new Button[numberOfPlayers];

            //initilize the labels which will display the amounts
            Label[] playerLabel = new Label[numberOfPlayers];
            Label[] brickLabel = new Label[numberOfPlayers];
            Label[] oreLabel = new Label[numberOfPlayers];
            Label[] stoneLabel = new Label[numberOfPlayers];
            Label[] woodLabel = new Label[numberOfPlayers];
            Label[] glassLabel = new Label[numberOfPlayers];
            Label[] loomLabel = new Label[numberOfPlayers];
            Label[] papyrusLabel = new Label[numberOfPlayers];
            Label[] bearTrapLabel = new Label[numberOfPlayers];
            Label[] sextantLabel = new Label[numberOfPlayers];
            Label[] tabletLabel = new Label[numberOfPlayers];
            Label[] victoryLabel = new Label[numberOfPlayers];
            Label[] shieldLabel = new Label[numberOfPlayers];
            Label[] coinLabel = new Label[numberOfPlayers];
            Label[] conflictLabel = new Label[numberOfPlayers];
            Label[] conflictTokensCountLabel = new Label[numberOfPlayers];
            Label[] lossLabel = new Label[numberOfPlayers];


            for (int i = 0; i < numberOfPlayers; i++)
            {
                //create the icons
                //create the view detail buttons
                viewDetails[i] = new Button();
                BitmapImage viewDetailsImageSource = new BitmapImage();
                viewDetailsImageSource.BeginInit();
                viewDetailsImageSource.UriSource = new Uri(currentPath + "\\Images\\details.png");
                viewDetailsImageSource.EndInit();
                viewDetails[i].Background = new ImageBrush(viewDetailsImageSource);
                viewDetails[i].Width = ICON_WIDTH;
                viewDetails[i].Height = ICON_WIDTH;
                //add that player's name as the tag
                viewDetails[i].Name = playerBarInformation.playerInfo[i].nickname;

                //create the brick pictures
                brickIcon[i] = new Image();
                BitmapImage brickIconImageSource = new BitmapImage();
                brickIconImageSource.BeginInit();
                brickIconImageSource.UriSource = new Uri(currentPath + "\\Images\\brick.png");
                brickIconImageSource.EndInit();
                brickIcon[i].Source = brickIconImageSource;
                brickIcon[i].Width = ICON_WIDTH;
                brickIcon[i].Height = ICON_WIDTH;

                //create the ore pictures
                oreIcon[i] = new Image();
                BitmapImage oreIconImageSource = new BitmapImage();
                oreIconImageSource.BeginInit();
                oreIconImageSource.UriSource = new Uri(currentPath + "\\Images\\ore.png");
                oreIconImageSource.EndInit();
                oreIcon[i].Source = oreIconImageSource;
                oreIcon[i].Width = ICON_WIDTH;
                oreIcon[i].Height = ICON_WIDTH;

                //create the stone pictures
                stoneIcon[i] = new Image();
                BitmapImage stoneIconImageSource = new BitmapImage();
                stoneIconImageSource.BeginInit();
                stoneIconImageSource.UriSource = new Uri(currentPath + "\\Images\\stone.png");
                stoneIconImageSource.EndInit();
                stoneIcon[i].Source = stoneIconImageSource;
                stoneIcon[i].Width = ICON_WIDTH;
                stoneIcon[i].Height = ICON_WIDTH;

                //create the wood picture
                woodIcon[i] = new Image();
                BitmapImage woodIconImageSource = new BitmapImage();
                woodIconImageSource.BeginInit();
                woodIconImageSource.UriSource = new Uri(currentPath + "\\Images\\wood.png");
                woodIconImageSource.EndInit();
                woodIcon[i].Source = woodIconImageSource;
                woodIcon[i].Width = ICON_WIDTH;
                woodIcon[i].Height = ICON_WIDTH;

                //create the glass pictures
                glassIcon[i] = new Image();
                BitmapImage glassIconImageSource = new BitmapImage();
                glassIconImageSource.BeginInit();
                glassIconImageSource.UriSource = new Uri(currentPath + "\\Images\\glass.png");
                glassIconImageSource.EndInit();
                glassIcon[i].Source = glassIconImageSource;
                glassIcon[i].Width = ICON_WIDTH;
                glassIcon[i].Height = ICON_WIDTH;

                //create the loom pictures
                loomIcon[i] = new Image();
                BitmapImage loomIconImageSource = new BitmapImage();
                loomIconImageSource.BeginInit();
                loomIconImageSource.UriSource = new Uri(currentPath + "\\Images\\loom.png");
                loomIconImageSource.EndInit();
                loomIcon[i].Source = loomIconImageSource;
                loomIcon[i].Width = ICON_WIDTH;
                loomIcon[i].Height = ICON_WIDTH;

                //create the papyrus pictures
                papyrusIcon[i] = new Image();
                BitmapImage papyrusIconImageSource = new BitmapImage();
                papyrusIconImageSource.BeginInit();
                papyrusIconImageSource.UriSource = new Uri(currentPath + "\\Images\\papyrus.png");
                papyrusIconImageSource.EndInit();
                papyrusIcon[i].Source = papyrusIconImageSource;
                papyrusIcon[i].Width = ICON_WIDTH;
                papyrusIcon[i].Height = ICON_WIDTH;

                //create the science pictures
                bearTrapIcon[i] = new Image();
                BitmapImage bearTrapIconImageSource = new BitmapImage();
                bearTrapIconImageSource.BeginInit();
                bearTrapIconImageSource.UriSource = new Uri(currentPath + "\\Images\\bearTrap.png");
                bearTrapIconImageSource.EndInit();
                bearTrapIcon[i].Source = bearTrapIconImageSource;
                bearTrapIcon[i].Width = ICON_WIDTH;
                bearTrapIcon[i].Height = ICON_WIDTH;

                sextantIcon[i] = new Image();
                BitmapImage sextantIconImageSource = new BitmapImage();
                sextantIconImageSource.BeginInit();
                sextantIconImageSource.UriSource = new Uri(currentPath + "\\Images\\sextant.png");
                sextantIconImageSource.EndInit();
                sextantIcon[i].Source = sextantIconImageSource;
                sextantIcon[i].Width = ICON_WIDTH;
                sextantIcon[i].Height = ICON_WIDTH;

                tabletIcon[i] = new Image();
                BitmapImage tabletIconImageSource = new BitmapImage();
                tabletIconImageSource.BeginInit();
                tabletIconImageSource.UriSource = new Uri(currentPath + "\\Images\\tablet.jpg");
                tabletIconImageSource.EndInit();
                tabletIcon[i].Source = tabletIconImageSource;
                tabletIcon[i].Width = ICON_WIDTH;
                tabletIcon[i].Height = ICON_WIDTH;

                //create the victory pictures
                victoryIcon[i] = new Image();
                BitmapImage victoryIconImageSource = new BitmapImage();
                victoryIconImageSource.BeginInit();
                victoryIconImageSource.UriSource = new Uri(currentPath + "\\Images\\victory.png");
                victoryIconImageSource.EndInit();
                victoryIcon[i].Source = victoryIconImageSource;
                victoryIcon[i].Width = ICON_WIDTH;
                victoryIcon[i].Height = ICON_WIDTH;

                //create the shield points pictures
                shieldIcon[i] = new Image();
                BitmapImage shieldIconImageSource = new BitmapImage();
                shieldIconImageSource.BeginInit();
                shieldIconImageSource.UriSource = new Uri(currentPath + "\\Images\\shield.jpg");
                shieldIconImageSource.EndInit();
                shieldIcon[i].Source = shieldIconImageSource;
                shieldIcon[i].Width = ICON_WIDTH;
                shieldIcon[i].Height = ICON_WIDTH;

                //create the coin pictures
                coinIcon[i] = new Image();
                BitmapImage coinIconImageSource = new BitmapImage();
                coinIconImageSource.BeginInit();
                coinIconImageSource.UriSource = new Uri(currentPath + "\\Images\\coin.png");
                coinIconImageSource.EndInit();
                coinIcon[i].Source = coinIconImageSource;
                coinIcon[i].Width = ICON_WIDTH;
                coinIcon[i].Height = ICON_WIDTH;

                //create the conflict pictures (total points from conflict)
                conflictIcon[i] = new Image();
                BitmapImage conflictIconImageSource = new BitmapImage();
                conflictIconImageSource.BeginInit();
                conflictIconImageSource.UriSource = new Uri(currentPath + "\\Images\\fight1.png");
                conflictIconImageSource.EndInit();
                conflictIcon[i].Source = conflictIconImageSource;
                conflictIcon[i].Width = ICON_WIDTH;
                conflictIcon[i].Height = ICON_WIDTH;

                //number of conflict tokens present (from 0 - 6)
                conflictTokensCountIcon[i] = new Image();
                BitmapImage conflictTokensCountSource = new BitmapImage();
                conflictTokensCountSource.BeginInit();
                conflictTokensCountSource.UriSource = new Uri(currentPath + "\\Images\\fight2.png");
                conflictTokensCountSource.EndInit();
                conflictTokensCountIcon[i].Source = conflictTokensCountSource;
                conflictTokensCountIcon[i].Width = ICON_WIDTH;
                conflictTokensCountIcon[i].Height = ICON_WIDTH;

                //create the loss pictures
                lossIcon[i] = new Image();
                BitmapImage lossIconImageSource = new BitmapImage();
                lossIconImageSource.BeginInit();
                lossIconImageSource.UriSource = new Uri(currentPath + "\\Images\\fight0.png");
                lossIconImageSource.EndInit();
                lossIcon[i].Source = lossIconImageSource;
                lossIcon[i].Width = ICON_WIDTH;
                lossIcon[i].Height = ICON_WIDTH;

                //create and add each player's bar
                playerBarPanel[i] = new StackPanel();
                playerBarPanel[i].Orientation = Orientation.Horizontal;
                playerPanel.Children.Add(playerBarPanel[i]);

                //add the Magnifying buttons
                playerBarPanel[i].Children.Add(viewDetails[i]);
                //add the action listeners
                viewDetails[i].Click += viewDetailsButtonPressed;

                //the player's name label
                playerLabel[i] = new Label();
                playerLabel[i].Content = playerBarInformation.playerInfo[i].nickname;
                playerLabel[i].Width = 50;
                playerBarPanel[i].Children.Add(playerLabel[i]);

                //brick icon
                playerBarPanel[i].Children.Add(brickIcon[i]);
                //brick label
                brickLabel[i] = new Label();
                brickLabel[i].Content = playerBarInformation.playerInfo[i].brick;
                playerBarPanel[i].Children.Add(brickLabel[i]);

                //ore icon
                playerBarPanel[i].Children.Add(oreIcon[i]);
                //ore label
                oreLabel[i] = new Label();
                oreLabel[i].Content = playerBarInformation.playerInfo[i].ore;
                playerBarPanel[i].Children.Add(oreLabel[i]);

                //stone icon
                playerBarPanel[i].Children.Add(stoneIcon[i]);
                //stone label
                stoneLabel[i] = new Label();
                stoneLabel[i].Content = playerBarInformation.playerInfo[i].stone;
                playerBarPanel[i].Children.Add(stoneLabel[i]);

                //wood icon
                playerBarPanel[i].Children.Add(woodIcon[i]);
                //wood label
                woodLabel[i] = new Label();
                woodLabel[i].Content = playerBarInformation.playerInfo[i].wood;
                playerBarPanel[i].Children.Add(woodLabel[i]);


                //glass icon
                playerBarPanel[i].Children.Add(glassIcon[i]);
                //glass label
                glassLabel[i] = new Label();
                glassLabel[i].Content = playerBarInformation.playerInfo[i].glass;
                playerBarPanel[i].Children.Add(glassLabel[i]);

                //papyrus icon
                playerBarPanel[i].Children.Add(papyrusIcon[i]);
                //papyrus label
                papyrusLabel[i] = new Label();
                papyrusLabel[i].Content = playerBarInformation.playerInfo[i].papyrus;
                playerBarPanel[i].Children.Add(papyrusLabel[i]);

                //loom icon
                playerBarPanel[i].Children.Add(loomIcon[i]);
                //loom label
                loomLabel[i] = new Label();
                loomLabel[i].Content = playerBarInformation.playerInfo[i].loom;
                playerBarPanel[i].Children.Add(loomLabel[i]);

                //science icons
                playerBarPanel[i].Children.Add(bearTrapIcon[i]);
                bearTrapLabel[i] = new Label();
                bearTrapLabel[i].Content = playerBarInformation.playerInfo[i].bear;
                playerBarPanel[i].Children.Add(bearTrapLabel[i]);

                playerBarPanel[i].Children.Add(sextantIcon[i]);
                sextantLabel[i] = new Label();
                sextantLabel[i].Content = playerBarInformation.playerInfo[i].sextant;
                playerBarPanel[i].Children.Add(sextantLabel[i]);

                playerBarPanel[i].Children.Add(tabletIcon[i]);
                tabletLabel[i] = new Label();
                tabletLabel[i].Content = playerBarInformation.playerInfo[i].tablet;
                playerBarPanel[i].Children.Add(tabletLabel[i]);

                //victory icons
                playerBarPanel[i].Children.Add(victoryIcon[i]);
                victoryLabel[i] = new Label();
                victoryLabel[i].Content = playerBarInformation.playerInfo[i].victory;
                playerBarPanel[i].Children.Add(victoryLabel[i]);

                //shield icons
                playerBarPanel[i].Children.Add(shieldIcon[i]);
                shieldLabel[i] = new Label();
                shieldLabel[i].Content = playerBarInformation.playerInfo[i].shield;
                playerBarPanel[i].Children.Add(shieldLabel[i]);

                //coin icons
                playerBarPanel[i].Children.Add(coinIcon[i]);
                coinLabel[i] = new Label();
                coinLabel[i].Content = playerBarInformation.playerInfo[i].coin;
                playerBarPanel[i].Children.Add(coinLabel[i]);

                //conflict icons
                playerBarPanel[i].Children.Add(conflictIcon[i]);
                conflictLabel[i] = new Label();
                conflictLabel[i].Content = playerBarInformation.playerInfo[i].conflict;
                playerBarPanel[i].Children.Add(conflictLabel[i]);

                //conflict tokens count icons
                playerBarPanel[i].Children.Add(conflictTokensCountIcon[i]);
                conflictTokensCountLabel[i] = new Label();
                conflictTokensCountLabel[i].Content = playerBarInformation.playerInfo[i].conflictTokensCount;
                playerBarPanel[i].Children.Add(conflictTokensCountLabel[i]);

                //loss icons
                playerBarPanel[i].Children.Add(lossIcon[i]);
                lossLabel[i] = new Label();
                lossLabel[i].Content = playerBarInformation.playerInfo[i].loss;
                playerBarPanel[i].Children.Add(lossLabel[i]);
            }
        }

        /// <summary>
        /// Action handler for the view details buttons
        /// Send a request for information to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewDetailsButtonPressed(object sender, RoutedEventArgs e)
        {
            Button viewDetailsButton = sender as Button;

            //store the name of the player that is being inspected
            String inspectedPlayerName = viewDetailsButton.Name;
            
            //send the information to the server
            coordinator.sendToHost("V" + inspectedPlayerName);
        }

        /// <summary>
        /// Create a new View Details UI
        /// </summary>
        /// <param name="information"></param>
        public void handleViewDetails(String information)
        {
            ViewDetails detailUI = new ViewDetails(information.Substring(1));
        }

        /// <summary>
        /// display the Cards in Player's hands and the available actions
        /// </summary>
        /// <param name="information"></param>
        public void showHandPanel(String information)
        {
            //the player is in a new turn now because his UI are still updating.
            //Therefore set playerPlayedHisturn to false
            playerPlayedHisTurn = false;

            //convert the String to an HandPanelInformation object
            HandPanelInformation handPanelInformation = (HandPanelInformation)Marshaller.StringToObject(information);

            //Update the Age label
            //since this method is only used in Age 1, 2, and 3, therefore, just show the age number
            //Age 0 is handled in showHandLeadersPhase(String information)

            currentAge.Content = handPanelInformation.currentAge;

            //update Images


            //get the number of cards
            int numberOfCards = handPanelInformation.id_buildable.Length;

            //create the appropriate image source files
            BitmapImage[] cardImageSource = new BitmapImage[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                cardImageSource[i] = new BitmapImage();
                cardImageSource[i].BeginInit();
                //Item1 of the id_buildable array of Tuples represents the id image
                cardImageSource[i].UriSource = new Uri(currentPath + "\\Images\\cards\\" + handPanelInformation.id_buildable[i].Item1 + ".jpg");
                cardImageSource[i].EndInit();
            }

            //Card Images
            //create the appropriate amount of Images
            //add them to the Card panel
            handPanel.Children.Clear();
            Image[] card = new Image[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                card[i] = new Image();
                card[i].Source = cardImageSource[i];
                card[i].Width = CARD_WIDTH;
                card[i].Height = CARD_HEIGHT;
                handPanel.Children.Add(card[i]);
            }

            //set the Stage of Wonder buildability
            String name = "Stage", content = "Build Stage";
            bool buildableStage = false;
            if (handPanelInformation.stageBuildable == 'T') { buildableStage = true; }
            else if (handPanelInformation.stageBuildable == 'C') { buildableStage = true; name = "StageCommerce"; content = "Commerce"; }

            //Names of the buttons
            //Contents (the word that will be shown in the UI) of the buttons
            String[] names = new String[numberOfCards];
            String[] contents = new String[numberOfCards];

            for (int i = 0; i < numberOfCards; i++)
            {
                contents[i] = "Build Structure";
                if (handPanelInformation.id_buildable[i].Item2 == 'T' || handPanelInformation.id_buildable[i].Item2 == 'F')
                {
                    names[i] = "Build";
                }
                else if (handPanelInformation.id_buildable[i].Item2 == 'C')
                {
                    names[i] = "BuildCommerce";
                    contents[i] = "Commerce";
                }
            }

            //add the appropriate buttons
            actionBuildPanel.Children.Clear();
            actionStagePanel.Children.Clear();
            actionDiscardPanel.Children.Clear();

            buildStructureButton = new Button[numberOfCards];
            buildStageButton = new Button[numberOfCards];
            discardButton = new Button[numberOfCards];


            //display the action Buttons
            for (int i = 0; i < numberOfCards; i++)
            {
                buildStructureButton[i] = new Button();
                buildStructureButton[i].Content = contents[i];
                buildStructureButton[i].Width = CARD_WIDTH;
                buildStructureButton[i].Height = ICON_WIDTH;
                buildStructureButton[i].Name = names[i] + "_" + handPanelInformation.id_buildable[i].Item1;
                buildStructureButton[i].IsEnabled = (handPanelInformation.id_buildable[i].Item2 == 'T' || handPanelInformation.id_buildable[i].Item2 == 'C');
                buildStructureButton[i].Click += cardActionButtonPressed;
                actionBuildPanel.Children.Add(buildStructureButton[i]);

                buildStageButton[i] = new Button();
                buildStageButton[i].Content = content;
                buildStageButton[i].Width = CARD_WIDTH;
                buildStageButton[i].Height = ICON_WIDTH;
                buildStageButton[i].Name = name + "_" + handPanelInformation.id_buildable[i].Item1;
                buildStageButton[i].IsEnabled = buildableStage;
                buildStageButton[i].Click += cardActionButtonPressed;
                actionStagePanel.Children.Add(buildStageButton[i]);

                discardButton[i] = new Button();
                discardButton[i].Content = "Discard Card";
                discardButton[i].Width = CARD_WIDTH;
                discardButton[i].Height = ICON_WIDTH;
                discardButton[i].Name = "Discard_" + handPanelInformation.id_buildable[i].Item1;
                discardButton[i].IsEnabled = true;
                discardButton[i].Click += cardActionButtonPressed;
                actionDiscardPanel.Children.Add(discardButton[i]);
            }
        }


        
        /// <summary>
        /// Event handler for the Card Action Buttons created in showActionPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cardActionButtonPressed(object sender, RoutedEventArgs e)
        {

            if (!playerPlayedHisTurn)
            {
                playedButton = sender as Button;
                String s = playedButton.Name;

                //send to the server the Action selected
                if (s.StartsWith("Build_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("B" + s.Substring(6));
                    coordinator.endTurn();
                }
                else if (s.StartsWith("Stage_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("S" + s.Substring(6));
                    coordinator.endTurn();
                }
                else if (s.StartsWith("Discard_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("D" + s.Substring(8));
                    coordinator.endTurn();
                }

                else if (s.StartsWith("BuildCommerce_"))
                {
                    coordinator.sendToHost("Cb" + s.Substring(14));
                }

                else if (s.StartsWith("StageCommerce_"))
                {
                    coordinator.sendToHost("Cs" + s.Substring(14));
                }

                else if (s.StartsWith("Recruit_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("l" + s.Substring(8));
                    coordinator.endTurn();
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        

        /// <summary>
        /// display the Board, given the String from Coordinator
        /// </summary>
        /// <param name="information"></param>
        public void showBoardImage(String information)
        {
            //information holds the board image file name
            BitmapImage boardImageSource = new BitmapImage();
            boardImageSource.BeginInit();
            boardImageSource.UriSource = new Uri(currentPath + "\\Images\\boards\\" + information + ".jpg");
            boardImageSource.EndInit();

            boardImage.Source = boardImageSource;
        }

        /// <summary>
        /// display the Played Cards combo boxes, given the String from Coordinator
        /// </summary>
        /// <param name="information"></param>
        public void showPlayedCardsPanel(String information)
        {
            //extract the colour
            //the name
            //the id number

            LastPlayedCardInformation lastPlayedCard = (LastPlayedCardInformation)Marshaller.StringToObject(information);

            string colour = lastPlayedCard.colour;
            string name = lastPlayedCard.name;
            int id = lastPlayedCard.id;
            
            //add a selection to the appropriate drop down menu
            ComboBoxItem combo = new ComboBoxItem();
            combo.Tag = id;
            combo.Content = name;

            if(colour == "Blue")
            {
                bluePlayedCards.Items.Add(combo);
            }
            else if(colour == "Brown")
            {
                brownPlayedCards.Items.Add(combo);
            }
            else if (colour == "Green")
            {
                greenPlayedCards.Items.Add(combo);
            }
            else if (colour == "Grey")
            {
                greyPlayedCards.Items.Add(combo);
            }
            else if (colour == "Purple")
            {
                purplePlayedCards.Items.Add(combo);
            }
            else if (colour == "Red")
            {
                redPlayedCards.Items.Add(combo);
            }
            else if (colour == "Yellow")
            {
                yellowPlayedCards.Items.Add(combo);
            }
            else if (colour == "White")
            {
                whitePlayedCards.Items.Add(combo);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //handler for the ComboBoxItems in Played Cards panel
        private void playedCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ca = e.Source as ComboBox;

            ComboBoxItem caaa = ca.SelectedItem as ComboBoxItem;

            //have the card Image change
            showCardImage(currentPath + "\\Images\\cards\\" + caaa.Tag + ".jpg");
        }

        /// <summary>
        /// Show the highlighted card's image from the Played Cards combobox
        /// </summary>
        /// <param name="path"></param>
        public void showCardImage(String path)
        {
            BitmapImage cardImageSource = new BitmapImage();
            cardImageSource.BeginInit();
            cardImageSource.UriSource = new Uri(path);
            cardImageSource.EndInit();
            cardImage.Source = cardImageSource;
        }

        /// <summary>
        /// Send a chat message to the Coordinator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            coordinator.sendChat();
        }

        /// <summary>
        /// Handle the Olympia power button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void olympiaButton_Click(object sender, RoutedEventArgs e)
        {
            coordinator.olympiaButtonClicked();
        }

        public void discardCommerce()
        {
            playerPlayedHisTurn = false;
            playedButton.IsEnabled = true;
        }

        private void image8_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            coordinator.createGame();
        }

        private void chatTextField_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) coordinator.sendChat(); 
        }

        private void joinGameIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            coordinator.displayJoinGameUI();
        }

        private void quitIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            coordinator.quit();
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            Help helpUI = new Help();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //If there is an ongoing game, then coordinator must quit the game first
            if (coordinator.hasGame == true)
            {
                coordinator.quit();
            }
        }

        public void showHandPanelLeadersPhase(string information)
        {
            //the player is in a new turn now because his UI are still updating.
            //Therefore set playerPlayedHisturn to false
            playerPlayedHisTurn = false;

            //convert the String to an HandPanelInformation object
            RecruitmentPhaseInformation handPanelInformation = (RecruitmentPhaseInformation)Marshaller.StringToObject(information);

            //Update the Age label
            //since this method is only used in Age 0, it shall say Leaders phase
            //Age 0 is handled in showHandLeadersPhase(String information)

            currentAge.Content = "Leaders Phase";


            //get the number of cards
            int numberOfCards = handPanelInformation.ids.Length;

            //create the appropriate image source files
            BitmapImage[] cardImageSource = new BitmapImage[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                cardImageSource[i] = new BitmapImage();
                cardImageSource[i].BeginInit();
                //Item1 of the id_buildable array of Tuples represents the id image
                cardImageSource[i].UriSource = new Uri(currentPath + "\\Images\\cards\\" + handPanelInformation.ids[i] + ".jpg");
                cardImageSource[i].EndInit();
            }

            //Card Images
            //create the appropriate amount of Images
            //add them to the Card panel
            handPanel.Children.Clear();
            Image[] card = new Image[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                card[i] = new Image();
                card[i].Source = cardImageSource[i];
                card[i].Width = CARD_WIDTH;
                card[i].Height = CARD_HEIGHT;
                handPanel.Children.Add(card[i]);
            }


            //Names of the buttons. (These buttons will all say "Recruit")
            //Contents (the word that will be shown in the UI) of the buttons. (All will say "Recruit")
            String[] names = new String[numberOfCards];
            String[] contents = new String[numberOfCards];

            for (int i = 0; i < numberOfCards; i++)
            {
                names[i] = "Recruit";
                contents[i] = "Recruit";
            }

            //add the appropriate buttons
            actionBuildPanel.Children.Clear();
            actionStagePanel.Children.Clear();
            actionDiscardPanel.Children.Clear();

            //Build structure button will say "Recruit" instead for this phase
            //other ones will not say anything
            buildStructureButton = new Button[numberOfCards];

            //display the action Buttons
            for (int i = 0; i < numberOfCards; i++)
            {
                buildStructureButton[i] = new Button();
                buildStructureButton[i].Content = contents[i];
                buildStructureButton[i].Width = CARD_WIDTH;
                buildStructureButton[i].Height = ICON_WIDTH;
                buildStructureButton[i].Name = names[i] + "_" + handPanelInformation.ids[i];
                buildStructureButton[i].IsEnabled = true;
                buildStructureButton[i].Click += cardActionButtonPressed;
                actionBuildPanel.Children.Add(buildStructureButton[i]);
            }
        }

        /// <summary>
        /// Player clicks the Esteban Button
        /// Freeze the passing of next turn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void estebanButton_Click(object sender, RoutedEventArgs e)
        {
            //tell server to toggle esteban on
            coordinator.sendToHost("Esteban");

            coordinator.sendToHost("#" + coordinator.nickname + " uses Esteban to freeze the next turn!");

            //disable Esteban button now
            estebanButton.IsEnabled = false;
        }

        /// <summary>
        /// Player clicks the Bilkis Button
        /// Should bring up the Bilkis interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bilkisButton_Click(object sender, RoutedEventArgs e)
        {
            coordinator.bilkisUIActivate();
        }
    }
}