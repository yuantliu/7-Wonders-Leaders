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
    /// Interaction logic for NewCommerce.xaml
    /// </summary>
    public partial class NewCommerce : Window
    {
        const int ICON_WIDTH = 25;

        //player's coordinator
        Coordinator c;

        //immutable original string cost
        string cardCost;

        //immutable core card/player information
        bool hasDiscount;
        bool leftRawMarket, leftManuMarket, rightRawMarket, rightManuMarket;
        string leftName, middleName, rightName;
        
        //DAGs
        DAG leftDag, middleDag, rightDag;

        //current accumulated resources
        string currentCost = "";
        //how much gold needed
        int goldCost = 0;

        /// <summary>
        /// Set the coordinator and handle CommerceInformation, which contains all necessary UI data, from GameManager
        /// </summary>
        public NewCommerce(Coordinator c, string data)
        {
            this.c = c;
            CommerceInformation commerceData = (CommerceInformation)Marshaller.StringToObject(data);

            //gather necessary UI information
            this.cardCost = commerceData.cardCost;
            this.hasDiscount = commerceData.hasDiscount;
            this.leftRawMarket = commerceData.leftRawMarket;
            this.rightRawMarket = commerceData.rightRawMarket;
            this.leftManuMarket = commerceData.leftManuMarket;
            this.rightManuMarket = commerceData.rightManuMarket;
            this.leftName = commerceData.playerCommerceInfo[0].name;
            this.middleName = commerceData.playerCommerceInfo[1].name;
            this.rightName = commerceData.playerCommerceInfo[2].name;

            leftDag = commerceData.playerCommerceInfo[0].dag;
            middleDag = commerceData.playerCommerceInfo[1].dag;
            rightDag = commerceData.playerCommerceInfo[2].dag;
            //all necessary UI information gathered

            //construct the UI
            string currentPath = Environment.CurrentDirectory;


            //set the name labels
            leftNameLabel.Content = leftName;
            middleNameLabel.Content = middleName;
            rightNameLabel.Content = rightName;

            //set the market images
            if(leftRawMarket == true)
                leftRawImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\1r.png"));
            else
                leftRawImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\2r.png"));
            if(leftManuMarket == true)
                leftManuImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\1m.png"));
            else
                leftManuImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\2m.png"));
            if (rightRawMarket == true)
                rightRawImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\1r.png"));
            else
                rightRawImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\2r.png"));
            if (rightManuMarket == true)
                rightManuImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\1m.png"));
            else
                rightManuImage.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\2m.png"));

            //set the discount label
            if (hasDiscount == true)
            {
                hasDiscountLabel.Visibility = Visibility.Visible;
            }
            else
            {
                hasDiscountLabel.Visibility = Visibility.Hidden;
            }

            InitializeComponent();
        }

        /// <summary>
        /// Submit button event handler. Client sends to server current UI state and receives response from server.
        /// Commerce UI will either close when receiving success signal or pop out an error.
        /// If no response within 3 seconds, then Commerce UI will close as if "Cancel" button was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
