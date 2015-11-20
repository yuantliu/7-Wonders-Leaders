using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    [Serializable]
    public class DAG
    {
        // private List<char[]> graph = new List<char[]>();

        // this list needs to be sorted in a particular order.  Simplest types at the top so those cards are used up first
        // when calculating whether a structure is affordable.  Those are RawMaterials which do not offer a choice
        // Goods and single-choice Resources come first.  Next come double RawMaterial cards.  They don't offer a
        // choice but are better than single Raw materials.  Next would come first-age resource cards that have a choice of
        // two.  There's a beter chance that by the time those cards are considered, one of the needed resources has
        // already been taken care of by a single-resource card.  Last are the cards that offer a choice of all 3 goods
        // or all 4 raw materials (Forum/Caravansery/Alexandria stages as they are the most flexible).  After those are
        // considered, we look at Bilkis and other leaders who provide a -1 discount on certain structure classes.
        // Also the Secret Warehouse and Black Market are in there somewhere.
        List<SimpleEffect> simpleResources = new List<SimpleEffect>();

        // choice cards.  Used after simpleResources are exhausted.
        List<ResourceChoiceEffect> effectChoices = new List<ResourceChoiceEffect>();

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
        public void add(Effect s)
        {
            /*
            char[] newInput = s.ToCharArray();
            //when there is a temp resource, insert into a non-last position so that the removal of temp resource works later
            if (hasTemp == true)
            {
                graph.Insert(0, newInput);
            }
            else
            */
            {
                if (s is SimpleEffect)
                {
                    simpleResources.Add((SimpleEffect)s);
                }
                else if (s is ResourceChoiceEffect)
                {
                    effectChoices.Add((ResourceChoiceEffect)s);
                }
            }
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

        public List<SimpleEffect> getSimpleStructures()
        {
            return simpleResources;
        }

        public List<ResourceChoiceEffect> getChoiceStructures(bool isSelf)
        {
            // if isSelf is true, all the resource structures are included in the returned list.
            // If it is false, only the RawMaterial or Goods structures are returned.
            return isSelf ? effectChoices : effectChoices.Where(x => x.canBeUsedByNeighbors).ToList();
        }

        public void setSimpleStructureList(List<SimpleEffect> graph) { this.simpleResources = graph; }

        public void setChoiceStructureList(List<ResourceChoiceEffect> graph) { this.effectChoices = graph; }

        /**
	     * Remove all letters that appear in B FROM A, then return the newly trimmed A
         * The interpretation of this, with respect to this program, is that given a Cost A, and available resources B
         * the return value represents unpaid Costs after using the B resources
         * For example, if the return value is "", then we know that with A was affordable with resources B
         * If the return value is "W", then we know that a Wood still must be paid.
	     * @param A = COST
	     * @param B = RESOURCES
	     */
        public static Cost eliminate(Cost structureCost, int multiplier, string resourceString)
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
                            c.wood = Math.Max(c.wood - multiplier, 0);
                            return c;
                        }
                        break;

                    case 'S':
                        if (c.stone != 0)
                        {
                            c.stone = Math.Max(c.stone - multiplier, 0);
                            return c;
                        }
                        break;

                    case 'B':
                        if (c.clay != 0)
                        {
                            c.clay = Math.Max(c.clay - multiplier, 0);
                            return c;
                        }
                        break;

                    case 'O':
                        if (c.ore != 0)
                        {
                            c.ore = Math.Max(c.ore - multiplier, 0);
                            return c;
                        }
                        break;

                    case 'C':
                        if (c.cloth != 0)
                        {
                            c.cloth = Math.Max(c.cloth - multiplier, 0);
                            return c;
                        }
                        break;

                    case 'G':
                        if (c.glass != 0)
                        {
                            c.glass = Math.Max(c.glass - multiplier, 0);
                            return c;
                        }
                        break;

                    case 'P':
                        if (c.papyrus != 0)
                        {
                            c.papyrus = Math.Max(c.papyrus - multiplier, 0);
                            return c;
                        }
                        break;

                    default:
                        throw new Exception();
                }
            }

            return c;

            /*
            for (int i = 0; i < B.Length; i++)
            {
                for (int j = 0; j < A.Length; j++)
                {
                    if (A[j] == B[i])
                    {
                        A = A.Substring(0, j) + A.Substring(j + 1);
                        break;
                    }
                }
            }

            return A;
            */

        }

        /**
         * Given a resource DAG graph, determine if a cost is affordable
         * @return
         */
        public static bool canAfford(DAG graph, Cost cost)
        {
            foreach (SimpleEffect e in graph.simpleResources)
            {
                if (eliminate(cost, e.multiplier, e.type.ToString()).IsZero())
                    return true;
            }

            foreach (ResourceChoiceEffect e in graph.effectChoices)
            {
                if (eliminate(cost, 1, e.strChoiceData).IsZero())
                    return true;
            }
            /*
            List<string> generated = graph.generateStrings();

            for(int i = 0; i < generated.Count; i++)
            {
                if(eliminate(cost, generated[i]).IsZero())
                {
                    return true;
                }
            }
            */

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
            DAG megaDAG = new DAG();
            megaDAG.setSimpleStructureList(A.getSimpleStructures().Concat(B.getSimpleStructures().Concat(C.getSimpleStructures())).ToList());
            megaDAG.setChoiceStructureList(A.getChoiceStructures(false).Concat(B.getChoiceStructures(true).Concat(C.getChoiceStructures(false))).ToList());
            // megaDAG.setGraph(A.getGraph().Concat(B.getGraph()).Concat(C.getGraph()).ToList());
            return megaDAG;
        }
    }
}
