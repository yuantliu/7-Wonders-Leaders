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
    /// Interaction logic for ViewDetails.xaml
    /// </summary>
    public partial class ViewDetails : Window
    {
        //Current directory
        String currentPath = Environment.CurrentDirectory;

        
        public ViewDetails(String information)
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);


            ViewDetailsInformation myInfo = (ViewDetailsInformation)(Serializer.StringToObject(information));


            //get the board name
            String boardName = myInfo.boardname;

            //figure out how many stages have been built by the player
            int numOfStagesBuilt = myInfo.numOfStagesBuilt;

            //load the board image
            BitmapImage boardSource = new BitmapImage();
            boardSource.BeginInit();
            boardSource.UriSource = new Uri(currentPath + "\\Images\\boards\\" + boardName + ".jpg");
            boardSource.EndInit();
            boardImage.Source = boardSource;

            //change the drop down menus
            //add the blue items
            for (int i = 0; i < myInfo.blueCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.blueCards[i].Item2;
                combo.Content = myInfo.blueCards[i].Item1;
                bluePlayedCards.Items.Add(combo);
            }

            //add the brown items
            for (int i = 0; i < myInfo.brownCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.brownCards[i].Item2;
                combo.Content = myInfo.brownCards[i].Item1;
                brownPlayedCards.Items.Add(combo);
            }

            //add the green items
            for (int i = 0; i < myInfo.greenCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.greenCards[i].Item2;
                combo.Content = myInfo.greenCards[i].Item1;
                greenPlayedCards.Items.Add(combo);
            }

            //add the grey items
            for (int i = 0; i < myInfo.greyCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.greyCards[i].Item2;
                combo.Content = myInfo.greyCards[i].Item1;
                greyPlayedCards.Items.Add(combo);
            }

            //add the purple items
            for (int i = 0; i < myInfo.purpleCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.purpleCards[i].Item2;
                combo.Content = myInfo.purpleCards[i].Item1;
                purplePlayedCards.Items.Add(combo);
            }

            //add the red items
            for (int i = 0; i < myInfo.redCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.redCards[i].Item2;
                combo.Content = myInfo.redCards[i].Item1;
                redPlayedCards.Items.Add(combo);
            }

            //add the yellow items
            for (int i = 0; i < myInfo.yellowCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.yellowCards[i].Item2;
                combo.Content = myInfo.yellowCards[i].Item1;
                yellowPlayedCards.Items.Add(combo);
            }

            //add the leader (white) cards
            for (int i = 0; i < myInfo.whiteCards.Count; i++)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Tag = myInfo.whiteCards[i].Item2;
                combo.Content = myInfo.whiteCards[i].Item1;
                whitePlayedCards.Items.Add(combo);
            }

            //change the display for number of stages built
            currentStageLabel.Content = "" + numOfStagesBuilt;

            //display the dialog
            ShowDialog();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            Close();
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
    }
}
