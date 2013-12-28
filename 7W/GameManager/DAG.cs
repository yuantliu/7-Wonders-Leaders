using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    [Serializable]
    class DAG
    {
        private List<char[]> graph = new List<char[]>();

        //Add an OR resource
        public void add(string s)
        {
            char[] newInput = s.ToCharArray();

            graph.Add(newInput);
        }

        /// <summary>
        /// Generate and return a List of all possible sequences from the DAG
        /// </summary>
        /// <returns>List of strings</returns>
        public List<string> generateStrings()
        {
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
        public static string eliminate(string A, string B)
        {
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
        }

        /**
         * Given a resource DAG graph, determine if a cost is affordable
         * @return
         */
        public static bool canAfford(DAG graph, string cost){
		    List<string> generated = graph.generateStrings();
		
		    for(int i = 0; i < generated.Count; i++){
			    if(eliminate(cost, generated[i]).Equals("")){
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
        public static bool canAffordOffByOne(DAG graph, string cost)
        {
            List<string> generated = graph.generateStrings();

            for (int i = 0; i < generated.Count; i++)
            {
                if (eliminate(cost, generated[i]).Length <= 1)
                {
                    return true;
                }
            }

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
