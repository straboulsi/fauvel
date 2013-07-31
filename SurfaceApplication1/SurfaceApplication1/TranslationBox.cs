using System;
using System.Windows;
using System.Text.RegularExpressions;

namespace SurfaceApplication1
{
    public class TranslationBox
    {

        public String tag = "";
        String oldFr = "";
        String modFr = "";
        String eng = "";
        public Point topL = new Point(0, 0);
        public Point bottomR = new Point(0, 0);
        public double width, height;
        public int lines;

        // Has string (contents) and coordinates
        // Class1 should take in page number and output array of translation boxes
        public TranslationBox(String aTag, String someOldFr, String someModFr, String someEng, Point aTopL, Point aBottomR)
        {
            tag = aTag;
            oldFr = someOldFr;
            modFr = someModFr;
            eng = someEng;
            topL = aTopL;
            bottomR = aBottomR;
            width = bottomR.X - topL.X;
            height = bottomR.Y - topL.Y;
            lines = oldFr.Length - oldFr.Replace(Environment.NewLine, string.Empty).Length;
        }

        public TranslationBox(String aTag, String someOldFr, Point aTopL, Point aBottomR)
        {
            tag = aTag;
            oldFr = someOldFr;
            topL = aTopL;
            bottomR = aBottomR;
        }

        public String getTag()
        {
            return tag;
        }

        public String getOldFr()
        {
            return oldFr;
        }

        public String getModFr()
        {
            return modFr;
        }

        public String getEng()
        {
            return eng;
        }

        public Point getTopLeft()
        {
            return topL;
        }

        public Point getBottomRight()
        {
            return bottomR;
        }

        public String pointsToString()
        {
            return topL.X + " " + topL.Y + "\r\n" + bottomR.X + " " + bottomR.Y;
        }

    }
}
