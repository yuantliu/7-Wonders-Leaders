using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Web;

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
        Coordinator coordinator;

        //immutable original string cost
        // string cardCost;
        Cost cardCost;

        //immutable core card/player information
        bool hasDiscount;
        bool leftRawMarket, leftManuMarket, rightRawMarket, rightManuMarket;
        string leftName, middleName, rightName;
        string structureName;
        // int ID;
        bool isStage;

        //current accumulated resources
        string strCurrentResourcesUsed = "";
        //how much coin to pay to left and right
        int leftcoin = 0, rightcoin = 0;
        //how many resources are still needed. 0 means no more resources are needed
        int resourcesNeeded;

        //DAGs
        DAG leftDag = new DAG(), middleDag = new DAG(), rightDag = new DAG();

        //DAG buttons. [level][number]
        //e.g. For a DAG that has only 1 level, consisting of WBO, to get O, use [0][2]
        Button[,] leftDagButton, middleDagButton, rightDagButton;

        //Icon Bitmap images
        //Order: BOTW-GLP
        BitmapImage[] resourceIcons = new BitmapImage[7];

        void CreateDag(DAG d, string sourceStr)
        {
            string[] playerEffectsSplit = sourceStr.Split(',');

            for (int i = 0; i < playerEffectsSplit.Length; ++i)
            {
                switch (playerEffectsSplit[i].Length)
                {
                    case 1:
                        {
                            SimpleEffect e = new SimpleEffect(1, playerEffectsSplit[i][0]);
                            d.add(e);
                        }
                        break;

                    case 2:
                        // if they are the same resource, it's two of the same (simple)
                        if (playerEffectsSplit[i][0] == playerEffectsSplit[i][1])
                        {
                            SimpleEffect e = new SimpleEffect(2, playerEffectsSplit[i][0]);
                            d.add(e);
                        }
                        else
                        {
                            // choice of two
                            ResourceChoiceEffect e = new ResourceChoiceEffect(true, playerEffectsSplit[i]);
                            d.add(e);
                        }
                        break;

                    default:

                        {
                            // The server will filter cards in the neighbor's city we cannnot use.  Therefore it's safe to assume
                            // that any resource choice card that has more than 2 resources is available as it must belong to the player
                            ResourceChoiceEffect e = new ResourceChoiceEffect(true, playerEffectsSplit[i]);
                            d.add(e);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Set the coordinator and handle CommerceInformation, which contains all necessary UI data, from GameManager
        /// </summary>
        public NewCommerce(Coordinator coordinator, List<Card> cardList, string cardName, int wonderStage, NameValueCollection qscoll )
        {
            //intialise all the UI components in the xaml file (labels, etc.) to avoid null pointer
            InitializeComponent();

            this.coordinator = coordinator;

            // Leader discount for the type of card being constructed
            hasDiscount = false;

            leftName = "Left Neighbor";
            middleName = "Player";
            rightName = "Right Neighbor";

            structureName = cardName;

            isStage = wonderStage != 0;

            if (isStage)
            {
                string[] wonderData = qscoll["wonderInfo"].Split('/');

                cardCost = cardList.Find(x => x.name == wonderData[1] && x.wonderStage == (int.Parse(wonderData[0]) + 1)).cost;
            }
            else
            {
                cardCost = cardList.Find(x => x.name == cardName).cost;
            }

            leftRawMarket = false;
            rightRawMarket = false;

            string commercialEffectCardList = qscoll["discountEffects"];

            if (commercialEffectCardList != null)
            {
                string[] commercialCardList = commercialEffectCardList.Split(',');

                for (int i = 0; i < commercialCardList.Length; ++i)
                {
                    CommercialDiscountEffect cde = cardList.Find(x => x.name == commercialCardList[i]).effect as CommercialDiscountEffect;

                    if (cde.appliesTo == CommercialDiscountEffect.AppliesTo.LeftNeighbor || cde.appliesTo == CommercialDiscountEffect.AppliesTo.BothNeighbors)
                    {
                        leftRawMarket = cde.affects == CommercialDiscountEffect.Affects.RawMaterial;
                        leftManuMarket = cde.affects == CommercialDiscountEffect.Affects.Goods;
                    }

                    if (cde.appliesTo == CommercialDiscountEffect.AppliesTo.RightNeighbor || cde.appliesTo == CommercialDiscountEffect.AppliesTo.BothNeighbors)
                    {
                        rightRawMarket = cde.affects == CommercialDiscountEffect.Affects.RawMaterial;
                        rightManuMarket = cde.affects == CommercialDiscountEffect.Affects.Goods;
                    }
                }
            }

            PLAYER_COIN = int.Parse(qscoll["coin"]);

            CreateDag(middleDag, qscoll["PlayerResources"]);
            CreateDag(leftDag, qscoll["LeftResources"]);
            CreateDag(rightDag, qscoll["RightResources"]);

            /*
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

            // this.ID = commerceData.id;
            this.structureName = commerceData.structureName;
            this.isStage = commerceData.isStage;

            leftDag = commerceData.playerCommerceInfo[0].dag;
            middleDag = commerceData.playerCommerceInfo[1].dag;
            rightDag = commerceData.playerCommerceInfo[2].dag;
            */
            //all necessary UI information gathered

            //construct the UI

            //initialise images
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

                resourceIcons[i].UriSource = new Uri("pack://application:,,,/7W;component/Resources/Images/" + filename + ".png");
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
                leftRawImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/1r.png"));
            else
                leftRawImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/2r.png"));
            if(leftManuMarket == true)
                leftManuImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/1m.png"));
            else
                leftManuImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/2m.png"));
            if (rightRawMarket == true)
                rightRawImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/1r.png"));
            else
                rightRawImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/2r.png"));
            if (rightManuMarket == true)
                rightManuImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/1m.png"));
            else
                rightManuImage.Source = new BitmapImage(new Uri("pack://application:,,,/7W;component/Resources/Images/Commerce/2m.png"));

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

        BitmapImage GetButtonIcon(char resource)
        {
            int resourceIconIndex = -1;

            switch(resource)
            {
                case 'B': resourceIconIndex = 0; break;
                case 'O': resourceIconIndex = 1; break;
                case 'S': resourceIconIndex = 2; break;
                case 'W': resourceIconIndex = 3; break;
                case 'G': resourceIconIndex = 4; break;
                case 'C': resourceIconIndex = 5; break;
                case 'P': resourceIconIndex = 6; break;
            }

            return resourceIcons[resourceIconIndex];
        }

        /// <summary>
        /// Use the 3 DAGs in the object to generate the necessary Buttons in the UI and add EventHandlers for these newly added Buttons
        /// </summary>
        private void generateOneDAG(StackPanel p, out Button[,] b, DAG dag, string buttonNamePrefix, bool isDagOwnedByPlayer)
        {
            //reset all DAG panels
            p.Children.Clear();

            //generate a DAG for self or a neighbor
            //generate the needed amount of stackPanels, each representing a level
            StackPanel[] levelPanels = new StackPanel[dag.getSimpleStructures().Count + dag.getChoiceStructures(isDagOwnedByPlayer).Count];
            //generate the needed amount of buttons
            b = new Button[dag.getSimpleStructures().Count + dag.getChoiceStructures(isDagOwnedByPlayer).Count, 7];

            List<SimpleEffect> dagGraphSimple = dag.getSimpleStructures();
            int i = 0;

            for ( ; i < dagGraphSimple.Count; ++i)
            {
                levelPanels[i] = new StackPanel();
                levelPanels[i].Orientation = Orientation.Horizontal;
                levelPanels[i].HorizontalAlignment = HorizontalAlignment.Center;

                for (int j = 0; j < dagGraphSimple[i].multiplier; j++)
                {
                    b[i, j] = new Button();
                    b[i, j].Content = dagGraphSimple[i];

                    b[i, j].FontSize = 1;
                    b[i, j].Background = new ImageBrush(GetButtonIcon(dagGraphSimple[i].type));
                    b[i, j].Width = DAG_BUTTON_WIDTH;
                    b[i, j].Height = DAG_BUTTON_WIDTH;

                    //set the name of the Button for eventHandler purposes
                    //Format: L_(level number)
                    b[i, j].Name = buttonNamePrefix + i;

                    b[i, j].IsEnabled = true;

                    //set action listener and add the button to the appropriate panel
                    b[i, j].Click += dagResourceButtonPressed;
                    levelPanels[i].Children.Add(b[i, j]);
                }
                p.Children.Add(levelPanels[i]);
            }

            //extract the graph (List of char arrays) from the DAG and store locally to reduce function calls
            List<ResourceChoiceEffect> dagGraphChoice = dag.getChoiceStructures(isDagOwnedByPlayer);

            //look at each level of the DAG
            for ( ; i < dagGraphSimple.Count + dagGraphChoice.Count; i++)
            {
                //initialise a StackPanels for the current level
                levelPanels[i] = new StackPanel();
                levelPanels[i].Orientation = Orientation.Horizontal;
                levelPanels[i].HorizontalAlignment = HorizontalAlignment.Center;

                //add to the StackPanels the appropriate buttons
                for (int j = 0; j < dagGraphChoice[i - dagGraphSimple.Count].strChoiceData.Length; j++)
                {
                    b[i, j] = new Button();
                    b[i, j].Content = dagGraphChoice[i - dagGraphSimple.Count];
                    b[i, j].FontSize = 1;

                    //set the Button's image to correspond with the resource
                    b[i, j].Background = new ImageBrush(GetButtonIcon(dagGraphChoice[i - dagGraphSimple.Count].strChoiceData[j]));

                    b[i, j].Width = DAG_BUTTON_WIDTH;
                    b[i, j].Height = DAG_BUTTON_WIDTH;

                    //set the name of the Button for eventHandler purposes
                    //Format: L_(level number)
                    b[i, j].Name = buttonNamePrefix + i + j;

                    b[i, j].IsEnabled = true;

                    //set action listener and add the button to the appropriate panel
                    b[i, j].Click += dagResourceButtonPressed;
                    levelPanels[i].Children.Add(b[i, j]);

                    // levelPanels[i] has b[i,j] added
                } // levelPanels[i] has added all the buttons appropriate for that level and its event handlers

                //add the stack to the parent panel.
                p.Children.Add(levelPanels[i]);
            }
        }

        private void generateDAGs()
        {
            generateOneDAG(leftDagPanel, out leftDagButton, leftDag, "L_", false);
            generateOneDAG(middleDagPanel, out middleDagButton, middleDag, "M_", true);
            generateOneDAG(rightDagPanel, out rightDagButton, rightDag, "R_", false);
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
            //the location of the button (whether left, right, or middle)
            char location = s[0];

            //resource obtained
            Effect effect = pressed.Content as Effect;

            char resource;
            int mulitplier = 1;

            if (effect is SimpleEffect)
            {
                SimpleEffect se = effect as SimpleEffect;

                resource = se.type;
                mulitplier = se.multiplier;
            }
            else
            {
                ResourceChoiceEffect rce = effect as ResourceChoiceEffect;

                int resourceStringIndex = Convert.ToInt32(s.Substring(3));

                resource = rce.strChoiceData[resourceStringIndex];
            }

            //remember the current resource obtained amount for comparison with new resource obtained amount later
            int previous = resourcesNeeded;

            //add to the currentResources
            string strPossibleNewResourceList = strCurrentResourcesUsed + resource;

            //check if the newResource gets us closer to paying the cost.
            //If the newResource has the same distance as previous, then we have not gotten closer, and therefore we have just added an unnecessar resource
            //pop out an error to show this.

            if ((resourcesNeeded == 1 && hasDiscount == true) || (resourcesNeeded == 0 && hasDiscount == false))
            {
                MessageBox.Show("You have for all necessary resources already");
                return;
            }
            else if (DAG.eliminate(cardCost.Copy(), false, mulitplier, strPossibleNewResourceList).Total() == previous)
            {
                MessageBox.Show("This resource will not help you pay for your cost");
                return;
            }

            bool isResourceRawMaterial = (resource == 'B' || resource == 'O' || resource == 'S' || resource == 'W');
            bool isResourceGoods = (resource == 'G' || resource == 'C' || resource == 'P');

            //add the appropriate amount of coins to the appropriate recepient
            //as well as doing appropriate checks
            if (location == 'L')
            {
                int coinsRequired = (isResourceRawMaterial && leftRawMarket) || (isResourceGoods && leftManuMarket) ? 1 : 2;

                if ((PLAYER_COIN - (leftcoin + rightcoin)) < coinsRequired)
                {
                    MessageBox.Show("You cannot afford this resource");
                    return;
                }

                leftcoin += coinsRequired;
            }
            else if (location == 'R')
            {
                int coinsRequired = (isResourceRawMaterial && rightRawMarket) || (isResourceGoods && rightManuMarket) ? 1 : 2;

                if ((PLAYER_COIN - (leftcoin + rightcoin)) < coinsRequired)
                {
                    MessageBox.Show("You cannot afford this resource");
                    return;
                }

                rightcoin += coinsRequired;
            }

            // The resource chosen is good: it is required and affordable.
            resourcesNeeded--;
            strCurrentResourcesUsed = strPossibleNewResourceList;

            //disable (make hidden) all buttons on the same level
            if (location == 'L')
            {
                // hmm, simple structures need to consider the multiplier.
                int c = leftDag.getSimpleStructures().Count;

                if (level < c)
                {
                    //hide the buttons
                    leftDagButton[level, 0].Visibility = Visibility.Hidden;
                }
                else
                {
                    for (int i = 0; i < leftDag.getChoiceStructures(false)[level - c].strChoiceData.Length; i++)
                    {
                        //hide the buttons
                        leftDagButton[level, i].Visibility = Visibility.Hidden;
                    }
                }
            }
            else if (location == 'M')
            {
                int c = middleDag.getSimpleStructures().Count;

                if (level < c)
                {
                    middleDagButton[level, 0].Visibility = Visibility.Hidden;
                }
                else
                {
                    for (int i = 0; i < middleDag.getChoiceStructures(true)[level - c].strChoiceData.Length; i++)
                    {
                        //hide the buttons
                        middleDagButton[level, i].Visibility = Visibility.Hidden;
                    }
                }
            }
            else if (location == 'R')
            {
                int c = rightDag.getSimpleStructures().Count;

                if (level < c)
                {
                    rightDagButton[level, 0].Visibility = Visibility.Hidden;
                }
                else
                {
                    for (int i = 0; i < rightDag.getChoiceStructures(false)[level - c].strChoiceData.Length; i++)
                    {
                        //hide the buttons
                        rightDagButton[level, i].Visibility = Visibility.Hidden;
                    }
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
            // generateCostPanelAndUpdateSubtotal(DAG.eliminate(cardCost, currentResource));
            generateCostPanelAndUpdateSubtotal(DAG.eliminate(cardCost.Copy(), false, 1, strCurrentResourcesUsed));
        }

        /// <summary>
        /// Construct the labels at the cost panel, given a cost
        /// </summary>
        /// <param name="cost"></param>
        private void generateCostPanelAndUpdateSubtotal(Cost cost)
        {
            costPanel.Children.Clear();
            Label[] costLabels = new Label[resourcesNeeded];

            Cost cpyCost = cost.Copy();

            //fill the labels with the appropriate image
            for (int i = 0; i < resourcesNeeded; i++)
            {
                BitmapImage iconImage = null;

                if (cpyCost.wood != 0)
                {
                    iconImage = GetButtonIcon('W');
                    --cpyCost.wood;
                }
                else if (cpyCost.stone != 0)
                {
                    iconImage = GetButtonIcon('S');
                    --cpyCost.stone;
                }
                else if (cpyCost.clay != 0)
                {
                    iconImage = GetButtonIcon('B');
                    --cpyCost.clay;
                }
                else if (cpyCost.ore != 0)
                {
                    iconImage = GetButtonIcon('O');
                    --cpyCost.ore;
                }
                else if (cpyCost.cloth != 0)
                {
                    iconImage = GetButtonIcon('C');
                    --cpyCost.cloth;
                }
                else if (cpyCost.glass != 0)
                {
                    iconImage = GetButtonIcon('G');
                    --cpyCost.glass;
                }
                else if (cpyCost.papyrus != 0)
                {
                    iconImage = GetButtonIcon('P');
                    --cpyCost.papyrus;
                }
                else
                {
                    // something went wrong
                    throw new Exception();
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
            strCurrentResourcesUsed = string.Empty;
            leftcoin = 0;
            rightcoin = 0;

            if (cardCost.coin != 0)
            {
                // not sure whether this will work or not.
                throw new NotImplementedException();
            }

            resourcesNeeded = cardCost.Total();

            generateCostPanel();
            generateDAGs();
        }

        /// <summary>
        /// Submit button event handler. Client sends to server the relevant data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if ((resourcesNeeded == 0 && hasDiscount == false) || (resourcesNeeded == 1 && hasDiscount == true))
            {
                string strResponse = string.Format("BldStrct&WonderStage={0}&Structure={1}&leftCoins={2}&rightCoins={3}", isStage ? "1" : "0", structureName, leftcoin, rightcoin);
                coordinator.sendToHost(strResponse);

                //end turn
                coordinator.endTurn();

                //signify to MainWindow that turn has been played
                coordinator.gameUI.playerPlayedHisTurn = true;

                Close();
            }
            //does not fulfill requirements
            else
            {
                MessageBox.Show("You must pay for unpaid resources");
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
