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

namespace SevenWonders
{
    /// <summary>
    /// Interaction logic for BabylonUI.xaml
    /// </summary>
    public partial class BabylonUI : Window
    {
		Coordinator coordinator;
		string cardName;
        bool closeButton = true;
        Buildable isCardBuildable, stageBuildable;

        public BabylonUI(Coordinator c, IList<KeyValuePair<string, string>> qscoll)
        {
			coordinator = c;

            InitializeComponent();

            cardName = qscoll[0].Key;

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

            BitmapImage cardImageSource = new BitmapImage();
            cardImageSource.BeginInit();
            cardImageSource.UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/cards/" + cardName + ".jpg");
            cardImageSource.EndInit();

            image1.Source = cardImageSource;

            isCardBuildable = (Buildable)Enum.Parse(typeof(Buildable), qscoll[0].Value);

            switch (isCardBuildable)
            {
                case Buildable.True:
                    buildStructureButton.Content = "Build this structure";
                    break;

                case Buildable.CommerceRequired:
                    buildStructureButton.Content = "Build this structure (commerce required)";
                    buildStructureButton.IsEnabled = true;
                    break;

                case Buildable.InsufficientResources:
                    buildStructureButton.Content = "Resource requirements not met for building this structure.";
                    buildStructureButton.IsEnabled = false;
                    break;

                case Buildable.InsufficientCoins:
                    buildStructureButton.Content = "You don't have enough coins to buy this structure.";
                    buildStructureButton.IsEnabled = false;
                    break;

                case Buildable.StructureAlreadyBuilt:
                    buildStructureButton.Content = "You have already built one of these structures.";
                    buildStructureButton.IsEnabled = false;
                    break;

            }

            stageBuildable = (Buildable)Enum.Parse(typeof(Buildable), qscoll[1].Value);

            switch (stageBuildable)
            {
                case Buildable.True:
                    buildStageButton.Content = "Build a wonder stage with this card";
                    break;

                case Buildable.CommerceRequired:
                    buildStageButton.Content = "Build a wonder stage with this card (commerce required)";
                    break;

                case Buildable.InsufficientCoins:
                case Buildable.InsufficientResources:
                    buildStageButton.Content = "Resource requirements not met";
                    buildStageButton.IsEnabled = false;
                    break;

                case Buildable.StructureAlreadyBuilt:
                    buildStageButton.Content = "All wonder stages have been built";
                    buildStageButton.IsEnabled = false;
                    break;
            }

            //do the discard button
            discardButton.Content = "Discard Card";
        }

        private void buildStructureButton_Click(object sender, RoutedEventArgs e)
        {
            //card is buildable
            if (isCardBuildable == Buildable.True)
            {
                //send the instruction for building the card
                //B(id)
                coordinator.sendToHost(string.Format("BldStrct&WonderStage=0&Structure={0}", cardName));
                closeButton = false;
                //end the turn
                coordinator.endTurn();
                Close();
            }
            else if (isCardBuildable == Buildable.CommerceRequired)
            {
                //use commerce
                //closeButton = false;
                coordinator.sendToHost("SendComm");     // the server's response will open the Commerce Dialog box
                Close();
            }
        }

        private void buildStageButton_Click(object sender, RoutedEventArgs e)
        {
            //send the instruction for building the stage
            //S(id)
            //card is buildable
            if (stageBuildable == Buildable.True)
            {
                coordinator.sendToHost(string.Format("BldStrct&WonderStage=1&Structure={0}", cardName));
                closeButton = false;
                //end the turn
                coordinator.endTurn();
                Close();
            }
            else if (stageBuildable == Buildable.CommerceRequired)
            {
                //use commerce
                //closeButton = false;
                coordinator.sendToHost("SendComm");     // the server's response will open the Commerce Dialog box
                Close();
            }
        }

        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (closeButton)
            {
                //default action: discard card
                coordinator.sendToHost(string.Format("Discards&Structure={0}", cardName));
                // coordinator.sendToHost("DiscardS" + id);
                //end the turn
                coordinator.endTurn();
            }
        }
    }
}
