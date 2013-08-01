using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurfaceApplication1
{
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
