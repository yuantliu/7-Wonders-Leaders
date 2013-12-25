﻿using System;
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

        //hand
        public Card[] hand;
        public int numOfHandCards { get; set; }

        //played structure
        public Card[] playedStructure;
        public int numOfPlayedCards { get; set; }

        //can activate wonder power?
        public bool hasOlympia { get; set; }
        public bool olympiaPowerEnabled { get; set; }
        
        public bool usedHalicarnassus { get; set; }

        public bool usedBabylon { get; set; }

        //bilkis (0 is nothing, 1 is ore, 2 is stone, 3 is glass, 4 is papyrus, 5 is loom, 6 is wood, 7 is brick
        public byte bilkis;
        public bool hasBilkis;

        //stored actions for the turn
        private string[] actions;
        private int numOfActions;
        //up to 10 stored actions allowed
        private const int MAX_ALLOWED_ACTIONS = 10;

        //stored actions for the end of the game
        private string[] endOfGameActions;
        private int numOfEndOfGameActions;
        //up to 20 stored actions allowed
        private const int MAX_ALLOWED_END_OF_GAME_ACTIONS = 20;

        //Player's left and right neighbours
        public Player leftNeighbour { get; set; }
        public Player rightNeighbour { get; set; }

        public Boolean changeNickName {get; set; }
        public String newNickName {get; set; }

        //market effect
        public bool leftRaw = false, rightRaw = false, leftManu = false, rightManu = false;

        //Leaders pile. The pile that holds the unplayed leaders cards
        public List<Card> leadersPile;

        //interface for vanilla AI
        public AIMoveBehaviour AIBehaviour;
        //interface for Leaders AI
        public LeadersAIMoveBehaviour LeadersAIBehaviour;

        private GameManager gm;

        //The Multiple Resource DAG
        DAG dag = new DAG();

        /// <summary>
        /// Constructor. Create a Player with a given nickname
        /// </summary>
        public Player(String nickname, bool isAI, GameManager gm)
        {
            //test
            /*
            brick = 5;
            ore = 5;
            stone = 5;
            wood = 5;

            glass = 5;
            loom = 5;
            papyrus = 5;
            */

            this.nickname = nickname;
            //set whether or not this is an AI
            this.isAI = isAI;
            //assume there can only be up to 10 stored actions
            actions = new string[MAX_ALLOWED_ACTIONS];
            endOfGameActions = new string[MAX_ALLOWED_END_OF_GAME_ACTIONS];
            hand = new Card[7];
            numOfHandCards = 0;
            playedStructure = new Card[25];
            currentStageOfWonder = 0;
            changeNickName = false;
            newNickName = "";
            leadersPile = new List<Card>();

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
        public void storeAction(String s)
        {
            actions[numOfActions++] = s;
        }

        /// <summary>
        /// Check if Salomon (aka Halicarnassus) is stored as an action.
        /// Return false if it is not
        /// Return true if it is, then remove it
        /// </summary>
        /// <returns></returns>
        public bool hasSalomon()
        {
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

            return false;
        }

        /// <summary>
        /// Check if Courtesan's guild is played and remove it if it is
        /// </summary>
        /// <returns></returns>
        public bool hasCourtesan()
        {
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

            return false;
        }

        /// <summary>
        /// Stored actions to be executed at the end of the game
        /// </summary>
        /// <param name="s"></param>
        public void storeEndOfGameAction(String s)
        {
            endOfGameActions[numOfEndOfGameActions++] = s;
        }

        //Execute actions
        //change the Player score information based on the actions
        public void executeAction(GameManager gm)
        {
            //Esteban and Bilkis can be implemented much easier if it has access to GameManager (LeadersGameManager to be exact)
            //Regular GameManager is not useful. Must have LeadersGameManager because nothing in the regular game requires reg GM
            if (gm is LeadersGameManager)
            {
                gm = (LeadersGameManager)gm;
            }

            //go through each action and execute the actions stored
            for (int i = 0; i < numOfActions; i++)
            {
                //this will be the string that represents the action for category 1
                String act = actions[i].Substring(1);

                //category $: deduct a given amount of coins
                if (actions[i][0] == '$')
                {
                    coin -= int.Parse(actions[i].Substring(1));
                }
                //category 1: give one kind of non-science thing
                else if (actions[i][0] == '1')
                {
                    //increase the appropriate field by num
                    int num = int.Parse(act[0] + "");

                    switch (act[1])
                    {
                        case 'S':
                            shield += num;
                            break;
                        case 'V':
                            victoryPoint += num;
                            break;
                        case 'O':
                            ore += num;
                            break;
                        case 'B':
                            brick += num;
                            break;
                        case 'T':
                            stone += num;
                            break;
                        case 'W':
                            wood += num;
                            break;
                        case '$':
                            coin += num;
                            break;
                        case 'L':
                            loom += num;
                            break;
                        case 'P':
                            papyrus += num;
                            break;
                        case 'G':
                            glass += num;
                            break;
                        case 'd':
                        case 'D':
                            break;
                        default:
                            throw new Exception();
                    }
                }
                //category 2: add one science
                else if (actions[i][0] == '2')
                {
                    switch (act[0])
                    {
                        case 'T':
                            tablet++;
                            break;
                        case 'B':
                            bearTrap++;
                            break;
                        case 'S':
                            sextant++;
                            break;
                        default:
                            throw new Exception();
                    }
                }
                //category 3: market effect
                else if (actions[i][0] == '3')
                {
                    //set the market effects
                    if(act == "LR"){
                        leftRaw = true;
                    }
                    else if (act == "RR")
                    {
                        rightRaw = true;
                    }
                    else if (act == "BR")
                    {
                        leftRaw = true; rightRaw = true;
                    }
                    if(act == "LM"){
                        leftManu = true;
                    }
                    else if (act == "RM")
                    {
                        rightManu = true;
                    }
                    else if (act == "BM")
                    {
                        leftManu = true; rightManu = true;
                    }
                }
                //category 4: gives a choice between different things
                //Add to the DAG
                else if (actions[i][0] == '4')
                {
                    dag.add(actions[i].Substring(1));
                }
                //category 5: gives some $ and and/or some victory depending on some conditions
                //these cards are usually yellow
                else if (actions[i][0] == '5')
                {
                    //add gold only if there are gold to add
                    if (act[4] != '0')
                    {
                        //colours that are being looked for: G = grey, B = brown, b = blue, N = green, Y = yellow, S = stage
                        char colour = act[3];
                    
                        //add the gold to the effects immediately
                        //look at the left
                        if (act[0] == 'L')
                        {
                            for (int j = 0; j < leftNeighbour.numOfPlayedCards; j++)
                            {
                                if (leftNeighbour.playedStructure[j].colour == "Grey" && colour == 'G') coin += int.Parse(act[4] + "");
                                else if (leftNeighbour.playedStructure[j].colour == "Brown" && colour == 'B') coin += int.Parse(act[4] + "");
                                else if (leftNeighbour.playedStructure[j].colour == "Yellow" && colour == 'Y') coin += int.Parse(act[4] + "");
                            }
                        }
                        //look at centre
                        if (act[1] == 'C')
                        {
                            for (int j = 0; j < numOfPlayedCards; j++)
                            {
                                if (playedStructure[j].colour == "Grey" && colour == 'G') coin += int.Parse(act[4] + "");
                                else if (playedStructure[j].colour == "Brown" && colour == 'B') coin += int.Parse(act[4] + "");
                                else if (playedStructure[j].colour == "Yellow" && colour == 'Y') coin += int.Parse(act[4] + "");
                            }
                        }
                        //look at right
                        if (act[2] == 'R')
                        {
                            for (int j = 0; j < rightNeighbour.numOfPlayedCards; j++)
                            {
                                if (rightNeighbour.playedStructure[j].colour == "Grey" && colour == 'G') coin += int.Parse(act[4] + "");
                                else if (rightNeighbour.playedStructure[j].colour == "Brown" && colour == 'B') coin += int.Parse(act[4] + "");
                                else if (rightNeighbour.playedStructure[j].colour == "Yellow" && colour == 'Y') coin += int.Parse(act[4] + "");
                                
                            }
                        }

                        //add the coins for the appropriate stages
                        if (colour == 'S') coin += ((leftNeighbour.currentStageOfWonder) * (int.Parse(act[4] + "")));
                        if (colour == 'S') coin += (currentStageOfWonder * (int.Parse(act[4] + "")));
                        if (colour == 'S') coin += ((rightNeighbour.currentStageOfWonder) * (int.Parse(act[4] + "")));
                    }

                    //for victory points, just copy the effect to endOfGameActions and have executeEndOfGameActions do it later
                    endOfGameActions[numOfEndOfGameActions++] = actions[i];

                }
                //category 6: special guild cards
                //put these directly into executeEndOfGameActions array
                else if (actions[i][0] == '6')
                {
                    endOfGameActions[numOfEndOfGameActions++] = actions[i];
                }
                //category 7: hard coded board powers
                else if (actions[i][0] == '7')
                {
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
                else if(actions[i][0] == '8')
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
                else
                {
                    //do nothing for now
                    //throw new NotImplementedException();
                }
            }

            numOfActions = 0;
        }

        /// <summary>
        /// Execute the end of game actions
        /// Most are hardcoded
        /// </summary>
        public void executeEndOfGameActions()
        {
            //2 types of effects: category 5 (yellow cards that add victory points) or category 6 (guild cards)
            for (int i = 0; i < numOfEndOfGameActions; i++)
            {
                String act = endOfGameActions[i].Substring(1);

                int points = 0;

                //category 5
                if (endOfGameActions[i][0] == '5')
                {
                    //add victory points
                    //colours that are being looked for: G = grey, B = brown, b = blue, N = green, Y = yellow, S = stage, L = loss, R=red, P=purple, W=White, c = conflict token
                    char colour = act[3];

                    //add the victory points
                    //look at the left
                    if (act[0] == 'L')
                    {
                        for (int j = 0; j < leftNeighbour.numOfPlayedCards; j++)
                        {
                            if (leftNeighbour.playedStructure[j].colour == "Grey" && colour == 'G') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "Brown" && colour == 'B') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "Blue" && colour == 'b') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "Green" && colour == 'N') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "Red" && colour == 'R') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "Yellow" && colour == 'Y') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "White" && colour == 'W') points += int.Parse(act[5] + "");
                            else if (leftNeighbour.playedStructure[j].colour == "Purple" && colour == 'P') points += int.Parse(act[5] + "");
                        }
                    }
                    //look at centre
                    if (act[1] == 'C')
                    {
                        for (int j = 0; j < numOfPlayedCards; j++)
                        {
                            if (playedStructure[j].colour == "Grey" && colour == 'G') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "Brown" && colour == 'B') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "Blue" && colour == 'b') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "Green" && colour == 'N') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "Red" && colour == 'R') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "Yellow" && colour == 'Y') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "White" && colour == 'W') points += int.Parse(act[5] + "");
                            else if (playedStructure[j].colour == "Purple" && colour == 'P') points += int.Parse(act[5] + "");

                        }
                    }
                    //look at right
                    if (act[2] == 'R')
                    {
                        for (int j = 0; j < rightNeighbour.numOfPlayedCards; j++)
                        {
                            if (rightNeighbour.playedStructure[j].colour == "Grey" && colour == 'G') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "Blue" && colour == 'b') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "Green" && colour == 'N') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "Red" && colour == 'R') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "Brown" && colour == 'B') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "Yellow" && colour == 'Y') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "White" && colour == 'W') points += int.Parse(act[5] + "");
                            else if (rightNeighbour.playedStructure[j].colour == "Purple" && colour == 'P') points += int.Parse(act[5] + "");
      
                        }
                    }

                    //look into stages and losses
                    if (colour == 'L') points += ((leftNeighbour.lossToken) * (int.Parse(act[5] + "")));
                    if (colour == 'S') points += ((leftNeighbour.currentStageOfWonder) * (int.Parse(act[5] + "")));
                    if (colour == 'L') points += ((lossToken) * (int.Parse(act[5] + "")));
                    if (colour == 'S') points += ((currentStageOfWonder) * (int.Parse(act[5] + "")));
                    if (colour == 'L') points += ((rightNeighbour.lossToken) * (int.Parse(act[5] + "")));
                    if (colour == 'S') points += ((rightNeighbour.currentStageOfWonder) * (int.Parse(act[5] + "")));
                }
                //category 6: special guild cards and leader cards
                //6_132 or 6_135
                else if (endOfGameActions[i][0] == '6')
                {
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
                                points++;
                            }
                        }

                    }

                    //card number 302 (Gamer's Guild) or 218 (Midas)
                    //add 1 victory point for each 3 coins at the end of the game
                    if (act.Substring(1) == "302" || act.Substring(2) == "218")
                    {
                        points += (int)(coin / 3);
                    }

                    //card number 203
                    //add 3 victory point for each 3 science at the end of the game
                    //so instead of adding 7 for each set of science, add 10 instead.

                    if (act.Substring(1) == "203")
                    {
                        int least = Math.Min(Math.Min(bearTrap, sextant), tablet);
                        points += (least * 3);
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
                        points += (least * 3);
                    }

                    //card number 224 (Platon)
                    //add 7 victory points for each set of brown, grey, blue, yellow, green, red, and purple card played
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
                        points += (least * 7);
                    }

                    //card number 200
                    //Alexander: one extra point per conflict token
                    if (act.Substring(1) == "200")
                    {
                        points += (conflictTokenOne + conflictTokenTwo + conflictTokenThree);
                    }

                    //card number 238
                    //Louis Armstrong
                    if (act.Substring(1) == "238")
                    {
                        points += (7 - (conflictTokenOne + conflictTokenTwo + conflictTokenThree));
                    }
                }

                //Announce the points granted and increment the points to victory points
                gm.gmCoordinator.sendMessage(this, "#At the end of the game, a card grants " + points + " to " + nickname + "."); 

                victoryPoint += points;
            }

        }

        /// <summary>
        /// Add a card to the Player's played structure pile
        /// </summary>
        /// <param name="card"></param>
        public void addPlayedCardStructure(Card card)
        {
            playedStructure[numOfPlayedCards++] = card;
        }

        /// <summary>
        /// Add a card to the Player's Hand
        /// </summary>
        /// <param name="card"></param>
        public void addHand(Card card)
        {
            hand[numOfHandCards++] = card;
        }

        /// <summary>
        /// Determines if a given card is buildable.
        /// Returns "T" if it is, returns "F" if it is not
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public char isCardBuildable(Card card)
        {
            //retrieve the cost
            String cost = card.cost;

            
            //if the player already owns a copy of the card, Return F immediatley
            for (int i = 0; i < numOfPlayedCards; i++)
            {
                if (playedStructure[i].name == card.name)
                {
                    return 'F';
                }
            }
            
            //if the cost is !, that means its free. Return T immediately
            if (cost == "!") return 'T';
            
            //if the player owns the prerequiste, Return T immediately
            for (int i = 0; i < numOfPlayedCards; i++)
            {
                if (playedStructure[i].name == card.freePreq)
                {
                    return 'T';
                }
            }

            //if the owner has built card 217: free leader cards
            //if the owner has Rome A board, then same
            //return T if the card is white
            if ((playerBoard.freeResource == 'D' || hasIDPlayed(217)) && card.colour == "White")
            {
                return 'T';
            }

            //if the owner has Rome B board,
            //then return T if its affordable after applying the 2 coin discount
            //return F otherwise
            if (playerBoard.freeResource == 'd' && card.colour == "White")
            {
                if ((card.cost.Length - 2) <= coin) return 'T';
                else return 'F';
            }

            //if a neighbour own Rome B board, then get a 1 coin discount
            else if (card.colour == "White" && (leftNeighbour.playerBoard.freeResource == 'd' || rightNeighbour.playerBoard.freeResource == 'd'))
            {
                if ((card.cost.Length - 1) <= coin) return 'T';
                else return 'F';
            }

            //if the owner has built card 228: free guild cards
            //return T if the card is purple
            if (card.colour == "Purple" && hasIDPlayed(228))
            {
                return 'T';
            }

            //202, 207, 216: Discount on green, blue and red respectively
            //If a discount applies, determine if it is possible to play the card
            if ((hasIDPlayed(202) && card.colour == "Green") || 
                (hasIDPlayed(207) && card.colour == "Blue") ||
                (hasIDPlayed(216) && card.colour == "Red"))
            {
                //remove a character from the cost to determine if it is possible to play the card

                string cardCost = card.cost;
                char bestOption = 'F';

                for (int i = 0; i < cardCost.Length; i++)
                {
                    string newCost = cardCost;
                    newCost = newCost.Remove(i, 1);
                    char newCostResult = isCostAffordable(newCost);

                    if (newCostResult != 'F')
                    {
                        if (newCostResult == 'C' && bestOption == 'F')
                        {
                            bestOption = 'C';
                        }
                        else if (newCostResult == 'T')
                        {
                            return 'T';
                        }
                    }
                }

                return bestOption;
            }

            return isCostAffordable(cost);
        }

        /// <summary>
        /// Assuming no pre-reqs, free cards, etc.
        /// Determine if a given cost is affordable
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cost"></param>
        /// <returns></returns>
        private char isCostAffordable(string cost)
        {
            //parse the cost of each possible resource
            int brickf = 0, oref = 0, stonef = 0, woodf = 0, glassf = 0, loomf = 0, papyrusf = 0, coinsf = 0;

            for (int i = 0; i < cost.Length; i++)
            {
                if (cost[i] == 'B') { brickf++; }
                else if (cost[i] == 'O') { oref++; }
                else if (cost[i] == 'T') stonef++;
                else if (cost[i] == 'W') woodf++;
                else if (cost[i] == 'G') glassf++;
                else if (cost[i] == 'L') loomf++;
                else if (cost[i] == 'P') papyrusf++;
                else if (cost[i] == '$') coinsf++;
            }

            //////////////////////////////////////////

            //if theres not enough coins, then return false
            if (coin < coinsf)
            {
                return 'F';
            }

            //check if the singletons are able to pay for the items
            if (brick >= brickf && ore >= oref && stone >= stonef && wood >= woodf && glass >= glassf && loom >= loomf && papyrus >= papyrusf && coin >= coinsf) return 'T';
            //singletons are NOT able to pay for the items

            //See if item is affordable with multiples
            //first, eliminate singletons
            string singletons = "";
            for(int i = 0; i < brick; i++) singletons += "B";
            for(int i = 0; i < ore; i++) singletons += "O";
            for(int i = 0; i < stone; i++) singletons += "T";
            for(int i = 0; i < wood; i++) singletons += "W";
            for(int i = 0; i < glass; i++) singletons += "G";
            for(int i = 0; i < loom; i++) singletons += "L";
            for(int i = 0; i < papyrus; i++) singletons += "P";
            
            //eliminate singletons from cost
            cost = DAG.eliminate(cost, singletons);

            //can I afford with my multiples?
            if (DAG.canAfford(dag, cost)) return 'T';
            //otherwise, must resort to neighbours
            else return isCostAffordableWithNeighbours(cost);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private char isCostAffordableWithNeighbours(string cost)
        {
            if (coin == 0) return 'F';

            //parse the cost of each possible resource
            int brickf = 0, oref = 0, stonef = 0, woodf = 0, glassf = 0, loomf = 0, papyrusf = 0;


            for (int i = 0; i < cost.Length; i++)
            {
                if (cost[i] == 'B') { brickf++; }
                else if (cost[i] == 'O') { oref++; }
                else if (cost[i] == 'T') { stonef++; }
                else if (cost[i] == 'W') { woodf++; }
                else if (cost[i] == 'G') { glassf++; }
                else if (cost[i] == 'L') { loomf++; }
                else if (cost[i] == 'P') { papyrusf++; }

            }

            ////////////////////////////////////////
            ////////temporaray for commerce/////////
            ////////////////////////////////////////
            int brickC = 0, oreC = 0, stoneC = 0, woodC = 0, glassC = 0, loomC = 0, papyrusC = 0;

            brickC = brick + leftNeighbour.brick + rightNeighbour.brick;
            oreC = ore + leftNeighbour.ore + rightNeighbour.ore;
            stoneC = stone + leftNeighbour.stone + rightNeighbour.stone;
            woodC = wood + leftNeighbour.wood + rightNeighbour.wood;
            glassC = glass + leftNeighbour.glass + rightNeighbour.glass;
            loomC = loom + leftNeighbour.loom + rightNeighbour.loom;
            papyrusC = papyrus + leftNeighbour.papyrus + rightNeighbour.papyrus;


            int totalCost = doHaveEnoughCoinsToCommerce(cost);


            //check if there are enough
            if (brickC >= brickf && oreC >= oref && stoneC >= stonef && woodC >= woodf && glassC >= glassf && loomC >= loomf && papyrusC >= papyrusf && coin >= totalCost) return 'C';
            else return 'F';
        }

        /// <summary>
        /// Determines if the Player's current stage is buildable
        /// Returns "T" if it is, returns "F" if it is not
        /// </summary>
        /// <returns></returns>
        public char isStageBuildable()
        {
            //check if the current Stage is already the maximum stage
            if (currentStageOfWonder >= playerBoard.numOfStages)
                return 'F';

            //retrieve the cost
            string cost = playerBoard.cost[currentStageOfWonder];
            

            if (hasIDPlayed(212) == true)
            {
                char bestOption = 'F';
                
                for (int i = 0; i < cost.Length; i++)
                {
                    string newCost = cost;
                    newCost = newCost.Remove(i, 1);
                    char newCostResult = isCostAffordable(newCost);
                    if (newCostResult != 'F')
                    {
                        if (newCostResult == 'C' && bestOption == 'F')
                        {
                            bestOption = 'C';
                        }
                        else if (newCostResult == 'T')
                        {
                            return 'T';
                        }
                    }
                }

                return bestOption;
            }

            return isCostAffordable(cost);
        }

        public String commerceInformation()
        {
            //(current)_(resources)|(leftPlayer)_(resources)|(rightPlayer)_(resources)
            String commerceInfo = "";

            String rightR = "F", rightM = "F", leftR = "F", leftM = "F";

            if (rightRaw) rightR = "T";
            if (leftRaw) leftR = "T";
            if (rightManu) rightM = "T";
            if (leftManu) leftM = "T";

            //"12_TWTO_Host_5_1_1_1_1_1_2_1|yunus_2_2_2_2_2_2_2_5|hello_3_3_3_3_3_3_3_8|T_F_T_F_)");


            //leftPlayer
            commerceInfo += leftNeighbour.nickname + "_";
            commerceInfo += leftNeighbour.brick + "_" + leftNeighbour.ore + "_" + leftNeighbour.stone + "_" + leftNeighbour.wood + "_" + leftNeighbour.glass + "_" + leftNeighbour.papyrus + "_" + leftNeighbour.loom + "_" + leftNeighbour.coin + "|";

            //current Player
            commerceInfo += nickname + "_";
            commerceInfo += brick + "_" + ore + "_" + stone + "_" + wood + "_" + glass + "_" + papyrus + "_" + loom + "_" + coin + "|";

            //rightPlayer
            commerceInfo += rightNeighbour.nickname + "_";
            commerceInfo += rightNeighbour.brick + "_" + rightNeighbour.ore + "_" + rightNeighbour.stone + "_" + rightNeighbour.wood + "_" + rightNeighbour.glass + "_" + rightNeighbour.papyrus + "_" + rightNeighbour.loom + "_" + rightNeighbour.coin + "|";

            //Add the market effect
            commerceInfo += leftR + "_" + leftM + "_" + rightR + "_" + rightM + "_";

            return commerceInfo;

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
        /// Return the last played card
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Card getLastPlayedCard()
        {
            return playedStructure[numOfPlayedCards - 1];
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
            else if (LeadersAIBehaviour != null)
            {
                LeadersAIBehaviour.makeMove(this, (LeadersGameManager)gm);
            }
        }

        /// <summary>
        /// Determine if player has played a card with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool hasIDPlayed(int id)
        {
            for (int i = 0; i < numOfPlayedCards; i++)
            {
                if (playedStructure[i].id == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}