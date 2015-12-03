using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    public class DAG
    {
        // this list needs to be sorted in a particular order.  Simplest types at the top so those cards are used up first
        // when calculating whether a structure is affordable.  Those are RawMaterials which do not offer a choice
        // Goods and single-choice Resources come first.  Next come double RawMaterial cards.  They don't offer a
        // choice but are better than single Raw materials.  Next would come first-age resource cards that have a choice of
        // two.  There's a beter chance that by the time those cards are considered, one of the needed resources has
        // already been taken care of by a single-resource card.  Last are the cards that offer a choice of all 3 goods
        // or all 4 raw materials (Forum/Caravansery/Alexandria stages as they are the most flexible).  After those are
        // considered, we look at Bilkis and other leaders who provide a -1 discount on certain structure classes.
        // Also the Secret Warehouse and Black Market are in there somewhere.
        List<ResourceEffect> resources = new List<ResourceEffect>();

        public bool hasTemp = false;

        /*
        //Bilkis: add a temporary resource
        //This temporary resource will be wiped after every turn
        //Whenever add() is called, we know we have reached another turn
        public void addTemp(char temp)
        {
            char[] tempResource = new char[1];
            tempResource[0] = temp;

            //add a temp resource
            if (hasTemp == false)
            {
                hasTemp = true;
            }
            //remove the existing temp resource
            else
            {
                graph.RemoveAt(graph.Count - 1);
            }

            graph.Add(tempResource);
        }

        /// <summary>
        /// Remove the temporary resource added by Bilkis if it is there
        /// </summary>
        public void removeTemp()
        {
            if (hasTemp == true)
            {
                hasTemp = false;
                graph.RemoveAt(graph.Count - 1);
            }
        }
        */

        //Add an OR resource
        public void add(ResourceEffect s)
        {
            /*
            char[] newInput = s.ToCharArray();
            //when there is a temp resource, insert into a non-last position so that the removal of temp resource works later
            if (hasTemp == true)
            {
                graph.Insert(0, newInput);
            }
            else
            {
            */
       
            // resource effect list is ordered.  First are resources which provide a single resource.
            // Then double resource structures, then either/or with a choice of two, and lastly one of
            // 3, 4 or 7.
            int nResources = s.resourceTypes.Length;

            int insertionIndex = -1;

            if (resources.Count != 0)
            {
                if (nResources == 2)
                {
                    // if the card has 2 resources, check whether it's an either/or or a double.
                    // put double resources ahead of either/or ones.
                    if (s.resourceTypes[0] == s.resourceTypes[1])
                    {
                        insertionIndex = resources.FindLastIndex(x => x.resourceTypes.Length == 2 && x.resourceTypes[0] == x.resourceTypes[1]);
                    }
                    else
                    {
                        insertionIndex = resources.FindLastIndex(x => x.resourceTypes.Length == nResources);
                    }
                }
                else
                {
                    // resource has 1, 3, 4, or 7 possibilities.
                    insertionIndex = resources.FindLastIndex(x => x.resourceTypes.Length == nResources);
                }

                while (insertionIndex == -1)
                {
                    if (nResources == 0)
                        throw new Exception();  // what happened here?
                    // no entries found with this number of resources.  Add the card after the last level 
                    // where there are some found.

                    --nResources;

                    insertionIndex = resources.FindLastIndex(x => x.resourceTypes.Length == nResources);
                }
            }

            resources.Insert(insertionIndex+1, s);
        }

        /*
        /// <summary>
        /// Generate and return a List of all possible sequences from the DAG
        /// </summary>
        /// <returns>List of strings</returns>
        public List<string> generateStrings()
        {
            // Not quite sure what this was doing.
            List<string> generated = new List<string>();

            generated.Add(" ");

            for (int i = 0; i < graph.Count; i++)
            {
                //Create new temp ArrayList at each depth level
                List<string> temp = new List<string>();

                //go through each existing string
                for (int j = 0; j < generated.Count; j++)
                {

                    //add each letter at the depth level
                    for (int k = 0; k < graph[i].Length; k++)
                    {
                        temp.Add(generated[j] + graph[i][k]);
                    }
                }

                //replace old arrayList with new one
                generated = temp;
            }

            return generated;
        }
        */

        public IEnumerable<ResourceEffect> getResourceList(bool isSelf)
        {
            if (isSelf)
            {
                return resources;
            }
            else
            {
                // remove resources that cannot be used by neighbors.
                return resources.Where(x => x.canBeUsedByNeighbors == true);
            }
        }

       /**
	     * Remove all letters that appear in B FROM A, then return the newly trimmed A
         * The interpretation of this, with respect to this program, is that given a Cost A, and available resources B
         * the return value represents unpaid Costs after using the B resources
         * For example, if the return value is "", then we know that with A was affordable with resources B
         * If the return value is "W", then we know that a Wood still must be paid.
	     * @param A = COST
	     * @param B = RESOURCES
	     */
        public static Cost eliminate(Cost structureCost, bool stopAfterAMatchIsFound, string resourceString)
        {
            // interesting.  structs do not need to be instantiated.  Classes do.  But structs
            // can only be PoD types, they cannot contain functions.

            Cost c = structureCost;

            foreach (char ch in resourceString)
            {
                switch (ch)
                {
                    case 'W':
                        if (c.wood != 0)
                        {
                            --c.wood;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    case 'S':
                        if (c.stone != 0)
                        {
                            --c.stone;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    case 'B':
                        if (c.clay != 0)
                        {
                            --c.clay;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    case 'O':
                        if (c.ore != 0)
                        {
                            --c.ore;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    case 'C':
                        if (c.cloth != 0)
                        {
                            --c.cloth;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    case 'G':
                        if (c.glass != 0)
                        {
                            --c.glass;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    case 'P':
                        if (c.papyrus != 0)
                        {
                            --c.papyrus;
                            if (stopAfterAMatchIsFound) return c;
                        }
                        break;

                    default:
                        throw new Exception();
                }
            }

            return c;
        }

        /**
         * Given a resource DAG graph, determine if a cost is affordable
         * @return
         */
        public static bool canAfford(DAG graph, Cost cost)
        {
            foreach (ResourceEffect e in graph.resources)
            {
                if (eliminate(cost, true, e.resourceTypes).IsZero())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Given a DAG and a cost (without $ in it), determine if the DAG can afford the cost
        /// </summary>
        /// <param name="graph">Player's DAG</param>
        /// <param name="cost">Card cost</param>
        /// <returns>Whether the DAG can afford the given card cost</returns>
        public static bool canAffordOffByOne(DAG graph, Cost cost)
        {
            throw new NotImplementedException();

            /*
            List<string> generated = graph.generateStrings();

            for (int i = 0; i < generated.Count; i++)
            {
                if (eliminate(cost, generated[i]).Length <= 1)
                {
                    return true;
                }
            }
            */

            return false;
        }

        /// <summary>
        /// Add Three DAGs together to form a mega DAG, used to determine if something is buyable via commerce
        /// </summary>
        /// <param name="A">Left DAG</param>
        /// <param name="B">Centre DAG</param>
        /// <param name="C">Right DAG</param>
        /// <returns>A Mega DAG that consists of A, B, C combined</returns>
        public static DAG addThreeDAGs(DAG A, DAG B, DAG C)
        {
            DAG returnedList = new DAG();

            IEnumerable<ResourceEffect> rA = A.getResourceList(false);
            IEnumerable<ResourceEffect> rB = B.getResourceList(true);
            IEnumerable<ResourceEffect> rC = C.getResourceList(false);

            foreach (ResourceEffect e in rA.Where(x => x.resourceTypes.Length == 1))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rB.Where(x => x.resourceTypes.Length == 1))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rC.Where(x => x.resourceTypes.Length == 1))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rA.Where(x => (x.resourceTypes.Length == 2) && (x.resourceTypes[0] == x.resourceTypes[1])))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rB.Where(x => (x.resourceTypes.Length == 2) && (x.resourceTypes[0] == x.resourceTypes[1])))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rC.Where(x => (x.resourceTypes.Length == 2) && (x.resourceTypes[0] == x.resourceTypes[1])))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rA.Where(x => (x.resourceTypes.Length == 2) && (x.resourceTypes[0] != x.resourceTypes[1])))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rB.Where(x => (x.resourceTypes.Length == 2) && (x.resourceTypes[0] != x.resourceTypes[1])))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rC.Where(x => (x.resourceTypes.Length == 2) && (x.resourceTypes[0] != x.resourceTypes[1])))
            {
                returnedList.add(e);
            }

            foreach (ResourceEffect e in rB.Where(x => x.resourceTypes.Length > 2))
            {
                returnedList.add(e);
            }

            return returnedList;
        }
    }
}
