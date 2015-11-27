using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SevenWonders
{
    public class Player
    {
        public bool isAI {get; set;}

        public String nickname { get; set; }

        public String GetNickName() { return nickname; }

        public Board playerBoard { get; set; }

        //current Stage of wonder
        public int currentStageOfWonder { get; set; }

        //resources
        public int brick { get; set; }

        public int ore { get; set; }

        public int stone { get; set; }

        public int wood { get; set; }

        public int glass { get; set; }

        public int loom { get; set; }

        public int papyrus { get; set; }

        public int coin { get; set; }

        public int GetCoin() { return coin; }

        //science
        public int bearTrap { get; set; }

        public int tablet { get; set; }

        public int sextant { get; set; }

        //Points and stuff
        public int victoryPoint { get; set; }

        public int shield { get; set; }

        public int lossToken { get; set; }

        public int conflictTokenOne { get; set; }

        public int conflictTokenTwo { get; set; }

        public int conflictTokenThree { get; set; }

        public int GetNumCardsInHand() { return hand.Count; }

        public List<Card> hand = new List<Card>(7);

        public List<Card> playedStructure = new List<Card>();

        //can activate wonder power?
        public bool hasOlympia { get; set; }
        public bool olympiaPowerEnabled { get; set; }
        
        public bool usedHalicarnassus { get; set; }

        public bool usedBabylon { get; set; }

        //bilkis (0 is nothing, 1 is ore, 2 is stone, 3 is glass, 4 is papyrus, 5 is loom, 6 is wood, 7 is brick
        public byte bilkis;
        public bool hasBilkis;

        //stored actions for the turn
        private List<Effect> actions = new List<Effect>();

        //stored actions for the end of the game
        private List<Effect> endOfGameActions = new List<Effect>();

        //Player's left and right neighbours
        public Player leftNeighbour { get; set; }

        public Player GetLeftNeighbour() { return leftNeighbour; }

        public Player rightNeighbour { get; set; }

        public Player GetRightNeighbour() { return rightNeighbour; }

        public Boolean changeNickName {get; set; }
        public String newNickName {get; set; }

        //market effect
        public bool leftRaw = false, rightRaw = false, leftManu = false, rightManu = false;

        public bool GetLeftRaw() { return leftRaw; }

        public bool GetLeftManu() { return leftManu; }

        public bool GetRightRaw() { return rightRaw; }

        public bool GetRightManu() { return rightManu; }

        //Leaders pile. The pile that holds the unplayed leaders cards
        public List<Card> leadersPile = new List<Card>();

        public List<Card> GetLeadersPile() { return leadersPile; }

        //interface for vanilla AI
        public AIMoveBehaviour AIBehaviour;
        //interface for Leaders AI
        // public LeadersAIMoveBehaviour LeadersAIBehaviour;

        private GameManager gm;

        //The Multiple Resource DAG
        public DAG dag { get; set; }

        public DAG GetDAG() { return dag; }

        public bool bUIRequiresUpdating { get; set; }

        /// <summary>
        /// Constructor. Create a Player with a given nickname
        /// </summary>
        public Player(String nickname, bool isAI, GameManager gm)
        {
            dag = new DAG();

            this.nickname = nickname;

            this.isAI = isAI;

            currentStageOfWonder = 0;
            changeNickName = false;
            newNickName = "";

            //set used halicarnassus and babylon to true, to make sure its not available
            usedHalicarnassus = true;
            usedBabylon = true;

            //set bilkis to nothing
            bilkis = 0;
            hasBilkis = false;

            //set the Game Manager
            this.gm = gm;
        }

        /// <summary>
        /// Set the neighbouring Players
        /// Used by GameManager.beginningOfSessionActions()
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void setNeighbours(Player left, Player right)
        {
            leftNeighbour = left;
            rightNeighbour = right;
        }

        /// <summary>
        /// Stored actions to be executed at the end of each turn
        /// </summary>
        /// <param name="s"></param>
        public void storeAction(Effect s)
        {
            actions.Add(s);
        }

        /// <summary>
        /// Check if Salomon (aka Halicarnassus) is stored as an action.
        /// Return false if it is not
        /// Return true if it is, then remove it
        /// </summary>
        /// <returns></returns>
        public bool hasSalomon()
        {
            /*
            for (int i = 0; i < numOfActions; i++)
            {
                //found Salomon
                if (actions[i] == "SALOMON")
                {
                    //remove the item and return true
                    for (int j = i; j < (numOfActions - 1); j++)
                    {
                        actions[j] = actions[j + 1];
                    }

                    numOfActions--;

                    return true;
                }
            }
            */

            return false;
        }

        /// <summary>
        /// Check if Stevie is stored as an action
        /// Return true if it is then remove it
        /// Return false if it is not
        /// </summary>
        /// <returns></returns>
        public bool hasStevie()
        {
            /*
            for (int i = 0; i < numOfActions; i++)
            {
                //found Stevie
                if (actions[i] == "@Pay X coins for board, where X is board cost")
                {
                    //remove the item and return true
                    for (int j = i; j < (numOfActions - 1); j++)
                    {
                        actions[j] = actions[j + 1];
                    }

                    numOfActions--;

                    return true;
                }
            }
            */

            return false;
        }

        /// <summary>
        /// Check if Courtesan's guild is played and remove it if it is
        /// </summary>
        /// <returns></returns>
        public bool hasCourtesan()
        {
            /*
            for (int i = 0; i < numOfActions; i++)
            {
                //found Courtesan's guild
                if (actions[i] == "@(Guild of Courtesans effect)")
                {
                    //remove the item and return true
                    for (int j = i; j < (numOfActions - 1); j++)
                    {
                        actions[j] = actions[j + 1];
                    }

                    numOfActions--;

                    return true;
                }
            }
            */

            return false;
        }

        /// <summary>
        /// Stored actions to be executed at the end of the game
        /// </summary>
        /// <param name="s"></param>
        public void storeEndOfGameAction(Effect s)
        {
            endOfGameActions.Add(s);
        }

        //Execute actions
        //change the Player score information based on the actions
        public void executeAction(GameManager gm)
        {
            /*
            //Esteban and Bilkis can be implemented much easier if it has access to GameManager (LeadersGameManager to be exact)
            //Regular GameManager is not useful. Must have LeadersGameManager because nothing in the regular game requires reg GM
            if (gm is LeadersGameManager)
            {
                gm = (LeadersGameManager)gm;
            }
            */

            //go through each action and execute the actions stored
            foreach (Effect act in actions)
            {
                //category $: deduct a given amount of coins
                // if (actactions[i][0] == '$')
                if (act is CoinEffect)
                {
                    // add or subtract coins
                    coin += ((CoinEffect)act).coins;
                }
                else if (act is MilitaryEffect)
                {
                    shield += ((MilitaryEffect)act).nShields;
                }
                //category 1: give one kind of non-science thing
                // else if (actions[i][0] == '1')
                else if (act is ResourceEffect)
                {
                    ResourceEffect e = act as ResourceEffect;
                    //increase the appropriate field by num
                    // int num = int.Parse(act[0] + "");
                   //  int num = e.multiplier;

                    switch (e.resourceTypes[0])
                    {
                        case 'O':
                            ++ore;
                            if (e.resourceTypes.Length == 2 && e.resourceTypes[1] == 'O') ++ore;
                            dag.add(e);
                            break;
                        case 'B':
                            ++brick;
                            if (e.resourceTypes.Length == 2 && e.resourceTypes[1] == 'B') ++brick;
                            dag.add(e);
                            break;
                        case 'S':
                            ++stone;
                            if (e.resourceTypes.Length == 2 && e.resourceTypes[1] == 'S') ++stone;
                            dag.add(e);
                            break;
                        case 'W':
                            ++wood;
                            if (e.resourceTypes.Length == 2 && e.resourceTypes[1] == 'W') ++wood;
                            dag.add(e);
                            break;
                        case '$':
                            throw new Exception();
                            // money should be transacted via CoinEffect types.
                            // coin += e.multiplier;
                            break;
                        case 'C':
                            ++loom;
                            dag.add(e);
                            break;
                        case 'P':
                            ++papyrus;
                            dag.add(e);
                            break;
                        case 'G':
                            ++glass;
                            dag.add(e);
                            break;
                            /*
                        case 'd':
                        case 'D':
                            break;
                            */
                        default:
                            throw new Exception();
                    }
                }
                //category 2: add one science
                // else if (actions[i][0] == '2')
                else if (act is ScienceEffect)
                {
                    switch (((ScienceEffect)act).symbol)
                    {
                        case ScienceEffect.Symbol.Compass:
                            sextant++;
                            break;
                        case ScienceEffect.Symbol.Gear:
                            bearTrap++;
                            break;
                        case ScienceEffect.Symbol.Tablet:
                            tablet++;
                            break;
                        default:
                            throw new Exception();
                    }
                }
                //category 3: market effect
                // else if (actions[i][0] == '3')
                else if (act is CommercialDiscountEffect)
                {
                    //set the market effects
                    CommercialDiscountEffect e = act as CommercialDiscountEffect;
                    if (e.affects == CommercialDiscountEffect.Affects.RawMaterial)
                    {
                        switch(e.appliesTo)
                        {
                            case CommercialDiscountEffect.AppliesTo.LeftNeighbor:
                                leftRaw = true;
                                break;

                            case CommercialDiscountEffect.AppliesTo.RightNeighbor:
                                rightRaw = true;
                                break;

                            case CommercialDiscountEffect.AppliesTo.BothNeighbors:
                                leftRaw = true; rightRaw = true;
                                break;
                        }
                    }
                    else if (e.affects == CommercialDiscountEffect.Affects.Goods)
                    {
                        switch (e.appliesTo)
                        {
                            case CommercialDiscountEffect.AppliesTo.LeftNeighbor:
                                leftManu = true;
                                break;

                            case CommercialDiscountEffect.AppliesTo.RightNeighbor:
                                rightManu = true;
                                break;

                            case CommercialDiscountEffect.AppliesTo.BothNeighbors:
                                leftManu = true; rightManu = true;
                                break;
                        }
                    }
                }
                //category 4: gives a choice between different things
                //Add to the DAG
                // else if (actions[i][0] == '4')
                else if (act is ResourceEffect)
                {
                    // dag.add(actions[i].Substring(1));
                    // TODO: there's a bug here: RawMaterial structures can be purchased by neighboring cities
                    // but Commercial structures (Forum & Caravansery) cannot.  DAG must account for this difference
                    dag.add(act);
                }
                //category 5: gives some $ and and/or some victory depending on some conditions
                //these cards are usually yellow
                // else if (actions[i][0] == '5')
                else if (act is CoinsAndPointsEffect)
                {
                    CoinsAndPointsEffect e = act as CoinsAndPointsEffect;

                    if (e.coinsGrantedAtTimeOfPlayMultiplier != 0)
                    {
                        if (e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.None)
                        {
                            coin += e.coinsGrantedAtTimeOfPlayMultiplier;
                        }

                        if (e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.PlayerAndNeighbors || e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.Neighbors)
                        {
                            coin += e.coinsGrantedAtTimeOfPlayMultiplier * leftNeighbour.playedStructure.Where(x => x.structureType == e.classConsidered).Count();
                            coin += e.coinsGrantedAtTimeOfPlayMultiplier * rightNeighbour.playedStructure.Where(x => x.structureType == e.classConsidered).Count();

                            if (e.classConsidered == StructureType.WonderStage)
                            {
                                coin += leftNeighbour.currentStageOfWonder * e.coinsGrantedAtTimeOfPlayMultiplier;
                                coin += rightNeighbour.currentStageOfWonder * e.coinsGrantedAtTimeOfPlayMultiplier;
                            }
                        }

                        if (e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.PlayerAndNeighbors || e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.Player)
                        {
                            coin += e.coinsGrantedAtTimeOfPlayMultiplier * playedStructure.Where(x => x.structureType == e.classConsidered).Count();

                            if (e.classConsidered == StructureType.WonderStage)
                            {
                                coin += currentStageOfWonder * e.coinsGrantedAtTimeOfPlayMultiplier;
                            }
                        }
                    }

                    if (e.victoryPointsAtEndOfGameMultiplier != 0)      // JDF: I added this line.  No point in adding Vineyard & Bazar to end of game actions.
                    //for victory points, just copy the effect to endOfGameActions and have executeEndOfGameActions do it later
                        endOfGameActions.Add(act);

                }
                //category 6: special guild cards
                //put these directly into executeEndOfGameActions array
                else if (act is SpecialAbilityEffect)
                {
                    endOfGameActions.Add(act);
                }
                /*
                //category 7: hard coded board powers
                // else if (actions[i][0] == '7')
                else if (act is SpecialBoardEffect)
                {
                    TODO: Fill this in after the board data is updated like the card one.
                    //format: 7(board name)

                    //BB: enable babylon power
                    if (act.Substring(0, 2) == "BB") usedBabylon = false;

                    //EB: (num of vic)(num of coins)
                    //7EB24
                    if (act.Substring(0, 2) == "EB")
                    {
                        victoryPoint += int.Parse(act[2] + "");
                        coin += int.Parse(act[3] + "");
                    }

                    //HA: (num of vic)
                    //enable halicarnassus for the turn
                    //7HA2
                    if (act.Substring(0, 2) == "HA")
                    {
                        victoryPoint += int.Parse(act[2] + "");
                        usedHalicarnassus = false;
                    }

                    //OA
                    //enable olympia power
                    if (act.Substring(0, 2) == "OA")
                    {
                        olympiaPowerEnabled = true;
                        hasOlympia = true;
                    }
                    //OB
                    //copy a purple card from a neighbour. Pass this off to the end of game stuff
                    if (act.Substring(0, 2) == "OB") endOfGameActions[numOfEndOfGameActions++] = actions[i];

                    //RB: (num of shields)(num of vic)(num of coins)
                    //7RB133
                    if (act.Substring(0, 2) == "RB")
                    {
                        shield++;
                        victoryPoint += int.Parse(act[3] + "");
                        coin += int.Parse(act[4] + "");
                    }
                    //LB1: Player gains 5 coins and 4 random new Leader cards
                    //LB2: player plays a leader for free, and 3 VPs
                    if (act.Substring(0, 2) == "LB")
                    {
                        if (act[2] == '1')
                        {
                            coin += 5;
                        }
                        else if (act[2] == '2')
                        {
                            victoryPoint += 3;
                        }
                    }
                }
                //Esteban and Bilkis
                // else if(actions[i][0] == '8')
                else if(act is SpecialLeaderEffect)
                {
                    if (act.Substring(0) == "Esteban")
                    {
                        //enable the Esteban button by sending the Esteban message to the client
                        gm.gmCoordinator.sendMessage(this, "EE");
                    }
                    else if (act.Substring(0) == "Bilkis")
                    {
                        hasBilkis = true;
                    }
                }
                */
                else
                {
                    //do nothing for now
                    throw new NotImplementedException();
                }
            }

            actions.Clear();
        }

        /// <summary>
        /// Execute the end of game actions
        /// Most are hardcoded
        /// </summary>
        public void executeEndOfGameActions()
        {
            foreach (Effect act in endOfGameActions)
            {
                if (act is CoinsAndPointsEffect)
                {
                    CoinsAndPointsEffect e = act as CoinsAndPointsEffect;

                    if (e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.PlayerAndNeighbors ||
                        e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.Neighbors)
                    {
                        victoryPoint += e.victoryPointsAtEndOfGameMultiplier * leftNeighbour.playedStructure.Where(x => x.structureType == e.classConsidered).Count();
                        victoryPoint += e.victoryPointsAtEndOfGameMultiplier * rightNeighbour.playedStructure.Where(x => x.structureType == e.classConsidered).Count();

                        if (e.classConsidered == StructureType.MilitaryLosses)
                        {
                            victoryPoint += leftNeighbour.lossToken * e.victoryPointsAtEndOfGameMultiplier;
                            victoryPoint += rightNeighbour.lossToken * e.victoryPointsAtEndOfGameMultiplier;
                        }

                        if (e.classConsidered == StructureType.MilitaryVictories)
                        {
                            victoryPoint += (leftNeighbour.conflictTokenOne + leftNeighbour.conflictTokenTwo + leftNeighbour.conflictTokenThree) * e.victoryPointsAtEndOfGameMultiplier;
                            victoryPoint += (rightNeighbour.conflictTokenOne + rightNeighbour.conflictTokenTwo + rightNeighbour.conflictTokenThree) * e.victoryPointsAtEndOfGameMultiplier;
                        }

                        if (e.classConsidered == StructureType.WonderStage)
                        {
                            victoryPoint += leftNeighbour.currentStageOfWonder* e.victoryPointsAtEndOfGameMultiplier;
                            victoryPoint += rightNeighbour.currentStageOfWonder * e.victoryPointsAtEndOfGameMultiplier;
                        }
                    }

                    if (e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.PlayerAndNeighbors || e.cardsConsidered == CoinsAndPointsEffect.CardsConsidered.Player)
                    {
                        victoryPoint += e.victoryPointsAtEndOfGameMultiplier * playedStructure.Where(x => x.structureType == e.classConsidered).Count();

                        if (e.classConsidered == StructureType.MilitaryLosses)
                        {
                            victoryPoint += lossToken * e.victoryPointsAtEndOfGameMultiplier;
                        }

                        if (e.classConsidered == StructureType.MilitaryVictories)
                        {
                            victoryPoint += (conflictTokenOne + conflictTokenTwo + conflictTokenThree) * e.victoryPointsAtEndOfGameMultiplier;
                        }

                        if (e.classConsidered == StructureType.WonderStage)
                        {
                            victoryPoint += currentStageOfWonder * e.victoryPointsAtEndOfGameMultiplier;
                        }
                    }
                }
                //category 6: special guild cards and leader cards
                //6_132 or 6_135
                else if (act is SpecialAbilityEffect)
                {
                    /*
                    //card number 132: Scientist guild
                    //award a science that gives the most points
                    if (act.Substring(1) == "132")
                    {
                        //try adding each one and see which artifact will give the highest score

                        //try adding bear trap
                        int sum1 = ((bearTrap + 1) * (bearTrap + 1)) + (sextant * sextant) + (tablet * tablet) + (Math.Min(Math.Min(bearTrap+1, sextant), tablet) * 7);
                        //try adding sextant
                        int sum2 = (bearTrap * bearTrap) + ((sextant+1) * (sextant+1)) + (tablet * tablet) + (Math.Min(Math.Min(bearTrap, sextant+1), tablet) * 7);
                        //try adding tablet
                        int sum3 = (bearTrap * bearTrap) + (sextant * sextant) + ((tablet+1) * (tablet+1)) + (Math.Min(Math.Min(bearTrap, sextant), (tablet+1)) * 7);
                        //choose the one that has the highest sum
                        //if the max is sum1
                        if (Math.Max(Math.Max(sum1, sum2), sum3) == sum1) bearTrap++;
                        //if the max is sum2
                        else if (Math.Max(Math.Max(sum1, sum2), sum3) == sum2) sextant++;
                        //if the max is sum3
                        else if (Math.Max(Math.Max(sum1, sum2), sum3) == sum3) tablet++;
                    }

                    //card number 135
                    //add 1 victory for each brown card, grey card, purple card played
                    if (act.Substring(1) == "135")
                    {
                        for (int j = 0; j < numOfPlayedCards; j++)
                        {
                            if (playedStructure[j].colour == "Brown" || playedStructure[j].colour == "Grey" || playedStructure[j].colour == "Purple")
                            {
                                victoryPoint++;
                            }
                        }

                    }

                    //card number 302 (Gamer's Guild) or 218 (Midas)
                    //add 1 victory point for each 3 coins at the end of the game
                    if (act.Substring(1) == "302" || act.Substring(2) == "218")
                    {
                        victoryPoint += (int)(coin / 3);
                    }

                    //card number 203
                    //add 3 victory point for each 3 science at the end of the game
                    //so instead of adding 7 for each set of science, add 10 instead.

                    if (act.Substring(1) == "203")
                    {
                        int least = Math.Min(Math.Min(bearTrap, sextant), tablet);
                        victoryPoint += (least * 3);
                    }

                    //card number 213 (Justinian)
                    //add 3 VP for every set of blue, red, and green card
                    if (act.Substring(1) == "213")
                    {
                        int blue = 0, red = 0, green = 0;

                        for (int j = 0; j < numOfPlayedCards; j++)
                        {
                            if (playedStructure[j].colour == "Blue") blue++;
                            else if (playedStructure[j].colour == "Green") green++;
                            else if (playedStructure[j].colour == "Red") red++;
                        }

                        int least = Math.Min(Math.Min(blue, red), green);
                        victoryPoint += (least * 3);
                    }

                    //card number 224 (Platon)
                    //add 7 victory point for each set of brown, grey, blue, yellow, green, red, and purple card played
                    if (act.Substring(1) == "224")
                    {
                        //let the 7 numbers be put into an array of integers. Sort these integers. The lowest number will be the least amount

                        int []colours = new int[7];
                        for(int j = 0; j < 7; j++) colours[j] = 0;

                        for (int j = 0; j < numOfPlayedCards; j++)
                        {
                            if (playedStructure[j].colour == "Blue") colours[0]++;
                            else if (playedStructure[j].colour == "Green") colours[1]++;
                            else if (playedStructure[j].colour == "Red") colours[2]++;
                            else if (playedStructure[j].colour == "Brown") colours[3]++;
                            else if (playedStructure[j].colour == "Grey") colours[4]++;
                            else if (playedStructure[j].colour == "Yellow") colours[5]++;
                            else if (playedStructure[j].colour == "Purple") colours[6]++;
                        }

                        //sort the numbers
                        Array.Sort(colours);

                        //the lowest number is the first element. That tells how many sets there are.
                        int least = colours[0];

                        //add multiple of 7 of the least amount.
                        victoryPoint += (least * 7);
                    }

                    //card number 200
                    //Alexander: one extra point per conflict token
                    if (act.Substring(1) == "200")
                    {
                        victoryPoint += (conflictTokenOne + conflictTokenTwo + conflictTokenThree);
                    }

                    //card number 238
                    //Louis Armstrong
                    if (act.Substring(1) == "238")
                    {
                        victoryPoint += (7 - (conflictTokenOne + conflictTokenTwo + conflictTokenThree));
                    }
                    */
                }
                //category 7: end of game board powers
                else if (act is SpecialAbilityEffect)
                {
                    /*
                    //copy best neighbouring purple card
                    if (endOfGameActions[i] == "7OB")
                    {
                        throw new NotImplementedException();
                    }
                    */

                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Add a card to the Player's played structure pile
        /// </summary>
        /// <param name="card"></param>
        public void addPlayedCardStructure(Card card)
        {
            playedStructure.Add(card);
            bUIRequiresUpdating = true;
        }

        /// <summary>
        /// Determines if a given card is buildable.
        /// Returns "T" if it is, returns "F" if it is not
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Buildable isCardBuildable(Card card)
        {
            //retrieve the cost
            Cost cost = card.cost;

            //if the player already owns a copy of the card, Return F immediatley
            if (playedStructure.Exists(x => x.name == card.name))
                return Buildable.StructureAlreadyBuilt;

            //if the cost is !, that means its free. Return T immediately
            if (cost.coin == 0 && cost.wood == 0 && cost.stone == 0 && cost.clay == 0 &&
                cost.ore == 0 &&  cost.cloth == 0 && cost.glass == 0 && cost.papyrus == 0)
            {
                return Buildable.True;
            }

            //if the player owns the prerequiste, Return T immediately
            if (playedStructure.Exists(x => (x.chain[0] == card.name) || (x.chain[1] == card.name)))
                return Buildable.True;

            //if the owner has built card 217: free leader cards
            //if the owner has Rome A board, then same
            //return T if the card is white
#if FALSE
            if ((playerBoard.freeResource == 'D' || hasIDPlayed(/*217*/"Maecenas")) && card.structureType == StructureType.Leader)
            {
                return 'T';
            }

            //if the owner has Rome B board, then get 2 coin discount
            //return F otherwise (since you cannot get more coins from initiating commerce; you can only get resources)
            if (playerBoard.freeResource == 'd' && card.structureType == StructureType.Leader)
            {
                if ((card.cost.coin - 2) <= coin) return 'T';
                else return 'F';
            }

            //if a neighbour own Rome B board, then get a 1 coin discount
            else if ((leftNeighbour.playerBoard.freeResource == 'd' || rightNeighbour.playerBoard.freeResource == 'd') && card.structureType == StructureType.Leader)
            {
                if ((card.cost.coin - 1) <= coin) return 'T';
                else return 'F';
            }
#endif

            //if the owner has built card 228: free guild cards
            //return T if the card is purple
            if (card.structureType == StructureType.Guild && playedStructure.Exists(x => x.name == "Ramses"))
            {
                return Buildable.True;
            }

            /*
            //202, 207, 216: Discount on green, blue and red respectively
            //If a discount applies, determine if it is possible to play the card
            if ((hasIDPlayed(202) && card.colour == "Green") || 
                (hasIDPlayed(207) && card.colour == "Blue") ||
                (hasIDPlayed(216) && card.colour == "Red"))
            {
                bool newCostResult = DAG.canAffordOffByOne(dag, cost);

                if (newCostResult == true) return 'T';
            }
            */

            if (coin < cost.coin)
            {
                // if the card has a coin cost and we don't have enough money, bail out and
                // return not buildable.
                return Buildable.InsufficientCoins;
            }

            //can player afford cost with DAG resources?
            if (isCostAffordableWithDAG(cost) == Buildable.True)
                return Buildable.True;

            //can player afford cost by conducting commerce?
            if (isCostAffordableWithNeighbours(cost) == Buildable.CommerceRequired)
                return Buildable.CommerceRequired;

            //absolutely all options have been exhausted
            //finally return 'F'
            return Buildable.InsufficientResources;
        }

        /// <summary>
        /// Assuming no pre-reqs, free cards, etc.
        /// Determine if a given cost is affordable
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        private Buildable isCostAffordableWithDAG(Cost cost)
        {
            // the passed-in cost structure must not be modified.  C# doesn't support const correctness?!?
            // WTF!
            cost = cost.Copy();

            //get rid of the coins from the cost, and see if DAG can afford the cost (already checked for coins at previous step)
            //this is relevant for the Black cards in the Cities expansion
            // cost = cost.Replace("$", "");
            cost.coin = 0;

            //can I afford the cost with resources in my DAG?
            if (DAG.canAfford(dag, cost)) return Buildable.True;

            return Buildable.InsufficientResources;
        }

        /// <summary>
        /// Determine, given a cost, if Player can afford a cost with his and his 2 neighbours' DAGs combined.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private Buildable isCostAffordableWithNeighbours(Cost cost)
        {
            cost = cost.Copy();

            cost.coin = 0;

            //combine the left, centre, and right DAG
            DAG combinedDAG = DAG.addThreeDAGs(leftNeighbour.dag, dag, rightNeighbour.dag);

            //determine if the combined DAG can afford the cost
            if (DAG.canAfford(combinedDAG, cost)) return Buildable.CommerceRequired;

            return Buildable.InsufficientResources;
        }

        /// <summary>
        /// Determines if the Player's current stage is buildable
        /// Returns "T" if it is, returns "F" if it is not
        /// </summary>
        /// <returns></returns>
        public Buildable isStageBuildable()
        {
            //check if the current Stage is already the maximum stage
            if (currentStageOfWonder >= playerBoard.numOfStages)
                return Buildable.StructureAlreadyBuilt;

            //retrieve the cost
            Cost cost = playerBoard.stageCard[currentStageOfWonder].cost;
            
            //check for the stage discount card (Imhotep)
            if (playedStructure.Exists(x => x.name == "Imhotep") == true)
            {
                bool newCostResult = DAG.canAffordOffByOne(dag, cost);

                if (newCostResult == true) return Buildable.True;
            }

            //can player afford cost with DAG resources
            if (isCostAffordableWithDAG(cost) == Buildable.True) return Buildable.True;

            //can player afford cost by conducting commerce?
            if (isCostAffordableWithNeighbours(cost) == Buildable.CommerceRequired)
                return Buildable.CommerceRequired;

            //absolutely all options exhausted. return F
            return Buildable.InsufficientResources;
        }

        /// <summary>
        /// return the final score
        /// </summary>
        /// <returns></returns>
        public int finalScore()
        {
            int score = 0;

            //1. Red: Add the military conflict points
            score += conflictTokenOne + (conflictTokenTwo * 3) + (conflictTokenThree * 5);
            score -= lossToken;

            //2. Count the coins. Every 3 coins counts for 1 score at the end
            score += ((int)(coin / 3));

            //3. Add victory points from blue cards
            score += victoryPoint;

            //4. Green: Add scientific structures  
            //add up each artifact
            //bearTraps
            score += bearTrap * bearTrap;
            score += sextant * sextant;
            score += tablet * tablet;

            //add 7 points for each three of a kind
            //find the least of the artifact among the three
            int least = Math.Min(Math.Min(bearTrap, sextant), tablet);
            score += (least * 7);

            return score;
        }

        public int doHaveEnoughCoinsToCommerce(String c)
        {
            //retrieve the cost
            String cost = c;

            //parse the tcost of each possible resource
            int brickf = 0, oref = 0, stonef = 0, woodf = 0, glassf = 0, loomf = 0, papyrusf = 0;

            Boolean b = false, o = false, t = false, w = false, g = false, l = false, p = false;


            for (int i = 0; i < cost.Length; i++)
            {
                if (cost[i] == 'B') { brickf++; b = true; }
                else if (cost[i] == 'O') { oref++; o = true; }
                else if (cost[i] == 'T') { stonef++; t = true; }
                else if (cost[i] == 'W') { woodf++; w = true; }
                else if (cost[i] == 'G') { glassf++; g = true; }
                else if (cost[i] == 'L') { loomf++; l = true; }
                else if (cost[i] == 'P') { papyrusf++; p = true; }
            }

            int costB = 0, costO = 0, costT = 0, costW = 0, costG = 0, costP = 0, costL = 0;
            int leftMultiRaw = 2, leftMultiManu = 2, rightMultiRaw = 2, rightMultiManu = 2;

            // left & right manu aren't necessary.  The only card that affects the cost of manufactured
            // goods is the Marketplace, and its effect applies to both neighbors.
            if (leftRaw) leftMultiRaw--;
            if (leftManu) leftMultiManu--;
            if (rightRaw) rightMultiRaw--;
            if (rightManu) rightMultiManu--;


            //calculate how much of each resource i need, for exampe i need to buy 1 ore, 1 wood things
            if (brickf > 0 && brickf > brick) costB = brickf - brick;
            if (oref > 0 && oref > ore) costO = oref - ore;
            if (stonef > 0 && stonef > stone) costT = stonef - stone;
            if (woodf > 0 && woodf > wood) costW = woodf - wood;
            if (glassf > 0 && glassf > glass) costG = glassf - glass;
            if (papyrusf > 0 && papyrusf > papyrus) costP = papyrusf - papyrus;
            if (loomf > 0 && loomf > loom) costL = loomf - loom;


            int amountOfRawFromLeft = 0, amountOfManuFromLeft = 0;
            int amountOfRawFromRight = 0, amountOfManuFromRight = 0;

            Boolean leftFirst = false;

            if (leftRaw) leftFirst = true;
            if (leftManu) leftFirst = true;


            for (int j = 0; j < 2; j++)
            {
                if (leftFirst)
                {
                    //if i can buy from the left player calculate the cost 
                    if (leftNeighbour.brick >= costB && b) { for (int i = 0; i < leftNeighbour.brick; i++) { if (costB > 0) { amountOfRawFromLeft++; costB--; } } }
                    if (leftNeighbour.ore >= costO && o) { for (int i = 0; i < leftNeighbour.ore; i++) { if (costO > 0) { amountOfRawFromLeft++; costO--; } } }
                    if (leftNeighbour.stone >= costT && t) { for (int i = 0; i < leftNeighbour.stone; i++) { if (costT > 0) { amountOfRawFromLeft++; costT--; } } }
                    if (leftNeighbour.wood >= costW && w) { for (int i = 0; i < leftNeighbour.wood; i++) { if (costW > 0) { amountOfRawFromLeft++; costW--; } } }
                    if (leftNeighbour.glass >= costG && g) { for (int i = 0; i < leftNeighbour.glass; i++) { if (costG > 0) { amountOfManuFromLeft++; costG--; } } }
                    if (leftNeighbour.papyrus >= costP && p) { for (int i = 0; i < leftNeighbour.papyrus; i++) { if (costP > 0) { amountOfManuFromLeft++; costP--; } } }
                    if (leftNeighbour.loom >= costL && l) { for (int i = 0; i < leftNeighbour.loom; i++) { if (costL > 0) { amountOfManuFromLeft++; costL--; } } }
                    leftFirst = false;
                }

                else
                {
                    //if i can buy from the right player calculate the cost 
                    if (rightNeighbour.brick >= costB && b) { for (int i = 0; i < rightNeighbour.brick; i++) { if (costB > 0) { amountOfRawFromRight++; costB--; } } }
                    if (rightNeighbour.ore >= costO && o) { for (int i = 0; i < rightNeighbour.ore; i++) { if (costO > 0) { amountOfRawFromRight++; costO--; } } }
                    if (rightNeighbour.stone >= costT && t) { for (int i = 0; i < rightNeighbour.stone; i++) { if (costT > 0) { amountOfRawFromRight++; costT--; } } }
                    if (rightNeighbour.wood >= costW && w) { for (int i = 0; i < rightNeighbour.wood; i++) { if (costW > 0) { amountOfRawFromRight++; costW--; } } }
                    if (rightNeighbour.glass >= costG && g) { for (int i = 0; i < rightNeighbour.glass; i++) { if (costG > 0) { amountOfManuFromRight++; costG--; } } }
                    if (rightNeighbour.papyrus >= costP && p) { for (int i = 0; i < rightNeighbour.papyrus; i++) { if (costP > 0) { amountOfManuFromRight++; costP--; } } }
                    if (rightNeighbour.loom >= costL && l) { for (int i = 0; i < rightNeighbour.loom; i++) { if (costL > 0) { amountOfManuFromRight++; costL--; } } }
                    if (!leftFirst && j == 0) leftFirst = true;
                }
            }

            int totalCost = 0;
            totalCost = (amountOfRawFromLeft * leftMultiRaw) + (amountOfManuFromLeft * leftMultiManu);
            totalCost += (amountOfRawFromRight * rightMultiRaw) + (amountOfManuFromRight * rightMultiManu);

            return totalCost;
        }

        /// <summary>
        /// AI Player makes a move
        /// </summary>
        /// <param name="gm"></param>
        public void makeMove(GameManager gm)
        {
            if (AIBehaviour != null)
            {
                AIBehaviour.makeMove(this, gm);
            }
            /*
            else if (LeadersAIBehaviour != null)
            {
                LeadersAIBehaviour.makeMove(this, (LeadersGameManager)gm);
            }
            */
        }
    }
}
