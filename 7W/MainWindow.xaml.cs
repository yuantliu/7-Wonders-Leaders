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
    public class PlayerState
    {
        public Dictionary<StructureType, StackPanel> structuresBuilt = new Dictionary<StructureType, StackPanel>(7);
        public Label lastCardPlayed;

        public PlayerStateWindow state;

        public PlayerState(PlayerStateWindow plyr, string name)
        {
            state = plyr;

            structuresBuilt[StructureType.RawMaterial] = plyr.ResourceStructures;
            structuresBuilt[StructureType.Goods] = plyr.GoodsStructures;
            structuresBuilt[StructureType.Commerce] = plyr.CommerceStructures;
            structuresBuilt[StructureType.Military] = plyr.MilitaryStructures;
            structuresBuilt[StructureType.Science] = plyr.ScienceStructures;
            structuresBuilt[StructureType.Civilian] = plyr.CivilianStructures;
            structuresBuilt[StructureType.Guild] = plyr.GuildStructures;

            plyr.PlayerName.Content = "Name: " + name;
        }
    };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //dimensions for the icons at the Player bars
        const int ICON_HEIGHT = 25;

        //Client's coordinator
        Coordinator coordinator;

        Dictionary<string, PlayerState> playerState = new Dictionary<string, PlayerState>();

        public bool playerPlayedHisTurn = false;

        Tuple<string, Buildable>[] id_buildable;

        Buildable stageBuildable;

        // List<Card> fullCardList = new List<Card>();

//        public string commerceStructure { get; private set; }
        // 0 if we're building a structure, 1-4 if we're building a stage

//        public int commerceStage { get; private set; }

        //constructor: create the UI. create the Coordinator object
        public MainWindow(Coordinator coordinator)
        {
            InitializeComponent();

            PlayerStateWindow[,] seatMap = new PlayerStateWindow[,] {
                { SeatA, SeatF, SeatD, null, null, null, null, null },      // 3 players
                { SeatA, SeatG, SeatE, SeatC, null, null, null, null },     // 4 players
                { SeatA, SeatG, SeatF, SeatD, SeatC, null, null, null },    // 5 players
                { SeatA, SeatH, SeatF, SeatE, SeatD, SeatB, null, null },   // 6 players
                { SeatA, SeatH, SeatG, SeatF, SeatD, SeatC, SeatB, null},   // 7 players
                { SeatA, SeatH, SeatG, SeatF, SeatE, SeatD, SeatC, SeatB }, // 8 players
           };

            this.coordinator = coordinator;

            for (int i = 0; i < coordinator.playerNames.Length; ++i)
            {
                playerState.Add(coordinator.playerNames[i], new PlayerState(seatMap[coordinator.playerNames.Length - 3, i], coordinator.playerNames[i]));
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Menu UI event handlers

#if FALSE
        //Event handlers for clicking the Create Table button
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            //tell the coordinator that create game Button is pressed
            //UC-01 R01
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
#endif


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // UI Updates
        // Receives Strings from Coordinator
        // to update various UI information

        /// <summary>
        /// Display the Player Bar Panel information, given the String from Coordinator
        /// </summary>
        /// <param name="playerBarPanelInformation"></param>
        public void showPlayerBarPanel(string playerName, string strCoins)
        {
            playerState[playerName].state.Coins.Content = string.Format("Coins: {0}", strCoins);
        }

        /// <summary>
        /// display the Cards in Player's hands and the available actions
        /// </summary>
        /// <param name="information"></param>
        public void showHandPanel(IList<KeyValuePair<string, string>> cardsAndStates/*String information*/)
        {
            //the player is in a new turn now because his UI are still updating.
            //Therefore set playerPlayedHisturn to false
            playerPlayedHisTurn = false;

            id_buildable = new Tuple<string, Buildable>[cardsAndStates.Count];

            int buildableIndexi = 0;
            foreach (KeyValuePair<string, string> kvp in cardsAndStates)
            {
                Tuple<string, Buildable> t = new Tuple<string, Buildable>(kvp.Key, (Buildable)Enum.Parse(typeof(Buildable), kvp.Value));

                if (!t.Item1.StartsWith("WonderStage"))
                    id_buildable[buildableIndexi++] = t;
                else
                    stageBuildable = t.Item2;
            }

            // should actually subtract the number of wonder stages that were included.  China can build them in any order the player wishes.
            int numberOfCards = id_buildable.Length - 1;

            handPanel.Items.Clear();

            for (int i = 0; i < numberOfCards; ++i)
            {
                BitmapImage bmpImg = new BitmapImage();
                bmpImg.BeginInit();
                //Item1 of the id_buildable array of Tuples represents the id image
                bmpImg.UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/cards/" + id_buildable[i].Item1 + ".jpg");
                bmpImg.EndInit();

                Image img = new Image();
                img.Source = bmpImg;

                ListBoxItem entry = new ListBoxItem();
                entry.Name = id_buildable[i].Item2.ToString();
                entry.Content = img;
                entry.BorderThickness = new Thickness(3);

                switch (id_buildable[i].Item2)
                {
                    case Buildable.True:
                        entry.BorderBrush = new SolidColorBrush(Colors.Green);
                        break;

                    case Buildable.CommerceRequired:
                        entry.BorderBrush = new SolidColorBrush(Colors.Yellow);
                        break;

                    default:
                        entry.BorderBrush = new SolidColorBrush(Colors.Gray);
                        break;
                }

                handPanel.Items.Add(entry);
            }

            // A card must be selected before the action buttons are activated.
            btnBuildStructure.IsEnabled = false;
            btnBuildWonderStage.IsEnabled = false;
            btnDiscardStructure.IsEnabled = false;
        }

        private void handPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (handPanel.SelectedItem == null)
                return;

            // Update the status of the build buttons when a card is selected.
            switch ((Buildable)Enum.Parse(typeof(Buildable), ((ListBoxItem)handPanel.SelectedItem).Name))
            {
                case Buildable.True:
                    btnBuildStructure.Content = "Build this structure";
                    btnBuildStructure.IsEnabled = true;
                    break;

                case Buildable.CommerceRequired:
                    btnBuildStructure.Content = "Build this structure (commerce required)";
                    btnBuildStructure.IsEnabled = true;
                    break;

                case Buildable.InsufficientResources:
                    btnBuildStructure.Content = "Resource requirements not met for building this structure.";
                    btnBuildStructure.IsEnabled = false;
                    break;

                case Buildable.InsufficientCoins:
                    btnBuildStructure.Content = "You don't have enough coins to buy this structure.";
                    btnBuildStructure.IsEnabled = false;
                    break;

                case Buildable.StructureAlreadyBuilt:
                    btnBuildStructure.Content = "You have already built one of these structures.";
                    btnBuildStructure.IsEnabled = false;
                    break;
            }

            switch (stageBuildable)
            {
                case Buildable.True:
                    btnBuildWonderStage.Content = "Build a wonder stage with this card";
                    btnBuildWonderStage.IsEnabled = true;
                    break;

                case Buildable.CommerceRequired:
                    btnBuildWonderStage.Content = "Build a wonder stage with this card (commerce required)";
                    btnBuildWonderStage.IsEnabled = true;
                    break;

                case Buildable.InsufficientCoins:
                case Buildable.InsufficientResources:
                    btnBuildWonderStage.Content = "Resource requirements not met";
                    btnBuildWonderStage.IsEnabled = false;
                    break;

                case Buildable.StructureAlreadyBuilt:
                    btnBuildWonderStage.Content = "All wonder stages have been built";
                    btnBuildWonderStage.IsEnabled = false;
                    break;
            }

            btnDiscardStructure.IsEnabled = true;
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
                Button playedButton = sender as Button;
                String s = playedButton.Name;

                //send to the server the Action selected
                switch (playedButton.Name)
                {
                    case "btnBuildStructure":
                        if (id_buildable[handPanel.SelectedIndex].Item2 == Buildable.True)
                        {
                            playedButton.IsEnabled = false;
                            playerPlayedHisTurn = true;
                            // bilkisButton.IsEnabled = false;
                            coordinator.sendToHost(string.Format("BldStrct&WonderStage=0&Structure={0}", id_buildable[handPanel.SelectedIndex].Item1));
                            coordinator.endTurn();
                        }
                        else
                        {
                            coordinator.sendToHost("SendComm&WonderStage=0&Structure=" + id_buildable[handPanel.SelectedIndex].Item1);     // the server's response will open the Commerce Dialog box
                        }
                        break;
                    case "btnBuildWonderStage":
                        if (stageBuildable == Buildable.True)
                        {
                            playedButton.IsEnabled = false;
                            playerPlayedHisTurn = true;
                            // bilkisButton.IsEnabled = false;
                            coordinator.sendToHost(string.Format("BldStrct&WonderStage=1&Structure={0}", id_buildable[handPanel.SelectedIndex].Item1));
                            coordinator.endTurn();
                        }
                        else
                        {
                            coordinator.sendToHost("SendComm&WonderStage=1&Structure=" + id_buildable[handPanel.SelectedIndex].Item1);     // the server's response will open the Commerce Dialog box
                        }
                        break;
                    case "btnDiscardStructure":
                        playedButton.IsEnabled = false;
                        playerPlayedHisTurn = true;
                        // bilkisButton.IsEnabled = false;
                        coordinator.sendToHost(string.Format("Discards&Structure={0}", id_buildable[handPanel.SelectedIndex].Item1));
                        coordinator.endTurn();
                        break;
                }

                /*
                if (s.StartsWith("Build_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    // bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("B" + s.Substring(6));
                    coordinator.endTurn();
                }
                else if (s.StartsWith("Stage_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    // bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("S" + s.Substring(6));
                    coordinator.endTurn();
                }
                else if (s.StartsWith("Discard_"))
                {
                    playedButton.IsEnabled = false;
                    playerPlayedHisTurn = true;
                    // bilkisButton.IsEnabled = false;
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
                    // bilkisButton.IsEnabled = false;
                    coordinator.sendToHost("l" + s.Substring(8));
                    coordinator.endTurn();
                }
                else
                {
                    throw new Exception();
                }
                */
            }
        }

        

        /// <summary>
        /// display the Board, given the String from Coordinator
        /// </summary>
        /// <param name="information"></param>
        public void showBoardImage(string player, String boardInformation)
        {
            //information holds the board image file name
            BitmapImage boardImageSource = new BitmapImage();
            boardImageSource.BeginInit();
            boardImageSource.UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/boards/" + boardInformation.Substring(2) + ".jpg");
            boardImageSource.EndInit();

            playerState[player].state.PlayerBoard.Source = boardImageSource;

            int nWonderStages = Int32.Parse(boardInformation.Substring(0, 1));

            for (int i = 0; i < nWonderStages; ++i)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);

                playerState[player].state.WonderStage.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < nWonderStages; ++i)
            {
                Label b = new Label();

                b.Background = new SolidColorBrush(Colors.Azure);
                Grid.SetColumn(b, i);
                playerState[player].state.WonderStage.Children.Add(b);
            }
        }

        /// <summary>
        /// display the Played Cards combo boxes, given the String from Coordinator
        /// </summary>
        /// <param name="player">Player ID (0..7)</param>
        /// <param name="cardName">Name of the card</param>
        public void updateCardsPlayed(string playerName, string cardName)
        {
            // some of these functions should be in the PlayerState class.
            if (cardName.Length == 12 && cardName.Substring(0, 11) == "WonderStage")
            {
                int stage = int.Parse(cardName.Substring(11));

                Label l = playerState[playerName].state.WonderStage.Children[stage - 1] as Label;

                l.Content = string.Format("Stage {0}", stage);
                l.Background = new SolidColorBrush(Colors.Yellow);
            }
            else if (cardName == "Discarded")
            {
                if (playerState[playerName].lastCardPlayed != null)
                    playerState[playerName].lastCardPlayed.Background = null;
            }
            else
            {
                Card lastPlayedCard = coordinator.FindCard(cardName);

                StructureType colour = lastPlayedCard.structureType;

                Label cardLabel = new Label();
                cardLabel.Background = new SolidColorBrush(Colors.LightGray);
                cardLabel.Background.Opacity = 0.5;
                cardLabel.Content = lastPlayedCard.name;

                if (playerState[playerName].lastCardPlayed != null)
                    playerState[playerName].lastCardPlayed.Background = null;

                playerState[playerName].lastCardPlayed = cardLabel;

                // This is how to get a control's DesiredSize:
                // cardLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                StackPanel cardData = new StackPanel();
                cardData.Orientation = Orientation.Horizontal;
                cardData.Children.Add(cardLabel);

                if (lastPlayedCard.iconName != string.Empty)
                {
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/Icons/" + lastPlayedCard.iconName + ".png");
                    bmi.EndInit();
                    Image iconImage = new Image();
                    iconImage.Source = bmi;
                    iconImage.Height = ICON_HEIGHT;
                    cardData.Children.Add(iconImage);
                }

                playerState[playerName].structuresBuilt[lastPlayedCard.structureType].Children.Add(cardData);
            }
        }

        public void updateMilitaryTokens(string playerName, string strConflictData)
        {
            // string should be age/victories in this age/total losses
            string[] s = strConflictData.Split('/');

            if (s.Length != 3)
                throw new Exception();

            int age = int.Parse(s[0]);
            int victoriesInThisAge = int.Parse(s[1]);
            int totalLosses = int.Parse(s[2]);

            switch (age)
            {
                case 1:
                    playerState[playerName].state.Age1ConflictTokens.Content = string.Format("Age {0} Victories: {1}", age, victoriesInThisAge);
                    break;

                case 2:
                    playerState[playerName].state.Age2ConflictTokens.Content = string.Format("Age {0} Conflicts: {1}", age, victoriesInThisAge);
                    break;

                case 3:
                    playerState[playerName].state.Age3ConflictTokens.Content = string.Format("Age {0} Conflicts: {1}", age, victoriesInThisAge);
                    break;
            }

            playerState[playerName].state.LossTokens.Content = string.Format("Loss tokens: {0}", totalLosses);
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

#if FALSE
        public void discardCommerce()
        {
            playerPlayedHisTurn = false;
            playedButton.IsEnabled = true;
        }

        private void image8_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            coordinator.createGame();
        }
#endif

        private void chatTextField_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) coordinator.sendChat(); 
        }

#if FALSE
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
#endif
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
            /*
            //the player is in a new turn now because his UI are still updating.
            //Therefore set playerPlayedHisturn to false
            playerPlayedHisTurn = false;

            //convert the String to an HandPanelInformation object
            RecruitmentPhaseInformation handPanelInformation = (RecruitmentPhaseInformation)Marshaller.StringToObject(information);

            //Update the Age label
            //since this method is only used in Age 0, it shall say Leaders phase
            //Age 0 is handled in showHandLeadersPhase(String information)

            // currentAge.Content = "Leaders Phase";


            //get the number of cards
            int numberOfCards = handPanelInformation.ids.Length;

            //create the appropriate image source files
            BitmapImage[] cardImageSource = new BitmapImage[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                cardImageSource[i] = new BitmapImage();
                cardImageSource[i].BeginInit();
                //Item1 of the id_buildable array of Tuples represents the id image
                cardImageSource[i].UriSource = new Uri(currentPath + @"\Resources\Images\cards\" + handPanelInformation.ids[i] + ".jpg");
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
            */
            //add the appropriate buttons
            //actionBuildPanel.Children.Clear();
            //actionStagePanel.Children.Clear();
            //actionDiscardPanel.Children.Clear();

            //Build structure button will say "Recruit" instead for this phase
            //other ones will not say anything
            /*
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
               // actionBuildPanel.Children.Add(buildStructureButton[i]);
            }
            */
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
            // estebanButton.IsEnabled = false;
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