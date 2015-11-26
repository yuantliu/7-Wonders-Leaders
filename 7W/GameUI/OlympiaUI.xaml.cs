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

namespace SevenWonders
{
    /// <summary>
    /// Interaction logic for OlympiaUI.xaml
    /// </summary>
    public partial class OlympiaUI : Window
    {
        ComboBoxItem[] cardComboBoxItems;
        Coordinator coordinator;

        //id of the selected Card
        int selectedCard;

        //Current directory
        String currentPath;

        //type of power
        //R is for Rome. O is for Olympia
        char mode;

        public OlympiaUI(Coordinator c, String information)
        {
            //make graphics better
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

            //Current directory
            InitializeComponent();
            currentPath = Environment.CurrentDirectory;
            coordinator = c;
            processInformation(information);
            ShowDialog();
        }

        public void processInformation(String information)
        {
#if FALSE
            //convert to Data
            PlayForFreeInformation info = (PlayForFreeInformation)(Marshaller.StringToObject(information));

            //extract the number of cards
            int numOfCards = info.cards.Length;

            //set the mode (Rome or Olympia)
            mode = info.mode;

            //display it on the UI elements
            cardComboBoxItems = new ComboBoxItem[numOfCards];

            //extract card information, add to combobox
            for (int i = 0; i < numOfCards; i++)
            {
                cardComboBoxItems[i] = new ComboBoxItem();

                //tag is ID. Corresponds to Item2 of tuple
                cardComboBoxItems[i].Tag = info.cards[i].Item2;
                //Content is the name. Corresponds to Item1 of tuple.
                cardComboBoxItems[i].Content = info.cards[i].Item1;

                cardComboBox.Items.Add(cardComboBoxItems[i]);
            }
#endif
        }

        /// <summary>
        /// handler for the Card ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cardComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ca = e.Source as ComboBox;

            ComboBoxItem caaa = ca.SelectedItem as ComboBoxItem;

            //show the image on the UI
            showCardImage(currentPath + @"\Resources\Images\cards\" + caaa.Tag + ".jpg");

            //set the selected ID
            selectedCard = int.Parse(caaa.Tag + "");

            //enable the confirm button
            confirmButton.IsEnabled = true;
        }

        private void showCardImage(String path)
        {
            BitmapImage cardImageSource = new BitmapImage();
            cardImageSource.BeginInit();
            cardImageSource.UriSource = new Uri(path);
            cardImageSource.EndInit();
            cardImage.Source = cardImageSource;
        }

        /// <summary>
        /// Handle the Confirm button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            //check if the User has actually selected anything
            //selectedCard, which is the ID of the selected card, should be > 0
            if (selectedCard > 0)
            {
                //Olympia:
                if (mode == 'O')
                {
                    //send the selected card
                    //o(id)
                    coordinator.sendToHost("o" + selectedCard);
                    //disable the Olympia button in gameUI
                    // coordinator.gameUI.olympiaButton.IsEnabled = false;
                    //end the turn
                    coordinator.endTurn();
                }
                else if (mode == 'R')
                {
                    coordinator.sendToHost("M" + selectedCard);
                    coordinator.endTurn();
                }

                //close the window
                Close();
            }
        }

        /// <summary>
        /// Handle the Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            //close the Window
            Close();
        }
    }
}
