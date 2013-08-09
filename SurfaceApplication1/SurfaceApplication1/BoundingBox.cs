using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SurfaceApplication1
{
    /** 
     * This Box represents a bounding box drawn for a piece of music, an image, etc.
     * BoundingBoxes are used in Translate.getGhostBoxes to return all boxes on a given page.
     * This is mostly needed for checking over the coordinates recorded in LayoutXML.
     * Primary Coder: Alison Y. Chang
     **/
    public class BoundingBox
    {

        public readonly double X, Y, Width, Height;
        public readonly Point topL, bottomR;
        public readonly String tag;


        public BoundingBox(String aTag, Point aTopL, Point aBottomR)
        {
            tag = aTag;
            topL = aTopL;
            bottomR = aBottomR;
            X = topL.X;
            Y = topL.Y;
            Width = bottomR.X - X;
            Height = bottomR.Y - Y;
        }

    }
}
