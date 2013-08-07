using System;
using System.Windows;
using System.Text.RegularExpressions;

namespace SurfaceApplication1
{

    /**
     * The TranslationBox object holds all poetry translations, for translation overlay functions.
     * It has Strings for each language's translation of the original.
     * In cases where some lines are longer, the font size and spacing are adjusted to display on the page in a true-to-original style.
     * Primary Coder: Alison Y. Chang
     * */
    public class TranslationBox
    {
        public static double minFontSize = 10;
        public String tag = "";
        String oldFr = "";
        String modFr = "";
        String eng = "";
        public Point topL = new Point(0, 0);
        public Point bottomR = new Point(0, 0);
        public double width, height, lineHeight;
        public int lines;

        // Has string (contents) and coordinates
        // Takes in page number and output array of translation boxes
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
            lines = oldFr.Length - oldFr.Replace(Environment.NewLine, string.Empty).Length + 2;
            lineHeight = ((double)2) * (height * SurfaceWindow1.minPageHeight / SurfaceWindow1.maxPageHeight) / lines;
            if (lineHeight < 1)
                lineHeight = 1;
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
