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
    /// Interaction logic for CommerceUI.xaml
    /// </summary>
    public partial class CommerceUI : Window
    {

        //ComboBox dimensions
        int WIDTH = 35;
        int HEIGHT = 25;
        int ICON_WIDTH = 25;

        Coordinator coordinator;
        String cardId = "";


        int coinForLeftPlayer = 0;
        int coinForRightPlayer = 0;


        Boolean CANCELED = true;
        Boolean isStageOfWonder = false;

        bool hasDiscount = false;


        //This is for keeping track of what user bought
        int leftB = 0, rightB = 0;
        int leftO = 0, rightO = 0;
        int leftT = 0, rightT = 0;
        int leftW = 0, rightW = 0;
        int leftG = 0, rightG = 0;
        int leftP = 0, rightP = 0;
        int leftL = 0, rightL = 0;


        //how much resources are needed to buy
        int[] resourcesToBuy;

        int leftPlayerMarketEffectForRaw = 2, rightPlayerMarketEffectForRaw = 2;
        int leftPlayerMarketEffectForManufacture = 2, rightPlayerMarketEffectForManufacture = 2;


        public CommerceUI(Coordinator c, string s)
        {
            InitializeComponent();
            coordinator = c;
            resourcesToBuy = new int[7];

            //unpack the information package
            CommerceInformationPackage infoPack = (CommerceInformationPackage)Serializer.StringToObject(s);

            hasDiscount = infoPack.hasDiscount;

            if (hasDiscount)
                discountLabel.Visibility = Visibility.Visible;
            else
                discountLabel.Visibility = Visibility.Hidden;

            string commerceInfo = infoPack.information;

            constructCommerceWindow(commerceInfo);
        }


        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            Boolean shouldContinue = true;

            //Get the nickname of the player who is doing commerce
            String nickname = currentPlayer.Content.ToString();

            //Get the total coin, and coin of the user commercing
            int totalCoins = int.Parse(totalLabel.Content.ToString());
            int yourCoins = int.Parse(coinsLabel.Content.ToString());

            String[] resources = { "Brick", "Ore", "Stone", "Wood", "Glass", "Papyrus", "Loom" };

            //Discount leader cards: keep track of whether a discount has been used
            bool usedDiscount = false;

            //check if we bought what we need
            for (int i = 0; i < 7; i++)
            {
                //there is one resource left out, but if discount is avaiable, then one resource is allowed to be left out
                if ((resourcesToBuy[i] == 1) && (usedDiscount == false) && (hasDiscount == true))
                {
                    usedDiscount = true;
                }
                else if (resourcesToBuy[i] != 0)
                {
                    shouldContinue = false;
                    MessageBox.Show("You Need " + resources[i]);
                    break;
                }
            }

            if (shouldContinue)
            {
                //Check if commerce is legal
                if (totalCoins <= yourCoins)
                {
                    //Create the information string
                    String information = "";

                    //Add the card id
                    information += cardId + "_";

                    //Decrease the coins by the total totalCoins for the player is doing commerce 
                    information += "" + totalCoins + "_";

                    //distribute coin for left player and right player
                    information += "" + coinForLeftPlayer + "_";
                    information += "" + coinForRightPlayer + "_";

                    //tell what happened in commerce window to coordinator
                    coordinator.endOfCommerce(information, isStageOfWonder);

                    CANCELED = false;

                    //close the dialog
                    Close();

                }
                else
                {
                    MessageBox.Show("You Do Not Have Enough Coins");
                }
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            //tell the coordinator that the user cancelled the commerce, take action on this
            CANCELED = true;

            this.Close();
        }

        /// <summary>
        /// Construct the Commerce Window
        /// </summary>
        /// <param name="information"></param>
        public void constructCommerceWindow(String information)
        {
            String currentPath = Environment.CurrentDirectory;

            //information is in the form
            if (information[information.Length - 1] == '&') isStageOfWonder = true;

            Image[] costIcons = new Image[7];
            Label[] costLabels = new Label[7];
            Image[] leftIcons = new Image[7];
            Image[] currentIcons = new Image[7];
            Image[] rightIcons = new Image[7];
            BitmapImage[] IconImageSources = new BitmapImage[7];

            for (int i = 0; i < 7; i++)
            {
                leftIcons[i] = new Image();
                costIcons[i] = new Image();
                currentIcons[i] = new Image();
                rightIcons[i] = new Image();
                IconImageSources[i] = new BitmapImage();
                IconImageSources[i].BeginInit();
                if (i == 0) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\brick.png");
                if (i == 1) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\ore.png");
                if (i == 2) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\stone.png");
                if (i == 3) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\wood.png");
                if (i == 4) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\glass.png");
                if (i == 5) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\papyrus.png");
                if (i == 6) IconImageSources[i].UriSource = new Uri(currentPath + "\\Images\\loom.png");
                IconImageSources[i].EndInit();
                costIcons[i].Source = IconImageSources[i];
                costIcons[i].Width = ICON_WIDTH;
                costIcons[i].Height = ICON_WIDTH;
                leftIcons[i].Source = IconImageSources[i];
                currentIcons[i].Source = IconImageSources[i];
                rightIcons[i].Source = IconImageSources[i];
                leftIcons[i].Width = ICON_WIDTH;
                leftIcons[i].Height = ICON_WIDTH;
                currentIcons[i].Width = ICON_WIDTH;
                currentIcons[i].Height = ICON_WIDTH;
                rightIcons[i].Width = ICON_WIDTH;
                rightIcons[i].Height = ICON_WIDTH;
            }

            ///////////////////
            ////(id of the card)_(card cost)_//(currentUserNickname)_(resources)|(leftPlayerNickName)_(resources)|(rightPlayerNickname)_(resources)_(RawMarketEffectLeft)_(ManufatortMrketEffectLeft)_(rightMarketEffect)
            ///Example:
            ////12_TWTO_Host_5_1_1_1_1_1_2_1|yunus_2_2_2_2_2_2_2_5|hello_3_3_3_3_3_3_3_8|T_F_T_F_)"
            ////////////////

            String cost = "";
            String[] name = new String[3];
            String[,] resources = new String[3, 8];

            int index = 0;

            //Extract the CardID
            while (information[index] != '_') cardId += information[index++];
            index++;

            //Extract the Cost of the Card
            while (information[index] != '_')
            {
                cost += information[index];
                addTheCostToResourcesToBuy(information[index]);
                index++;
            } index++;

            //Extract the names of players and their resources
            for (int i = 0; i < 3; i++)
            {
                while (information[index] != '_') name[i] += information[index++];

                index++;
                for (int j = 0; j < 8; j++)
                {
                    while (information[index] != '_')
                    {
                        resources[i, j] += information[index++];
                        if (information[index] == '|') break;
                    }
                    index++;
                }
            }


            //Extract the market effect information
            while (information[index] != '_')
            {
                if (information[index++] == 'T') leftPlayerMarketEffectForRaw--;
            } index++;
            while (information[index] != '_')
            {
                if (information[index++] == 'T') leftPlayerMarketEffectForManufacture--;
            } index++;
            while (information[index] != '_')
            {
                if (information[index++] == 'T') rightPlayerMarketEffectForRaw--;
            } index++;
            while (information[index] != '_')
            {
                if (information[index++] == 'T') rightPlayerMarketEffectForManufacture--;
            }

            //BitmapImage source = 
         

            //Set the market Effects to display the cost of the materials
            leftRaw.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\" + leftPlayerMarketEffectForRaw + "r.png"));
            leftMan.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\" + leftPlayerMarketEffectForManufacture + "m.png"));
            rightRaw.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\" + rightPlayerMarketEffectForRaw + "r.png"));
            rightMan.Source = new BitmapImage(new Uri(currentPath + "\\Images\\Commerce\\" + rightPlayerMarketEffectForManufacture + "m.png"));
          /* leftManLabel.Content = "" + leftPlayerMarketEffectForManufacture + " Coins Each";
            rightRawLabel.Content = "" + rightPlayerMarketEffectForRaw + " Coins Each";
            rightManLabel.Content = "" + rightPlayerMarketEffectForManufacture + " Coins Each";

            */
            //Set the appropriate fields of the UI
            //costInfoLabel.Content = costFullName; 

            leftPlayer.Content = name[0];
            currentPlayer.Content = name[1];
            rightPlayer.Content = name[2];


            Label[] leftResourceLabel = new Label[7];
            Label[] currentResourceLabel = new Label[7];
            Label[] rightResourceLabel = new Label[7];


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (i == 0)
                    {
                        leftResourceLabel[j] = new Label();
                        leftResourceLabel[j].Content = resources[0, j];
                    }
                    if (i == 1)
                    {
                        currentResourceLabel[j] = new Label();
                        currentResourceLabel[j].Content = resources[1, j];
                    }
                    if (i == 2)
                    {
                        rightResourceLabel[j] = new Label();
                        rightResourceLabel[j].Content = resources[2, j];
                    }
                }
            }



            //How coins the user have
            coinsLabel.Content = resources[1, 7];



            ComboBox[] leftPlayerComboBox = new ComboBox[7];
            ComboBox[] rightPlayerComboBox = new ComboBox[7];


            for (int i = 0; i < 7; i++)
            {

                //move this here
                if ((resourcesToBuy[i] - int.Parse(resources[1, i])) >= 0) resourcesToBuy[i] -= int.Parse(resources[1, i]);
                else resourcesToBuy[i] = 0;

                //change this
                if (resourcesToBuy[i] > 0)
                {
                    //left Neighbour
                    leftPlayerComboBox[i] = new ComboBox();
                    leftPlayerComboBox[i].Width = WIDTH;
                    leftPlayerComboBox[i].Height = HEIGHT;
                    leftPlayerComboBox[i].Name = "left_" + i;
                    leftPlayerComboBox[i].SelectionChanged += comboBox_SelectionChanged;

                    for (int j = 0; j < int.Parse(resources[0, i]) + 1; j++)
                    {
                        leftPlayerComboBox[i].Items.Add(j);
                        if (j == 0) leftPlayerComboBox[i].SelectedItem = 0;
                    }

                    //RightNeigbour
                    rightPlayerComboBox[i] = new ComboBox();
                    rightPlayerComboBox[i].Width = WIDTH;
                    rightPlayerComboBox[i].Height = HEIGHT;
                    rightPlayerComboBox[i].Name = "right_" + i;
                    rightPlayerComboBox[i].SelectionChanged += comboBox_SelectionChanged;
                    for (int j = 0; j < int.Parse(resources[2, i]) + 1; j++)
                    {
                        rightPlayerComboBox[i].Items.Add(j);
                        if (j == 0) rightPlayerComboBox[i].SelectedItem = 0;
                    }

                    //add appropriate fields to the players
                    if (i < 4)
                    {
                        //Left Neighbour Raw Materials
                        LeftPlayerComboPanel.Children.Add(leftPlayerComboBox[i]);
                        leftPlayerResources.Children.Add(leftIcons[i]);
                        leftResourceLabelPanel.Children.Add(leftResourceLabel[i]);

                        //Current Player Raw materials
                        currentPlayerResources.Children.Add(currentIcons[i]);
                        currentPlayerLabelPanel.Children.Add(currentResourceLabel[i]);

                        //Right Neighbor Raw Materials 
                        rightPlayerComboPanel.Children.Add(rightPlayerComboBox[i]);
                        rightPlayerResources.Children.Add(rightIcons[i]);
                        rightResourceLabelPanel.Children.Add(rightResourceLabel[i]);
                    }
                    else
                    {
                        //left Neightbour Man Materials
                        LeftPlayerComboPanelMan.Children.Add(leftPlayerComboBox[i]);
                        leftPlayerResourcesMan.Children.Add(leftIcons[i]);
                        leftResourceLabelPanelMan.Children.Add(leftResourceLabel[i]);

                        //current Player man Materials
                        currentPlayerResourcesMan.Children.Add(currentIcons[i]);
                        currentPlayerLabelPanelMan.Children.Add(currentResourceLabel[i]);

                        //right Neighbour Man Materials
                        rightPlayerComboPanelMan.Children.Add(rightPlayerComboBox[i]);
                        rightPlayerResourcesMan.Children.Add(rightIcons[i]);
                        rightResourceLabelPanelMan.Children.Add(rightResourceLabel[i]);
                    }
                }
            }

            //show what i need to buy
            for (int i = 0; i < 7; i++)
            {
                costLabels[i] = new Label();
                if (resourcesToBuy[i] != 0)
                {
                    stackPanel1.Children.Add(costIcons[i]);
                    costLabels[i].Content = "" + resourcesToBuy[i];
                    stackPanel1.Children.Add(costLabels[i]);
                }
            }
        }


        //Actual name of the cost
        private void addTheCostToResourcesToBuy(char s)
        {
            int index = 0;

            switch (s)
            {
                case 'B': index = 0; break;
                case 'O': index = 1; break;
                case 'T': index = 2; break;
                case 'W': index = 3; break;
                case 'G': index = 4; break;
                case 'P': index = 5; break;
                case 'L': index = 6; break;
                default: { throw new Exception(); }
            }
            resourcesToBuy[index]++;
        }



        //handler for the ComboBoxItems in Played Cards panel
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox selectedComboBox = e.Source as ComboBox;
            ComboBoxItem selectedItem = selectedComboBox.SelectedItem as ComboBoxItem;

            String comboBoxName = selectedComboBox.Name;
            int selectedItemValue = int.Parse(selectedComboBox.SelectedItem.ToString());


            int resource = 0;
            int amount = 0;
            int total = int.Parse(totalLabel.Content.ToString());


            if (comboBoxName.StartsWith("left_"))
            {

                resource = int.Parse(selectedComboBox.Name.Substring(5));

                switch (resource)
                {
                    case 0:
                        if (leftB <= selectedItemValue)
                        {
                            resourcesToBuy[0] -= (selectedItemValue - leftB);
                            amount = (selectedItemValue - leftB) * leftPlayerMarketEffectForRaw;
                            leftB = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[0] += leftB - selectedItemValue;
                            amount = (leftB * leftPlayerMarketEffectForRaw - selectedItemValue * leftPlayerMarketEffectForRaw);
                            total -= amount;
                            leftB = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;
                    case 1:
                        if (leftO <= selectedItemValue)
                        {
                            resourcesToBuy[1] -= (selectedItemValue - leftO);
                            amount = (selectedItemValue - leftO) * leftPlayerMarketEffectForRaw;
                            leftO = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[1] += leftO - selectedItemValue;
                            amount = (leftO * leftPlayerMarketEffectForRaw - selectedItemValue * leftPlayerMarketEffectForRaw);
                            total -= amount;
                            leftO = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;
                    case 2:
                        if (leftT <= selectedItemValue)
                        {
                            resourcesToBuy[2] -= (selectedItemValue - leftT);
                            amount = (selectedItemValue - leftT) * leftPlayerMarketEffectForRaw;
                            leftT = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[2] += leftT - selectedItemValue;
                            amount = (leftT * leftPlayerMarketEffectForRaw - selectedItemValue * leftPlayerMarketEffectForRaw);
                            total -= amount;
                            leftT = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;
                    case 3:
                        if (leftW <= selectedItemValue)
                        {
                            resourcesToBuy[3] -= (selectedItemValue - leftW);
                            amount = (selectedItemValue - leftW) * leftPlayerMarketEffectForRaw;
                            leftW = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[3] += leftW - selectedItemValue;
                            amount = (leftW * leftPlayerMarketEffectForRaw - selectedItemValue * leftPlayerMarketEffectForRaw);
                            total -= amount;
                            leftW = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;
                    case 4:
                        if (leftG <= selectedItemValue)
                        {
                            resourcesToBuy[4] -= (selectedItemValue - leftG);
                            amount = (selectedItemValue - leftG) * leftPlayerMarketEffectForManufacture;
                            leftG = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[4] += leftG - selectedItemValue;
                            amount = (leftG * leftPlayerMarketEffectForManufacture - selectedItemValue * leftPlayerMarketEffectForManufacture);
                            total -= amount;
                            leftG = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;
                    case 5:
                        if (leftP <= selectedItemValue)
                        {
                            resourcesToBuy[5] -= (selectedItemValue - leftP);
                            amount = (selectedItemValue - leftP) * leftPlayerMarketEffectForManufacture;
                            leftP = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[5] += leftP - selectedItemValue;
                            amount = (leftP * leftPlayerMarketEffectForManufacture - selectedItemValue * leftPlayerMarketEffectForManufacture);
                            total -= amount;
                            leftP = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;
                    case 6:
                        if (leftL <= selectedItemValue)
                        {
                            resourcesToBuy[6] -= (selectedItemValue - leftL);
                            amount = (selectedItemValue - leftL) * leftPlayerMarketEffectForManufacture;
                            leftL = selectedItemValue;
                            coinForLeftPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[6] += leftL - selectedItemValue;
                            amount = (leftL * leftPlayerMarketEffectForManufacture - selectedItemValue * leftPlayerMarketEffectForManufacture);
                            total -= amount;
                            leftL = selectedItemValue;
                            coinForLeftPlayer -= amount;
                        }
                        break;

                }



            }

            else if (selectedComboBox.Name.StartsWith("right_"))
            {
                resource = int.Parse(selectedComboBox.Name.Substring(6));

                switch (resource)
                {
                    case 0:
                        if (rightB <= selectedItemValue)
                        {
                            resourcesToBuy[0] -= (selectedItemValue - rightB);
                            amount = (selectedItemValue - rightB) * rightPlayerMarketEffectForRaw;
                            rightB = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;

                        }
                        else
                        {
                            resourcesToBuy[0] += rightB - selectedItemValue;
                            amount = (rightB * rightPlayerMarketEffectForRaw - selectedItemValue * rightPlayerMarketEffectForRaw);
                            total -= amount;
                            rightB = selectedItemValue;
                            coinForRightPlayer -= amount;

                        }
                        break;
                    case 1:
                        if (rightO <= selectedItemValue)
                        {
                            resourcesToBuy[1] -= (selectedItemValue - rightO);
                            amount = (selectedItemValue - rightO) * rightPlayerMarketEffectForRaw;
                            rightO = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[1] += rightO - selectedItemValue;
                            amount = (rightO * rightPlayerMarketEffectForRaw - selectedItemValue * rightPlayerMarketEffectForRaw);
                            total -= amount;
                            rightO = selectedItemValue;
                            coinForRightPlayer -= amount;
                        }
                        break;
                    case 2:
                        if (rightT <= selectedItemValue)
                        {
                            resourcesToBuy[2] -= (selectedItemValue - rightT);
                            amount = (selectedItemValue - rightT) * rightPlayerMarketEffectForRaw;
                            rightT = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[2] += rightT - selectedItemValue;
                            amount = (rightT * rightPlayerMarketEffectForRaw - selectedItemValue * rightPlayerMarketEffectForRaw);
                            total -= amount;
                            rightT = selectedItemValue;
                            coinForRightPlayer -= amount;
                        }
                        break;
                    case 3:
                        if (rightW <= selectedItemValue)
                        {
                            resourcesToBuy[3] -= (selectedItemValue - rightW);
                            amount = (selectedItemValue - rightW) * rightPlayerMarketEffectForRaw;
                            rightW = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[3] += rightW - selectedItemValue;
                            amount = (rightW * rightPlayerMarketEffectForRaw - selectedItemValue * rightPlayerMarketEffectForRaw);
                            total -= amount;
                            rightW = selectedItemValue;
                            coinForRightPlayer -= amount;
                        }
                        break;
                    case 4:
                        if (rightG <= selectedItemValue)
                        {
                            resourcesToBuy[4] -= (selectedItemValue - rightG);
                            amount = (selectedItemValue - rightG) * rightPlayerMarketEffectForManufacture;
                            rightG = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[4] += rightG - selectedItemValue;
                            amount = (rightG * rightPlayerMarketEffectForManufacture - selectedItemValue * rightPlayerMarketEffectForManufacture);
                            total -= amount;
                            rightG = selectedItemValue;
                            coinForRightPlayer -= amount;
                        }
                        break;
                    case 5:
                        if (rightP <= selectedItemValue)
                        {
                            resourcesToBuy[5] -= (selectedItemValue - rightP);
                            amount = (selectedItemValue - rightP) * rightPlayerMarketEffectForManufacture;
                            rightP = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[5] += rightP - selectedItemValue;
                            amount = (rightP * rightPlayerMarketEffectForManufacture - selectedItemValue * rightPlayerMarketEffectForManufacture);
                            total -= amount;
                            rightP = selectedItemValue;
                            coinForRightPlayer -= amount;
                        }
                        break;
                    case 6:

                        if (rightL <= selectedItemValue)
                        {
                            resourcesToBuy[6] -= (selectedItemValue - rightL);
                            amount = (selectedItemValue * rightPlayerMarketEffectForManufacture - rightL * rightPlayerMarketEffectForManufacture);
                            rightL = selectedItemValue;
                            coinForRightPlayer += amount;
                            total += amount;
                        }
                        else
                        {
                            resourcesToBuy[6] += rightL - selectedItemValue;
                            amount = (rightL * rightPlayerMarketEffectForManufacture - selectedItemValue * rightPlayerMarketEffectForManufacture);
                            total -= amount;
                            rightL = selectedItemValue;
                            coinForRightPlayer -= amount;

                        }
                        break;

                }


            }

            //Show the Total with the Current Change
            totalLabel.Content = total;


        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CANCELED)
            {
                coordinator.endOfCommerce("Cancel", false);
            }

        }


    }
}
