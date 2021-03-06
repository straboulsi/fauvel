﻿using System;
using System.Windows;
using System.Text.RegularExpressions;

namespace DigitalFauvel
{

    /**
     * The TranslationBox object holds all poetry translations, for translation overlay functions.
     * It has Strings for each language's translation of the original.
     * In cases where some lines are longer, the font size and spacing are adjusted to display on the page in a true-to-original style.
     * Primary Coder: Alison Y. Chang
     * */
    public class TranslationBox
    {
        public double width, height, lineHeight;
        public static double minFontSize = 10;
        public int lines;
        public Point topL = new Point(0, 0);
        public Point bottomR = new Point(0, 0);
        public String tag = "", oldFr = "", modFr = "", eng = "";
        public bool expanded = false;


        /**
         * An object that holds together strings (translations in various languages) and coordinates
         * Used in Overlay.getTranslationOverlay to return all TranslationBoxes for a given page.
         * */
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

        /**
         * A simpler constructor that only has the original French text, without additional translations.
         * */
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

        /**
         * Returns the coordinates in a more print-friendly format.
         * */
        public String pointsToString()
        {
            return topL.X + " " + topL.Y + "\r\n" + bottomR.X + " " + bottomR.Y;
        }

    }
}
