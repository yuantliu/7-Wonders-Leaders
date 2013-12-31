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
    /// Computation is mostly local. 
    /// </summary>
    public partial class NewCommerce : Window
    {
        const int ICON_WIDTH = 25;
        const int DAG_BUTTON_WIDTH = ICON_WIDTH;

        //player's coin that will be resetted to everytime the reset button is pressed
        //unfortunately, cant make this value constant.
        int PLAYER_COIN;

        //player's coordinator
        Coordinator c;

        //immutable original string cost
        string cardCost;

        //immutable core card/player information
        bool hasDiscount;
        bool leftRawMarket, leftManuMarket, rightRawMarket, rightManuMarket;
        string leftName, middleName, rightName;
        int ID;
        bool isStage;

        //current accumulated resources
        string currentResource = "";
        //how much coin to pay to left and right
        int leftcoin = 0, rightcoin = 0;
        //how many resources are still needed. 0 means no more resources are needed
        int resourcesNeeded;

        //DAGs
        DAG leftDag, middleDag, rightDag;

        //DAG buttons. [level][number]
        //e.g. For a DAG that has only 1 level, consisting of WBO, to get O, use [0][2]
        Button[,] leftDagButton, middleDagButton, rightDagButton;

        //Icon Bitmap images
        //Order: BOTW-GLP
        BitmapImage[] resourceIcons = new BitmapImage[7];

        /// <summary>
        /// Set the coordinator and handle CommerceInformation, which contains all necessary UI data, from GameManager
        /// </summary>
        public NewCommerce(Coordinator c, string data)
        {
            //intialise all the UI components in the xaml file (labels, etc.) to avoid null pointer
            InitializeComponent();

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

            this.PLAYER_COIN = commerceData.playerCoins;

            this.ID = commerceData.id;
            this.isStage = commerceData.isStage;

            leftDag = commerceData.playerCommerceInfo[0].dag;
            middleDag = commerceData.playerCommerceInfo[1].dag;
            rightDag = commerceData.playerCommerceInfo[2].dag;
            //all necessary UI information gathered

            //construct the UI

            //initialise images
            string currentPath = Environment.CurrentDirectory;
            for (int i = 0; i < 7; i++)
            {
                resourceIcons[i] = new BitmapImage();
                resourceIcons[i].BeginInit();
                string filename = "";

                switch (i)
                {
                    case 0:
                        filename = "brick";
                        break;
                    case 1:
                        filename = "ore";
                        break;
                    case 2:
                        filename = "stone";
                        break;
                    case 3:
                        filename = "wood";
                        break;
                    case 4:
                        filename = "glass";
                        break;
                    case 5:
                        filename = "loom";
                        break;
                    case 6:
                        filename = "papyrus";
                        break;
                }

                resourceIcons[i].UriSource = new Uri(currentPath + "\\Images\\" + filename + ".png");
                resourceIcons[i].EndInit();
            }

            //set the name labels
            leftNameLabel.Content = leftName;
            middleNameLabel.Content = middleName;
            rightNameLabel.Content = rightName;

            //set the player's total coins
            playerCoinsLabel.Content = PLAYER_COIN;

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

            //generate mutable elements (DAG buttons, Price representations, currentResources, etc.)
            reset();
        }

        /// <summary>
        /// Use the 3 DAGs in the object to generate the necessary Buttons in the UI and add EventHandlers for these newly added Buttons
        /// </summary>
        private void generateDAGs()
        {
            //reset all DAG panels
            leftDagPanel.Children.Clear();

            //generate left DAG
            //generate the needed amount of stackPanels, each representing a level
            StackPanel[] leftLevelPanels = new StackPanel[leftDag.getGraph().Count];
            //generate the needed amount of buttons
            leftDagButton = new Button[leftDag.getGraph().Count, 7];

            //extract the graph (List of char arrays) from the DAG and store locally to reduce function calls
            List<char[]> leftDagGraph = leftDag.getGraph();

            //look at each level of the DAG
            for (int i = 0; i < leftDagGraph.Count; i++)
            {
                //initialise a StackPanels for the current level
                leftLevelPanels[i] = new StackPanel();
                leftLevelPanels[i].Orientation = Orientation.Horizontal;

                //add to the StackPanels the appropriate buttons
                for (int j = 0; j < leftDagGraph[i].Length; j++)
                {
                    leftDagButton[i, j] = new Button();
                    leftDagButton[i, j].Content = leftDagGraph[i][j] + "";
                    leftDagButton[i, j].FontSize = 1;

                    //set the Button's image to correspond with the resource
                    switch (leftDagGraph[i][j])
                    {
                        case 'B':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[0]);
                            break;
                        case 'O':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[1]);
                            break;
                        case 'T':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[2]);
                            break;
                        case 'W':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[3]);
                            break;
                        case 'G':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[4]);
                            break;
                        case 'L':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[5]);
                            break;
                        case 'P':
                            leftDagButton[i, j].Background = new ImageBrush(resourceIcons[6]);
                            break;
                    }

                    leftDagButton[i, j].Width = DAG_BUTTON_WIDTH;
                    leftDagButton[i, j].Height = DAG_BUTTON_WIDTH;

                    //set the name of the Button for eventHandler purposes
                    //Format: L_(level number)
                    leftDagButton[i, j].Name = "L_" + i;

                    leftDagButton[i, j].IsEnabled = true;

                    //set action listener and add the button to the appropriate panel
                    leftDagButton[i, j].Click += dagResourceButtonPressed;
                    leftLevelPanels[i].Children.Add(leftDagButton[i, j]);

                    //leftLevelPanels[i] has leftDagButton[i,j] added
                } //leftLevelPanels[i] has added all the buttons appropriate for that level and its event handlers

                //add leftLevelPanels[i]
                leftDagPanel.Children.Add(leftLevelPanels[i]);
            }

            ////////////////////////////////////////////////////////////////////////////////

            //Same for middle DAG

            //reset all DAG panels
            middleDagPanel.Children.Clear();

            //generate middle DAG
            //generate the needed amount of stackPanels, each representing a level
            StackPanel[] middleLevelPanels = new StackPanel[middleDag.getGraph().Count];
            //generate the needed amount of buttons
            middleDagButton = new Button[middleDag.getGraph().Count, 7];

            //extract the graph (List of char arrays) from the DAG and store locally to reduce function calls
            List<char[]> middleDagGraph = middleDag.getGraph();

            //look at each level of the DAG
            for (int i = 0; i < middleDag.getGraph().Count; i++)
            {
                //initialise a StackPanels for the current level
                middleLevelPanels[i] = new StackPanel();
                middleLevelPanels[i].Orientation = Orientation.Horizontal;

                //add to the StackPanels the appropriate buttons
                for (int j = 0; j < middleDagGraph[i].Length; j++)
                {
                    middleDagButton[i, j] = new Button();
                    middleDagButton[i, j].Content = middleDagGraph[i][j] + "";
                    middleDagButton[i, j].FontSize = 1;

                    //set the Button's image to correspond with the resource
                    switch (middleDagGraph[i][j])
                    {
                        case 'B':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[0]);
                            break;
                        case 'O':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[1]);
                            break;
                        case 'T':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[2]);
                            break;
                        case 'W':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[3]);
                            break;
                        case 'G':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[4]);
                            break;
                        case 'L':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[5]);
                            break;
                        case 'P':
                            middleDagButton[i, j].Background = new ImageBrush(resourceIcons[6]);
                            break;
                    }

                    middleDagButton[i, j].Width = DAG_BUTTON_WIDTH;
                    middleDagButton[i, j].Height = DAG_BUTTON_WIDTH;

                    //set the name of the Button for eventHandler purposes
                    //Format: M_(level)
                    middleDagButton[i, j].Name = "M_" + i;

                    middleDagButton[i, j].IsEnabled = true;

                    //set action listener and add the button to the appropriate panel
                    middleDagButton[i, j].Click += dagResourceButtonPressed;
                    middleLevelPanels[i].Children.Add(middleDagButton[i, j]);

                    //middleLevelPanels[i] has middleDagButton[i,j] added
                } //middleLevelPanels[i] has added all the buttons appropriate for that level and its event handlers

                //add middleLevelPanels[i]
                middleDagPanel.Children.Add(middleLevelPanels[i]);
            }

            /////////////////////////////////////////////////////////////////////////

            //same for right DAG

            //reset all DAG panels
            rightDagPanel.Children.Clear();

            //generate right DAG
            //generate the needed amount of stackPanels, each representing a level
            StackPanel[] rightLevelPanels = new StackPanel[rightDag.getGraph().Count];
            //generate the needed amount of buttons
            rightDagButton = new Button[rightDag.getGraph().Count, 7];

            //extract the graph (List of char arrays) from the DAG and store locally to reduce function calls
            List<char[]> rightDagGraph = rightDag.getGraph();

            //look at each level of the DAG
            for (int i = 0; i < rightDag.getGraph().Count; i++)
            {
                //initialise a StackPanels for the current level
                rightLevelPanels[i] = new StackPanel();
                rightLevelPanels[i].Orientation = Orientation.Horizontal;

                //add to the StackPanels the appropriate buttons
                for (int j = 0; j < rightDagGraph[i].Length; j++)
                {
                    rightDagButton[i, j] = new Button();
                    rightDagButton[i, j].Content = rightDagGraph[i][j] + "";
                    rightDagButton[i, j].FontSize = 1;

                    //set the Button's image to correspond with the resource
                    switch (rightDagGraph[i][j])
                    {
                        case 'B':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[0]);
                            break;
                        case 'O':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[1]);
                            break;
                        case 'T':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[2]);
                            break;
                        case 'W':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[3]);
                            break;
                        case 'G':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[4]);
                            break;
                        case 'L':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[5]);
                            break;
                        case 'P':
                            rightDagButton[i, j].Background = new ImageBrush(resourceIcons[6]);
                            break;
                    }

                    rightDagButton[i, j].Width = DAG_BUTTON_WIDTH;
                    rightDagButton[i, j].Height = DAG_BUTTON_WIDTH;

                    //set the name of the Button for eventHandler purposes
                    //Format: R_(level)
                    rightDagButton[i, j].Name = "R_" + i;

                    rightDagButton[i, j].IsEnabled = true;

                    //set action listener and add the button to the appropriate panel
                    rightDagButton[i, j].Click += dagResourceButtonPressed;
                    rightLevelPanels[i].Children.Add(rightDagButton[i, j]);

                    //rightLevelPanels[i] has rightDagButton[i,j] added
                } //rightLevelPanels[i] has added all the buttons appropriate for that level and its event handlers

                //add rightLevelPanels[i]
                rightDagPanel.Children.Add(rightLevelPanels[i]);
            }
        }

        /// <summary>
        /// Event handler for the DAG buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dagResourceButtonPressed(object sender, RoutedEventArgs e)
        {
            //determine which button was pressed
            Button pressed = sender as Button;
            string s = pressed.Name;

            //determine some information about the pressed button
            
            //level of the resource
            int level = Convert.ToInt32(s.Substring(2));
            //resource obtained
            char resource = ((string)pressed.Content)[0];
            //the location of the button (whether left, right, or middle)
            char location = s[0];

            //remember the current resource obtained amount for comparison with new resource obtained amount later
            int previous = resourcesNeeded;

            //add to the currentResources
            string newResource = currentResource + resource;

            //check if the newResource gets us closer to paying the cost.
            //If the newResource has the same distance as previous, then we have not gotten closer, and therefore we have just added an unnecessar resource
            //pop out an error to show this.

            if ((resourcesNeeded == 1 && hasDiscount == true) || (resourcesNeeded == 0 && hasDiscount == false))
            {
                MessageBox.Show("You have for all necessary resources already");
                return;
            }
            else if (DAG.eliminate(cardCost, newResource).Length == previous)
            {
                MessageBox.Show("This resource will not help you pay for your cost");
                return;
            }

            //add the appropriate amount of coins to the appropriate recepient
            //as well as doing appropriate checks
            if (location == 'L')
            {
                if (leftRawMarket == true && (resource == 'B' || resource == 'O' || resource == 'T' || resource == 'W'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 0)
                    {
                        leftcoin++;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
                else if (leftRawMarket == false && (resource == 'B' || resource == 'O' || resource == 'T' || resource == 'W'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 1)
                    {
                        leftcoin += 2;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
                else if (leftManuMarket == true && (resource == 'G' || resource == 'L' || resource == 'P'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 0)
                    {
                        leftcoin++;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
                else if (leftManuMarket == false && (resource == 'G' || resource == 'L' || resource == 'P'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 1)
                    {
                        leftcoin += 2;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
            }
            else if (location == 'R')
            {
                if (rightRawMarket == true && (resource == 'B' || resource == 'O' || resource == 'T' || resource == 'W'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 0)
                    {
                        rightcoin++;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
                else if (rightRawMarket == false && (resource == 'B' || resource == 'O' || resource == 'T' || resource == 'W'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 1)
                    {
                        rightcoin += 2;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
                else if (rightManuMarket == true && (resource == 'G' || resource == 'L' || resource == 'P'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 0)
                    {
                        rightcoin++;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
                else if (rightManuMarket == false && (resource == 'G' || resource == 'L' || resource == 'P'))
                {
                    if ((PLAYER_COIN - (leftcoin + rightcoin)) > 1)
                    {
                        rightcoin += 2;
                    }
                    else
                    {
                        MessageBox.Show("You cannot afford this resource");
                        return;
                    }
                }
            }

            //We have gotten closer to paying for the resource
            //set the new values
            resourcesNeeded--;
            currentResource = newResource;

            //disable (make hidden) all buttons on the same level
            if (location == 'L')
            {
                for (int i = 0; i < leftDag.getGraph()[level].Length; i++)
                {
                    //hide the buttons
                    leftDagButton[level, i].Visibility = Visibility.Hidden;
                }
            }
            else if (location == 'M')
            {
                for (int i = 0; i < middleDag.getGraph()[level].Length; i++)
                {
                    //hide the buttons
                    middleDagButton[level, i].Visibility = Visibility.Hidden;
                }
            }
            else if (location == 'R')
            {
                for (int i = 0; i < rightDag.getGraph()[level].Length; i++)
                {
                    //hide the buttons
                    rightDagButton[level, i].Visibility = Visibility.Hidden;
                }
            }

            //refresh the cost panel
            generateCostPanel();
        }

        /// <summary>
        /// Construct the labels at the cost panel, given the overall cost minus the current paid cost
        /// </summary>
        private void generateCostPanel()
        {
            generateCostPanelAndUpdateSubtotal(DAG.eliminate(cardCost, currentResource));
        }

        /// <summary>
        /// Construct the labels at the cost panel, given a cost
        /// </summary>
        /// <param name="cost"></param>
        private void generateCostPanelAndUpdateSubtotal(string cost)
        {
            costPanel.Children.Clear();
            Label[] costLabels = new Label[cost.Length];

            //fill the labels with the appropriate image
            for (int i = 0; i < cost.Length; i++)
            {
                BitmapImage iconImage = null;

                switch (cost[i])
                {
                    case 'B':
                        iconImage = resourceIcons[0];
                        break;
                    case 'O':
                        iconImage = resourceIcons[1];
                        break;
                    case 'T':
                        iconImage = resourceIcons[2];
                        break;
                    case 'W':
                        iconImage = resourceIcons[3];
                        break;
                    case 'G':
                        iconImage = resourceIcons[4];
                        break;
                    case 'L':
                        iconImage = resourceIcons[5];
                        break;
                    case 'P':
                        iconImage = resourceIcons[6];
                        break;
                }

                costLabels[i] = new Label();

                costLabels[i].Background = new ImageBrush(iconImage);
                costLabels[i].Width = ICON_WIDTH;
                costLabels[i].Height = ICON_WIDTH;

                //add the labels to costPanel
                costPanel.Children.Add(costLabels[i]);
            }

            //update the subtotals
            leftSubtotalLabel.Content = leftcoin;
            rightSubtotalLabel.Content = rightcoin;
            subTotalLabel.Content = leftcoin + rightcoin;
        }

        /// <summary>
        /// Reset all information back to the beginning state (before user has taken any action)
        /// Called by constructor and resetButton
        /// </summary>
        private void reset()
        {
            currentResource = "";
            leftcoin = 0;
            rightcoin = 0;

            generateCostPanel();
            generateDAGs();
            resourcesNeeded = cardCost.Length;
        }

        /// <summary>
        /// Submit button event handler. Client sends to server the relevant data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (resourcesNeeded != 0)
            {
                MessageBox.Show("You must pay for unpaid resources");
            }
            else
            {
                //construct the information
                CommerceClientToServerResponse response = new CommerceClientToServerResponse();
                response.leftCoins = leftcoin;
                response.rightCoins = rightcoin;
                response.id = ID;

                string serializedResponse = Marshaller.ObjectToString(response);

                //send the appropriate information
                if (isStage == true)
                {
                    //stage
                    c.sendToHost("CS" + serializedResponse);
                }
                else
                {
                    //build struct
                    c.sendToHost("CB" + serializedResponse);
                }

                //end turn
                c.endTurn();

                //signify to MainWindow that turn has been played
                c.gameUI.playerPlayedHisTurn = true;

                Close();
            }
        }

        /// <summary>
        /// Event handler for the Reset button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        /// <summary>
        /// Event handler for the Close button
        /// Just close the window without any further actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
