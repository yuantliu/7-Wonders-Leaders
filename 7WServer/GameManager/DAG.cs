using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    [Serializable]
    public class DAG
    {
        private List<char[]> graph = new List<char[]>();

        public bool hasTemp = false;

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

        //Add an OR resource
        public void add(string s)
        {
            char[] newInput = s.ToCharArray();
            //when there is a temp resource, insert into a non-last position so that the removal of temp resource works later
            if (hasTemp == true)
            {
                graph.Insert(0, newInput);
            }
            else
            {
                graph.Add(newInput);
            }
        }

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

        public List<char[]> getGraph() { return graph; }
        public void setGraph(List<char[]> graph) { this.graph = graph; }

        /**
	     * Remove all letters that appear in B FROM A, then return the newly trimmed A
         * The interpretation of this, with respect to this program, is that given a Cost A, and available resources B
         * the return value represents unpaid Costs after using the B resources
         * For example, if the return value is "", then we know that with A was affordable with resources B
         * If the return value is "W", then we know that a Wood still must be paid.
	     * @param A = COST
	     * @param B = RESOURCES
	     */
        public static Cost eliminate(Cost structureCost, string B)
        {
            // interesting.  structs do not need to be instantiated.  Classes do.  But structs
            // can only be PoD types, they cannot contain functions.

            Cost c = new Cost();

            c.coin = Math.Max(structureCost.coin - B.Count(x => x == '$'), 0);
            c.wood = Math.Max(structureCost.wood - B.Count(x => x == 'W'), 0);
            c.stone = Math.Max(structureCost.stone - B.Count(x => x == 'T'), 0);
            c.clay = Math.Max(structureCost.clay - B.Count(x => x == 'B'), 0);
            c.ore = Math.Max(structureCost.ore - B.Count(x => x == 'O'), 0);
            c.cloth = Math.Max(structureCost.cloth - B.Count(x => x == 'L'), 0);
            c.glass = Math.Max(structureCost.glass - B.Count(x => x == 'G'), 0);
            c.papyrus = Math.Max(structureCost.papyrus - B.Count(x => x == 'P'), 0);


            /*
            c.coin = Math.Max(structureCost.coin - resourcesAvailable.coin, 0);
            c.wood = Math.Max(structureCost.wood - resourcesAvailable.wood, 0);
            c.stone = Math.Max(structureCost.stone - resourcesAvailable.stone, 0);
            c.clay = Math.Max(structureCost.clay - resourcesAvailable.clay, 0);
            c.ore = Math.Max(structureCost.ore - resourcesAvailable.ore, 0);
            c.cloth = Math.Max(structureCost.cloth - resourcesAvailable.cloth, 0);
            c.glass = Math.Max(structureCost.glass - resourcesAvailable.glass, 0);
            c.papyrus = Math.Max(structureCost.papyrus - resourcesAvailable.papyrus, 0);
            */

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
		    List<string> generated = graph.generateStrings();
	
		    for(int i = 0; i < generated.Count; i++)
            {
			    if(eliminate(cost, generated[i]).IsZero())
                {
				    return true;
			    }
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
            DAG megaDAG = new DAG();
            megaDAG.setGraph(A.getGraph().Concat(B.getGraph()).Concat(C.getGraph()).ToList());
            return megaDAG;
        }
    }
}
