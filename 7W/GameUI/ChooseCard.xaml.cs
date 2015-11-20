using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SevenWonders
{
    /// <summary>
    /// Interaction logic for ChooseCard.xaml
    /// </summary>
    public partial class ChooseCard : Window
    {
        Coordinator coordinator;

        public const int ICON_WIDTH = 25;

        public const int CARD_WIDTH = 112;
        public const int CARD_HEIGHT = 206;

        //Buttons
        public Button[] buildStructureButton;
        Button[] buildStageButton;
        Button[] discardButton;

        public bool playerPlayedHisTurn = false;
        //variable that represent the button that was pressed in the cardActionPanel
        Button playedButton = new Button();

        public ChooseCard(Coordinator c, HandPanelInformation handPanelData)
        {
            coordinator = c;

            InitializeComponent();

            int numberOfCards = handPanelData.id_buildable.Length;

            //create the appropriate image source files
            BitmapImage[] cardImageSource = new BitmapImage[numberOfCards];
            for (int i = 0; i < numberOfCards; i++)
            {
                cardImageSource[i] = new BitmapImage();
                cardImageSource[i].BeginInit();
                //Item1 of the id_buildable array of Tuples represents the id image
                cardImageSource[i].UriSource = new Uri(Environment.CurrentDirectory + @"\Resources\Images\cards\" + handPanelData.id_buildable[i].Item1 + ".jpg");
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
            if (handPanelData.stageBuildable == Buildable.True) { buildableStage = true; }
            else if (handPanelData.stageBuildable == Buildable.CommerceRequired) { buildableStage = true; name = "StageCommerce"; content = "Commerce"; }

            //Names of the buttons
            //Contents (the word that will be shown in the UI) of the buttons
            String[] names = new String[numberOfCards];
            String[] contents = new String[numberOfCards];

            for (int i = 0; i < numberOfCards; i++)
            {
                contents[i] = "Build Structure";
                if (handPanelData.id_buildable[i].Item2 == Buildable.True || handPanelData.id_buildable[i].Item2 == Buildable.False)
                {
                    names[i] = "Build";
                }
                else if (handPanelData.id_buildable[i].Item2 == Buildable.CommerceRequired)
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
                buildStructureButton[i].Name = names[i] + "_" + handPanelData.id_buildable[i].Item1;
                buildStructureButton[i].IsEnabled = (handPanelData.id_buildable[i].Item2 == Buildable.True || handPanelData.id_buildable[i].Item2 == Buildable.CommerceRequired);
                buildStructureButton[i].Click += cardActionButtonPressed;
                actionBuildPanel.Children.Add(buildStructureButton[i]);

                buildStageButton[i] = new Button();
                buildStageButton[i].Content = content;
                buildStageButton[i].Width = CARD_WIDTH;
                buildStageButton[i].Height = ICON_WIDTH;
                buildStageButton[i].Name = name + "_" + handPanelData.id_buildable[i].Item1;
                buildStageButton[i].IsEnabled = buildableStage;
                buildStageButton[i].Click += cardActionButtonPressed;
                actionStagePanel.Children.Add(buildStageButton[i]);

                discardButton[i] = new Button();
                discardButton[i].Content = "Discard Card";
                discardButton[i].Width = CARD_WIDTH;
                discardButton[i].Height = ICON_WIDTH;
                discardButton[i].Name = "Discard_" + handPanelData.id_buildable[i].Item1;
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
            }
        }
    }
}
