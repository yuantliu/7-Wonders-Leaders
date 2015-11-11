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
    /// Interaction logic for HalicarnassusUI.xaml
    /// </summary>
    public partial class HalicarnassusUI : Window
    {
        ComboBoxItem []cardComboBoxItems;
        Coordinator coordinator;

        //id of the selected Card
        int selectedCard;

        //Current directory
        String currentPath;

        public HalicarnassusUI(Coordinator c, String information)
        {
            //make graphics better
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

            //Current directory
            InitializeComponent();
            currentPath = Environment.CurrentDirectory;
            coordinator = c;
            processInformation(information);
        }

        public void processInformation(String information)
        {
            //H_(num of cards)_(id1)&(name)_(id2)&(name)_...(id_last)&(name)|
            //H_100
            //H_10_
            //H_1_1
            //01234

            //extract the number of cards
            //start at position 2, parse the appropriate string based on the number of digits
            //otherwise it is a two digit number
            int numOfCardsLength = 0;
            for (numOfCardsLength = 0; information[2 + numOfCardsLength] != '_'; numOfCardsLength++) { }

            int numOfCards = int.Parse(information.Substring(2, numOfCardsLength));

            //start at position (2+numOfCardsLength), look for underscores, store the id of cards and name of cards
            int[] id = new int[numOfCards];
            string[] name = new string[numOfCards];

            int beginpoint = 2 + numOfCardsLength + 1;
            for (int i = 0; i < numOfCards; i++)
            {
                //extract id
                int length = 0;
                while (information[beginpoint + length] != '&')
                {
                    length++;
                }

                //H_(num of cards)_(id1)&(name)_(id2)&(name)_...(id_last)&(name)|
                //num of cards can be double digits, this doesnt handle it
                id[i] = int.Parse(information.Substring(beginpoint, length));

                //go to next starting point
                beginpoint = beginpoint + length + 1;

                //extract name
                length = 0;
                while (information[beginpoint + length] != '_' && information[beginpoint + length] != '|')
                {
                    length++;
                }

                name[i] = information.Substring(beginpoint, length);

                //go to the next starting point
                beginpoint = beginpoint + length + 1;
            }

            //display it on the UI elements
            cardComboBoxItems = new ComboBoxItem[numOfCards];

            //first, add the option to not play anything
            ComboBoxItem emptyItem = new ComboBoxItem();
            emptyItem.Tag = 0;
            emptyItem.Content = "Play nothing";
            cardComboBox.Items.Add(emptyItem);

            for (int i = 0; i < numOfCards; i++)
            {
                cardComboBoxItems[i] = new ComboBoxItem();

                cardComboBoxItems[i].Tag = id[i];
                cardComboBoxItems[i].Content = name[i];

                cardComboBox.Items.Add(cardComboBoxItems[i]);
            }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //end the turn
            coordinator.endTurn();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            //check if the User has selected a valid discard pile card
            if (selectedCard > 0)
            {
                //send the selected card
                //o(id)
                coordinator.sendToHost("H" + selectedCard);
            }

            //close the window
            Close();
        }
    }
}