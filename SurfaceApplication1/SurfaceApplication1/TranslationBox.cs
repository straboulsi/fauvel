using System;
using System.Windows;

namespace SurfaceApplication1
{
    public class TranslationBox
    {

        public String tag = "";
        String oldFr = "";
        String eng = "";
        Point topL = new Point(0, 0);
        Point bottomR = new Point(0, 0);

        // Has string (contents) and coordinates
        // Class1 should take in page number and output array of translation boxes
        public TranslationBox(String aTag, String someOldFr, String someEng, Point aTopL, Point aBottomR)
        {
            tag = aTag;
            oldFr = someOldFr;
            eng = someEng;
            topL = aTopL;
            bottomR = aBottomR;
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

    }
}
