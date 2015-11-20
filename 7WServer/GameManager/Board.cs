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
        public enum Wonder
        {
            Alexandria_A,
            Alexandria_B,
            Babylon_A,
            Babylon_B,
            Ephesos_A,
            Ephesos_B,
            Giza_A,
            Giza_B,
            Halikarnassos_A,
            Halikarnassos_B,
            Olympia_A,
            Olympia_B,
            Rhodos_A,
            Rhodos_B,
            Roma_A,
            Roma_B,
        };

        public Wonder otherSide;

        public string name { get; private set; }

        //number of stages
        public int numOfStages { get; private set; }

        public Effect freeResource { get; private set; }

        //stage costs
        public Cost[] cost;

        //stage effects
        public Effect[] effect;

        //the free resource that the board provides
        //e.g. W, O, P, T, etc.
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numOfStages"></param>
        /// <param name="name"></param>
        /// <param name="effect"></param>
        /// <param name="path"></param>
        public Board(Wonder otherside, string name, Effect boardEffect, int nStages)
        {
            this.otherSide = otherside;
            this.name = name;
            this.freeResource = boardEffect;
            this.numOfStages = nStages;
        }
    }
}
