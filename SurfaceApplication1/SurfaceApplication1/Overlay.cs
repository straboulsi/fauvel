using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.IO;
using System.Globalization;

namespace DigitalFauvel
{

    /**
     * This class has two purposes:
     * 1) Display translation overlays for poetry in various languages.
     * 2) Returns the bounding box objects for a given page to check layout coordinates.
     * Primary Coder: Alison Y. Chang
     **/
    public static class Overlay
    {

        public static Brush backBrush = (Brush)(new BrushConverter().ConvertFrom("#CCE0D0B0"));
        public static Brush blockBrush = (Brush)(new BrushConverter().ConvertFrom("#ccffffff"));
        public static Brush blockFillerBrush = (Brush)(new BrushConverter().ConvertFrom("#33000000"));
        public static Brush textBrush = (Brush)(new BrushConverter().ConvertFrom("#663311"));


        public static Grid getGrid(TranslationBox tb, TextBlock t)
        {
            double width, x, y, height;
            x = tb.getTopLeft().X;
            y = tb.getTopLeft().Y;
            width = (tb.getBottomRight().X - tb.getTopLeft().X);
            height = (tb.getBottomRight().Y - tb.getTopLeft().Y);

            Grid g = new Grid();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(x, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(width, GridUnitType.Star);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(SurfaceWindow1.maxPageWidth - x - width, GridUnitType.Star);
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(y, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(height, GridUnitType.Star);
            RowDefinition r3 = new RowDefinition();
            r3.Height = new GridLength(SurfaceWindow1.maxPageHeight - y - height, GridUnitType.Star);

            g.ColumnDefinitions.Add(c1);
            g.ColumnDefinitions.Add(c2);
            g.ColumnDefinitions.Add(c3);
            g.RowDefinitions.Add(r1);
            g.RowDefinitions.Add(r2);
            g.RowDefinitions.Add(r3);

            t.Foreground = textBrush;
            t.Background = backBrush;
            t.FontSize = TranslationBox.minFontSize;
            t.TextWrapping = TextWrapping.NoWrap;
            t.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            t.FontFamily = new FontFamily("Tw Cen MT Condensed");
            Grid.SetRow(t, 1);
            Grid.SetColumn(t, 1);
            g.Children.Add(t);
            t.LineHeight = tb.lineHeight;

            return g;
        }

        public static Grid getGrid(BoundingBox bb)
        {
            double width, x, y, height;
            x = bb.topL.X;
            y = bb.topL.Y;
            width = (bb.bottomR.X - bb.topL.X);
            height = (bb.bottomR.Y - bb.topL.Y);

            Grid g = new Grid();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(x, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(width, GridUnitType.Star);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(SurfaceWindow1.maxPageWidth - x - width, GridUnitType.Star);
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(y, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(height, GridUnitType.Star);
            RowDefinition r3 = new RowDefinition();
            r3.Height = new GridLength(SurfaceWindow1.maxPageHeight - y - height, GridUnitType.Star);

            g.ColumnDefinitions.Add(c1);
            g.ColumnDefinitions.Add(c2);
            g.ColumnDefinitions.Add(c3);
            g.RowDefinitions.Add(r1);
            g.RowDefinitions.Add(r2);
            g.RowDefinitions.Add(r3);

            Border B = new Border();
            B.BorderBrush = blockBrush;
            B.BorderThickness = new Thickness(2);
            Grid filla = new Grid();
            Grid.SetRow(B, 1);
            Grid.SetColumn(B, 1);
            g.Children.Add(B);
            B.Child = filla;
            filla.Background = blockFillerBrush;

            return g;
        }





        /**
         * Creates an ArrayList of TranslationBox objects when given a folio page.
         * Consults the Content (Old French), Layout, and English XML files. 
         * Calls on other methods in this class to fetch English, French, or coordinates.
         * Expects folio without the "Fo" - i.e. 1v, 35r, 28tr
         **/
        public static List<TranslationBox> getTranslationOverlay(String page)
        {

            List<TranslationBox> boxes = new List<TranslationBox>();

            try
            {

                // Looks for each poetry object indicated in the layoutXML on a given page
                XmlNodeList foundNode = SurfaceWindow1.layoutXml.DocumentElement.SelectNodes("//surface[@id='" + page + "']/zone");

                foreach (XmlNode xn in foundNode)
                {
                    String tag = xn.Attributes["id"].Value;
                    if (tag.StartsWith("Te")) // Poetry objects only
                    {
                        XmlNodeList boxList = xn.SelectNodes("box"); // Gets each set of coordinates
                        foreach (XmlNode node in boxList)
                        {
                            String s = node.Attributes["id"].Value; // Returns line range - i.e. "Te_4500-4590"
                            int index = s.IndexOf("_");
                            int mid = s.IndexOf("-");
                            int start = Convert.ToInt32(s.Substring(index + 1, 4)); // First line number in the range
                            int end = Convert.ToInt32(s.Substring(mid + 1)); // Last line number in the range

                            // Gets translations in every available language
                            boxes.Add(new TranslationBox(s, Search.getPoetry(start, end, SurfaceWindow1.xml), Search.getPoetry(start, end, SurfaceWindow1.modFrXml),
                                Search.getPoetry(start, end, SurfaceWindow1.engXml), Search.getPoint(s, 1), Search.getPoint(s, 2)));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return boxes;
        }



        /**
         * Returns a List of BoundingBox objects for an indicated page.
         * Primarily used to check whether coordinates indicated in the LayoutXML are accurate.
         * */
        public static List<BoundingBox> getGhostBoxes(String page)
        {
            List<BoundingBox> boxes = new List<BoundingBox>();

            try
            {
                // Looks at every box in layoutXml 
                XmlNodeList foundNodes = SurfaceWindow1.layoutXml.DocumentElement.SelectNodes("//surface[@id='" + page + "']/zone/box");
                foreach (XmlNode node in foundNodes)
                {
                    String tag = node.ParentNode.Attributes["id"].Value;

                    Point topL = new Point(Convert.ToDouble(node.Attributes["ulx"].Value), Convert.ToDouble(node.Attributes["uly"].Value));
                    Point bottomR = new Point(Convert.ToDouble(node.Attributes["lrx"].Value), Convert.ToDouble(node.Attributes["lry"].Value));
                    BoundingBox newBB = new BoundingBox(tag, topL, bottomR);
                    boxes.Add(newBB);
                }

            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                Console.Read();
            }

            return boxes;

        }



        

    }
}
