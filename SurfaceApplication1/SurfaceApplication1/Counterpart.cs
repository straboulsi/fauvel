using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfaceApplication1
{
    /**
     * This class helps to organize all the music objects in Fauvel that have multiple parts.
     * The first string "name" is the first tag for that object.
     * The other strings in "otherNames" are tags for all other counterparts.
     * Counterpart objects are used in Thumbnailer.cs to help with Thumbnail returns.
     **/
    class Counterpart
    {
        public String name;
        public List<String> otherNames;

        public Counterpart(String aName, List<String> someOtherNames)
        {
            name = aName;
            otherNames = someOtherNames;
        }

    }
}
