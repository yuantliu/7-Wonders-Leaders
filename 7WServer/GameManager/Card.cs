using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{
    [Serializable]
    public class Card
    {

        //ID: 1-X
        public int id { get; set; }
        public string name { get; set; }
        //Age: 1-3
        public int age { get; set; }
        public int numberOfPlayers { get; set; }
        //Cost: SSS means 3 stones. ! means free, # means number of gold, e.g. 2WS = 2 coins 1 wood 1 stone
        public String cost { get; set; }
        //Free Prerequiste: ! means none
        public String freePreq { get; set; }
        //Colour: valid colours are ...
        public String colour { get; set; }
        /*
         * Kinds of effects
         * - gives one type of resource a certain amount:
         *      #L. 
         *      e.g. 2V (2 victory), 3O (3 Ores), 5$ (5 coins)
         *      
         * - gives one science: 
         *      e.g. B (bear trap)
         *           S (sextant)
         *           T (tablet)
         *           
         * - gives a choice of one or more type of resource: 
         *      e.g. WS (wood or stone)
         * 
         * - gives market effect:
         *      L = left, R = right, B = both, 
         *      M = manufactured, R = Raw, 
         *      e.g. LR = Left raw, BR = both manufactured
         * 
         * - gives some $ and/or Victory, depending on neighbour's cards or wonders:
         *      direction: L = left, C = centre, R = right
         *      card: Y = Yellow, N = green, G = grey, R = red, B = brown, S = stage
         *      gold: #
         *      Victory: #
         *      e.g. LCRG20 means This card will give 2 gold immediately and 2 victory points at the end of the game for each Green card for LCR
         *           _C_G22 means This card will give 2 gold and 2 victory points at the end of the game for each Green card for Centre
         */
        public String effect { get; set; }

        public Card(int id, String name, int age, int numberOfPlayers, String cost, String freePreq, String colour, String effect)
        {
            this.id = id;
            this.name = name;
            this.age = age;
            this.numberOfPlayers = numberOfPlayers;
            this.cost = cost;
            this.freePreq = freePreq;
            this.colour = colour;
            this.effect = effect;
        }

        //return a reference to an exact copy of this card
        public Card copy()
        {
            return new Card(id, name, age, numberOfPlayers, cost, freePreq, colour, effect);
        }
    }
}
