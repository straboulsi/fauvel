using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SurfaceApplication1
{
    /** 
     * This Box represents a bounding box drawn for a piece of music, an image, etc.
     **/
    public class BoundingBox
    {
        public String tag = "";
        public Point topL = new Point(0, 0);
        public Point bottomR = new Point(0, 0);

        public BoundingBox(String aTag, Point aTopL, Point aBottomR)
        {
            tag = aTag;
            topL = aTopL;
            bottomR = aBottomR;
        }

    }
}
