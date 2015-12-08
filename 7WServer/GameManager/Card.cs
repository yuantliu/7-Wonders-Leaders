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

        public Cost(string initStr)
        {
            coin = wood = stone = clay = ore = cloth = glass = papyrus = 0;

            foreach (char c in initStr)
            {
                switch (c)
                {
                    case '$': ++coin; break;
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
    };

    public enum Buildable
    {
        True,
        CommerceRequired,
        InsufficientResources,
        InsufficientCoins,
        StructureAlreadyBuilt,      // for Wonder stages, this means all wonders stages have been built already.
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
        // MilitaryVictories,
        WonderStage,

        // Expansions
        Leader,
        City,
    };

    public abstract class Effect
    {
        public enum Type
        {
            // Cost,            // Spend coins.  Not used in the card list
            Military,           // All military cards, no others
            Resource,           // All browns, greys, plus the Forum, Caravansery, Alexendria wonder effects
            Science,            // All science cards, not including Scientist's Guild or Babylon wonder
            Commerce,           // Marketplace, Trading Posts, Olympia B stage 1
            CoinsPoints,        // All civilian, most guilds, all age 3 commerce, Vineyard, Bazar, most wonder stages

            // These cards don't fit into one of the above categories
            ShipOwnersGuild,                // ShipOwners Guild
            ScienceWild,                    // Science wild card

            // Special wonder stages
            PlayLastCardInAge,              // Babylon (B) 2nd stage
            PlayDiscardedCardForFree,       // Halikarnassos (A) 2nd stage
            PlayDiscardedCardForFree_1VP,   // Halikarnassos (B) 2nd stage
            PlayDiscardedCardForFree_2VP,   // Halikarnassos (B) 1st stage
            PlayACardForFreeOncePerAge,     // Olympia (A) 2nd stage
            CopyGuildFromNeighbor,          // Olympia (B) 3rd stage
            Rhodos_B_Stage1,                // Rhodos (B) 1st stage
            Rhodos_B_Stage2,                // Rhodos (B) 2nd stage
        };
    };

    // formerly category 0 or '$'
    // Used when gaining (coins parameter is > 0 or losing (coins parameter is < 0) money
    public class CoinEffect : Effect
    {
        public int coins;

        public CoinEffect(int coins)
        {
            this.coins = coins;
        }
    }

    public class MilitaryEffect : Effect
    {
        public int nShields { get; }

        public MilitaryEffect(int nShields)
        {
            this.nShields = nShields;
        }
    }

    public class ResourceEffect : Effect
    {
        public bool canBeUsedByNeighbors;
        public string resourceTypes { get; }

        public ResourceEffect(bool canBeUsedByNeighbors, string resourceTypes)
        {
            this.canBeUsedByNeighbors = canBeUsedByNeighbors;
            this.resourceTypes = resourceTypes;
        }

        public bool IsDoubleResource()
        {
            return resourceTypes.Length == 2 && resourceTypes[0] == resourceTypes[1];
        }
    }

    // formerly category 2
    public class ScienceEffect : Effect
    {
        public enum Symbol
        {
            Compass,
            Gear,
            Tablet,
        };

        public Symbol symbol {
            get
            {
                switch (chSymbol)
                {
                    case 'C': return Symbol.Compass;
                    case 'G': return Symbol.Gear;
                    case 'T': return Symbol.Tablet;
                    default: throw new Exception();
                }
            }
        }

        char chSymbol;

        public ScienceEffect(string symbol)
        {
            this.chSymbol = symbol[0];
        }
    };

    // formerly category 3
    public class CommercialDiscountEffect : Effect
    {
        public enum RawMaterials
        {
            None,
            LeftNeighbor,
            RightNeighbor,
            BothNeighbors,
        };

        public enum Goods
        {
            None,
            BothNeighbors,
        };

        public string effectString { get; }

        public CommercialDiscountEffect(string effectString)
        {
            this.effectString = effectString;
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

    public class ShipOwnersGuildEffect : Effect
    {
    }

    public class ScienceWildEffect : Effect
    {
    }

    public class PlayLastCardInAgeEffect : Effect
    {
    }

    public class PlayDiscardedCardForFreeEffect : Effect
    {
    }

    public class PlayDiscardedCardForFree_1VPEffect : Effect
    {
    }

    public class PlayDiscardedCardForFree_2VPEffect : Effect
    {
    }

    public class PlayACardForFreeOncePerAgeEffect : Effect
    {
    }

    public class CopyGuildFromNeighborEffect : Effect
    {
    }

    public class Rhodos_B_Stage1Effect : Effect
    {
    }

    public class Rhodos_B_Stage2Effect : Effect
    {
    }

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
                wonderStage = int.Parse(createParams[29]);
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
                case Effect.Type.Military:
                    effect = new MilitaryEffect(int.Parse(createParams[21]));
                    break;

                case Effect.Type.Resource:
                    effect = new ResourceEffect(structureType == StructureType.RawMaterial || structureType == StructureType.Goods,
                        createParams[22]);
                    break;

                case Effect.Type.Science:
                    effect = new ScienceEffect(createParams[23]);
                    break;

                case Effect.Type.Commerce:
                    effect = new CommercialDiscountEffect(createParams[24]);
                    break;

                    /*
                case Effect.Type.ResourceChoice:
                    // player can choose one of the RawMaterials or Goods provided.  Raw Materials or
                    // Goods can be bought by neighbors.  Yellow cards and wonder stages cannot be
                    // used by neighboring cities.
                    effect = new ResourceChoiceEffect(structureType == StructureType.RawMaterial || structureType == StructureType.Goods, createParams[25]);
                    break;
                    */

                case Effect.Type.CoinsPoints:
                    CoinsAndPointsEffect.CardsConsidered cardsConsidered = (CoinsAndPointsEffect.CardsConsidered)
                        Enum.Parse(typeof(CoinsAndPointsEffect.CardsConsidered), createParams[25]);

                    StructureType classConsidered =
                        (StructureType)Enum.Parse(typeof(StructureType), createParams[26]);

                    effect = new CoinsAndPointsEffect(cardsConsidered, classConsidered,
                        int.Parse(createParams[27]), int.Parse(createParams[28]));
                    break;

                case Effect.Type.ShipOwnersGuild:
                    effect = new ShipOwnersGuildEffect();
                    break;

                case Effect.Type.ScienceWild:
                    effect = new ScienceWildEffect();
                    break;

                case Effect.Type.PlayLastCardInAge:
                    effect = new PlayLastCardInAgeEffect();
                    break;

                case Effect.Type.PlayDiscardedCardForFree:
                    effect = new PlayDiscardedCardForFreeEffect();
                    break;

                case Effect.Type.PlayDiscardedCardForFree_1VP:
                    effect = new PlayDiscardedCardForFree_1VPEffect();
                    break;

                case Effect.Type.PlayDiscardedCardForFree_2VP:
                    effect = new PlayDiscardedCardForFree_2VPEffect();
                    break;

                case Effect.Type.PlayACardForFreeOncePerAge:
                    effect = new PlayACardForFreeOncePerAgeEffect();
                    break;
                case Effect.Type.CopyGuildFromNeighbor:
                    effect = new CopyGuildFromNeighborEffect();
                    break;
                case Effect.Type.Rhodos_B_Stage1:
                    effect = new Rhodos_B_Stage1Effect();
                    break;
                case Effect.Type.Rhodos_B_Stage2:
                    effect = new Rhodos_B_Stage2Effect();
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
