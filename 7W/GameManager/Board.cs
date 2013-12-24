using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    /// <summary>
    /// This public class will load up board information from a textfile
    /// After that, it will essentially be read-only
    /// </summary>
    public class Board
    {
        public string name { get; set; }

        //number of stages
        public int numOfStages { get; set; }
        //stage effects
        public string[] effect;
        //stage costs
        public string[] cost;


        //the free resource that the board provides
        //e.g. W, O, P, T, etc.
        public char freeResource { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numOfStages"></param>
        /// <param name="name"></param>
        /// <param name="effect"></param>
        /// <param name="path"></param>
        public Board(string name, int numOfStages, char freeResource, string [] cost, string [] effect)
        {
            this.name = name;
            this.numOfStages = numOfStages;
            this.freeResource = freeResource;

            this.cost = cost;
            this.effect = effect;
        }


    }
}
