using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    [Serializable]
    class DAG
    {
        public List<char[]> graph = new List<char[]>();

        //Add an OR resource
        public void add(string s)
        {
            char[] newInput = s.ToCharArray();

            graph.Add(newInput);
        }

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
    }
}
