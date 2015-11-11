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
    /// Interaction logic for CourtesanUI.xaml
    /// </summary>
    public partial class CourtesanUI : Window
    {
        ComboBoxItem[] leaderComboBoxItems;

        string currentPath;

        int selectedCard;

        Coordinator c;

        public CourtesanUI(Coordinator c, string information)
        {
            CourtesanGuildInformation info = (CourtesanGuildInformation)(Marshaller.StringToObject(information));

            this.c = c;

            InitializeComponent();

            leaderComboBoxItems = new ComboBoxItem[info.card.Count];

            //first, add a "Choose none" option
            ComboBoxItem noneComboBoxItem = new ComboBoxItem();

            noneComboBoxItem.Tag = 0;
            noneComboBoxItem.Content = "Choose nothing";
            leaderComboBox.Items.Add(noneComboBoxItem);

            //populate the Combo box with names and id
            for (int i = 0; i < info.card.Count; i++)
            {
                leaderComboBoxItems[i] = new ComboBoxItem();

                leaderComboBoxItems[i].Tag = (int)info.card[i].Item2;
                leaderComboBoxItems[i].Content = (string)info.card[i].Item1;

                leaderComboBox.Items.Add(leaderComboBoxItems[i]);
            }

            //make graphics bettter
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            currentPath = Environment.CurrentDirectory;

            
        }

        /// <summary>
        /// Changed a selection on the Leaders combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leaderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ca = e.Source as ComboBox;

            ComboBoxItem caaa = ca.SelectedItem as ComboBoxItem;

            //show the image on the UI
            showCardImage(currentPath + @"\Resources\Images\cards\" + caaa.Tag + ".jpg");

            //set the selected ID
            selectedCard = int.Parse(caaa.Tag + "");

            //enable the confirm button
            copyButton.IsEnabled = true;
        }

        private void showCardImage(string path)
        {
            BitmapImage cardImageSource = new BitmapImage();
            cardImageSource.BeginInit();
            cardImageSource.UriSource = new Uri(path);
            cardImageSource.EndInit();
            leaderCardImage.Source = cardImageSource;
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCard > 0)
            {
                //send to host the id of the selected card
                c.sendToHost("c" + selectedCard);
            }

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //end the turn
            c.endTurn();
        }
    }
}
