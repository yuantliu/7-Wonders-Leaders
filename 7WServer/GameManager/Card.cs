using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    [Serializable]
    public class Card
    {
        //ID: 1-X
        public int id { get; set; }
        public string name { get; set; }
        //Age: 1-3
        public int age { get; set; }
        public int numberOfPlayers { get; set; }
        //Cost: SSS means 3 stones. ! means free, # means number of gold, e.g. 2WS = 2 coins 1 wood 1 stone
        public String cost { get; set; }
        //Free Prerequiste: ! means none
        public String freePreq { get; set; }
        //Colour: valid colours are ...
        public String colour { get; set; }
        /*
         * Kinds of effects
         * - gives one type of resource a certain amount:
         *      #L. 
         *      e.g. 2V (2 victory), 3O (3 Ores), 5$ (5 coins)
         *      
         * - gives one science: 
         *      e.g. B (bear trap)
         *           S (sextant)
         *           T (tablet)
         *           
         * - gives a choice of one or more type of resource: 
         *      e.g. WS (wood or stone)
         * 
         * - gives market effect:
         *      L = left, R = right, B = both, 
         *      M = manufactured, R = Raw, 
         *      e.g. LR = Left raw, BR = both manufactured
         * 
         * - gives some $ and/or Victory, depending on neighbour's cards or wonders:
         *      direction: L = left, C = centre, R = right
         *      card: Y = Yellow, N = green, G = grey, R = red, B = brown, S = stage
         *      gold: #
         *      Victory: #
         *      e.g. LCRG20 means This card will give 2 gold immediately and 2 victory points at the end of the game for each Green card for LCR
         *           _C_G22 means This card will give 2 gold and 2 victory points at the end of the game for each Green card for Centre
         */
        public String effect { get; set; }

        public Card(int id, String name, int age, int numberOfPlayers, String cost, String freePreq, String colour, String effect)
        {
            this.id = id;
            this.name = name;
            this.age = age;
            this.numberOfPlayers = numberOfPlayers;
            this.cost = cost;
            this.freePreq = freePreq;
            this.colour = colour;
            this.effect = effect;
        }

        //return a reference to an exact copy of this card
        public Card copy()
        {
            return new Card(id, name, age, numberOfPlayers, cost, freePreq, colour, effect);
        }
    }

    public struct Cost
    {
        public int coin;
        public int wood;
        public int stone;
        public int clay;
        public int ore;
        public int cloth;
        public int glass;
        public int papyrus;
    };

    public struct Effect
    {
        public int type;   // 1 to 8, possibly more after Cities are added.

        public struct Simple
        {
            public int multiplier;
            public char type;
        };

        public enum Science
        {
            Compass,
            Gear,
            Tablet,
        };

        public struct CommercialDiscount
        {
            public enum AppliesTo
            {
                LeftNeighbor,
                RightNeighbor,
                BothNeighbors,
            };

            public enum Affects
            {
                RawMaterial,
                Goods,
            };

            public AppliesTo appliesTo;
            public Affects affects;
        };

        public struct CoinsAndPoints
        {
            public enum CardsConsidered
            {
                Player,
                Neighbors,
                PlayerAndNeighbors,
            };

            public enum ClassConsidered
            {
                RawMaterial,
                Goods,
                Civilian,
                Commerce,
                MilitaryVictories,
                MilitaryLosses,
                Science,
                WonderStage,
            }

            public CardsConsidered cardsConsidered;
            public ClassConsidered classConsidered;
            public int coinsGrantedAtTimeOfPlayMultiplier;
            public int victoryPointsAtEndOfGameMultiplier;
        }

        public Simple simpleInfo;                      // category 1
        public Science scienceSymbol;                  // category 2
        public CommercialDiscount commercialDiscount;  // category 3
        public string resourceChoiceData;              // category 4
        public CoinsAndPoints coinsAndPoints;          // category 5
    };

    [Serializable]
    public class Card2
    {
        public enum StructureType
        {
            RawMaterial,
            Goods,
            Civilian,
            Commerce,
            Military,
            Science,
            Guild,
            Leader,
            City,
        };

        // Name Age Type Description Icon	3 players	4 players	5 players	6 players	7 players Cost(coins)    Cost(wood) Cost(stone)    Cost(clay) Cost(ore)  Cost(cloth)    Cost(glass)    Cost(papyrus)  Chains to(1)   Chains to(2)   Effect Category Category 1 multiplier Category 1 effect Catgory 2 symbol Category 3 effect Category 4 effect Category 5: Multiplier(P = player, N = neighbours, B = both player & neighbours)   Category 5: Card/token type Category 5: coins given when card enters play multiplier Category 5: End of game VP granted
        public string name { get; private set; }
        public int age { get; private set; }
        public StructureType structureType { get;  private set; }
        public string description { get; private set; }
        public string iconName { get; private set; }
        int[] numAvailableByNumPlayers = new int[5];
        public Cost cost;
        public string[] chain = new string[2];
        public Effect effect;

        public Card2(string[] createParams)
        {
            name = createParams[0];
            age = int.Parse(createParams[1]);
            structureType = (StructureType)Enum.Parse(typeof(StructureType), createParams[2]);
            description = createParams[3];
            iconName = createParams[4];
            for (int i = 0, j = 5; i < numAvailableByNumPlayers.Length;  ++i, ++j)
                numAvailableByNumPlayers[i] = int.Parse(createParams[j]);

            // Structure cost
            int.TryParse(createParams[10], out cost.coin);
            int.TryParse(createParams[11], out cost.wood);
            int.TryParse(createParams[12], out cost.stone);
            int.TryParse(createParams[13], out cost.clay);
            int.TryParse(createParams[14], out cost.ore);
            int.TryParse(createParams[15], out cost.cloth);
            int.TryParse(createParams[16], out cost.glass);
            int.TryParse(createParams[17], out cost.papyrus);

            // build chains (Cards that can be built for free in the following age)
            chain[0] = createParams[18];
            chain[1] = createParams[19];

            effect.type = int.Parse(createParams[20]);

            switch (effect.type)
            {
                case 1:
                    effect.simpleInfo.multiplier = int.Parse(createParams[21]);
                    effect.simpleInfo.type = createParams[22][0];
                    break;

                case 2:
                    switch (createParams[23])
                    {
                        case "C": effect.scienceSymbol = Effect.Science.Compass; break;
                        case "G": effect.scienceSymbol = Effect.Science.Gear; break;
                        case "T": effect.scienceSymbol = Effect.Science.Tablet; break;
                    }
                    break;

                case 3:
                    switch (createParams[24][0])
                    {
                        case 'L':
                            effect.commercialDiscount.appliesTo = Effect.CommercialDiscount.AppliesTo.LeftNeighbor;
                            break;

                        case 'R':
                            effect.commercialDiscount.appliesTo = Effect.CommercialDiscount.AppliesTo.RightNeighbor;
                            break;

                        case 'B':
                            effect.commercialDiscount.appliesTo = Effect.CommercialDiscount.AppliesTo.BothNeighbors;
                            break;
                    }

                    switch (createParams[24][1])
                    {
                        case 'R': effect.commercialDiscount.affects = Effect.CommercialDiscount.Affects.RawMaterial;
                            break;
                        case 'G': effect.commercialDiscount.affects = Effect.CommercialDiscount.Affects.Goods;
                            break;
                    }
                    break;

                case 4:
                    effect.resourceChoiceData = createParams[25];       // player can choose one of the RawMaterials or Goods provided
                    break;

                case 5:
                    switch(createParams[26])
                    {
                        case "P": effect.coinsAndPoints.cardsConsidered = Effect.CoinsAndPoints.CardsConsidered.Player; break;
                        case "N": effect.coinsAndPoints.cardsConsidered = Effect.CoinsAndPoints.CardsConsidered.Neighbors; break;
                        case "B": effect.coinsAndPoints.cardsConsidered = Effect.CoinsAndPoints.CardsConsidered.PlayerAndNeighbors; break;
                    }
                    effect.coinsAndPoints.classConsidered = (Effect.CoinsAndPoints.ClassConsidered)Enum.Parse(typeof(Effect.CoinsAndPoints.ClassConsidered), createParams[27]);
                    effect.coinsAndPoints.coinsGrantedAtTimeOfPlayMultiplier = int.Parse(createParams[28]);
                    effect.coinsAndPoints.victoryPointsAtEndOfGameMultiplier = int.Parse(createParams[29]);
                    break;

                case 6:
                    // handled with card-specific code; nothing further we need to do here.
                    break;
            }
        }

        public int GetNumCardsAvailble(int age, int numPlayers)
        {
            if (age != this.age)
                return 0;

            return numAvailableByNumPlayers[numPlayers - 3];
        }
    }
}
