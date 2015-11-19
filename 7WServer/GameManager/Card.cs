using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public enum StrctureName
    {
        Lumber_Yard,
        Stone_Pit,
        Clay_Pool,
        Ore_Vein,
        Tree_Farm,
        Excavation,
        Clay_Pit,
        Timber_Yard,
        Forest_Cave,
        Mine,
        Loom,
        Glassworks_1,
        Press_1,
        Pawnshop_2,
        Baths,
        Altar,
        Theatre,
        Tavern,
        East_Trading_Post,
        West_Trading_Post,
        Marketplace,
        Stockade,
        Barracks,
        Guard_Tower,
        Apothecary,
        Workshop,
        Scriptorium,
        Sawmill,
        Quarry,
        Brickyard,
        Foundry,
        Loom_2,
        Glassworks_2,
        Press_2,
        Aqueduct,
        Temple,
        Statue,
        Courthouse,
        Forum,
        Caravansery,
        Vineyard,
        Bazar,
        Walls,
        Training_Ground,
        Stables,
        Archery_Range,
        Dispensary,
        Laboratory,
        Library,
        School,
        Pantheon,
        Gardens,
        Town_Hall,
        Palace,
        Senate,
        Haven,
        Lighthouse,
        Chamber_of_Commerce,
        Arena,
        Fortifications,
        Circus,
        Arsenal,
        Seige_Workshop,
        Lodge,
        Observatory,
        University,
        Academy,
        Study,
        Workers_Guild,
        Craftmens_Guild,
        Traders_Guild,
        Philosophers_Guild,
        Spy_Guild,
        Strategy_Guild,
        Shipowners_Guild,
        Scientists_Guild,
        Magistrates_Guild,
        Builders_Guild,
    };


    // will be used for Wonder stages as well as card structures
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

    public enum StructureType
    {
        // Basic cards
        RawMaterial,
        Goods,
        Civilian,
        Commerce,
        Military,
        Science,
        Guild,

        // These cards are not played, but they are used in some effects to determine coins and/or points
        MilitaryLosses,
        MilitaryVictories,
        WonderStage,

        // Expansions
        Leader,
        City,
    };

    public abstract class Effect
    {
        public enum Type
        {
            Money,              // Gain or lose coins
            Simple,             // One of a kind, non-science
            Science,            // science
            Commerce,           // Marketplace, Trading Posts
            ResourceChoice,     // Age 1 either-other resource, Forum, Caravansery
            CoinsPoints,        // Most guilds, All age 3 commerce, Vineyard, Bazar
            SpecializedGuild,   // Science guild, Shipowner's guild
            SpecializedBoard,   // Special board effects
            SpecializedLeader   // Special leader effects (Esteban, Bilkis)
        };

        protected Type effectType;   // $, 1 to 8, possibly more after Cities are added.  // TODO: get rid of this.  Use classes derived from Effect instead.
    };

    // formerly category 0 or '$'
    // Used when losing money
    public class MoneyEffect : Effect
    {
        public int coins;

        public MoneyEffect(int coins)
        {
            this.effectType = Effect.Type.Money;
            this.coins = coins;
        }
    }

    // formerly category 1
    public class SimpleEffect : Effect
    {
        public int multiplier;
        public char type;                                                   // Make this an enum! Coins/Wood/Stone/Clay/Ore/Cloth/Glass/Papyrus

        public SimpleEffect(int multiplier, char type)
        {
            this.effectType = Effect.Type.Simple;
            this.multiplier = multiplier;
            this.type = type;
        }
    };

    // formerly category 2
    public class ScienceEffect : Effect
    {
        public enum Symbol
        {
            Compass,
            Gear,
            Tablet,
        };

        public Symbol symbol;

        public ScienceEffect(Symbol s)
        {
            this.effectType = Effect.Type.Science;
            this.symbol = s;
        }
    };

    // formerly category 3
    public class CommercialDiscountEffect : Effect
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

        public CommercialDiscountEffect(AppliesTo who, Affects productionType)
        {
            this.effectType = Effect.Type.Commerce;
            this.appliesTo = who;
            this.affects = productionType;
        }
    };

    // formerly category 4
    public class ResourceChoiceEffect : Effect
    {
        public string strChoiceData;

        public ResourceChoiceEffect(string s)
        {
            this.effectType = Effect.Type.ResourceChoice;
            this.strChoiceData = s;
        }
    };

    // formerly category 5
    public class CoinsAndPointsEffect : Effect
    {
        public enum CardsConsidered
        {
            Player,
            Neighbors,
            PlayerAndNeighbors,
        };

        public CardsConsidered cardsConsidered;
        public StructureType classConsidered;
        public int coinsGrantedAtTimeOfPlayMultiplier;
        public int victoryPointsAtEndOfGameMultiplier;

        public CoinsAndPointsEffect(CardsConsidered cardsConsidered, StructureType classConsidered, int coinsGrantedAtTimeOfPlayMultiplier, int victoryPointsAtEndOfGameMultiplier)
        {
            this.effectType = Effect.Type.CoinsPoints;
            this.cardsConsidered = cardsConsidered;
            this.classConsidered = classConsidered;
            this.coinsGrantedAtTimeOfPlayMultiplier = coinsGrantedAtTimeOfPlayMultiplier;
            this.victoryPointsAtEndOfGameMultiplier = victoryPointsAtEndOfGameMultiplier;
        }
    };

    // formerly category 6
    public class SpecialGuildEffect : Effect
    {
    }

    // formerly category 7
    public class SpecialBoardEffect : Effect
    {
    }

    // formerly category 8
    public class SpecialLeaderEffect : Effect
    {
    }

    [Serializable]
    public class Card
    {
         // Name Age Type Description Icon	3 players	4 players	5 players	6 players	7 players Cost(coins)    Cost(wood) Cost(stone)    Cost(clay) Cost(ore)  Cost(cloth)    Cost(glass)    Cost(papyrus)  Chains to(1)   Chains to(2)   Effect Category Category 1 multiplier Category 1 effect Catgory 2 symbol Category 3 effect Category 4 effect Category 5: Multiplier(P = player, N = neighbours, B = both player & neighbours)   Category 5: Card/token type Category 5: coins given when card enters play multiplier Category 5: End of game VP granted
        public string name { get; private set; }    // TODO: make this an enum

        public int age { get; private set; }
        public StructureType structureType { get;  private set; }
        public string description { get; private set; }
        public string iconName { get; private set; }
        int[] numAvailableByNumPlayers = new int[5];
        public Cost cost;
        public string[] chain = new string[2];
        public Effect effect;

        public Card(string[] createParams)
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

            var effectType = (Effect.Type)Enum.Parse(typeof(Effect.Type), createParams[20]);

            switch (effectType)
            {
                case Effect.Type.Simple:
                    effect = new SimpleEffect(int. Parse(createParams[21]), createParams[22][0]);
                    break;

                case Effect.Type.Science:
                    switch (createParams[23])
                    {
                        case "C": effect = new ScienceEffect(ScienceEffect.Symbol.Compass); break;
                        case "G": effect = new ScienceEffect(ScienceEffect.Symbol.Gear); break;
                        case "T": effect = new ScienceEffect(ScienceEffect.Symbol.Tablet); break;
                    }
                    break;

                case Effect.Type.Commerce:
                    CommercialDiscountEffect.AppliesTo appliesTo = CommercialDiscountEffect.AppliesTo.BothNeighbors;
                    CommercialDiscountEffect.Affects affects = CommercialDiscountEffect.Affects.RawMaterial;

                    switch (createParams[24][0])
                    {
                        case 'L':
                            appliesTo = CommercialDiscountEffect.AppliesTo.LeftNeighbor;
                            break;

                        case 'R':
                            appliesTo = CommercialDiscountEffect.AppliesTo.RightNeighbor;
                            break;

                        case 'B':
                            // appliesTo = CommercialDiscountEffect.AppliesTo.BothNeighbors;
                            break;

                        default:
                            throw new Exception();
                    }

                    switch (createParams[24][1])
                    {
                        case 'R':
                            // affects = CommercialDiscountEffect.Affects.RawMaterial;
                            break;

                        case 'G':
                            affects = CommercialDiscountEffect.Affects.Goods;
                            break;

                        default:
                            throw new Exception();
                    }

                    effect = new CommercialDiscountEffect(appliesTo, affects);
                    break;

                case Effect.Type.ResourceChoice:
                    // player can choose one of the RawMaterials or Goods provided
                    effect = new ResourceChoiceEffect(createParams[25]);
                    break;

                case Effect.Type.CoinsPoints:
                    CoinsAndPointsEffect.CardsConsidered cardsConsidered = CoinsAndPointsEffect.CardsConsidered.Player;
                    switch (createParams[26])
                    {
                        case "P": /*cardsConsidered = CoinsAndPointsEffect.CardsConsidered.Player; */ break;
                        case "N": cardsConsidered = CoinsAndPointsEffect.CardsConsidered.Neighbors; break;
                        case "B": cardsConsidered = CoinsAndPointsEffect.CardsConsidered.PlayerAndNeighbors; break;
                        default: throw new Exception();
                    }
                    StructureType classConsidered =
                        (StructureType)Enum.Parse(typeof(StructureType), createParams[27]);

                    effect = new CoinsAndPointsEffect(cardsConsidered, classConsidered,
                        int.Parse(createParams[28]), int.Parse(createParams[29]));
                    break;

                case Effect.Type.SpecializedGuild:
                    // No other information is provided in the card list.
                    break;

                case Effect.Type.Money:
                case Effect.Type.SpecializedBoard:
                case Effect.Type.SpecializedLeader:
                    throw new Exception("Unexpected value in cards file");
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
