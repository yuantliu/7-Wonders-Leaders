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
        public int coins;

        public Grid playerGrid;
        public Image boardImage;
        public Dictionary<StructureType, StackPanel> structuresBuilt = new Dictionary<StructureType, StackPanel>(7);
        public Label coinsLabel;

        public PlayerState()
        {
            coins = 3;
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

        PlayerState[] playerState;

        public bool playerPlayedHisTurn = false;

        Tuple<string, Buildable>[] id_buildable;

        Buildable stageBuildable;

        List<Card> fullCardList = new List<Card>();

        //constructor: create the UI. create the Coordinator object
        public MainWindow(Coordinator c)
        {
            InitializeComponent();

            this.coordinator = c;

            playerState = new PlayerState[c.numPlayers];

            for (int i = 0; i < c.numPlayers; ++i)
            {
                playerState[i] = new PlayerState();
            }

            playerState[0].playerGrid = SeatA;
            playerState[0].boardImage = PlayerBoardA;
            playerState[0].structuresBuilt[StructureType.RawMaterial] = ResourceStructuresA;
            playerState[0].structuresBuilt[StructureType.Goods] = GoodsStructuresA;
            playerState[0].structuresBuilt[StructureType.Commerce] = CommerceStructuresA;
            playerState[0].structuresBuilt[StructureType.Military] = MilitaryStructuresA;
            playerState[0].structuresBuilt[StructureType.Science] = ScienceStructuresA;
            playerState[0].structuresBuilt[StructureType.Civilian] = CivilianStructuresA;
            playerState[0].structuresBuilt[StructureType.Guild] = GuildStructuresA;
            playerState[0].coinsLabel = CoinsA;

            switch (c.numPlayers)
            {
                case 3:
                    playerState[1].playerGrid = SeatF;
                    playerState[1].boardImage = PlayerBoardF;
                    playerState[1].structuresBuilt[StructureType.RawMaterial] = ResourceStructuresF;
                    playerState[1].structuresBuilt[StructureType.Goods] = GoodsStructuresF;
                    playerState[1].structuresBuilt[StructureType.Commerce] = CommerceStructuresF;
                    playerState[1].structuresBuilt[StructureType.Military] = MilitaryStructuresF;
                    playerState[1].structuresBuilt[StructureType.Science] = ScienceStructuresF;
                    playerState[1].structuresBuilt[StructureType.Civilian] = CivilianStructuresF;
                    playerState[1].structuresBuilt[StructureType.Guild] = GuildStructuresF;
                    playerState[1].coinsLabel = CoinsF;

                    playerState[2].playerGrid = SeatD;
                    playerState[2].boardImage = PlayerBoardD;
                    playerState[2].structuresBuilt[StructureType.RawMaterial] = ResourceStructuresD;
                    playerState[2].structuresBuilt[StructureType.Goods] = GoodsStructuresD;
                    playerState[2].structuresBuilt[StructureType.Commerce] = CommerceStructuresD;
                    playerState[2].structuresBuilt[StructureType.Military] = MilitaryStructuresD;
                    playerState[2].structuresBuilt[StructureType.Science] = ScienceStructuresD;
                    playerState[2].structuresBuilt[StructureType.Civilian] = CivilianStructuresD;
                    playerState[2].structuresBuilt[StructureType.Guild] = GuildStructuresD;
                    playerState[2].coinsLabel = CoinsD;
                    break;

                case 4:
                    playerState[1].playerGrid = SeatC;
                    playerState[1].boardImage = PlayerBoardC;
                    playerState[1].structuresBuilt[StructureType.RawMaterial] = ResourceStructuresC;
                    playerState[1].structuresBuilt[StructureType.Goods] = GoodsStructuresC;
                    playerState[1].structuresBuilt[StructureType.Commerce] = CommerceStructuresC;
                    playerState[1].structuresBuilt[StructureType.Military] = MilitaryStructuresC;
                    playerState[1].structuresBuilt[StructureType.Science] = ScienceStructuresC;
                    playerState[1].structuresBuilt[StructureType.Civilian] = CivilianStructuresC;
                    playerState[1].structuresBuilt[StructureType.Guild] = GuildStructuresC;
                    playerState[1].coinsLabel = CoinsC;

                    playerState[2].playerGrid = SeatE;
                    playerState[2].boardImage = PlayerBoardE;
                    playerState[2].structuresBuilt[StructureType.RawMaterial] = ResourceStructuresE;
                    playerState[2].structuresBuilt[StructureType.Goods] = GoodsStructuresE;
                    playerState[2].structuresBuilt[StructureType.Commerce] = CommerceStructuresE;
                    playerState[2].structuresBuilt[StructureType.Military] = MilitaryStructuresE;
                    playerState[2].structuresBuilt[StructureType.Science] = ScienceStructuresE;
                    playerState[2].structuresBuilt[StructureType.Civilian] = CivilianStructuresE;
                    playerState[2].structuresBuilt[StructureType.Guild] = GuildStructuresE;
                    playerState[2].coinsLabel = CoinsE;

                    playerState[3].playerGrid = SeatG;
                    playerState[3].boardImage = PlayerBoardG;
                    playerState[3].structuresBuilt[StructureType.RawMaterial] = ResourceStructuresG;
                    playerState[3].structuresBuilt[StructureType.Goods] = GoodsStructuresG;
                    playerState[3].structuresBuilt[StructureType.Commerce] = CommerceStructuresG;
                    playerState[3].structuresBuilt[StructureType.Military] = MilitaryStructuresG;
                    playerState[3].structuresBuilt[StructureType.Science] = ScienceStructuresG;
                    playerState[3].structuresBuilt[StructureType.Civilian] = CivilianStructuresG;
                    playerState[3].structuresBuilt[StructureType.Guild] = GuildStructuresG;
                    playerState[3].coinsLabel = CoinsG;

                    break;

                case 5:
                    playerState[1].playerGrid = SeatC;
                    playerState[2].playerGrid = SeatD;
                    playerState[3].playerGrid = SeatE;
                    playerState[4].playerGrid = SeatF;
                    break;

                case 6:
                    playerState[1].playerGrid = SeatB;
                    playerState[2].playerGrid = SeatD;
                    playerState[3].playerGrid = SeatE;
                    playerState[4].playerGrid = SeatF;
                    playerState[5].playerGrid = SeatH;
                    break;

                case 7:
                    playerState[1].playerGrid = SeatB;
                    playerState[2].playerGrid = SeatC;
                    playerState[3].playerGrid = SeatD;
                    playerState[4].playerGrid = SeatF;
                    playerState[5].playerGrid = SeatG;
                    playerState[6].playerGrid = SeatH;
                    break;

                case 8:
                    playerState[1].playerGrid = SeatB;
                    playerState[2].playerGrid = SeatC;
                    playerState[3].playerGrid = SeatD;
                    playerState[4].playerGrid = SeatE;
                    playerState[5].playerGrid = SeatF;
                    playerState[6].playerGrid = SeatG;
                    playerState[7].playerGrid = SeatH;
                    break;
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
        public void showPlayerBarPanel(int player, string playerBarPanelInformation)
        {
            playerState[player].coins = int.Parse(playerBarPanelInformation);
            playerState[player].coinsLabel.Content = string.Format("Coins: {0}", playerState[player].coins);
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

            for (int i = 0; i < cardsAndStates.Count; ++i)
            {
                Tuple<string, Buildable> t = new Tuple<string, Buildable>(cardsAndStates[i].Key,
                    (Buildable)Enum.Parse(typeof(Buildable), cardsAndStates[i].Value));

                if (!t.Item1.StartsWith("WonderStage"))
                    id_buildable[i] = t;
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

                    case Buildable.False:
                        entry.BorderBrush = new SolidColorBrush(Colors.Gray);
                        break;
                }

                handPanel.Items.Add(entry);
            }

            // The player must choose a card before 
            btnBuildStructure.IsEnabled = false;
            btnDiscardStructure.IsEnabled = false;

            if (stageBuildable == Buildable.True)
            {
                btnBuildWonderStage.Content = "Build a wonder stage with this card";
                btnBuildWonderStage.IsEnabled = true;
            }
            else if (stageBuildable == Buildable.CommerceRequired)
            {
                btnBuildWonderStage.Content = "Build a wonder stage with this card (commerce required)";
                btnBuildWonderStage.IsEnabled = true;
            }
            else
            {
                btnBuildWonderStage.Content = "Wonder stage not buildable";
                btnBuildWonderStage.IsEnabled = false;
            }
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

                case Buildable.False:
                    btnBuildStructure.Content = "Resource requirements not met for building this card";
                    btnBuildStructure.IsEnabled = false;
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
                            coordinator.sendToHost("B" + id_buildable[handPanel.SelectedIndex].Item1);
                            coordinator.endTurn();
                        }
                        else
                        {
                            coordinator.sendToHost("Cb" + id_buildable[handPanel.SelectedIndex].Item1);
                        }
                        break;
                    case "btnBuildWonderStage":
                        if (stageBuildable == Buildable.True)
                        {
                            playedButton.IsEnabled = false;
                            playerPlayedHisTurn = true;
                            // bilkisButton.IsEnabled = false;
                            coordinator.sendToHost("S" + id_buildable[handPanel.SelectedIndex].Item1);
                            coordinator.endTurn();
                        }
                        else
                        {
                            coordinator.sendToHost("Cs" + id_buildable[handPanel.SelectedIndex].Item1);
                        }
                        break;
                    case "btnDiscardStructure":
                        playedButton.IsEnabled = false;
                        playerPlayedHisTurn = true;
                        // bilkisButton.IsEnabled = false;
                        coordinator.sendToHost("D" + id_buildable[handPanel.SelectedIndex].Item1);
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
        public void showBoardImage(int player, String information)
        {
            //information holds the board image file name
            BitmapImage boardImageSource = new BitmapImage();
            boardImageSource.BeginInit();
            boardImageSource.UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/boards/" + information + ".jpg");
            boardImageSource.EndInit();

            playerState[player].boardImage.Source = boardImageSource;
        }

        /// <summary>
        /// display the Played Cards combo boxes, given the String from Coordinator
        /// </summary>
        /// <param name="player">Player ID (0..7)</param>
        /// <param name="cardName">Name of the card</param>
        public void showPlayedCardsPanel(int player, string cardName)
        {
            Card lastPlayedCard = fullCardList.Find(x => x.name == cardName);

            StructureType colour = lastPlayedCard.structureType;

            Label cardLabel = new Label();
            cardLabel.Content = lastPlayedCard.name;

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

            playerState[player].structuresBuilt[lastPlayedCard.structureType].Children.Add(cardData);
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