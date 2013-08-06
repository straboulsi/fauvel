using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfaceApplication1
{

    /**
     * This SpecialString object is used in the searchMultipleWords function of Translate.cs.
     * It is used when every search word in the overall string must be found.
     * */
    public class SpecialString : IComparable
    {
        public String str;
        public Boolean isFound;
        public int isStyled; // 0 for no and 1 for yes
        public int spotInResult; // Used when searching for multiple words to figure out order in which search words appear in the results

        public SpecialString(String aStr)
        {
            str = aStr;
        }

        public SpecialString(String aStr, Boolean isF)
        {
            str = aStr;
            isFound = isF;
        }

        public SpecialString(String aStr, int anInt)
        {
            str = aStr;
            isStyled = anInt;
        }


        public int CompareTo(object other)
        {
            SpecialString otherSS = other as SpecialString;

            if (this.spotInResult < otherSS.spotInResult)
                return -1;
            else if (this.spotInResult > otherSS.spotInResult)
                return 1;
            else
                return 0;
        }


    }
}
