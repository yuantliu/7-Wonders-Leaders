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

    [Serializable]
    // will be used for Wonder stages as well as card structures
    public class Cost
    {
        public int coin;
        public int wood;
        public int stone;
        public int clay;
        public int ore;
        public int cloth;
        public int glass;
        public int papyrus;

        public Cost()
        {
            coin = wood = stone = clay = ore = cloth = glass = papyrus = 0;
        }

        /*
        public Cost(string initStr)
        {
            coin = wood = stone = clay = ore = cloth = glass = papyrus = 0;

            foreach (char c in initStr)
            {
                switch (c)
                {
                    case 'W': ++wood; break;
                    case 'S': ++stone; break;
                    case 'B': ++clay; break;
                    case 'O': ++ore; break;
                    case 'C': ++cloth; break;
                    case 'G': ++glass; break;
                    case 'P': ++papyrus; break;
                    default:
                        throw new Exception();
                }
            }
        }
        */

        public bool IsZero()
        {
            return coin == 0 && wood == 0 && stone == 0 && clay == 0 && ore == 0 && cloth == 0 && glass == 0 && papyrus == 0;
        }

        public Cost Copy()
        {
            Cost c = new Cost();

            c.coin = this.coin;
            c.wood = this.wood;
            c.stone = this.stone;
            c.clay = this.clay;
            c.ore = this.ore;
            c.cloth = this.cloth;
            c.glass = this.glass;
            c.papyrus = this.papyrus;

            return c;
        }

        public int Total()
        {
            return coin + wood + stone + clay + ore + cloth + glass + papyrus;
        }

        /*
        public static bool operator == (Cost a, Cost b)
        {
            return a.coin == b.coin && a.wood == b.wood && a.stone == b.stone && a.clay == b.clay &&
                a.ore == b.ore && a.cloth == b.cloth && a.glass == b.glass && a.papyrus == b.papyrus;
        }

        public static bool operator != (Cost a, Cost b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            Cost c = obj as Cost;
            if (c != null)
            {
                return c == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetHashCode();
        }

        public static Cost operator -(Cost input, Cost subtractionAmount)
        {
            Cost result = new Cost();

            result.coin =    Math.Max(input.coin -    subtractionAmount.coin, 0);
            result.wood =    Math.Max(input.wood -    subtractionAmount.wood, 0);
            result.stone =   Math.Max(input.stone -   subtractionAmount.stone, 0);
            result.clay =    Math.Max(input.clay -    subtractionAmount.clay, 0);
            result.ore =     Math.Max(input.ore -     subtractionAmount.ore, 0);
            result.cloth =   Math.Max(input.cloth -   subtractionAmount.cloth, 0);
            result.glass =   Math.Max(input.wood -    subtractionAmount.glass, 0);
            result.papyrus = Math.Max(input.papyrus - subtractionAmount.papyrus, 0);

            return result;
        }

                    */

    };

    public enum Buildable
    {
        True,
        False,
        CommerceRequired,
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

        // Tavern, Ephesos (A) stage 2, Ephesos (B), all 3 stages.  The number of coins and points is NOT dependent on external factors.
        Constant,

        // These cards are not played, but they are used in some effects to determine coins and/or points
        MilitaryLosses,
        MilitaryVictories,
        WonderStage,

        // Expansions
        Leader,
        City,
    };

    [Serializable]
    public abstract class Effect
    {
        public enum Type
        {
            //Money,              // Gain or lose coins
            Simple,             // One of a kind, non-science
            Science,            // science
            Commerce,           // Marketplace, Trading Posts
            ResourceChoice,     // Age 1 either-other resource, Forum, Caravansery
            CoinsPoints,        // Most guilds, All age 3 commerce, Vineyard, Bazar
            SpecialAbility,     // Science guild, Shipowner's guild, some Wonder board stages with unique effects
        };
    };

    // formerly category 0 or '$'
    // Used when losing money
    public class MoneyEffect : Effect
    {
        public int coins;

        public MoneyEffect(int coins)
        {
            this.coins = coins;
        }
    }

    [Serializable]
    // formerly category 1
    public class SimpleEffect : Effect
    {
        public int multiplier;
        public char type;                                                   // Make this an enum! Coins/Wood/Stone/Clay/Ore/Cloth/Glass/Papyrus

        public SimpleEffect(int multiplier, char type)
        {
            this.multiplier = multiplier;
            this.type = type;
        }
    };

    [Serializable]
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
            this.symbol = s;
        }
    };

    [Serializable]
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
            this.appliesTo = who;
            this.affects = productionType;
        }
    };

    [Serializable]
    // formerly category 4
    public class ResourceChoiceEffect : Effect
    {
        public string strChoiceData;

        public bool canBeUsedByNeighbors;

        public ResourceChoiceEffect(bool canBeUsedByNeighbors, string s)
        {
            this.canBeUsedByNeighbors = canBeUsedByNeighbors;
            this.strChoiceData = s;
        }
    };

    // formerly category 5
    public class CoinsAndPointsEffect : Effect
    {
        public enum CardsConsidered
        {
            None,
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
            this.cardsConsidered = cardsConsidered;
            this.classConsidered = classConsidered;
            this.coinsGrantedAtTimeOfPlayMultiplier = coinsGrantedAtTimeOfPlayMultiplier;
            this.victoryPointsAtEndOfGameMultiplier = victoryPointsAtEndOfGameMultiplier;
        }
    };


    // formerly category 6
    public class SpecialAbilityEffect : Effect
    {
        public enum SpecialType
        {
            ShipOwnerGuild,
            ScienceWild,
            PlayLastCardInAge,
            PlayDiscardedCardForFree,
            PlayDiscardedCardForFree_2VP,
            PlayDiscardedCardForFree_1VP,
            PlayACardForFreeOncePerAge,
            CopyGuildFromNeighbor,
            Rhodos_B_1M3VP3C,
            Rhodos_B_1M4VP4C,
        };

        SpecialType type;

        public SpecialAbilityEffect(string initStr)
        {
            type = (SpecialType)Enum.Parse(typeof(SpecialType), initStr);
        }
    }

    [Serializable]
    public class Card
    {
         // Name Age Type Description Icon	3 players	4 players	5 players	6 players	7 players Cost(coins)    Cost(wood) Cost(stone)    Cost(clay) Cost(ore)  Cost(cloth)    Cost(glass)    Cost(papyrus)  Chains to(1)   Chains to(2)   Effect Category Category 1 multiplier Category 1 effect Catgory 2 symbol Category 3 effect Category 4 effect Category 5: Multiplier(P = player, N = neighbours, B = both player & neighbours)   Category 5: Card/token type Category 5: coins given when card enters play multiplier Category 5: End of game VP granted
        public string name { get; private set; }    // TODO: make this an enum

        public int age;

        public int wonderStage;

        public StructureType structureType { get;  private set; }
        public string description { get; private set; }
        public string iconName { get; private set; }
        int[] numAvailableByNumPlayers = new int[5];
        public Cost cost = new Cost();      // TODO: is it possible to make this immutable?
        public string[] chain = new string[2];
        public Effect effect;

        public Card(string[] createParams)
        {
            name = createParams[0];
            if (createParams[1] != string.Empty)
            {
                age = int.Parse(createParams[1]);
                wonderStage = 0;
            }
            else
            {
                age = 0;
                wonderStage = int.Parse(createParams[31]);
            }

            structureType = (StructureType)Enum.Parse(typeof(StructureType), createParams[2]);
            description = createParams[3];
            iconName = createParams[4];

            if (structureType != StructureType.WonderStage)
            {
                for (int i = 0, j = 5; i < numAvailableByNumPlayers.Length;  ++i, ++j)
                    numAvailableByNumPlayers[i] = int.Parse(createParams[j]);
            }

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
                            // appliesTo = CommercialDiscountEffect.AppliesTo.BothNeighbors; (defaulted value)
                            break;

                        default:
                            throw new Exception();
                    }

                    switch (createParams[24][1])
                    {
                        case 'R':
                            // affects = CommercialDiscountEffect.Affects.RawMaterial; (defaulted value)
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
                    // player can choose one of the RawMaterials or Goods provided.  Raw Materials or
                    // Goods can be bought by neighbors.  Yellow cards and wonder stages cannot be
                    // used by neighboring cities.
                    effect = new ResourceChoiceEffect(structureType == StructureType.RawMaterial || structureType == StructureType.Goods, createParams[25]);
                    break;

                case Effect.Type.CoinsPoints:
                    CoinsAndPointsEffect.CardsConsidered cardsConsidered = (CoinsAndPointsEffect.CardsConsidered)
                        Enum.Parse(typeof(CoinsAndPointsEffect.CardsConsidered), createParams[26]);

                    StructureType classConsidered =
                        (StructureType)Enum.Parse(typeof(StructureType), createParams[27]);

                    effect = new CoinsAndPointsEffect(cardsConsidered, classConsidered,
                        int.Parse(createParams[28]), int.Parse(createParams[29]));
                    break;

                case Effect.Type.SpecialAbility:
                    effect = new SpecialAbilityEffect(createParams[30]);
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
