using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenWonders
{

    /// <summary>
    /// Immutable, Static Blacklist class that contains a list of problematic cards that "simple" AIs should not build
    /// Namely, 
    /// 12, 13, 24, 25, 46, 47 (Market place effects)
    /// 101, 102, 103, 107, 201, 212, 231 (involves Stages)
    /// 204, 209, 237, 229 (Problematic Leaders)
    /// 301 (Courtesan's Guild)
    /// </summary>
    public static class AI_LEADERS_BLACKLIST
    {
        private static int[] myBLACKLIST = { 12, 13, 24, 25, 46, 47, 101, 102, 103, 107, 201, 212, 231, 204, 208, 237, 229, 301 };

        public static bool Contains(int num)
        {
            return myBLACKLIST.Contains(num);
        }
    }
}
